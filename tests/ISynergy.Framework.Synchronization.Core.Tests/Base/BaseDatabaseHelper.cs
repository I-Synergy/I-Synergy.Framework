using ISynergy.Framework.Synchronization.Core.Abstractions.Tests;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ISynergy.Framework.Synchronization.Core.Tests.Base
{
    /// <summary>
    /// Abstract class for the database helper.
    /// </summary>
    public abstract class BaseDatabaseHelper : IDatabaseHelper
    {
        protected readonly IConfigurationRoot _configuration;

        protected BaseDatabaseHelper()
        {
            _configuration = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json", false, true)
              .AddJsonFile("appsettings.local.json", true, true)
              .Build();
        }

        /// <summary>
        /// Get randon name.
        /// </summary>
        /// <param name="pref"></param>
        /// <returns></returns>
        public string GetRandomName(string pref = default)
        {
            var str1 = Path.GetRandomFileName().Replace(".", "").ToLowerInvariant();
            return $"{pref}{str1}";
        }

        public abstract string GetConnectionString(string dbName);

        /// <summary>
        /// Gets if the tests are running on AppVeyor
        /// </summary>
        protected bool IsOnAppVeyor
        {
            get
            {
                // check if we are running on appveyor or not
                var isOnAppVeyor = Environment.GetEnvironmentVariable("APPVEYOR");
                return !string.IsNullOrEmpty(isOnAppVeyor) && isOnAppVeyor.ToLowerInvariant() == "true";
            }
        }

        /// <summary>
        /// Gets if the tests are running on Azure Dev
        /// </summary>
        protected bool IsOnAzureDev
        {
            get
            {
                // check if we are running on appveyor or not
                var isOnAzureDev = Environment.GetEnvironmentVariable("AZUREDEV");
                return !string.IsNullOrEmpty(isOnAzureDev) && isOnAzureDev.ToLowerInvariant() == "true";
            }
        }

        public abstract Task CreateDatabaseAsync(string dbName, bool recreateDb = true);
        public abstract void DropDatabase(string dbName);
        public abstract Task ExecuteScriptAsync(string dbName, string script);
        public abstract void ClearAllPools();
    }
}
