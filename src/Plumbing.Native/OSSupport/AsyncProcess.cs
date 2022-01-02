namespace Plisky.Platform {

    using Microsoft.Win32.SafeHandles;

#if NET45

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public static class AsyncProcessSupport {

        public static async Task<int> StartProcess(ProcessStartInfo psi, int? timeout = null, StringBuilder outConsole = null, StringBuilder outError = null) {
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            psi.RedirectStandardOutput = outConsole != null;
            psi.RedirectStandardError = outError != null;
            using (var process = new Process()) {
                process.StartInfo = psi;
                process.Start();

                var cancellationTokenSource = timeout.HasValue ? new CancellationTokenSource(timeout.Value) : new CancellationTokenSource();

                var tasks = new List<Task>();

                tasks.Add(WaitForExitAsync(process, cancellationTokenSource.Token));
                //tasks.Add(WaitForExitAsync2(process, new TimeSpan(0, 0, 100)));
                if (outConsole != null) {
                    var tsk = ReadAsync(
                        x => {
                            process.OutputDataReceived += x;
                            process.BeginOutputReadLine();
                        },
                        x => process.OutputDataReceived -= x,
                        outError,
                        cancellationTokenSource.Token);
                    tasks.Add(tsk);
                }

                if (outError != null) {
                    var tsk = ReadAsync(
                        x => {
                            process.ErrorDataReceived += x;
                            process.BeginErrorReadLine();
                        },
                        x => process.ErrorDataReceived -= x,
                        outError,
                        cancellationTokenSource.Token);
                    tasks.Add(tsk);
                }

                await Task.WhenAll(tasks);
                return process.ExitCode;
            }
        }

        //SOF CODE not from original sample
        public static Task<bool> WaitForExitAsync2(this Process process, TimeSpan timeout) {
            ManualResetEvent processWaitObject = new ManualResetEvent(false);
            processWaitObject.SafeWaitHandle = new SafeWaitHandle(process.Handle, false);

            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            RegisteredWaitHandle registeredProcessWaitHandle = null;
            registeredProcessWaitHandle = ThreadPool.RegisterWaitForSingleObject(
                processWaitObject,
                delegate (object state, bool timedOut) {
                    if (!timedOut) {
                        registeredProcessWaitHandle.Unregister(null);
                    }

                    processWaitObject.Dispose();
                    tcs.SetResult(!timedOut);
                },
                null /* state */,
                timeout,
                true /* executeOnlyOnce */);

            return tcs.Task;
        }

        /// <summary>
        /// Waits asynchronously for the process to exit.
        /// </summary>
        /// <param name="process">The process to wait for cancellation.</param>
        /// <param name="cancellationToken">A cancellation token. If invoked, the task will return immediately as cancelled.</param>
        /// <returns>A Task representing waiting for the process to end.</returns>
        private static Task WaitForExitAsync(Process process, CancellationToken cancellationToken = default(CancellationToken)) {
            process.EnableRaisingEvents = true;

            var taskCompletionSource = new TaskCompletionSource<object>();

            EventHandler handler = null;
            handler = (sender, args) => {
                process.Exited -= handler;
                taskCompletionSource.TrySetResult(null);
            };
            process.Exited += handler;

            if (cancellationToken != default(CancellationToken)) {
                cancellationToken.Register(
                    () => {
                        process.Exited -= handler;
                        taskCompletionSource.TrySetCanceled();
                    });
            }

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Reads the data from the specified data recieved event and writes it to the
        /// <paramref name="dataStore"/>.
        /// </summary>
        /// <param name="addHandler">Adds the event handler.</param>
        /// <param name="removeHandler">Removes the event handler.</param>
        /// <param name="dataStore">the string builder that has the text appended to it.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private static Task ReadAsync(Action<DataReceivedEventHandler> addHandler, Action<DataReceivedEventHandler> removeHandler, StringBuilder dataStore, CancellationToken cancellationToken = default(CancellationToken)) {
            var taskCompletionSource = new TaskCompletionSource<object>();

            DataReceivedEventHandler handler = null;
            handler = new DataReceivedEventHandler(
                (sender, e) => {
                    if (e.Data == null) {
                        removeHandler(handler);
                        taskCompletionSource.TrySetResult(null);
                    } else {
                        dataStore.Append(e.Data);
                    }
                });

            addHandler(handler);

            if (cancellationToken != default(CancellationToken)) {
                cancellationToken.Register(
                    () => {
                        removeHandler(handler);
                        taskCompletionSource.TrySetCanceled();
                    });
            }

            return taskCompletionSource.Task;
        }
    }

#endif
}