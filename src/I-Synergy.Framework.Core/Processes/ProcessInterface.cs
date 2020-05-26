using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ISynergy.Framework.Core.Processes
{
    /// <summary>
    /// A ProcessEventHandler is a delegate for process input/output events.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="args">The <see cref="ProcessEventArgs"/> instance containing the event data.</param>
    public delegate void ProcessEventHanlder(object sender, ProcessEventArgs args);

    /// <summary>
    /// A class the wraps a process, allowing programmatic input and output.
    /// </summary>
    public class ProcessInterface : IDisposable
    {
        ~ProcessInterface()
        {
            Dispose(false);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessInterface"/> class.
        /// </summary>
        public ProcessInterface()
        {
            //  Configure the output worker.
            OutputWorker.WorkerReportsProgress = true;
            OutputWorker.WorkerSupportsCancellation = true;
            OutputWorker.DoWork += OutputWorker_DoWork;
            OutputWorker.ProgressChanged += OutputWorker_ProgressChanged;

            //  Configure the error worker.
            ErrorWorker.WorkerReportsProgress = true;
            ErrorWorker.WorkerSupportsCancellation = true;
            ErrorWorker.DoWork += ErrorWorker_DoWork;
            ErrorWorker.ProgressChanged += ErrorWorker_ProgressChanged;
        }

        /// <summary>
        /// Handles the ProgressChanged event of the outputWorker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.ProgressChangedEventArgs"/> instance containing the event data.</param>
        void OutputWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //  We must be passed a string in the user state.
            if (e.UserState is string userState)
            {
                //  Fire the output event.
                FireProcessOutputEvent(userState);
            }
        }

        /// <summary>
        /// Handles the DoWork event of the outputWorker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.DoWorkEventArgs"/> instance containing the event data.</param>
        void OutputWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (OutputWorker.CancellationPending == false && OutputReader != null)
            {
                //  Any lines to read?
                int count;
                var buffer = new char[1024];
                do
                {
                    var builder = new StringBuilder();
                    count = OutputReader.Read(buffer, 0, 1024);
                    builder.Append(buffer, 0, count);
                    OutputWorker.ReportProgress(0, builder.ToString());
                } while (count > 0);

                System.Threading.Thread.Sleep(200);
            }
        }

        /// <summary>
        /// Handles the ProgressChanged event of the errorWorker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.ProgressChangedEventArgs"/> instance containing the event data.</param>
        void ErrorWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //  The userstate must be a string.
            if (e.UserState is string userState)
            {
                //  Fire the error event.
                FireProcessErrorEvent(userState);
            }
        }

        /// <summary>
        /// Handles the DoWork event of the errorWorker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.DoWorkEventArgs"/> instance containing the event data.</param>
        void ErrorWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (ErrorWorker.CancellationPending == false && ErrorReader != null)
            {
                //  Any lines to read?
                int count;
                var buffer = new char[1024];
                do
                {
                    var builder = new StringBuilder();
                    count = ErrorReader.Read(buffer, 0, 1024);
                    builder.Append(buffer, 0, count);
                    ErrorWorker.ReportProgress(0, builder.ToString());
                } while (count > 0);

                System.Threading.Thread.Sleep(200);
            }
        }

        /// <summary>
        /// Runs a process.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="arguments">The arguments.</param>
        public void StartProcess(string fileName, string arguments, string workingDirectory)
        {
            //  Create the process start info.
            var processStartInfo = new ProcessStartInfo(fileName)
            {
                Arguments = arguments,

                //  Set the options.
                UseShellExecute = false,
                ErrorDialog = false,
                CreateNoWindow = true,

                //  Specify redirection.
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true
            };

            if (!string.IsNullOrEmpty(workingDirectory))
            {
                processStartInfo.WorkingDirectory = workingDirectory;
            }

            //  Create the process.
            Process = new Process
            {
                EnableRaisingEvents = true,
                StartInfo = processStartInfo
            };

            Process.Exited += CurrentProcess_Exited;

            //  Start the process.
            try
            {
                Process.Start();
            }
            catch (Exception e)
            {
                //  Trace the exception.
                Trace.WriteLine("Failed to start process " + fileName + " with arguments '" + arguments + "'");
                Trace.WriteLine(e.ToString());
                return;
            }

            //  Store name and arguments.
            ProcessFileName = fileName;
            ProcessArguments = arguments;

            //  Create the readers and writers.
            InputWriter = Process.StandardInput;
            OutputReader = TextReader.Synchronized(Process.StandardOutput);
            ErrorReader = TextReader.Synchronized(Process.StandardError);

            //  Run the workers that read output and error.
            OutputWorker.RunWorkerAsync();
            ErrorWorker.RunWorkerAsync();
        }

        /// <summary>
        /// Stops the process.
        /// </summary>
        public void StopProcess()
        {
            //  Handle the trivial case.
            if (IsProcessRunning == false)
                return;

            //  Kill the process.
            Process?.Kill();
        }

        /// <summary>
        /// Handles the Exited event of the currentProcess control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void CurrentProcess_Exited(object sender, EventArgs e)
        {
            //  Fire process exited.
            if(Process != null) FireProcessExitEvent(Process.ExitCode);

            //  Disable the threads.
            OutputWorker.CancelAsync();
            ErrorWorker.CancelAsync();
            InputWriter = null;
            OutputReader = null;
            ErrorReader = null;
            Process = null;
            ProcessFileName = null;
            ProcessArguments = null;
        }

        /// <summary>
        /// Fires the process output event.
        /// </summary>
        /// <param name="content">The content.</param>
        private void FireProcessOutputEvent(string content)
        {
            //  Get the event and fire it.
            OnProcessOutput?.Invoke(this, new ProcessEventArgs(content));
        }

        /// <summary>
        /// Fires the process error output event.
        /// </summary>
        /// <param name="content">The content.</param>
        private void FireProcessErrorEvent(string content)
        {
            //  Get the event and fire it.
            OnProcessError?.Invoke(this, new ProcessEventArgs(content));
        }

        /// <summary>
        /// Fires the process input event.
        /// </summary>
        /// <param name="content">The content.</param>
        private void FireProcessInputEvent(string content)
        {
            //  Get the event and fire it.
            OnProcessInput?.Invoke(this, new ProcessEventArgs(content));
        }

        /// <summary>
        /// Fires the process exit event.
        /// </summary>
        /// <param name="code">The code.</param>
        private void FireProcessExitEvent(int code)
        {
            //  Get the event and fire it.
            OnProcessExit?.Invoke(this, new ProcessEventArgs(code));
        }

        /// <summary>
        /// Writes the input.
        /// </summary>
        /// <param name="input">The input.</param>
        public void WriteInput(string input)
        {
            if (IsProcessRunning && InputWriter != null)
            {
                InputWriter.WriteLine(input);
                InputWriter.Flush();
            }
        }

        /// <summary>
        /// The current process.
        /// </summary>
        public Process Process { get; private set; }

        /// <summary>
        /// The input writer.
        /// </summary>
        public StreamWriter InputWriter { get; private set; }

        /// <summary>
        /// The output reader.
        /// </summary>
        public TextReader OutputReader { get; private set; }

        /// <summary>
        /// The error reader.
        /// </summary>
        public TextReader ErrorReader { get; private set; }

        /// <summary>
        /// The output worker.
        /// </summary>
        private readonly BackgroundWorker OutputWorker = new BackgroundWorker();

        /// <summary>
        /// The error worker.
        /// </summary>
        private readonly BackgroundWorker ErrorWorker = new BackgroundWorker();

        /// <summary>
        /// Current process file name.
        /// </summary>
        public string ProcessFileName { get; private set; }

        /// <summary>
        /// Arguments sent to the current process.
        /// </summary>
        public string ProcessArguments { get; private set; }

