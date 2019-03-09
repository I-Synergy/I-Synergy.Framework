using System.Threading.Tasks;

namespace ISynergy.Background.Tasks.RegistryTests.Mocks
{
    public class CtorJob : IJob
    {
        public CtorJob()
        {
            ++Calls;
        }

        public static int Calls { get; private set; }

        public Task ExecuteAsync() => Task.CompletedTask;
    }
}
