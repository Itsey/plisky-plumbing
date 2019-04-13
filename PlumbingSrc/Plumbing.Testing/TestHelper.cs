using System;
using System.Collections.Generic;
using System.Text;

namespace Plisky.Test {
    public class TestData {
        private readonly Random r = new Random();

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
        public IEnumerable<string> GetURLs(int howMany=-1) {

            howMany = howMany < 0 ? hardcodedUrls.Length : howMany;            

            for(int i=0; i<howMany; i++) {
                yield return hardcodedUrls[r.Next(hardcodedUrls.Length - 1)];
            }
        }
    }
}
