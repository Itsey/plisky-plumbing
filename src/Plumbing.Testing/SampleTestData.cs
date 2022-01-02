using System;
using System.Collections.Generic;
using System.Text;

namespace Plisky.Test {
    public class SampleTestData {
        private readonly Random r = new Random();
        
        /// <summary>
        /// Provides access to a Random class stored within the unit test helper.  No benefit to using it over a normal one
        /// just saves having to create them or store them.
        /// </summary>
        public Random RandomStore {
            get {
                return r;
            }
        }


        /// <summary>
        /// Generic string used in unit testing, when any old token string will do.  This one is lower case with no spaces
        /// </summary>
        public const string GENERIC_STRING1 = "arflebarflegloop";

        /// <summary>
        /// Second Generic string used in unit testing, when any old token string will do. This one has spaces and is pascal cased.
        /// </summary>
        public const string GENERNIC_STRING2 = "Bilge And Flimflam";

        /// <summary>
        /// Third Generic string used in unit testing, when any old token string will do.  This is a sentence with a full stop.
        /// </summary>
        public const string GENERIC_STRING3 = "Spontralification of the spire.";



        #region hard coded data
        private string[] hardcodedUrls = new string[] {
            "https://www.example.com/bear/attraction.php",
        "http://bite.example.com/arm.php",
        "http://actor.example.org/bottle/action",
        "http://www.example.com/",
        "https://example.com/",
        "http://example.com/achiever.html?authority=authority&base=bat",
        "https://example.edu/attack",
        "http://www.example.org/agreement.html",
        "http://www.example.net/",
        "https://example.com/beds/blow",
        "https://www.example.org/",
        "http://www.example.com/appliance/bath.html",
        "http://www.example.com/bee.php#actor",
        "https://www.example.com/balance",
        "http://bite.example.net/arithmetic/breath",
        "http://www.example.org/",
        "https://example.com/bead.aspx#beef",
        "http://example.com/",
        "http://www.example.org/bedroom.html",
        "https://example.net/",
        "https://birds.example.com/amount",
        "http://books.example.com/boy/bear",
        "https://www.example.com/berry.php#back",
        "http://example.edu/advice/boy",
        "http://beginner.example.com/",
        "https://airplane.example.edu/brick/approval",
        "http://example.com/",
        "https://www.example.com/berry/baby.aspx",
        "http://angle.example.com/boy",
        "http://www.example.org/",
        "http://example.org/branch/anger?blood=basin&bell=ball",
        "https://www.example.com/",
        "https://example.com/",
        "https://www.example.com/",
        "https://example.edu/beef",
        "https://example.com/aunt",
        "http://example.com/bell?airplane=book",
        "https://afternoon.example.com/baby",
        "http://www.example.edu/",
        "https://example.com/action.html",
        "http://example.com/",
        "http://birds.example.com/",
        "https://www.example.com/approval.aspx",
        "http://example.com/actor",
        "http://www.example.com/account",
        "https://www.example.net/",
        "https://apparel.example.com/",
        "http://www.example.com/",
        "https://example.com/board/bells",
        "http://www.example.org/",
        "http://www.example.com/breath.html?blade=ball&afterthought=birthday",
        "http://apparel.example.com/",
        "https://www.example.com/",
        "https://example.com/",
        "https://bikes.example.com/bike/anger.htm",
        "https://example.com/#bubble",
        "http://www.example.com/",
        "http://example.com/advice/bottle.html?branch=believe",
        "https://example.com/babies.php",
        "http://www.example.com/"
        };
        #endregion
        public IEnumerable<string> GetTestURLs(int howMany=-1) {

            howMany = howMany < 0 ? hardcodedUrls.Length : howMany;            

            for(int i=0; i<howMany; i++) {
                yield return hardcodedUrls[r.Next(hardcodedUrls.Length - 1)];
            }
        }


        private int m_limitStringsTo = 2000;

        /// <summary>
        /// The maximum length which random strings are returned for any method which is called without
        /// specifying the maximum
        /// </summary>
        public int LimitStringsTo {
            get { return m_limitStringsTo; }
            set { m_limitStringsTo = value; }
        }

        /// <summary>
        /// Returns a generated filename safe string between the lengths of 3 and 30 characters.
        /// </summary>
        /// <returns>A random, filename friendly string between 3 and 30 characters long</returns>
        public string GenerateFriendlyString() {
            return GenerateSpecificRandomString(3, 30, false, true);
        }

