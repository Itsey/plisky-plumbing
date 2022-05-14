namespace Plisky.Test {
    using Plisky.Diagnostics;
    using Plisky.Plumbing;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;


    /// <summary>
    /// Class designed to support the adoption of unit tests and provide helper functions for typical common elements.
    /// </summary>
    public sealed class UnitTestHelper {
        private List<string> m_storedFilenames = new List<string>();
        private Bilge b = new Bilge("Plisky-UnitTestHelper");
        
        /// <summary>
        /// Creates a new instance of the UnitTestHelper class
        /// </summary>
        public UnitTestHelper() {
          
            CaseSensitiveMatches = false;
        }

        ~UnitTestHelper() {
            ClearUpTestFiles();
        }

        /// <summary>
        /// GetTestDataFile, returns a test data file stored to the file system and read from the referenced assembly.  To use this pass in the partial name of the
        /// embedded resource and a partial name of a referenced assembly of the current appdomain.  This defaults to "TestData", therefore if you create a project
        /// called TestData, reference it in your unit test project and load a class from it the match will work.
        /// </summary>
        /// <param name="partialRefName">The partial name of the manifest reference to load as a file.</param>
        /// <param name="assemblyName">The partial name of the assembly where the manifest lives, defaults TestData</param>
        /// <returns>The contents of the resource file</returns>
        public string GetTestDataFromFile(string partialRefName, string assemblyName = "TestData") {

            #region parameter validation
            if (string.IsNullOrWhiteSpace(partialRefName)) {
                throw new ArgumentOutOfRangeException(nameof(partialRefName), "The resource name must be specified, at least partially, to retrieve it.");
            }

            if (string.IsNullOrWhiteSpace(assemblyName)) {
                throw new ArgumentOutOfRangeException(nameof(assemblyName), "The assembly name should be defaulted or specified, it can not be empty.");
            }
            #endregion

            assemblyName = assemblyName.ToLowerInvariant();


            b.Info.Log($"GetTestDataFile >> Finding ({assemblyName})  reference {partialRefName}");
            
            var matchedTestData = GetMatchedTestDataFromAssembly(assemblyName);
          
            if (matchedTestData == null) {
                b.Warning.Log("Unable to Match assembly, Exception being thrown");
                throw new InvalidOperationException($"No referenced assembly {assemblyName}.  Did you reference and use the assembly in the test project?");
            }

            string resMatched = null;
            string[] allNames = matchedTestData.GetManifestResourceNames();
            foreach (string x in allNames) {
                b.Verbose.Log($"Looking for matched Resource {x}");
                if (x.Contains(partialRefName)) {
                    resMatched = x;
                }
            }

            if (string.IsNullOrEmpty(resMatched)) {
                b.Warning.Log("No Res Matched, Exception");
                throw new InvalidOperationException("Unable to find the resource");
            }

            string result;
            using (var stream = matchedTestData.GetManifestResourceStream(resMatched)) {
                if (stream != null) {
                    var reader = new StreamReader(stream);
                    result = reader.ReadToEnd();                    
                } else {
                    b.Warning.Log("Could not find stream, failing to update the file. Exception being thrown");
                    throw new InvalidOperationException("The stream could not be read from the matched resource. No data.");
                }
            }
            return result;
        }

        /// <summary>
        /// GetTestDataFile, returns a test data file stored to the file system and read from the referenced assembly.  To use this pass in the partial name of the
        /// embedded resource and a partial name of a referenced assembly of the current appdomain.  This defaults to "TestData", therefore if you create a project
        /// called TestData, reference it in your unit test project and load a class from it the match will work.
        /// </summary>
        /// <param name="partialRefName">The partial name of the manifest reference to load as a file.</param>
        /// <param name="assemblyName">The partial name of the assembly where the manifest lives, defaults TestData</param>
        /// <returns>A temporary filename with the contents of the resource file</returns>
        public string GetTestDataFile(string partialRefName, string assemblyName = "TestData", string forceFilename=null) {
         
            string rd = GetTestDataFromFile(partialRefName, assemblyName);

            string fname = NewTemporaryFileName(true);

            if (forceFilename != null) {
                string pth = Path.GetDirectoryName(fname);
                fname = Path.Combine(pth, forceFilename);
                RegisterTemporaryFilename(fname);
            }
            File.WriteAllText(fname, rd);

            b.Verbose.Log($"Returns {fname}");
            return fname;
        }

        //Assembly name is already tolower.
        private Assembly GetMatchedTestDataFromAssembly(string assemblyName) {
            #region constants
            const string MS_ASM_NAMEPREFIX = "microsoft";
            const string SYSTEM_ASM_NAMEPREFIX = "system";
            const string XUNIT_ASM_NAMEPREFIX = "xunit";
            #endregion
            
            var refAsms = AppDomain.CurrentDomain.GetAssemblies(); 

            foreach (var f in refAsms) {
                string str = f.FullName.ToLower();

                if ((str.StartsWith(MS_ASM_NAMEPREFIX)) || (str.StartsWith(SYSTEM_ASM_NAMEPREFIX)) || (str.StartsWith(XUNIT_ASM_NAMEPREFIX))) {
                    b.Verbose.Log($"Skipping Well Known ASM {str}");
                    continue;
                }


                if (str.Contains(assemblyName)) {
                    return f;                    
                }
            }

            return null;
        }

        /// <summary>
        /// Gets or Sets a value which determines whether the string comparisons performed within the helper are case insensitive or not.
        /// </summary>
        /// <remarks>Defaults to false, making all matches insensitive</remarks>
        public bool CaseSensitiveMatches { get; set; }

        

        /// <summary>
        /// Returns a TemporaryFilename which can be used for storing data during unit tests.  This filename is stored within
        /// the UnitTestHelper class such that it can be cleaned up with a call to ClearUpTestFiles.
        /// </summary>
        /// <returns></returns>
        public string NewTemporaryFileName(bool deleteOnCreate = false) {
            string result = Path.GetTempFileName();
            m_storedFilenames.Add(result);
            if (deleteOnCreate) {
                File.Delete(result);
            }
            return result;
        }

        public void RegisterTemporaryFilename(string tmp) {
            m_storedFilenames.Add(tmp);
        }

        /// <summary>
        /// Compares two objects to see if they are alike, very simple implementation for top levle only at the moment, should
        /// be expanded to use recursion to compare any objects
        /// </summary>
        /// <param name="target1">First object for comparison</param>
        /// <param name="target2">Second object for comparison</param>
        /// <returns>True if they are the same</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "This confuses the interface considerably and its only a unit test helper therefore performance not important")]
        public bool ReflectionBasedCompare(object target1, object target2) {
            // Either they are both null or neither are
            if ((target1 == null) && (target2 == null)) { return true; }
            if ((target1 == null) && (target2 != null)) { return false; }
            if ((target2 == null) && (target1 != null)) { return false; }
            // Now they must be of the same type
            var t = target1.GetType();
            if (t != target2.GetType()) { return false; }

            if (target1.GetType().IsArray) {
                return CompareArrays((Array)target1, (Array)target2);
            }

            if (target1.GetType().IsValueType) {
                return target1.Equals(target2);
            }

            if (target1.GetType() == typeof(string)) {
                return target1.Equals(target2);
            }

            foreach (var pi in t.GetProperties()) {
                if (pi.GetIndexParameters().Length == 0) {
                    // Non indexed properties

                    object v1 = pi.GetValue(target1, null);
                    object v2 = pi.GetValue(target2, null);
                    // If either one of them is null then they must both be null
                    if ((v1 == null) || (v2 == null)) {
                        if ((v1 == null) && (v2 != null)) { return false; }
                        if ((v2 == null) && (v1 != null)) { return false; }
                    } else {
                        if (ReflectionBasedCompare(v1, v2) != true) {
                            return false;
                        }
                    }
                } else {
                    // This is for indexed properties.
                    b.Info.Log("Skipping indexed property");
                }
            }

            foreach (var fi in t.GetFields()) {
                object v1 = fi.GetValue(target1);
                object v2 = fi.GetValue(target2);

                if ((v1 == null) || (v2 == null)) {
                    if ((v1 == null) && (v2 != null)) { return false; }
                    if ((v2 == null) && (v1 != null)) { return false; }
                } else {
                    if (v1.GetType().IsArray) {
                        if (!v2.GetType().IsArray) {
                            return false;
                        }

                        // This is not a very comprehensive test for equality, if they are arrays and they
                        // are the same length then we guess they are the same.
                    } else {
                        if (v1.Equals(v2) == false) {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private static bool CompareArrays(Array o1, Array o2) {
            return o1.Length == o2.Length;
        }

        /// <summary>
        /// Designed to use reflection to work out the type of a value passed to it then alter that value to something different, aimed at
        /// changing values for unit tests.  All that can be said is that the return value will not equal the entry value for supported simple types.
        /// </summary>
        /// <remarks>If the val object is of an unsupported type it is returned unchanged.</remarks>
        /// <param name="target">The value to change</param>
        /// <returns>A new value for the object val</returns>
        public object AlterValue(object target) {
            var td = new SampleTestData();

            if (target == null) { return null; }

            var t = target.GetType();
            if (t == typeof(long)) {
                return (long)target + td.RandomStore.Next(5);
            }
            if (t == typeof(uint)) {
                return (uint)target + td.RandomStore.Next(5);
            }
            if (t == typeof(double)) {
                return (double)target + td.RandomStore.Next(5);
            }
            if (t == typeof(int)) {
                return (int)target + td.RandomStore.Next(5);
            }

            if (t == typeof(bool)) {
                return (!(bool)target);  // Invert it for bools
            }
            if (t == typeof(string)) {
                string s = td.GenerateFriendlyString();
                int length = td.RandomStore.Next(5);
                if (s.Length < length) {
                    length = s.Length;
                }
                return (string)target + s.Substring(0, length);
            }
            return target;
        }



        /// <summary>
        /// Attempts to delete all of the test files that are used.
        /// </summary>
        public void ClearUpTestFiles() {
            string nextFile = null;
            try {
                foreach (string s in m_storedFilenames) {
                    nextFile = s;
                    File.Delete(nextFile);
                }
            } catch (IOException iox) {
                b.Info.Dump(iox, "Exception trying to clear up file:" + nextFile);
            }
        }

        /// <summary>
        /// Changes all of the values on a type a little bit.  This will incrememt numbers, alter strings and try and change all of the
        /// values on any object that is passed in.  This is purely designed for testing to change fields.
        /// </summary>
        /// <param name="target">The object to alter.</param>
        public void AlterAllValuesOnType(object target) {
            var t = target.GetType();
            object val;
            object newOne;

            foreach (var pi in t.GetProperties()) {
                if (pi.CanWrite) {
                    val = pi.GetValue(target, null);
                    newOne = AlterValue(val);
                    pi.SetValue(target, newOne, null);
                }
            }
            foreach (var fi in t.GetFields()) {
                val = fi.GetValue(target);
                newOne = AlterValue(val);
                fi.SetValue(target, newOne);
            }
        }

        /// <summary>
        /// Verifies that a string contains all of the partial string matches that are passed in the required elements parameter.
        /// </summary>
        /// <param name="dataToTest">The string to check for all words</param>
        /// <param name="requiredElements">The array of words</param>
        /// <remarks>This function is not case sensitive.</remarks>
        /// <returns>True if all of the partial strings are found, false otherwise</returns>
        public bool StringContainsAll(string dataToTest, params string[] requiredElements) {
            for (int i = 0; i < requiredElements.Length; i++) {
                if (!StringContains(dataToTest, requiredElements[i])) {

                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Determines whether a string loosely conforms to a set of ideas, the ideas are passed as strings to the
        /// method and these strings must be mentioned and must be mentioned in order so that the gist holds true.
        /// If any of the strings are not present or if they are present in the wrong order then this returns false.
        /// </summary>
        /// <remarks>This function is not case sensitive.</remarks>
        /// <param name="valueToCheck">The name of the string to parse</param>
        /// <param name="gistElements">Each of the gist elements to check for</param>
        /// <returns>True if each is found in order, false otherwise.</returns>
        public static bool StringContainsAllInOrder(string valueToCheck, params string[] gistElements) {
            int indexOfLast = 0;

            valueToCheck = valueToCheck.ToUpper();
            for (int i = 0; i < gistElements.Length; i++) {
                string s = gistElements[i].ToUpper();

                int index = valueToCheck.IndexOf(s);
                if (index < indexOfLast) {
                    // This is the case if the match is not met or if its met out of order.
                    return false;
                }
                indexOfLast = index;
            }
            return true;
        }

        /// <summary>
        /// Returns true if the specified filename is readonly.
        /// </summary>
        /// <param name="fileNameToCheck">The filename to check</param>
        /// <returns>True if the file is readonly.</returns>
        /// <exception cref="System.ArgumentException">Thrown if the filename is empty or null</exception>
        public static bool IsReadOnly(string fileNameToCheck) {
            if ((fileNameToCheck == null) || (fileNameToCheck.Length == 0)) {
                throw new ArgumentException("The fileName must be valid.", "fileNameToCheck");
            }
            var fa = File.GetAttributes(fileNameToCheck);
            return ((fa & FileAttributes.ReadOnly) == FileAttributes.ReadOnly);
        }

        private bool StringContains(string doesThis, string containThis) {
            if (this.CaseSensitiveMatches) {
                return doesThis.Contains(containThis);
            } else {
                return doesThis.ToLower().Contains(containThis.ToLower());
            }
        }



        /// <summary>
        /// Performs a match on matchData within searchData, checking each of the search strings to see if it contains an isntance
        /// of the match strings.  This method checks the matchData strings in the order specified in the array and will not return
        /// true if the matches occur out of order.
        /// </summary>
        /// <remarks>The match strings are matched only once, and all comparisons are case insensitive</remarks>
        /// <param name="searchData">The list of strings to check</param>
        /// <param name="matchData">The list of strings to find, in the specified order in the searchData strings</param>
        /// <returns>True if each of the matchData strings were found within the searchData strings in the correct order</returns>
        public bool SearchDataContainsMatchDataInOrder(string[] searchData, string[] matchData) {
            bool[] haveMatched = new bool[matchData.Length];
            bool[] haveConsumed = new bool[searchData.Length];

            int searchCount = 0;

            for (int matchCheckCount = 0; matchCheckCount < matchData.Length; matchCheckCount++) {
                if (haveMatched[matchCheckCount]) { continue; }

                for (; searchCount < searchData.Length; searchCount++) {
                    if (haveConsumed[searchCount]) { continue; }
                    if (StringContains(searchData[searchCount], matchData[matchCheckCount])) {
                        haveConsumed[searchCount] = true;
                        haveMatched[matchCheckCount] = true;
                        break;
                    }
                }
            }

            for (int i = 0; i < haveMatched.Length; i++) {
                if (haveMatched[i] == false) {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Performs a match on matchData within searchData, checking each of the search strings to see if it contains an isntance
        /// of the match strings.  This method checks the matchData strings and will return true if they are all found, no matter
        /// what the order of the matching is.
        /// </summary>
        /// <remarks>The match strings are matched only once, and all comparisons are based on the value of CaseSensiteMatches property </remarks>
        /// <param name="searchData">The list of strings to check</param>
        /// <param name="matchData">The list of strings to find</param>
        /// <returns>True if each of the matchData strings were found within the searchData strings in the correct order</returns>
        public bool SearchDataContainsMatchData(string[] searchData, string[] matchData) {
            bool[] haveMatched = new bool[matchData.Length];
            bool[] haveConsumed = new bool[searchData.Length];

            for (int matchCheckCount = 0; matchCheckCount < matchData.Length; matchCheckCount++) {
                if (haveMatched[matchCheckCount]) { continue; }

                for (int searchCount = 0; searchCount < searchData.Length; searchCount++) {
                    if (haveConsumed[searchCount]) { continue; }
                    if (StringContains(searchData[searchCount], matchData[matchCheckCount])) {
                        haveConsumed[searchCount] = true;
                        haveMatched[matchCheckCount] = true;
                        break;
                    }
                }
            }

            for (int i = 0; i < haveMatched.Length; i++) {
                if (haveMatched[i] == false) {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Reverses the case of a string passed, making all lower case letters upper case and all upper case letters lower case.
        /// </summary>
        /// <param name="data">The string to have its case reversed</param>
        /// <returns>The string with its case reversed.</returns>
        public static string ReverseCase(string data) {
            var result = new StringBuilder(data.Length);

            foreach (char c in data) {
                result.Append(char.IsUpper(c) ? char.ToLower(c) : char.ToUpper(c));
            }

            return result.ToString();
        }

#if false
        static object Process(object obj) {
            if (obj == null)
                return null;
            Type type = obj.GetType();
            if (type.IsValueType || type == typeof(string)) {
                return obj;
            } else if (type.IsArray) {
                Type elementType = Type.GetType(
                     type.FullName.Replace("[]", string.Empty));
                var array = obj as Array;
                Array copied = Array.CreateInstance(elementType, array.Length);
                for (int i = 0; i < array.Length; i++) {
                    copied.SetValue(Process(array.GetValue(i)), i);
                }
                return Convert.ChangeType(copied, obj.GetType());
            } else if (type.IsClass) {
                object toret = Activator.CreateInstance(obj.GetType());
                FieldInfo[] fields = type.GetFields(BindingFlags.Public |
                            BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (FieldInfo field in fields) {
                    object fieldValue = field.GetValue(obj);
                    if (fieldValue == null)
                        continue;
                    field.SetValue(toret, Process(fieldValue));
                }
                return toret;
            } else
                throw new ArgumentException("Unknown type");
        }
#endif

        private object CloneObjectImplementation(object source) {
            if (source == null) { return null; }

            var srcType = source.GetType();
            var flgs = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            object result;

            if (srcType == typeof(string)) {
                // String must be checked for ahead of class even though its less likely.
                return string.Copy(source as string);
            }

            if (srcType.IsArray) {
                //Again must check before clas.
                var arrayelementType = Type.GetTypeArray((object[])source)[0];
                var sauce = (Array)source;
                var newArray = Array.CreateInstance(arrayelementType, sauce.Length);
                for (int idx = 0; idx < newArray.Length; idx++) {
                    newArray.SetValue(CloneObjectImplementation(sauce.GetValue(idx)), idx);
                }
                return (object)newArray;  //Convert.ChangeType(newArray,srcType);
            }

            if (srcType.IsClass) {
                result = Activator.CreateInstance(srcType);
                foreach (var f in srcType.GetFields(flgs)) {
                    object actualField = f.GetValue(source);
                    if (actualField == null) {
                        f.SetValue(result, null);
                    } else {
                        f.SetValue(result, CloneObjectImplementation(actualField));
                    }
                }
                return result;
            }

            if (srcType.IsValueType) {
                return source;
            }

            throw new NotImplementedException();
        }

        public T CloneObject<T>(T source) {
            if (source == null) { throw new ArgumentNullException("source", "The source parameter can not be null when cloning."); }
            return (T)CloneObjectImplementation(source);
        }
    }
}