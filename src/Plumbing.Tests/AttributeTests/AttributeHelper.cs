using System.Diagnostics;

namespace Plisky.PliskyLibTests.AttributeTests {

    public static class AttributeHelper {

        public static T FindAttributeOnMethod<T>() {
            var f = new StackTrace(1, false);
            foreach (var stackFramesToCheck in f.GetFrames()) {
                object[] matchingTimeSavedAttributes = stackFramesToCheck.GetMethod().GetCustomAttributes(typeof(T), true);

                if (matchingTimeSavedAttributes.Length > 0) {
                    return ((T)matchingTimeSavedAttributes[0]);
                }
            }
            return default;
        }

        /// <summary>
        /// Finds a matching attribute up the call stack, finding the attribute on a method first then if not on the method
        /// on the declaring class.  Will stop at the first method or class that has an instance of the type.  Only returns
        /// the first one.
        /// </summary>
        /// <typeparam name="T">The type of attribute to find</typeparam>
        /// <returns>The matching attribute</returns>
        public static T FindAttributeOnMethodOrClass<T>() {
            var f = new StackTrace(1, false);
            foreach (var stackFramesToCheck in f.GetFrames()) {
                var nextMethod = stackFramesToCheck.GetMethod();

                object[] attributeMatches = nextMethod.GetCustomAttributes(typeof(T), true);
                if (attributeMatches.Length == 0) {
                    attributeMatches = nextMethod.DeclaringType.GetCustomAttributes(typeof(T), true);
                }

                if (attributeMatches.Length > 0) {
                    return ((T)attributeMatches[0]);
                }
            }
            return default;
        }
    }
}