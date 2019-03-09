namespace ISynergy.Background.Tasks.RegistryTests
{
    using ISynergy.Framework.Tests.Base;
    using ISynergy.Background.Tasks.RegistryTests.Mocks;
    using System.Threading;
    using Xunit;

    public class JobConstructionTests : UnitTest
    {
        [Fact]
        public void Should_Call_Ctor()
        {
            JobManager.AddJob<CtorJob>(s => s.ToRunNow());
            Thread.Sleep(50);
            Assert.Equal(1, CtorJob.Calls);

            JobManager.AddJob<CtorJob>(s => s.ToRunNow());
            Thread.Sleep(50);
            Assert.Equal(2, CtorJob.Calls);

            JobManager.AddJob<CtorJob>(s => s.ToRunNow());
            Thread.Sleep(50);
            Assert.Equal(3, CtorJob.Calls);
        }

		[Fact]
		public void Should_Call_Dispose()
        {
            JobManager.AddJob<DisposableJob>(s => s.ToRunNow());
            Thread.Sleep(50);
            Assert.True(DisposableJob.Disposed);
		}
	}
}
