namespace Plisky.Helpers {

    using Plisky.Diagnostics;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    /// <summary>
    /// CommandArgumentSupport provides assistance with using command line arguments and is designed to work in conjunction with the
    /// CmdLineArgAttribute class which should be used to decorate a class with command line attributes.  This decorated class can be
    /// passed to this CommandLineArgument support class to process any arguments passed to the application.
    /// </summary>
    /// <remarks> Developed in conjunction with Nemingalator, therefore may not be suitable for reuse</remarks>
    public class CommandArgumentSupport {
        private Bilge b = new Bilge("CommandLineArguments");
        private List<string> argumentErrorsDuringLastParse = new List<string>();

        /// <summary>
        /// Returns the name(s) of arguments that errored (rather than were not matched at all) and a brief description
        /// of why the error occured.
        /// </summary>
        public string[] ListArgumentsWithErrors() {
            return argumentErrorsDuringLastParse.ToArray();
        }

        /// <summary>
        /// Creates a new instance of the CommandArgumentSupport class.
        /// </summary>
        public CommandArgumentSupport() {
            ArgumentPostfix = string.Empty;
        }

        private string argumentPrefix = "-";

        /// <summary>
        /// ArgumentPrefix determines the prefixed text that should be on the front of each argument specifier.  This is commonly a forwards
        /// slash however the default is a hyphen in line with the Microsoft powershell standards.
        /// </summary>
        public string ArgumentPrefix {
            get { return argumentPrefix; }
            set {
                if (value == null) {
                    argumentPrefix = string.Empty;
                }
                argumentPrefix = value;
            }
        }

        /// <summary>
        /// Set when the arguments have a specific post fix before the value.  Typically this is a colon, however there is no default for this
        /// as it is not necessary for many types of argument.
        /// </summary>
        public string ArgumentPostfix { get; set; }

        /// <summary>
        /// ArgumentPrefixOptional determines whether the ArgumentPrefix string must be on the front of a parameter for the match to be valid
        /// or whether the match will occur whether the prefix is there or not.  If this is set to false then an exact match must occur including
        /// any prefix that is specified.
        /// </summary>
        public bool ArgumentPrefixOptional { get; set; }

        /// <summary>
        /// ProcessArguments will process the command line arguments and pass the values into the class passed as argumentVals, assuming
        /// that the argumentVals class has been correctly decorated with attributes to identify which of the arguments belong to which
        /// of the fields and properties in the class.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Thrown when argumentVals is null</exception>
        /// <exception cref="System.ArgumentException">Thrown when the argumentVals class has no atrribute specified or has no fields.</exception>
        /// <param name="argumentValuesClassInstance">The class representing the arguments, decorated by CmdLineArgAttributes</param>
        /// <param name="arguments">The string list of arguments passed to the application.</param>
        public void ProcessArguments(object argumentValuesClassInstance, string[] arguments) {
            b.Info.E();
            try {

                #region entry code

                if (argumentValuesClassInstance == null) { throw new ArgumentNullException("argumentValuesClassInstance", "The argumentVals class can not be null for a call to ProcessArguments"); }
                if (arguments.Length == 0) { return; }

                // validate that the first parameter has the CommandLineArgumentsAttribute set.
                object[] ats = argumentValuesClassInstance.GetType().GetCustomAttributes(typeof(CommandLineArgumentsAttribute), true);
                bool cmdLineArgsAttFound = false;
                if (ats.Length > 0) {
                    foreach (object o in ats) {
                        if (o is CommandLineArgumentsAttribute) { cmdLineArgsAttFound = true; break; }
                    }
                }

                if (!cmdLineArgsAttFound) { throw new ArgumentException("The argumentVals class must have CommandLineArgumentsAttribute specified", "argumentValuesClassInstance"); }

                #endregion

                b.Info.Log("ProcessArguments about to process all passed arguments.");
                b.Verbose.Log("Prefix:" + argumentPrefix, "PostFix:" + ArgumentPostfix);
                argumentErrorsDuringLastParse.Clear();

                Type argumentClass = argumentValuesClassInstance.GetType();
                List<MemberInfo> allMembersThatCanBeUpdated = GetMembersFromArgumentClassAndVerify(argumentClass);

                // We now have a list of all of the fields that we are expecting to find command line argument
                // information on.  We run through this trying to find which argument for which field.
                List<FieldArgumentMapping> mappingsOfFieldsToArguments = new List<FieldArgumentMapping>();

                PopulateFieldMappings(allMembersThatCanBeUpdated, mappingsOfFieldsToArguments);

                // We now have a series of mappings established.
                b.Info.Log("Mappings established, there are " + mappingsOfFieldsToArguments.Count.ToString() + " mappings");
                b.Info.Dump(mappingsOfFieldsToArguments, "Array of mappigns");
                List<string> unmatchedParameters = new List<string>();

                foreach (string individualArgument in arguments) {
                    bool matchOccuredForThisArgument = false;  // Determines whether any match occured for this argument

                    foreach (FieldArgumentMapping singleArgumentMapping in mappingsOfFieldsToArguments) {
                        if (singleArgumentMapping.MatchArgumentToField(individualArgument)) {
                            matchOccuredForThisArgument = true;
                            b.Verbose.Log("Match discovered for " + singleArgumentMapping.TargetField.Name + " trying to assign value now ", " using value " + individualArgument);

                            string convertedArgValue = ConvertArgumentToRemovePostfixes(singleArgumentMapping.LastArgumentValue());

                            try {
                                AssignValueToMember(singleArgumentMapping.TargetField, argumentValuesClassInstance, convertedArgValue);
                            } catch (ArgumentException aex) {
                                string errorText = aex.Message;
                                if (aex.InnerException != null) { errorText = aex.InnerException.Message; }
                                argumentErrorsDuringLastParse.Add(singleArgumentMapping.ParameterMatches.First() + " Error (" + errorText + ")");
                                b.Info.Dump(aex, "ArgumentException while trying to match the value for field " + singleArgumentMapping.FieldArgumentMappingDebug());
                                singleArgumentMapping.HasBeenMatchedToArgument = false;
                            }
                            break;
                        }
                    }

                    if (!matchOccuredForThisArgument) {
                        unmatchedParameters.Add(individualArgument);
                    }
                }

                // Now we have gone through all of the arguments and seen if we can match them to any of the mappings that were
                // established we look to see what the user wants to do with the unmatched arguments.  If they want them kept
                // then there should be a default field.
                foreach (FieldArgumentMapping fam in mappingsOfFieldsToArguments) {
                    if (fam.MatchesAllUnmatchedArguments) {
                        CommandArgumentSupport.AssignValueToMember(fam.TargetField, argumentValuesClassInstance, unmatchedParameters.ToArray());
                        break;
                    }
                }
            } finally {
                b.Info.X();
            }
        }

