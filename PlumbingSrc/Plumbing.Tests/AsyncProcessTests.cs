namespace Plisky.Test {
#if false
    using Plisky.Platform;
    using System.Diagnostics;
    using System.Text;
    using Xunit;


    public class AsyncProcessTests {

        [Fact][Trait("xunit","regression")]
        public void RunHelpAsync() {
            //Assert.Fail();
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = @"C:\windows\system32\help.exe";
            StringBuilder one = new StringBuilder();
            StringBuilder two = new StringBuilder();
            var t = AsyncProcessSupport.StartProcess(psi, null, one, two);

            int i = t.Result;

            var a = one.ToString();
            var b = two.ToString();
            Assert.True((a.Length + b.Length) > 0, "There should be some output from help");
        }
    }
#endif
}