namespace ISynergy.Background.Tasks.RegistryTests.Mocks
{
    using System;
    using System.Threading.Tasks;

    public class DisposableJob : IJob, IDisposable
    {
        public DisposableJob()
        {
            Disposed = false;
        }

        public static bool Disposed { get; private set; }

        public Task ExecuteAsync() => Task.CompletedTask;

        #region IDisposable
        // Dispose() calls Dispose(true)
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // NOTE: Leave out the finalizer altogether if this class doesn't
        // own unmanaged resources, but leave the other methods
        // exactly as they are.
        //~ObservableClass()
        //{
        //    // Finalizer calls Dispose(false)
        //    Dispose(false);
        //}

        // The bulk of the clean-up code is implemented in Dispose(bool)
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
            }

            Disposed = true;
        }
        #endregion
    }
}