        private List<MemberInfo> GetMembersFromArgumentClassAndVerify(Type argumentClass) {
            FieldInfo[] allFieldsFromClass = argumentClass.GetFields();
            PropertyInfo[] allPropertiesFromClass = argumentClass.GetProperties();

            List<MemberInfo> result = new List<MemberInfo>();
            result.AddRange(allFieldsFromClass);

            foreach (var propertyFound in allPropertiesFromClass) {
                if (propertyFound.CanWrite) {
                    result.Add(propertyFound);
                }
            }

            if (result.Count == 0) {
                b.Warning.Log("Performed GetFields and GetProperties(Property|SetProperty) on the class, returned nothing writable.");
                throw new ArgumentException("The argumentVals class does not appear to have any Fields or Writable Properties available for Population");
            }
            return result;
        }

        /// <summary>
        /// This is the default assignment which attepmts to put the value in as an object
        /// </summary>
        /// <param name="theMember">The field on the object to set</param>
        /// <param name="theObject">The object containing the field</param>
        /// <param name="theValue">The value to set it to.</param>
        private void AssignValueToMember(MemberInfo theMember, object theObject, string theValue) {
            if (theMember.MemberType == MemberTypes.Field) {
                AssignValueToField((FieldInfo)theMember, theObject, theValue);
            } else {
                AssignValueToProperty((PropertyInfo)theMember, theObject, theValue);
            }
        }

        private static void AssignValueToMember(MemberInfo theMember, object theObject, object theValue) {
            if (theMember.MemberType == MemberTypes.Field) {
                AssignValueToField((FieldInfo)theMember, theObject, theValue);
            } else {
                AssignValueToProperty((PropertyInfo)theMember, theObject, theValue);
            }
        }

