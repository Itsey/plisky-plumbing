﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tools.DotNet;
using Plisky.Nuke.Fusion;
using Serilog;

public partial class Build : NukeBuild {

    // ArrangeStep = Well Known Initial Step for correctness and Linting. [Arrange] Construct Examine Package Release Test
    public Target ArrangeStep => _ => _
        .Before(ConstructStep, Wrapup)
        .DependsOn(Initialise)
        .Triggers(Clean, MollyCheck, RestoreStep)
        .Executes(() => {
            Log.Information("--> Arrange <-- ");
        });

    private Target Clean => _ => _
        .DependsOn(Initialise)
        .After(ArrangeStep, Initialise)
        .Before(ConstructStep)
        .Executes(() => {
            b.Info.Log("Clean Step in Arrange Starts");

            if (settings == null) {
                Log.Error("Build>ApplyVersion>Settings is null.");
                throw new InvalidOperationException("The settings must be set");
            }

            if (Solution == null) {
                Log.Error("Build>ApplyVersion>Solution is null.");
                throw new InvalidOperationException("The solution must be set");
            }

            DotNetTasks.DotNetClean(s => s
                .SetProject(Solution)
                .SetConfiguration(Configuration)
            );

            b.Verbose.Log("Clean completed, cleaning artifact directory");

            settings.ArtifactsDirectory.CreateOrCleanDirectory();
        });

    private Target MollyCheck => _ => _
       .After(Clean, ArrangeStep)
       .DependsOn(Initialise, NexusLive)
       .Before(ConstructStep)
       .Executes(() => {
           Log.Information("Mollycoddle Structure Linting Starts.");

           if (settings == null) {
               throw new InvalidOperationException("The settings are not configured, Mollycoddle cannot run.");
           }
           if (GitRepository == null) {
               throw new InvalidOperationException("The Git Repository is not configured, Mollycoddle cannot run.");
           }

           var mcOk = ValidateMollySettings(settings.MollyRulesToken, GitRepository.LocalDirectory.Exists());
           if (mcOk != ValidationResult.Success) {
               Log.Error("Mollycoddle Structure Linting Skipped - Validation Failed.");
               foreach (string item in mcOk!.MemberNames) {
                   Log.Error(item);
               }
               return;
           }


           Log.Verbose($"MC ({settings.MollyRulesToken}) ({settings.MollyPrimaryToken}) ({GitRepository.LocalDirectory})");
           var mc = new MollycoddleTasks();


           string formatter = IsLocalBuild ? "plain" : "azdo";

           var mcs = new MollycoddleSettings();
           mcs.AddRuleHelp(true);

           if (!string.IsNullOrEmpty(settings.MollyRulesVersion)) {
               mcs.AddRulesetVersion(settings.MollyRulesVersion);
           }

           mcs.SetRulesFile(settings.MollyRulesToken);
           mcs.SetPrimaryRoot(settings.MollyPrimaryToken);
           mcs.SetFormatter(formatter);
           mcs.SetDirectory(GitRepository.LocalDirectory);

           Log.Verbose("About to perform MC check");
           mc.PerformScan(mcs);

           Log.Information("Mollycoddle Structure Linting Completes.");
       });

    public Target RestoreStep => _ => _
        .After(ArrangeStep, Clean, MollyCheck)
        .DependsOn(Initialise)
        .Before(ConstructStep)
        .Executes(() => {
            Log.Information("--> NuGet Restore <--");
            DotNetTasks.DotNetRestore(s => s.SetProjectFile(Solution));
        });

    [Pure]
    private ValidationResult? ValidateMollySettings(string? mollyRulesToken, bool localDirectoryExists) {
        var errors = new List<string>();

        if (!localDirectoryExists) {
            errors.Add("Mollycoddle: Local Working Directory Error.  Directory Does Not Exist.");
        }
        if (string.IsNullOrWhiteSpace(mollyRulesToken)) {
            errors.Add("Mollycoddle: Ruleset Initialisation Token Not Set.");
        }

        if (errors.Count > 0) {
            return new ValidationResult("Mollycoddle: Parameter Validation Failed.", errors);
        }
        return ValidationResult.Success;

    }
}

