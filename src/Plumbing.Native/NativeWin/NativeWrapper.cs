using System;

namespace Plisky.Win32 {

    public static class NativeWrapper {

        /// <summary>
        /// Returns true if the user has an interactive desktop, false if no interactive desktop is associated with the user
        /// </summary>
        /// <returns>Bool indicating whether there is an interactive desktop for this user</returns>
        public static bool IsInteractiveUser() {
            bool returnMe = true;

            if (Environment.OSVersion.Platform == System.PlatformID.Win32NT) {
                // Only NT variants can support non interactive desktops.

                IntPtr hStation = NativeMethods.GetProcessWindowStation();

                uint neededSize = 0;
                byte[] myByteArray = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

                NativeMethods.GetUserObjectInformation(hStation, NativeMethods.UOI_FLAGS, myByteArray, 12, out neededSize);
                if (myByteArray[8] == 1) {   //Not nice bodge
                    returnMe = true;
                } else {
                    // non interfaced code
                    returnMe = false;  // error w/ GetProcessWindowStation
                }
            } else {
                // Not windows NT therefore only interactive OSes exist
                returnMe = true;
            }

            return returnMe;
        }

        /// <summary>
        /// This will take a single long string and return it as a series of truncated strings with the length that is
        /// specified in theLength parameter used to do the chopping up.  There is nothing clever or special about this
        /// routine it does not break on words or aynthing like that.
        /// </summary>
        /// <param name="theLongString">The string that is to be chopped up into smaller strings</param>
        /// <param name="theLength">The length at which the smaller strings are to be created</param>
        /// <returns></returns>
        public static string[] MakeManyStrings(string theLongString, int theLength) {

            #region entry code

            if (theLongString == null) { return null; }
            if (theLength <= 0) { throw new ArgumentException("theLength parameter cannot be <=0 for MakeManyStrings method"); }

            #endregion

            string[] result;

            if (theLongString.Length <= theLength) {
                // Special case where no splitting is necessary;
                result = new string[1];
                result[0] = theLongString;
                return result;
            }

            double exactNoChops = (double)((double)theLongString.Length / (double)theLength);
            int noChops = (int)Math.Ceiling(exactNoChops);

            result = new string[noChops];

            // All other cases where theLongString actually needs to be chopped up into smaller chunks
            int remainingChars = theLongString.Length;
            int currentOffset = 0;
            int currentChopCount = 0;
            while (remainingChars > theLength) {
                result[currentChopCount++] = theLongString.Substring(currentOffset, theLength);
                remainingChars -= theLength;
                currentOffset += theLength;
            }
            result[currentChopCount] = theLongString.Substring(currentOffset, remainingChars);

#if DEBUG
            if (currentChopCount != (noChops - 1)) {
                throw new NotSupportedException("This really should not happen");
            }
#endif

            return result;
        }
    }
}