        private  void AssignValueToProperty(PropertyInfo prop, object theObject, string theValue) {
            #region entry code

            if (theValue == null) { theValue = string.Empty; }
            if (!prop.CanWrite) { throw new ArithmeticException("The property must be writable"); }

            #endregion

            try {
                b.Verbose.Log("AssignValueToField called for property type " + prop.ToString() + " assigning value " + theValue);

                if (prop.PropertyType == typeof(bool)) {
                    bool tbool = StringToBool(theValue);             // Booleans also support y and n as well as true and false
                    prop.SetValue(theObject, tbool, null);
                    return;
                }

                if (prop.PropertyType == typeof(int)) {
                    int tint = int.Parse(theValue);
                    prop.SetValue(theObject, tint, null);
                    return;
                }

                if (prop.PropertyType == typeof(long)) {
                    long tlong = long.Parse(theValue);
                    prop.SetValue(theObject, tlong, null);
                    return;
                }

                b.Verbose.Log("No special type identified for the field, going to try string assignment");
                prop.SetValue(theObject, theValue, null);
            } catch (OverflowException ofx) {
                b.Warning.Log("Error parsing the argument that was passed, unable to convert the data into a boolean.");
                throw new ArgumentException("The value could not be parsed to a " + prop.PropertyType.ToString(), "theValue", ofx);
            } catch (FormatException fex) {
                // Do this to make it consistant with the error from SetValue
                b.Warning.Log("Error parsing the argument that was passed, unable to convert the data into a boolean.");
                throw new ArgumentException("The value could not be parsed to a " + prop.PropertyType.ToString(), "theValue", fex);
            }
        }

        private static void AssignValueToProperty(PropertyInfo prop, object theObject, object theValue) {
            prop.SetValue(theObject, theValue, null);
        }

        private static void AssignValueToField(FieldInfo theField, object theObject, object theValue) {
            theField.SetValue(theObject, theValue);
        }

        /// <summary>
        /// Assigns a value which is specified in a string to a reflected field, parsing the value into the type of the field that is expected, if the
        /// value in the string can not be parsed into the value that the field is expecting correctly then an ArgumentException is thrown.
        /// </summary>
        /// <remarks>If the FieldInfo type is boolean then the string value supports True/Yes/Y/T as values.</remarks>
        /// <remarks>This method should not be called with theValue parameter being null, if it is theValue is converted to an empty string</remarks>
        /// <exception cref="System.ArgumentException">Thrown if the value can not be assigned to the field</exception>
        /// <param name="theField">The reflected FieldInfo type describing the field that is to be filled/</param>
        /// <param name="theObject">The object which is to have the value passed into it</param>
        /// <param name="theValue">The string representation of the value</param>
        private void AssignValueToField(FieldInfo theField, object theObject, string theValue) {

            #region entry code

            if (theValue == null) { theValue = string.Empty; }

            #endregion

            try {
                b.Verbose.Log("AssignValueToField called for field type " + theField.ToString() + " assigning value " + theValue);

                // If the target field is a boolean we also support y and yes for true values.
                if (theField.FieldType == typeof(bool)) {
                    bool tbool = StringToBool(theValue);
                    theField.SetValue(theObject, tbool);
                    return;
                }

                // If the target field is an integer....
                if (theField.FieldType == typeof(int)) {
                    int tint = int.Parse(theValue);
                    theField.SetValue(theObject, tint);
                    return;
                }

                // if the target field is a long
                if (theField.FieldType == typeof(long)) {
                    long tlong = long.Parse(theValue);
                    theField.SetValue(theObject, tlong);
                    return;
                }

                b.Verbose.Log("No special type identified for the field, going to try string assignment");
                theField.SetValue(theObject, theValue);
            } catch (OverflowException ofx) {
                b.Warning.Log("Error parsing the argument that was passed, unable to convert the data into a boolean.");
                throw new ArgumentException("The value could not be parsed to a " + theField.FieldType.ToString(), "theValue", ofx);
            } catch (FormatException fex) {
                // Do this to make it consistant with the error from SetValue
                b.Warning.Log("Error parsing the argument that was passed, unable to convert the data into a boolean.");
                throw new ArgumentException("The value could not be parsed to a " + theField.FieldType.ToString(), "theValue", fex);
            }
        }

        private static bool StringToBool(string theValue) {
            bool tbool = false;

            string argWorkingString = theValue.ToLower();
            if ((argWorkingString == "y") || (argWorkingString == "yes") || (argWorkingString == "t")) {
                tbool = true;
            } else if ((argWorkingString == "n") || (argWorkingString == "no" || (argWorkingString == "f"))) {
                tbool = false;
            } else {
                tbool = bool.Parse(theValue);
            }
            return tbool;
        }

        /// <summary>
        /// Massage the argument values to remove the argument postfix from them if it is specified.
        /// </summary>
        /// <param name="arg">The argument values themselves</param>
        /// <returns>The argument values minus any postfix.</returns>
        private string ConvertArgumentToRemovePostfixes(string arg) {
            if ((ArgumentPostfix != null) && (ArgumentPostfix.Length > 0) && (arg.StartsWith(ArgumentPostfix))) {
                return arg.Substring(ArgumentPostfix.Length);
            }
            return arg;
        }

