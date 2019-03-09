namespace ISynergy.Background.Tasks.RegistryTests.Mocks
{
    using System;
    using System.Threading.Tasks;

    public class StronglyTypedTestJob : IJob
    {
        public Task ExecuteAsync()
        {
            Console.WriteLine("Hi");

            return Task.CompletedTask;
        }
    }
}