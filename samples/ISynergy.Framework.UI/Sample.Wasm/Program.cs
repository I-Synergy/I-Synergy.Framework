using Windows.UI.Xaml;

namespace Sample.Wasm
{
    /// <summary>
    /// Class Program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The application
        /// </summary>
        private static App _app;

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>System.Int32.</returns>
        static int Main(string[] args)
        {
            Application.Start(_ => _app = new App());
            return 0;
        }
    }
}