        /// <summary>
        /// PopulateFieldMappings populates a list of FieldArgumentMappings which describes how arguments are matched to the fields of the object
        /// that is being populated.  This method runs through the fieldInfos that are provided and checks each for instances of the
        /// cmdLineArgument attributes.  It then creates a FieldArgumentMapping for each which descirbes the parameters and any defaults that
        /// are associated with the field.  This list of mappings is used later to populate the values.
        /// </summary>
        /// <param name="members">Array of FieldInfo values taken from the object that is to be populated</param>
        /// <param name="fim">List of FieldArgumentMappings which is populated representing the mappings to fieldInfos</param>
        private void PopulateFieldMappings(IEnumerable<MemberInfo> members, List<FieldArgumentMapping> fim) {

            #region entry code

            // TODO Bilge.Assert(members != null, "The array of fieldInfos can not be null");
            // TODO Bilge.Assert(fim != null, "the list of fieldArgumentMappings can not be null");

            #endregion

            // Look at each of the fields in the class in turn, identifying the attributes and using them to determine how to
            // map parameters to the values.
            foreach (MemberInfo f in members) {
                // Each field in the class will be mapped to a FieldArgumenMapping allowing us to describe how tof ill it.
                FieldArgumentMapping nextMapping = new FieldArgumentMapping(b);
                nextMapping.TargetField = f;

                CommandLineArgumentBaseAttribute[] custAttribs = (CommandLineArgumentBaseAttribute[])f.GetCustomAttributes(typeof(CommandLineArgumentBaseAttribute), true);
                foreach (CommandLineArgumentBaseAttribute claba in custAttribs) {
                    // TODO Bilge.Assert(claba.Description != null, "The description should be empty or full, never null");
                    // TODO Bilge.Assert(claba.FullDescription != null, "The description should be empty or full, never null");

                    nextMapping.ShortDescription = claba.Description;
                    nextMapping.LongDescription = claba.FullDescription;

                    if (claba is CommandLineArgAttribute argAtt) {
                        // TODO Bilge.Assert(argumentPrefix != null, "The argument prefix should be empty if it is not required");

                        nextMapping.IsDefaultSingleArgument = argAtt.IsSingleParameterDefault;

                        if (this.argumentPrefix.Length > 0) {
                            // there is an argument prefix therefore add this to the argument.
                            nextMapping.AddParameterMatch(this.argumentPrefix + argAtt.ArgumentIdentifier);
                            if (this.ArgumentPrefixOptional) {
                                nextMapping.AddParameterMatch(argAtt.ArgumentIdentifier);
                            }
                        } else {
                            // No Argument prefix therefore just add it in verbatum
                            nextMapping.AddParameterMatch(argAtt.ArgumentIdentifier);
                        }
                    } else {
                        // TODO Bilge.Assert(claba is CommandLineArgDefaultAttribute, "Only the CmdLineArgAttribute or cmdLineArgDefaultAttribute classes are supported");
                        // In this case we have found the default field.  This is used to fill up with any parameters that are not matched
                        // and therefore MUST be a List<string>
                        nextMapping.MatchesAllUnmatchedArguments = true;
                    }
                }

                fim.Add(nextMapping);
            }
        }

