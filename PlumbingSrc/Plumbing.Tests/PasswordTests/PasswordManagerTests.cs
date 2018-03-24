#if false
namespace Plisky.PliskyLibTests.PasswordTests {
    using Plisky.Security;
    using Xunit;


    public class PasswordManagerTests {


        [Fact][Trait("xunit","regression")]
        public void SaltyPassword_HashesNotEqualWithInequalSalt() {
            //SaltyPassword sp = new SaltyPassword();
            SaltyPassword sp = SaltyPassword.CreateFromCleartext("this is the password");
            SaltyPassword sp2 = SaltyPassword.CreateFromCleartext("this is the password");
            Assert.NotEqual(sp.PasswordSaltHash, sp2.PasswordSaltHash);
        }

        [Fact][Trait("xunit","regression")]
        public void SaltyPassword_SpecifySalt_HashesAreEqual() {
            //SaltyPassword sp = new SaltyPassword();
            SaltyPassword sp = SaltyPassword.CreateFromCleartext("this is the password");
            SaltyPassword sp2 = SaltyPassword.CreateFromCleartext("this is the password");
            sp2.Salt.IntegerSalt = sp.Salt.IntegerSalt;
            Assert.Equal(sp.PasswordSaltHash, sp2.PasswordSaltHash);
        }
    }
}
#endif