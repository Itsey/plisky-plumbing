using Plisky.Plumbing;

namespace Plumbing.ITest;


public class Exploratory {


    [Fact]
    public void PortedFromTestConsole1() {
        // Its unclear what this test was doing, it was somthing related to a live bug at one point.  Remove if still unclear once investigated.
        var f = new Feature("JSB_FEATURED_HALLOWEEN", true);
        f.SetDateRange(new DateTime(2019, 10, 1), new DateTime(2019, 10, 31), true);
    }


    [Fact]
    public void PortedFromTestConsole2() {

    }


    [Fact]
    public void PortedFromTestConsole3() {

    }

    [Fact]
    public void PortedFromTestConsole4() {

    }
}