#nullable disable
        /// <summary>
        /// Occurs when process output is produced.
        /// </summary>
        public event ProcessEventHanlder OnProcessOutput;

        /// <summary>
        /// Occurs when process error output is produced.
        /// </summary>
        public event ProcessEventHanlder OnProcessError;

        /// <summary>
        /// Occurs when process input is produced.
        /// </summary>
        public event ProcessEventHanlder OnProcessInput;

        /// <summary>
        /// Occurs when the process ends.
        /// </summary>
        public event ProcessEventHanlder OnProcessExit;
#nullable enable

        /// <summary>
        /// Gets a value indicating whether this instance is process running.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is process running; otherwise, <c>false</c>.
        /// </value>
        public bool IsProcessRunning
        {
            get
            {
                try
                {
                    return Process != null && Process.HasExited == false;
                }
                catch
                {
                    return false;
                }
            }
        }

        // The bulk of the clean-up code is implemented in Dispose(bool)
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (OutputWorker != null)
                {
                    OutputWorker.Dispose();
                }

                if (ErrorWorker != null)
                {
                    ErrorWorker.Dispose();
                }

                if (Process != null)
                {
                    Process.Kill();
                    Process.Dispose();
                    Process = null;
                }

                if (InputWriter != null)
                {
                    InputWriter = null;
                }

                if (OutputReader != null)
                {
                    OutputReader = null;
                }

                if (ErrorReader != null)
                {
                    ErrorReader = null;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
