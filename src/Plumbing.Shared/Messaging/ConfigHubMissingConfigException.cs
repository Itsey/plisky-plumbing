namespace Plisky.Plumbing {

    using System;

    [Serializable]
    public class ConfigHubMissingConfigException : Exception {

        public ConfigHubMissingConfigException() { }

        public ConfigHubMissingConfigException(string message) : base(message) { }

        public ConfigHubMissingConfigException(string message, Exception inner) : base(message, inner) { }

        protected ConfigHubMissingConfigException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class ConfigHubConfigurationFailureException : Exception {

        public ConfigHubConfigurationFailureException() { }

        public ConfigHubConfigurationFailureException(string message) : base(message) { }

        public ConfigHubConfigurationFailureException(string message, Exception inner) : base(message, inner) { }

        protected ConfigHubConfigurationFailureException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}