        /// <summary>
        /// Generates the short form of help which is typically shown when the program is called with no arguments.  The short help is generated
        /// by specifying the Description attribute on the decoration of the command line arguments class.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Thrown when ArgumentVals is null</exception>
        /// <exception cref="System.ArgumentException">Thrown when no fields are found.</exception>
        /// <param name="argumentValues">The class with the [CommandLineArguments] attribute used to generate the help.</param>
        /// <param name="appName">The application name to write out into the help.</param>
        /// <returns>A string of short form comments, with newlines in to format correctly.</returns>
        public string GenerateShortHelp(object argumentValues, string appName) {
            b.Info.E();
            try {

                #region entry code

                if (argumentValues == null) { throw new ArgumentNullException("argumentValues", "The argumentVals class can not be null for a call to ProcessArguments"); }

                // validate that the first parameter has the CommandLineArgumentsAttribute set.
                object[] ats = argumentValues.GetType().GetCustomAttributes(typeof(CommandLineArgumentsAttribute), true);
                bool cmdLineArgsAttFound = false;
                if (ats.Length > 0) {
                    foreach (object o in ats) {
                        if (o is CommandLineArgumentsAttribute) { cmdLineArgsAttFound = true; break; }
                    }
                }

                if (!cmdLineArgsAttFound) { throw new ArgumentException("The argumentValues class must have CommandLineArgumentsAttribute specified", "argumentValues"); }

                #endregion

                b.Info.Log("Initial entry code passed, about to inspect the argument vals class");

                Type argumentClass = argumentValues.GetType();
                List<MemberInfo> membersToCheckForHelp = GetMembersFromArgumentClassAndVerify(argumentClass);

                // We now have a list of all of the fields that we are expecting to find command line argument
                // information on.  We run through this trying to find which argument for which field.
                List<FieldArgumentMapping> fams = new List<FieldArgumentMapping>();

                // Look at all of the arguments on each of the fields within the target class, this will allow us to map the arguments
                // to the parameters that are passed in.

                PopulateFieldMappings(membersToCheckForHelp, fams);

                StringBuilder sb = new StringBuilder();
                sb.Append("Parameter help for " + appName + Environment.NewLine + Environment.NewLine);
                sb.Append(appName + " ");

                foreach (FieldArgumentMapping fam in fams) {
                    if (fam.ParameterMatchesCount > 0) {
                        sb.Append(fam.ParameterMatches.First() + " ");
                    }
                }
                sb.Append(Environment.NewLine + Environment.NewLine);

                foreach (FieldArgumentMapping fam in fams) {
                    if (fam.ParameterMatchesCount > 0) {
                        sb.Append(fam.ParameterMatches.First() + " " + fam.ShortDescription + Environment.NewLine);
                    }
                }

                return sb.ToString();
            } finally {
                b.Info.X();
            }
        }

        /// <summary>
        /// Generates the longer form of help which is typically shown when the program is called with no arguments.  The FullDescription
        /// text is used for outputting the content of this help screen.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Thrown when ArgumentVals is null</exception>
        /// <exception cref="System.ArgumentException">Thrown when the argumentVals has no attributes or no fields.</exception>
        /// <param name="commandLineArgumentClass">The class with the [CommandLineArguments] attribute used to generate the help.</param>
        /// <param name="appName">The application name</param>
        /// <returns>A string of short form comments</returns>
        public string GenerateHelp(object commandLineArgumentClass, string appName) {
            b.Info.E();
            try {

                #region entry code

                if (commandLineArgumentClass == null) { throw new ArgumentNullException("commandLineArgumentClass", "The argumentVals class can not be null for a call to ProcessArguments"); }

                // validate that the first parameter has the CommandLineArgumentsAttribute set.
                object[] ats = commandLineArgumentClass.GetType().GetCustomAttributes(typeof(CommandLineArgumentsAttribute), true);
                bool cmdLineArgsAttFound = false;
                if (ats.Length > 0) {
                    foreach (object o in ats) {
                        if (o is CommandLineArgumentsAttribute) { cmdLineArgsAttFound = true; break; }
                    }
                }

                if (!cmdLineArgsAttFound) { throw new ArgumentException("The argumentVals class must have CommandLineArgumentsAttribute specified", "commandLineArgumentClass"); }

                #endregion

                b.Info.Log("Initial entry code passed, about to inspect the argument vals class");

                Type argumentClass = commandLineArgumentClass.GetType();

                List<MemberInfo> getMembersToPopulate = GetMembersFromArgumentClassAndVerify(argumentClass);

                // We now have a list of all of the fields that we are expecting to find command line argument
                // information on.  We run through this trying to find which argument for which field.
                List<FieldArgumentMapping> fams = new List<FieldArgumentMapping>();

                // Look at all of the arguments on each of the fields within the target class, this will allow us to map the arguments
                // to the parameters that are passed in.

                PopulateFieldMappings(getMembersToPopulate, fams);

                StringBuilder sb = new StringBuilder();
                sb.Append("Parameter help for " + appName + Environment.NewLine + Environment.NewLine);
                sb.Append(appName + " ");

                foreach (FieldArgumentMapping fam in fams) {
                    if (fam.ParameterMatchesCount > 0) {
                        sb.Append(fam.ParameterMatches.First() + " ");
                    }
                }
                sb.Append(Environment.NewLine + Environment.NewLine);

                foreach (FieldArgumentMapping fam in fams) {
                    if (fam.ParameterMatchesCount > 0) {
                        sb.Append(fam.ParameterMatches.First() + " " + fam.LongDescription + Environment.NewLine);
                    }
                }

                return sb.ToString();
            } finally {
                b.Info.X();
            }
        }
    }
}