        /// <summary>
        /// Generates a random string between the specified minimum and maximum lengths,
        /// </summary>
        /// <param name="minLength">The minimum length of the returned string, can be 0 or greater</param>
        /// <param name="maxLength">The maximum length of the returned string must be >= minLength</param>
        /// <param name="makeFileNameSafe">If true only filename safe characters are returned</param>
        /// <returns>A randomly generated string between minLength and maxLength characters in length.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if min length is less than zero or greater than max length</exception>
        public string GenerateString(int minLength, int maxLength, bool makeFileNameSafe) {
            if (makeFileNameSafe) {
                return GenerateSpecificRandomString(minLength, maxLength, false, true);
            } else {
                return GenerateSpecificRandomString(minLength, maxLength, false, true);
            }
        }

        public string GenerateString(int minLength, int maxLength, bool makeFileNameSafe, bool useNumbers) {
            return GenerateSpecificRandomString(minLength, maxLength, makeFileNameSafe, useNumbers);
        }

        private string GenerateSpecificRandomString(int minLength, int maxLength, bool allowPunctuation, bool allowNumbers) {

            #region entry code

            if (minLength < 0) {
                throw new ArgumentOutOfRangeException("minLength", "minLength must be 0 or greater");
            }
            if (minLength > maxLength) {
                throw new ArgumentOutOfRangeException("minLength", "minLength must be less than maxLength");
            }
            if (maxLength == 0) {
                return string.Empty;
            }

            #endregion

            string sampleRange = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            if (allowPunctuation) {
                sampleRange += ",.<>;':@#/~?+_-=!\"£$%^&*()`¬|\\";
            }
            if (allowNumbers) {
                sampleRange += "1234567890";
            }

            char[] possibleCharacters = sampleRange.ToCharArray();

            int characters = r.Next(minLength, maxLength);

            var result = new StringBuilder(characters);   // I seriously doubt this is faster.

            for (; characters > 0; characters--) {
                result.Append(possibleCharacters[r.Next(0, possibleCharacters.Length)]);
            }

            return result.ToString();
        }

        private string GenerateRandomString(int minLength, int maxLength) {
            int characters = RandomStore.Next(minLength, maxLength);

            var result = new StringBuilder(characters);   // I seriously doubt this is faster.

            for (; characters > 0; characters--) {
                result.Append((char)RandomStore.Next(15, 125));
            }

            return result.ToString();
        }

        /// <summary>
        /// Generates a random string with which testing can be performed.  This string will contain a variety
        /// of characters within the ASCII caharacter range 15-125 ish.
        /// </summary>
        /// <returns>A random length generated string</returns>
        public string GenerateString() {
            return GenerateRandomString(1, m_limitStringsTo);
        }

        /// <summary>
        /// Generates a random string with which testing can be performed.  This string will contain a variety
        /// of characters within the ASCII caharacter range 15-125 ish.
        /// </summary>
        /// <param name="minimumLength"> The minimum length of this string</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the minimum Length specified is less than 0 or > LimitStringsTo</exception>
        /// <returns>A random length generated string</returns>
        public string GenerateString(int minimumLength) {
            if (minimumLength < 0) {
                throw new ArgumentOutOfRangeException("minimumLength", "The minimum length for a generated string can not be less than zero");
            }
            if (minimumLength > m_limitStringsTo) {
                throw new ArgumentOutOfRangeException("minimumLength", "The minimum length for a generated string exceeds the LimitStringsTo property length");
            }
            return GenerateRandomString(minimumLength, m_limitStringsTo);
        }

        /// <summary>
        /// Generates a random string with which testing can be performed.  This string will contain a variety
        /// of characters within the ASCII caharacter range 15-125 ish.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the minimum length or maximum length are specified as less than zero</exception>
        /// <param name="minimumLength">The minimum length of this string</param>
        /// <param name="maximumLength">The maximum length of this string</param>
        /// <returns>A random length generated string</returns>
        public string GenerateString(int minimumLength, int maximumLength) {
            if (minimumLength < 0) {
                throw new ArgumentOutOfRangeException("minimumLength", "The minimum length for a generated string can not be less than zero");
            }
            if (maximumLength < 0) {
                throw new ArgumentOutOfRangeException("minimumLength", "The maximum length for a generated string can not be less than zero");
            }
            if (maximumLength < minimumLength) {
                throw new ArgumentOutOfRangeException("maximumLength", "The maximum length must be greater than the minimum length");
            }

            return GenerateRandomString(minimumLength, maximumLength);
        }
    }
}
