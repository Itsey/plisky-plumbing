namespace Plisky.Plumbing {

    using System.Diagnostics;

    public static class Diags {
        public static TraceSwitch PliskyMessagingSwitch = new TraceSwitch("PliskyMessagingSwitch", "Switch to enable all hub related diagnostics", "Off");
    }
}