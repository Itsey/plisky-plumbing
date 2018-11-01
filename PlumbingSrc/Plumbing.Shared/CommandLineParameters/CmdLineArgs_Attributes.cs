namespace Plisky.Helpers {

    using System;

    /// <summary>
    /// The CmdLineArgumentBase class represents the base for the attributes that are used to indicate a class is based on command line attributes
    /// it should not be applied to any class or field directly.
    /// </summary>
    public abstract class CommandLineArgumentBaseAttribute : Attribute {
        private const int MAXLENGTH_SHORTDESCRIPTION = 80;

        /// <summary>
        /// Describes the command line option to the user of the tool, in a way in which it can be displayed on the screen, this should
        /// be a short one liner describing the option simply.  It is limited to a total of 80 characters
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when Description is set to a length greater than 80 Charcters.</exception>
        public string Description {
            get { return m_description; }

            set {
                if (value == null) {
                    m_description = string.Empty;
                    return;
                }
                if (value.Length > MAXLENGTH_SHORTDESCRIPTION) {
                    throw new ArgumentOutOfRangeException("Description", "The description must be a short comment limited to " + MAXLENGTH_SHORTDESCRIPTION.ToString() + " characters.");
                }
                m_description = value;
            }
        }

        /// <summary>
        /// Describes the command line option to the user more fully.  This can be any length and is displayed when the user asks for
        /// help about a specific command. This should be a fully fledged description of the command and can occupy many lines.
        /// </summary>
        public string FullDescription {
            get { return m_fullDescription; }
            set {
                if (value == null) {
                    m_fullDescription = string.Empty;
                    return;
                }
                m_fullDescription = value;
            }
        }

        private string m_fullDescription = string.Empty;
        private string m_description = string.Empty;
    }

    /// <summary>
    /// CommandLineArgumentsAttribute used to describe a class as a command line argument.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class CommandLineArgumentsAttribute : Attribute {
    }

    /// <summary>
    /// The UnmatchedCmdLineArgAttribute acn be applied to an array of strings to indicate that any unmatched parameters should be
    /// placed into this array.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class CommandLineArgDefaultAttribute : CommandLineArgumentBaseAttribute {
    }

    /// <summary>
    /// CmdLineArgAttribute class is an attribute that can be used to decorate Fields and Properties on a Class that represents arguments
    /// passed to the application.  This attribute describes how command line arguments map to the fields in the class and can be used
    /// by the CommandArgumentSupport class to convert a set of command line parameters into values within the class.
    /// </summary>
    /// <remarks>This attribute can target Fields or Properties, is allowed multiple times and is inherited.</remarks>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public sealed class CommandLineArgAttribute : CommandLineArgumentBaseAttribute {
        private string m_argumentDescriptor = string.Empty;
        private bool m_isSingleParameterDefaultArgument;

        /// <summary>
        /// The CmdLineArgAttribute is applied to a field or a property within a class to indicate that it can be initialised from
        /// the command line.  This constructor passes the argument identifier which is the part of the argument that identifies
        /// which of the passed parameters matches the decorated field or property.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">The argumentIdentifier parameter is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">When the argumentIdentifier length is 0.</exception>
        /// <param name="argumentIdentifier">The string to match on the command line</param>
        public CommandLineArgAttribute(string argumentIdentifier) {
            if (argumentIdentifier == null) { throw new ArgumentNullException(nameof(argumentIdentifier), "The identifier for a parameter can not be null, for default parameters use default=true"); }
            if (argumentIdentifier.Length == 0) { throw new ArgumentOutOfRangeException(nameof(argumentIdentifier), "The identifier for a parameter can not have a length of zero.  For default parameters use Default=true"); }

            m_argumentDescriptor = argumentIdentifier;
        }

        /// <summary>
        /// The ArgumentIdentifier specifies the text that identifiers the argument, this is typically passed as the first part of
        /// any argument string therefore something like /n:xxxx could be the argument where /n: is the argument identifier and the
        /// xxxx is the vaule assigned to that identifier.
        /// </summary>
        /// <remarks>The ArgumentIdentifier can not be null or empty.</remarks>
        public string ArgumentIdentifier {
            get { return m_argumentDescriptor; }
        }

        /// <summary>
        /// The Default Property indicates whether this is the default argument for the program and therefore if it should have the
        /// value assigned to it for any arguments that do not have a prefix that is matched.  This allows the code to pass for example
        /// a filename without a prefix and have it route through to the decorated field or property.  It is therefore not possible to
        /// have more than one Default decorated field or property for a class.
        /// </summary>
        public bool IsSingleParameterDefault { get; set; }

        /// <summary>
        /// Where the argument is an array then this must be used to indicate the separator char, therefore arrays of ints can be
        /// passed using int,int,int if the separator char is set to ,
        /// </summary>
        public string ArraySeparatorChar { get; set; }
    }
}