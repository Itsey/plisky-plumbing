namespace Plisky.Helpers {
    using Plisky.Diagnostics;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;

    /// <summary>
    /// This class is used by the command line support to map fields within objects to command line arguments.  It is used internally
    /// by the command line arguments class to actually hold a field and a mapping and to set the value to the field for a specific
    /// command line argument.
    /// </summary>
    [DebuggerDisplay("{FieldArgumentMappingDebug()}")]
    internal class FieldArgumentMapping {
        Bilge b;
        private List<string> parameterMatches = new List<string>();

        /// <summary>
        /// The target reflected field that is to be populated if there is a match.
        /// </summary>
        public MemberInfo TargetField;

        /// <summary>
        /// The list of parameter match forms that should be checked against.  This should be a full list with
        /// any relevant prefixes applied.
        /// </summary>
        internal IEnumerable<string> ParameterMatches {
            get {
                return parameterMatches;
            }
        }

        internal int ParameterMatchesCount {
            get { return parameterMatches.Count; }
        }

        private int CompareStringOnLength(string first, string second) {
            return second.Length - first.Length;
        }

        internal void AddParameterMatch(string nextParameterMatch) {
            parameterMatches.Add(nextParameterMatch);
            if (parameterMatches.Count > 1) {
                parameterMatches.Sort(new Comparison<string>(CompareStringOnLength));
            }
        }

        /// <summary>
        /// Determines whether this field has already been matched and therefoer should not be checked again.
        /// </summary>
        internal bool HasBeenMatchedToArgument; 
        

        /// <summary>
        /// This is the short description which describes the parameter.  This should be around 50 characters in length to allow it to fit on
        /// the screen when /? is passed to the command line application.
        /// </summary>
        internal string ShortDescription = string.Empty;

        /// <summary>
        /// This is the full help string that will be printed when a full help request is made.  This should be descriptive and contain
        /// as much information descriving the paramter as possible.
        /// </summary>
        internal string LongDescription = string.Empty;

        // TODO : Awful awful design
        private string m_lastArgVal;

        internal string LastArgumentValue() {
            return m_lastArgVal;
        }

        /// <summary>
        /// If the mapping represents the default match case then all of the arguments that have not been assigned to the other matches
        /// will be put into
        /// </summary>
        internal bool MatchesAllUnmatchedArguments; 

        /// <summary>
        /// Determines whether this parameter is the match for a single unnamed argument, this alows you to pass across filenames and so on
        /// without a prefix.  Once this has been matched to the first non prefixed arguments then all of the rest will go into the
        /// unmatched store.
        /// </summary>
        internal bool IsDefaultSingleArgument;  

        /// <summary>
        /// Method attempts to match an argument passed in against the field parameterMatches that have been specified.  If no
        /// parameter matches have been specified this method does noting.
        /// </summary>
        /// <remarks> Specify both parameterMatches and TargetField before calling this method.</remarks>
        /// <param name="argument"></param>
        /// <returns></returns>
        internal bool MatchArgumentToField(string argument,string activePostfix) {

            #region entry code

            if (argument == null) { throw new ArgumentNullException("argument", "The argument parameter can not be null, MatchArgumentToField should not be called with null arguments"); }
            if (argument.Length == 0) { return false; }
            if (TargetField == null) { throw new InvalidOperationException("The TargetField member must be set before calling MatchArgumentToField"); }
            if (ParameterMatchesCount == 0) {
                b.Warning.Log("No parameterMatches have been specified.  The arguments will never match against this field.");
                return false;
            }

            #endregion

            if (HasBeenMatchedToArgument) { return false; }  

            //Parameter matches must be sorted longest first....

            // Check each of the possible parameter matches and if one is found then assign the value to the field and report
            // that the match has been made.  Case must be handled outside of this method.
            foreach (string s in ParameterMatches) {

                string thingToCompare = s.ToLowerInvariant();
                string argumentText = argument.ToLowerInvariant();

                if ((!string.IsNullOrEmpty(activePostfix) && (argument.IndexOf(activePostfix)>=0)) {
                    // The postfix indicates the lenght of the argument.
                    argumentText = argument.Substring(0, argument.IndexOf(activePostfix)).ToLowerInvariant();
                } else {
                    if (argument.Length >= s.Length) {
                        argumentText = argument.Substring(0, s.Length).ToLowerInvariant();
                    }
                }
               
                
                if (string.CompareOrdinal(argumentText,thingToCompare)==0) { 
                    // Match made on the argument.

                    m_lastArgVal = argument.Substring(s.Length);

                    if (TargetField.MemberType == MemberTypes.Field) {
                        FieldInfo fi = (FieldInfo)TargetField;
                        if ((fi.FieldType == typeof(Boolean)) && (m_lastArgVal.Length == 0)) {
                            // This allows things like /Y to specify true and the absence of it to specify false.
                            m_lastArgVal = "True";
                        }
                        HasBeenMatchedToArgument = true;
                        return true;
                    } else {
                        // TODO Bilge.Assert(TargetField is PropertyInfo, "The target fields should all be FieldInfo or PropertyInfo types");

                        PropertyInfo pi = (PropertyInfo)TargetField;
                        if ((pi.PropertyType == typeof(Boolean)) && (m_lastArgVal.Length == 0)) {
                            // This allows things like /Y to specify true and the absence of it to specify false.
                            m_lastArgVal = "True";
                        }
                        HasBeenMatchedToArgument = true;
                        return true;
                    }
                } else {
                    if (IsDefaultSingleArgument) {
                        // If this represents the default single argument then we should match against it.
                        m_lastArgVal = argument;
                        HasBeenMatchedToArgument = true;
                        return true;
                    }
                }
            }

            return false;
        }

        
        internal FieldArgumentMapping(Bilge logger) {
            parameterMatches.Clear();
            b = logger;
        }

        internal string FieldArgumentMappingDebug() {
            string debugStr = "FieldArgMap:";
            if (this.HasBeenMatchedToArgument) {
                debugStr += "(matched):";
            } else {
                debugStr += "(notmatc):";
            }
            debugStr += TargetField.Name ?? "NoField";
            debugStr += "  Maps:" + ParameterMatchesCount.ToString() + " for " + ShortDescription;

            return debugStr;
        }

        public override string ToString() {
            return FieldArgumentMappingDebug();
        }
    }
}