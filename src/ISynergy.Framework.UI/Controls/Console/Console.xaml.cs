using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.System;

#if (__UWP__ || HAS_UNO)
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
#elif (__WINUI__)
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
#endif

namespace ISynergy.Framework.UI.Controls
{
    /// <summary>
    /// Class Console. This class cannot be inherited.
    /// Implements the <see cref="UserControl" />
    /// Implements the <see cref="IComponentConnector" />
    /// </summary>
    /// <seealso cref="UserControl" />
    /// <seealso cref="IComponentConnector" />
    public sealed partial class Console : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Console"/> class.
        /// </summary>
        public Console()
        {
            InitializeComponent();
        }

#if __UWP__
        /// <summary>
        /// Writes the output to the console control.
        /// </summary>
        /// <param name="output">The output.</param>
        /// <param name="color">The color.</param>
        public async Task WriteOutputAsync(string output, Color color)
        {

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => WriteOutput(output, color));
            RichTextBox_Scroll.ChangeView(null, RichTextBox_Console.ActualHeight, null);

        }

        private void WriteOutput(string output, Color color)
        {

            var run = new Run();
            var sb = new StringBuilder();
            sb.AppendLine(output);
            run.Text = sb.ToString();
            run.Foreground = new SolidColorBrush(color);


            if (RichTextBox_Console.Blocks.Count == 0)
                RichTextBox_Console.Blocks.Add(new Paragraph());

            (RichTextBox_Console.Blocks.First() as Paragraph)?.Inlines.Add(run);
        }

        /// <summary>
        /// Clears the output.
        /// </summary>
        public void ClearOutput()
        {
            RichTextBox_Console.Blocks.Clear();
        }

        /// <summary>
        /// Runs a process.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="workingDirectory">The working directory.</param>
        public async Task StartProcessAsync(string fileName, string arguments, string workingDirectory = null)
        {

            //  Are we showing diagnostics?
            if (ShowDiagnostics)
            {
                await WriteOutputAsync("Preparing to run " + fileName, Color.FromArgb(255, 0, 255, 0));

                if (!string.IsNullOrEmpty(arguments))
                    await WriteOutputAsync(" with arguments " + arguments + "." + Environment.NewLine, Color.FromArgb(255, 0, 255, 0));
                else
                    await WriteOutputAsync("." + Environment.NewLine, Color.FromArgb(255, 0, 255, 0));
            }


            await CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                try
                {
                    var result = await ProcessLauncher.RunToCompletionAsync(
                        fileName,
                        arguments,
                        new ProcessLauncherOptions
                        {
                            WorkingDirectory = workingDirectory,
                            StandardOutput = OutputStream,
                            StandardError = ErrorStream
                        });

                    using (var outStreamRedirect = OutputStream.GetInputStreamAt(0))
                    {
                        var size = OutputStream.Size;
                        using (var dataReader = new DataReader(outStreamRedirect))
                        {
                            var bytesLoaded = await dataReader.LoadAsync((uint)size);
                            var stringRead = dataReader.ReadString(bytesLoaded);
                            await WriteOutputAsync(stringRead, Colors.White);
                        }
                    }

                    using (var errStreamRedirect = ErrorStream.GetInputStreamAt(0))
                    {
                        using (var dataReader = new DataReader(errStreamRedirect))
                        {
                            var size = ErrorStream.Size;
                            var bytesLoaded = await dataReader.LoadAsync((uint)size);
                            var stringRead = dataReader.ReadString(bytesLoaded);
                            await WriteOutputAsync(stringRead, Colors.Red);
                        }
                    }
                }
                catch (UnauthorizedAccessException uex)
                {
                    await WriteOutputAsync($"Exception Thrown: {uex.Message}{Environment.NewLine}", Colors.Red);
                    await WriteOutputAsync($"Make sure you're allowed to run the specified exe; either{Environment.NewLine}" +
                                             $"\t1) Add the exe to the AppX package, or{Environment.NewLine}" +
                                             $"\t2) Add the absolute path of the exe to the allow list:{Environment.NewLine}" +
                                             $"\t\tHKLM\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\EmbeddedMode\\ProcessLauncherAllowedExecutableFilesList.{Environment.NewLine}{Environment.NewLine}" +
                                             $"Also, make sure the <iot:Capability Name=\"systemManagement\" /> has been added to the AppX manifest capabilities.{Environment.NewLine}", Colors.Red);
                }
                catch (Exception ex)
                {
                    await WriteOutputAsync($"Exception Thrown: {ex.Message}{Environment.NewLine}", Colors.Red);
                    await WriteOutputAsync($"{ex.StackTrace}{Environment.NewLine}", Colors.Red);
                }
            });
        }
#endif

    /// <summary>
    /// The show diagnostics property
    /// </summary>
    private static readonly DependencyProperty ShowDiagnosticsProperty =
          DependencyProperty.Register(nameof(ShowDiagnostics), typeof(bool), typeof(Console),
          new PropertyMetadata(false, OnShowDiagnosticsChanged));

        /// <summary>
        /// Gets or sets a value indicating whether to show diagnostics.
        /// </summary>
        /// <value><c>true</c> if show diagnostics; otherwise, <c>false</c>.</value>
        public bool ShowDiagnostics
        {
            get { return (bool)GetValue(ShowDiagnosticsProperty); }
            set { SetValue(ShowDiagnosticsProperty, value); }
        }

        /// <summary>
        /// Handles the <see cref="E:ShowDiagnosticsChanged" /> event.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="args">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnShowDiagnosticsChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
        }

        /// <summary>
        /// The output stream property
        /// </summary>
        internal static readonly DependencyProperty OutputStreamProperty =
            DependencyProperty.Register(nameof(OutputStream), typeof(InMemoryRandomAccessStream), typeof(Console),
            new PropertyMetadata(default));

        /// <summary>
        /// Gets the output stream.
        /// </summary>
        /// <value>The output stream.</value>
        public InMemoryRandomAccessStream OutputStream
        {
            get { return (InMemoryRandomAccessStream)GetValue(OutputStreamProperty); }
            private set { SetValue(OutputStreamProperty, value); }
        }

        /// <summary>
        /// The error stream property
        /// </summary>
        internal static readonly DependencyProperty ErrorStreamProperty =
            DependencyProperty.Register(nameof(ErrorStream), typeof(InMemoryRandomAccessStream), typeof(Console),
            new PropertyMetadata(default));

        /// <summary>
        /// Gets the error stream.
        /// </summary>
        /// <value>The error stream.</value>
        public InMemoryRandomAccessStream ErrorStream
        {
            get { return (InMemoryRandomAccessStream)GetValue(ErrorStreamProperty); }
            private set { SetValue(ErrorStreamProperty, value); }
        }
    }
}
