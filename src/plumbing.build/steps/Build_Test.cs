﻿
using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.Tools.DotNet;
using Serilog;

public partial class Build : NukeBuild {

    // TestStep is the well known post release integration test step. Arrange Construct Examine Package Release [Test]
    public Target TestStep => _ => _
        .After(ConstructStep)
        .Before(Wrapup)
        .DependsOn(Initialise)
        .Triggers(IntegrationTest)
        .Executes(() => {
            Log.Information("--> Test Step <-- ");
        });

    private Target IntegrationTest => _ => _
      .Executes(() => {
          if (Solution == null) {
              Log.Error("Build>IntegrationTest>Solution is null.");
              throw new InvalidOperationException("The solution must be set");
          }

          var testProjects = Solution.GetAllProjects("*.ITest");
          if (testProjects.Any()) {
              DotNetTasks.DotNetTest(s => s
                  .EnableNoRestore()
                  //.EnableNoBuild()
                  .SetConfiguration(Configuration)
                  .SetProjectFile(testProjects.First().Directory));
          }
      });
}
