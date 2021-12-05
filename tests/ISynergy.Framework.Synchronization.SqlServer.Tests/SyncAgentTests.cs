using ISynergy.Framework.Synchronization.Core;
using ISynergy.Framework.Synchronization.Core.Enumerations;
using ISynergy.Framework.Synchronization.Core.Setup;
using ISynergy.Framework.Synchronization.SqlServer.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Synchronization.SqlServer.Tests
{
    public class SyncAgentTests
    {
       private void CheckConstructor(SyncAgent agent)
        {
            Assert.AreEqual(SyncSessionState.Ready, agent.SessionState);
            Assert.IsNull(agent.Schema);
            Assert.IsNotNull(agent.LocalOrchestrator);
            Assert.IsNotNull(agent.RemoteOrchestrator);
            Assert.IsNotNull(agent.LocalOrchestrator.Options);
            Assert.IsNotNull(agent.RemoteOrchestrator.Options);
            Assert.IsNotNull(agent.LocalOrchestrator.Setup);
            Assert.IsNotNull(agent.RemoteOrchestrator.Setup);
            Assert.AreSame(agent.LocalOrchestrator.Options, agent.RemoteOrchestrator.Options);
            Assert.AreSame(agent.LocalOrchestrator.Setup, agent.RemoteOrchestrator.Setup);

        }

        [TestMethod]
        public void SyncAgent_FirstConstructor_LocalOrchestrator_ShouldMatch_RemoteOrchestrator()
        {
            var clientProvider = new SqlSyncProvider();
            var serverProvider = new SqlSyncProvider();
            var tables = new string[] { "Customer" };

            var agent = new SyncAgent(clientProvider, serverProvider, tables);

            CheckConstructor(agent);

            Assert.AreEqual(SyncOptions.DefaultScopeName, agent.ScopeName);
            Assert.AreEqual(SyncOptions.DefaultScopeName, agent.LocalOrchestrator.ScopeName);
            Assert.AreEqual(SyncOptions.DefaultScopeName, agent.RemoteOrchestrator.ScopeName);
            Assert.AreEqual(1, agent.LocalOrchestrator.Setup.Tables.Count);
            Assert.AreEqual(1, agent.RemoteOrchestrator.Setup.Tables.Count);
            Assert.AreEqual("Customer", agent.LocalOrchestrator.Setup.Tables[0].TableName);
            Assert.AreEqual("Customer", agent.RemoteOrchestrator.Setup.Tables[0].TableName);
        }

        [TestMethod]
        public void SyncAgent_FirstConstructor_SetupTables_ShouldBe_Empty_When_TablesArgIsNull()
        {
            var clientProvider = new SqlSyncProvider();
            var serverProvider = new SqlSyncProvider();
            string[] tables = null;

            var agent = new SyncAgent(clientProvider, serverProvider, tables);

            CheckConstructor(agent);
            Assert.AreEqual(SyncOptions.DefaultScopeName, agent.ScopeName);
            Assert.AreEqual(SyncOptions.DefaultScopeName, agent.LocalOrchestrator.ScopeName);
            Assert.AreEqual(SyncOptions.DefaultScopeName, agent.RemoteOrchestrator.ScopeName);
            Assert.AreEqual(0, agent.LocalOrchestrator.Setup.Tables.Count);
            Assert.AreEqual(0, agent.RemoteOrchestrator.Setup.Tables.Count);
        }


        [TestMethod]
        public void SyncAgent_FirstConstructor_LocalOrchestrator_ShouldMatch_RemoteOrchestrator_When_ScopeNameIsDefined()
        {
            var clientProvider = new SqlSyncProvider();
            var serverProvider = new SqlSyncProvider();
            var tables = new string[] { "Customer" };

            var agent = new SyncAgent(clientProvider, serverProvider, tables, "CustomerScope");

            CheckConstructor(agent);

            Assert.AreEqual("CustomerScope", agent.ScopeName);
            Assert.AreEqual("CustomerScope", agent.LocalOrchestrator.ScopeName);
            Assert.AreEqual("CustomerScope", agent.RemoteOrchestrator.ScopeName);
            Assert.AreEqual(1, agent.LocalOrchestrator.Setup.Tables.Count);
            Assert.AreEqual(1, agent.RemoteOrchestrator.Setup.Tables.Count);
            Assert.AreEqual("Customer", agent.LocalOrchestrator.Setup.Tables[0].TableName);
            Assert.AreEqual("Customer", agent.RemoteOrchestrator.Setup.Tables[0].TableName);
        }

        [TestMethod]
        public void SyncAgent_SecondConstructor_LocalOrchestrator_ShouldMatch_RemoteOrchestrator()
        {
            var clientProvider = new SqlSyncProvider();
            var serverProvider = new SqlSyncProvider();

            var agent = new SyncAgent(clientProvider, serverProvider);

            CheckConstructor(agent);

            Assert.AreEqual(SyncOptions.DefaultScopeName, agent.ScopeName);
            Assert.AreEqual(SyncOptions.DefaultScopeName, agent.LocalOrchestrator.ScopeName);
            Assert.AreEqual(SyncOptions.DefaultScopeName, agent.RemoteOrchestrator.ScopeName);
            Assert.AreEqual(0, agent.LocalOrchestrator.Setup.Tables.Count);
            Assert.AreEqual(0, agent.RemoteOrchestrator.Setup.Tables.Count);
        }

        [TestMethod]
        public void SyncAgent_SecondConstructor_LocalOrchestrator_ShouldMatch_RemoteOrchestrator_When_ScopeNameIsDefined()
        {
            var clientProvider = new SqlSyncProvider();
            var serverProvider = new SqlSyncProvider();

            var agent = new SyncAgent(clientProvider, serverProvider, "CustomerScope");

            CheckConstructor(agent);

            Assert.AreEqual("CustomerScope", agent.ScopeName);
            Assert.AreEqual("CustomerScope", agent.LocalOrchestrator.ScopeName);
            Assert.AreEqual("CustomerScope", agent.RemoteOrchestrator.ScopeName);
            Assert.AreEqual(0, agent.LocalOrchestrator.Setup.Tables.Count);
            Assert.AreEqual(0, agent.RemoteOrchestrator.Setup.Tables.Count);
        }

        [TestMethod]
        public void SyncAgent_ThirdConstructor_LocalOrchestrator_ShouldMatch_RemoteOrchestrator()
        {
            var clientProvider = new SqlSyncProvider();
            var serverProvider = new SqlSyncProvider();
            var options = new SyncOptions();
            var tables = new string[] { "Customer" };

            var agent = new SyncAgent(clientProvider, serverProvider, options, tables);

            CheckConstructor(agent);
            Assert.AreSame(options, agent.LocalOrchestrator.Options);
            Assert.AreSame(options, agent.RemoteOrchestrator.Options);
            Assert.AreEqual(SyncOptions.DefaultScopeName, agent.ScopeName);
            Assert.AreEqual(SyncOptions.DefaultScopeName, agent.LocalOrchestrator.ScopeName);
            Assert.AreEqual(SyncOptions.DefaultScopeName, agent.RemoteOrchestrator.ScopeName);
            Assert.AreEqual(1, agent.LocalOrchestrator.Setup.Tables.Count);
            Assert.AreEqual(1, agent.RemoteOrchestrator.Setup.Tables.Count);
            Assert.AreEqual("Customer", agent.LocalOrchestrator.Setup.Tables[0].TableName);
            Assert.AreEqual("Customer", agent.RemoteOrchestrator.Setup.Tables[0].TableName);
        }

        [TestMethod]
        public void SyncAgent_FourthConstructor_LocalOrchestrator_ShouldMatch_RemoteOrchestrator()
        {
            var clientProvider = new SqlSyncProvider();
            var serverProvider = new SqlSyncProvider();
            var options = new SyncOptions();
            var setup = new SyncSetup(new string[] { "Customer" });

            var agent = new SyncAgent(clientProvider, serverProvider, options, setup);

            CheckConstructor(agent);
            Assert.AreSame(options, agent.LocalOrchestrator.Options);
            Assert.AreSame(options, agent.RemoteOrchestrator.Options);
            Assert.AreSame(setup, agent.LocalOrchestrator.Setup);
            Assert.AreSame(setup, agent.RemoteOrchestrator.Setup);
            Assert.AreEqual(SyncOptions.DefaultScopeName, agent.ScopeName);
            Assert.AreEqual(SyncOptions.DefaultScopeName, agent.LocalOrchestrator.ScopeName);
            Assert.AreEqual(SyncOptions.DefaultScopeName, agent.RemoteOrchestrator.ScopeName);
            Assert.AreEqual(1, agent.LocalOrchestrator.Setup.Tables.Count);
            Assert.AreEqual(1, agent.RemoteOrchestrator.Setup.Tables.Count);
            Assert.AreEqual("Customer", agent.LocalOrchestrator.Setup.Tables[0].TableName);
            Assert.AreEqual("Customer", agent.RemoteOrchestrator.Setup.Tables[0].TableName);
        }

        [TestMethod]
        public void SyncAgent_FourthConstructor_LocalOrchestrator_ShouldMatch_RemoteOrchestrator_When_TablesArgIsNull()
        {
            var clientProvider = new SqlSyncProvider();
            var serverProvider = new SqlSyncProvider();
            var options = new SyncOptions();
            var setup = new SyncSetup();

            var agent = new SyncAgent(clientProvider, serverProvider, options, setup);

            CheckConstructor(agent);
            Assert.AreSame(options, agent.LocalOrchestrator.Options);
            Assert.AreSame(options, agent.RemoteOrchestrator.Options);
            Assert.AreSame(setup, agent.LocalOrchestrator.Setup);
            Assert.AreSame(setup, agent.RemoteOrchestrator.Setup);
            Assert.AreEqual(SyncOptions.DefaultScopeName, agent.ScopeName);
            Assert.AreEqual(SyncOptions.DefaultScopeName, agent.LocalOrchestrator.ScopeName);
            Assert.AreEqual(SyncOptions.DefaultScopeName, agent.RemoteOrchestrator.ScopeName);
            Assert.AreEqual(0, agent.LocalOrchestrator.Setup.Tables.Count);
            Assert.AreEqual(0, agent.RemoteOrchestrator.Setup.Tables.Count);
        }


        [TestMethod]
        public void SyncAgent_FifthConstructor_LocalOrchestrator_ShouldMatch_RemoteOrchestrator()
        {
            var clientProvider = new SqlSyncProvider();

            // this options and setup will be overriden by the constructor
            var remoteOptions = new SyncOptions();
            var remoteSetup = new SyncSetup(new string[] { "Product", "ProductCategory" });

            var remoteOrchestrator = new RemoteOrchestrator(new SqlSyncProvider(), remoteOptions, remoteSetup);

            var agent = new SyncAgent(clientProvider, remoteOrchestrator, new string[] { "Customer" });

            CheckConstructor(agent);
            Assert.AreEqual(SyncOptions.DefaultScopeName, agent.ScopeName);
            Assert.AreEqual(SyncOptions.DefaultScopeName, agent.LocalOrchestrator.ScopeName);
            Assert.AreEqual(SyncOptions.DefaultScopeName, agent.RemoteOrchestrator.ScopeName);
            Assert.AreEqual(1, agent.LocalOrchestrator.Setup.Tables.Count);
            Assert.AreEqual(1, agent.RemoteOrchestrator.Setup.Tables.Count);
            Assert.AreEqual("Customer", agent.LocalOrchestrator.Setup.Tables[0].TableName);
            Assert.AreEqual("Customer", agent.RemoteOrchestrator.Setup.Tables[0].TableName);

        }

        //[TestMethod]
        //public void SyncAgent_FifthConstructor_LocalOrchestrator_ShouldMatch_WebClientOrchestrator()
        //{
        //    var clientProvider = new SqlSyncProvider();
        //    var remoteOrchestrator = new WebClientOrchestrator("http://localhost/api");

        //    var agent = new SyncAgent(clientProvider, remoteOrchestrator, new string[] { "Customer" });

        //    CheckConstructor(agent);
        //    Assert.AreEqual(SyncOptions.DefaultScopeName, agent.ScopeName);
        //    Assert.AreEqual(SyncOptions.DefaultScopeName, agent.LocalOrchestrator.ScopeName);
        //    Assert.AreEqual(SyncOptions.DefaultScopeName, agent.RemoteOrchestrator.ScopeName);
        //    Assert.AreEqual(1, agent.LocalOrchestrator.Setup.Tables.Count);
        //    Assert.AreEqual(1, agent.RemoteOrchestrator.Setup.Tables.Count);
        //    Assert.AreEqual("Customer", agent.LocalOrchestrator.Setup.Tables[0].TableName);
        //    Assert.AreEqual("Customer", agent.RemoteOrchestrator.Setup.Tables[0].TableName);

        //}

        [TestMethod]
        public void SyncAgent_FifthConstructor_LocalOrchestrator_ShouldMatch_RemoteOrchestrator_With_ScopeNameDefined()
        {
            var clientProvider = new SqlSyncProvider();
            // this options and setup will be overriden by the constructor
            var remoteOptions = new SyncOptions();
            var remoteSetup = new SyncSetup(new string[] { "Product", "ProductCategory" });
            var remoteOrchestrator = new RemoteOrchestrator(new SqlSyncProvider(), remoteOptions, remoteSetup);

            var agent = new SyncAgent(clientProvider, remoteOrchestrator, new string[] { "Customer" }, "CustomerScope");

            CheckConstructor(agent);
            Assert.AreEqual("CustomerScope", agent.ScopeName);
            Assert.AreEqual("CustomerScope", agent.LocalOrchestrator.ScopeName);
            Assert.AreEqual("CustomerScope", agent.RemoteOrchestrator.ScopeName);
            Assert.AreEqual(1, agent.LocalOrchestrator.Setup.Tables.Count);
            Assert.AreEqual(1, agent.RemoteOrchestrator.Setup.Tables.Count);
            Assert.AreEqual("Customer", agent.LocalOrchestrator.Setup.Tables[0].TableName);
            Assert.AreEqual("Customer", agent.RemoteOrchestrator.Setup.Tables[0].TableName);

        }

        //[TestMethod]
        //public void SyncAgent_FifthConstructor_LocalOrchestrator_ShouldMatch_WebClientOrchestrator_With_ScopeNameDefined()
        //{
        //    var clientProvider = new SqlSyncProvider();
        //    var remoteOrchestrator = new WebClientOrchestrator("http://localhost/api");

        //    var agent = new SyncAgent(clientProvider, remoteOrchestrator, new string[] { "Customer" }, "CustomerScope");

        //    CheckConstructor(agent);
        //    Assert.AreEqual("CustomerScope", agent.ScopeName);
        //    Assert.AreEqual("CustomerScope", agent.LocalOrchestrator.ScopeName);
        //    Assert.AreEqual("CustomerScope", agent.RemoteOrchestrator.ScopeName);
        //    Assert.AreEqual(1, agent.LocalOrchestrator.Setup.Tables.Count);
        //    Assert.AreEqual(1, agent.RemoteOrchestrator.Setup.Tables.Count);
        //    Assert.AreEqual("Customer", agent.LocalOrchestrator.Setup.Tables[0].TableName);
        //    Assert.AreEqual("Customer", agent.RemoteOrchestrator.Setup.Tables[0].TableName);

        //}


        [TestMethod]
        public void SyncAgent_SixthConstructor_LocalOrchestrator_ShouldMatch_RemoteOrchestrator()
        {
            var clientProvider = new SqlSyncProvider();

            // this options and setup will be overriden by the constructor
            var remoteOptions = new SyncOptions();
            var remoteSetup = new SyncSetup(new string[] { "Product", "ProductCategory" });

            var remoteOrchestrator = new RemoteOrchestrator(new SqlSyncProvider(), remoteOptions, remoteSetup);

            var agent = new SyncAgent(clientProvider, remoteOrchestrator);

            CheckConstructor(agent);
            Assert.AreEqual(SyncOptions.DefaultScopeName, agent.ScopeName);
            Assert.AreEqual(SyncOptions.DefaultScopeName, agent.LocalOrchestrator.ScopeName);
            Assert.AreEqual(SyncOptions.DefaultScopeName, agent.RemoteOrchestrator.ScopeName);
            Assert.AreEqual(0, agent.LocalOrchestrator.Setup.Tables.Count);
            Assert.AreEqual(0, agent.LocalOrchestrator.Setup.Tables.Count);

        }

        //[TestMethod]
        //public void SyncAgent_SixthConstructor_LocalOrchestrator_ShouldMatch_WebClientOrchestrator()
        //{
        //    var clientProvider = new SqlSyncProvider();
        //    var remoteOrchestrator = new WebClientOrchestrator("http://localhost/api");

        //    var agent = new SyncAgent(clientProvider, remoteOrchestrator);

        //    CheckConstructor(agent);
        //    Assert.AreEqual(SyncOptions.DefaultScopeName, agent.ScopeName);
        //    Assert.AreEqual(SyncOptions.DefaultScopeName, agent.LocalOrchestrator.ScopeName);
        //    Assert.AreEqual(SyncOptions.DefaultScopeName, agent.RemoteOrchestrator.ScopeName);
        //    Assert.AreEqual(0, agent.LocalOrchestrator.Setup.Tables.Count);
        //    Assert.AreEqual(0, agent.LocalOrchestrator.Setup.Tables.Count);

        //}

        [TestMethod]
        public void SyncAgent_SixthConstructor_LocalOrchestrator_ShouldMatch_RemoteOrchestrator_With_ScopeNameDefined()
        {
            var clientProvider = new SqlSyncProvider();
            // this options and setup will be overriden by the constructor
            var remoteOptions = new SyncOptions();
            var remoteSetup = new SyncSetup(new string[] { "Product", "ProductCategory" });
            var remoteOrchestrator = new RemoteOrchestrator(new SqlSyncProvider(), remoteOptions, remoteSetup);

            var agent = new SyncAgent(clientProvider, remoteOrchestrator, "CustomerScope");

            CheckConstructor(agent);
            Assert.AreEqual("CustomerScope", agent.ScopeName);
            Assert.AreEqual("CustomerScope", agent.LocalOrchestrator.ScopeName);
            Assert.AreEqual("CustomerScope", agent.RemoteOrchestrator.ScopeName);
            Assert.AreEqual(0, agent.LocalOrchestrator.Setup.Tables.Count);
            Assert.AreEqual(0, agent.LocalOrchestrator.Setup.Tables.Count);

        }

        //[TestMethod]
        //public void SyncAgent_SixthConstructor_LocalOrchestrator_ShouldMatch_WebClientOrchestrator_With_ScopeNameDefined()
        //{
        //    var clientProvider = new SqlSyncProvider();
        //    var remoteOrchestrator = new WebClientOrchestrator("http://localhost/api");

        //    var agent = new SyncAgent(clientProvider, remoteOrchestrator, "CustomerScope");

        //    CheckConstructor(agent);
        //    Assert.AreEqual("CustomerScope", agent.ScopeName);
        //    Assert.AreEqual("CustomerScope", agent.LocalOrchestrator.ScopeName);
        //    Assert.AreEqual("CustomerScope", agent.RemoteOrchestrator.ScopeName);
        //    Assert.AreEqual(0, agent.LocalOrchestrator.Setup.Tables.Count);
        //    Assert.AreEqual(0, agent.LocalOrchestrator.Setup.Tables.Count);

        //}



        [TestMethod]
        public void SyncAgent_SeventhConstructor_LocalOrchestrator_ShouldMatch_RemoteOrchestrator()
        {
            var clientProvider = new SqlSyncProvider();
            var options = new SyncOptions();

            // this options and setup will be overriden by the constructor
            var remoteOptions = new SyncOptions();
            var remoteSetup = new SyncSetup(new string[] { "Product", "ProductCategory" });

            var remoteOrchestrator = new RemoteOrchestrator(new SqlSyncProvider(), remoteOptions, remoteSetup);

            var agent = new SyncAgent(clientProvider, remoteOrchestrator, options, new string[] { "Customer" });

            CheckConstructor(agent);
            Assert.AreSame(options, agent.LocalOrchestrator.Options);
            Assert.AreSame(options, agent.RemoteOrchestrator.Options);
            Assert.AreEqual(SyncOptions.DefaultScopeName, agent.ScopeName);
            Assert.AreEqual(SyncOptions.DefaultScopeName, agent.LocalOrchestrator.ScopeName);
            Assert.AreEqual(SyncOptions.DefaultScopeName, agent.RemoteOrchestrator.ScopeName);
            Assert.AreEqual(1, agent.LocalOrchestrator.Setup.Tables.Count);
            Assert.AreEqual(1, agent.RemoteOrchestrator.Setup.Tables.Count);
            Assert.AreEqual("Customer", agent.LocalOrchestrator.Setup.Tables[0].TableName);
            Assert.AreEqual("Customer", agent.RemoteOrchestrator.Setup.Tables[0].TableName);

        }

        //[TestMethod]
        //public void SyncAgent_SeventhConstructor_LocalOrchestrator_ShouldMatch_WebClientOrchestrator()
        //{
        //    var clientProvider = new SqlSyncProvider();
        //    var options = new SyncOptions();

        //    var remoteOrchestrator = new WebClientOrchestrator("http://localhost/api");

        //    var agent = new SyncAgent(clientProvider, remoteOrchestrator, options, new string[] { "Customer" });

        //    CheckConstructor(agent);
        //    Assert.AreSame(options, agent.LocalOrchestrator.Options);
        //    Assert.AreSame(options, agent.RemoteOrchestrator.Options);
        //    Assert.AreEqual(SyncOptions.DefaultScopeName, agent.ScopeName);
        //    Assert.AreEqual(SyncOptions.DefaultScopeName, agent.LocalOrchestrator.ScopeName);
        //    Assert.AreEqual(SyncOptions.DefaultScopeName, agent.RemoteOrchestrator.ScopeName);
        //    Assert.AreEqual(1, agent.LocalOrchestrator.Setup.Tables.Count);
        //    Assert.AreEqual(1, agent.RemoteOrchestrator.Setup.Tables.Count);
        //    Assert.AreEqual("Customer", agent.LocalOrchestrator.Setup.Tables[0].TableName);
        //    Assert.AreEqual("Customer", agent.RemoteOrchestrator.Setup.Tables[0].TableName);

        //}



        [TestMethod]
        public void SyncAgent_SeventhConstructor_LocalOrchestrator_ShouldMatch_RemoteOrchestrator_With_ScopeNameDefined()
        {
            var clientProvider = new SqlSyncProvider();
            var options = new SyncOptions();

            // this options and setup will be overriden by the constructor
            var remoteOptions = new SyncOptions();
            var remoteSetup = new SyncSetup(new string[] { "Product", "ProductCategory" });

            var remoteOrchestrator = new RemoteOrchestrator(new SqlSyncProvider(), remoteOptions, remoteSetup);

            var agent = new SyncAgent(clientProvider, remoteOrchestrator, options, new string[] { "Customer" }, "CustomerScope");

            CheckConstructor(agent);
            Assert.AreSame(options, agent.LocalOrchestrator.Options);
            Assert.AreSame(options, agent.RemoteOrchestrator.Options);
            Assert.AreEqual("CustomerScope", agent.ScopeName);
            Assert.AreEqual("CustomerScope", agent.LocalOrchestrator.ScopeName);
            Assert.AreEqual("CustomerScope", agent.RemoteOrchestrator.ScopeName);
            Assert.AreEqual(1, agent.LocalOrchestrator.Setup.Tables.Count);
            Assert.AreEqual(1, agent.RemoteOrchestrator.Setup.Tables.Count);
            Assert.AreEqual("Customer", agent.LocalOrchestrator.Setup.Tables[0].TableName);
            Assert.AreEqual("Customer", agent.RemoteOrchestrator.Setup.Tables[0].TableName);

        }

        //[TestMethod]
        //public void SyncAgent_SeventhConstructor_LocalOrchestrator_ShouldMatch_WebClientOrchestrator_With_ScopeNameDefined()
        //{
        //    var clientProvider = new SqlSyncProvider();
        //    var options = new SyncOptions();

        //    var remoteOrchestrator = new WebClientOrchestrator("http://localhost/api");

        //    var agent = new SyncAgent(clientProvider, remoteOrchestrator, options, new string[] { "Customer" }, "CustomerScope");

        //    CheckConstructor(agent);
        //    Assert.AreSame(options, agent.LocalOrchestrator.Options);
        //    Assert.AreSame(options, agent.RemoteOrchestrator.Options);
        //    Assert.AreEqual("CustomerScope", agent.ScopeName);
        //    Assert.AreEqual("CustomerScope", agent.LocalOrchestrator.ScopeName);
        //    Assert.AreEqual("CustomerScope", agent.RemoteOrchestrator.ScopeName);
        //    Assert.AreEqual(1, agent.LocalOrchestrator.Setup.Tables.Count);
        //    Assert.AreEqual(1, agent.RemoteOrchestrator.Setup.Tables.Count);
        //    Assert.AreEqual("Customer", agent.LocalOrchestrator.Setup.Tables[0].TableName);
        //    Assert.AreEqual("Customer", agent.RemoteOrchestrator.Setup.Tables[0].TableName);

        //}



        [TestMethod]
        public void SyncAgent_EighthConstructor_LocalOrchestrator_ShouldMatch_RemoteOrchestrator()
        {
            var clientProvider = new SqlSyncProvider();
            var options = new SyncOptions();
            var setup = new SyncSetup(new string[] { "Customer" });

            // this options and setup will be overriden by the constructor
            var remoteOptions = new SyncOptions();
            var remoteSetup = new SyncSetup(new string[] { "Product", "ProductCategory" });

            var remoteOrchestrator = new RemoteOrchestrator(new SqlSyncProvider(), remoteOptions, remoteSetup);

            var agent = new SyncAgent(clientProvider, remoteOrchestrator, options, setup);

            CheckConstructor(agent);
            Assert.AreSame(options, agent.LocalOrchestrator.Options);
            Assert.AreSame(options, agent.RemoteOrchestrator.Options);
            Assert.AreSame(setup, agent.LocalOrchestrator.Setup);
            Assert.AreSame(setup, agent.RemoteOrchestrator.Setup);
            Assert.AreEqual(SyncOptions.DefaultScopeName, agent.ScopeName);
            Assert.AreEqual(SyncOptions.DefaultScopeName, agent.LocalOrchestrator.ScopeName);
            Assert.AreEqual(SyncOptions.DefaultScopeName, agent.RemoteOrchestrator.ScopeName);
            Assert.AreEqual(1, agent.LocalOrchestrator.Setup.Tables.Count);
            Assert.AreEqual(1, agent.RemoteOrchestrator.Setup.Tables.Count);
            Assert.AreEqual("Customer", agent.LocalOrchestrator.Setup.Tables[0].TableName);
            Assert.AreEqual("Customer", agent.RemoteOrchestrator.Setup.Tables[0].TableName);

        }

        //[TestMethod]
        //public void SyncAgent_EighthConstructor_LocalOrchestrator_ShouldMatch_WebClientOrchestrator()
        //{
        //    var clientProvider = new SqlSyncProvider();
        //    var options = new SyncOptions();
        //    var setup = new SyncSetup(new string[] { "Customer" });

        //    var remoteOrchestrator = new WebClientOrchestrator("http://localhost/api");

        //    var agent = new SyncAgent(clientProvider, remoteOrchestrator, options, setup);

        //    CheckConstructor(agent);
        //    Assert.AreSame(options, agent.LocalOrchestrator.Options);
        //    Assert.AreSame(options, agent.RemoteOrchestrator.Options);
        //    Assert.AreSame(setup, agent.LocalOrchestrator.Setup);
        //    Assert.AreSame(setup, agent.RemoteOrchestrator.Setup);
        //    Assert.AreEqual(SyncOptions.DefaultScopeName, agent.ScopeName);
        //    Assert.AreEqual(SyncOptions.DefaultScopeName, agent.LocalOrchestrator.ScopeName);
        //    Assert.AreEqual(SyncOptions.DefaultScopeName, agent.RemoteOrchestrator.ScopeName);
        //    Assert.AreEqual(1, agent.LocalOrchestrator.Setup.Tables.Count);
        //    Assert.AreEqual(1, agent.RemoteOrchestrator.Setup.Tables.Count);
        //    Assert.AreEqual("Customer", agent.LocalOrchestrator.Setup.Tables[0].TableName);
        //    Assert.AreEqual("Customer", agent.RemoteOrchestrator.Setup.Tables[0].TableName);

        //}



        [TestMethod]
        public void SyncAgent_EighthConstructor_LocalOrchestrator_ShouldMatch_RemoteOrchestrator_With_ScopeNameDefined()
        {
            var clientProvider = new SqlSyncProvider();
            var options = new SyncOptions();
            var setup = new SyncSetup(new string[] { "Customer" });

            // this options and setup will be overriden by the constructor
            var remoteOptions = new SyncOptions();
            var remoteSetup = new SyncSetup(new string[] { "Product", "ProductCategory" });

            var remoteOrchestrator = new RemoteOrchestrator(new SqlSyncProvider(), remoteOptions, remoteSetup);

            var agent = new SyncAgent(clientProvider, remoteOrchestrator, options, setup, "CustomerScope");

            CheckConstructor(agent);
            Assert.AreSame(options, agent.LocalOrchestrator.Options);
            Assert.AreSame(options, agent.RemoteOrchestrator.Options);
            Assert.AreSame(setup, agent.LocalOrchestrator.Setup);
            Assert.AreSame(setup, agent.RemoteOrchestrator.Setup);
            Assert.AreEqual("CustomerScope", agent.ScopeName);
            Assert.AreEqual("CustomerScope", agent.LocalOrchestrator.ScopeName);
            Assert.AreEqual("CustomerScope", agent.RemoteOrchestrator.ScopeName);
            Assert.AreEqual(1, agent.LocalOrchestrator.Setup.Tables.Count);
            Assert.AreEqual(1, agent.RemoteOrchestrator.Setup.Tables.Count);
            Assert.AreEqual("Customer", agent.LocalOrchestrator.Setup.Tables[0].TableName);
            Assert.AreEqual("Customer", agent.RemoteOrchestrator.Setup.Tables[0].TableName);

        }

        //[TestMethod]
        //public void SyncAgent_EighthConstructor_LocalOrchestrator_ShouldMatch_WebClientOrchestrator_With_ScopeNameDefined()
        //{
        //    var clientProvider = new SqlSyncProvider();
        //    var options = new SyncOptions();
        //    var setup = new SyncSetup(new string[] { "Customer" });

        //    var remoteOrchestrator = new WebClientOrchestrator("http://localhost/api");

        //    var agent = new SyncAgent(clientProvider, remoteOrchestrator, options, setup, "CustomerScope");

        //    CheckConstructor(agent);
        //    Assert.AreSame(options, agent.LocalOrchestrator.Options);
        //    Assert.AreSame(options, agent.RemoteOrchestrator.Options);
        //    Assert.AreSame(setup, agent.LocalOrchestrator.Setup);
        //    Assert.AreSame(setup, agent.RemoteOrchestrator.Setup);
        //    Assert.AreEqual("CustomerScope", agent.ScopeName);
        //    Assert.AreEqual("CustomerScope", agent.LocalOrchestrator.ScopeName);
        //    Assert.AreEqual("CustomerScope", agent.RemoteOrchestrator.ScopeName);
        //    Assert.AreEqual(1, agent.LocalOrchestrator.Setup.Tables.Count);
        //    Assert.AreEqual(1, agent.RemoteOrchestrator.Setup.Tables.Count);
        //    Assert.AreEqual("Customer", agent.LocalOrchestrator.Setup.Tables[0].TableName);
        //    Assert.AreEqual("Customer", agent.RemoteOrchestrator.Setup.Tables[0].TableName);

        //}
    }
}
