namespace Plisky.Test {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit.Abstractions;
    using Xunit.Sdk;



    public class CategoryDiscoverer : ITraitDiscoverer {
        internal const string DISCOVERER_TYPENAME = Traits.Namespace + "." + nameof(CategoryDiscoverer);
        public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute) {
            var ctorArgs = traitAttribute.GetConstructorArguments().ToList();
            yield return new KeyValuePair<string, string>(Traits.Category, ctorArgs[0]?.ToString() ?? "");
        }
    }


    [TraitDiscoverer(CategoryDiscoverer.DISCOVERER_TYPENAME, Traits.AssemblyName)]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class CategoryAttribute : Attribute, ITraitAttribute {
        public CategoryAttribute(string category) {
            Category = category;
        }
        public string Category { get; set; }
    }

    public class IntegrationDiscoverer : ITraitDiscoverer {
        internal const string DISCOVERER_TYPENAME = Traits.Namespace + "." + nameof(IntegrationDiscoverer);
        public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute) {
            yield return new KeyValuePair<string, string>(Traits.Category, Traits.Integration);
        }
    }

    [TraitDiscoverer(IntegrationDiscoverer.DISCOVERER_TYPENAME, Traits.AssemblyName)]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class IntegrationAttribute : Attribute, ITraitAttribute {
        public IntegrationAttribute() { }
    }

    public class UnitDiscoverer : ITraitDiscoverer {
        internal const string DISCOVERER_TYPENAME = Traits.Namespace + "." + nameof(UnitDiscoverer);
        public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute) {
            yield return new KeyValuePair<string, string>(Traits.Category, Traits.Unit);
        }
    }

    [TraitDiscoverer(UnitDiscoverer.DISCOVERER_TYPENAME, Traits.AssemblyName)]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class UnitAttribute : Attribute, ITraitAttribute {
        public UnitAttribute() { }
    }

    public class FreshDiscoverer : ITraitDiscoverer {
        internal const string DISCOVERER_TYPENAME = Traits.Namespace + "." + nameof(FreshDiscoverer);
        public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute) {
            yield return new KeyValuePair<string, string>(Traits.Category, "Fresh");
        }
    }

    [TraitDiscoverer(FreshDiscoverer.DISCOVERER_TYPENAME, Traits.AssemblyName)]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class FreshAttribute : Attribute, ITraitAttribute {
        public FreshAttribute() { }
    }

    public class BuildDiscoverer : ITraitDiscoverer {
        internal const string DISCOVERER_TYPENAME = Traits.Namespace + "." + nameof(BuildDiscoverer);
        public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute) {

            string buildType = traitAttribute.GetNamedArgument<string>("BuildType");
            var buildTypes = Enum.GetNames(typeof(BuildType)).ToList();
            if (!buildTypes.Contains(buildType)) {
                yield return new KeyValuePair<string, string>("Build", "");
            }
            yield return new KeyValuePair<string, string>("Build", buildType ?? "");
        }
    }

    [TraitDiscoverer(BuildDiscoverer.DISCOVERER_TYPENAME, Traits.AssemblyName)]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class BuildAttribute : Attribute, ITraitAttribute {
        public BuildAttribute(BuildType type) {
            BuildType = type.ToString();
        }
        public string BuildType { get; set; }
    }

    public enum BuildType {
        CI,
        Local,
        Nightly,
        Release,
        Any
    }

    public class BugDiscoverer : ITraitDiscoverer {
        internal const string DISCOVERER_TYPENAME = Traits.Namespace + "." + nameof(BugDiscoverer);
        public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute) {

            string workItemId = traitAttribute.GetNamedArgument<string>("WorkItemId");

            if (!int.TryParse(workItemId, out _)) {
                yield return new KeyValuePair<string, string>(Traits.LiveBug, "");
            }
            yield return new KeyValuePair<string, string>(Traits.LiveBug, workItemId ?? "");
        }
    }

    [TraitDiscoverer(BugDiscoverer.DISCOVERER_TYPENAME, Traits.AssemblyName)]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class BugAttribute : Attribute, ITraitAttribute {
        public BugAttribute(int workItemId) {
            WorkItemId = workItemId.ToString();
        }
        public string WorkItemId { get; set; }
    }

    public class IsolatedDiscoverer : ITraitDiscoverer {
        internal const string DISCOVERER_TYPENAME = Traits.Namespace + "." + nameof(IsolatedDiscoverer);
        public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute) {
            yield return new KeyValuePair<string, string>(Traits.Category, Traits.Isolated);
        }
    }

    [TraitDiscoverer(IsolatedDiscoverer.DISCOVERER_TYPENAME, Traits.AssemblyName)]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class IsolatedAttribute : Attribute, ITraitAttribute {
        public IsolatedAttribute() { }
    }

}
