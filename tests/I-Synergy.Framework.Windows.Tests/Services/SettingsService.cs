using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISynergy.Services
{
    public class SettingsService : BaseSettingsService
    {
        public override Task LoadSettingsAsync() => Task.CompletedTask;
        public override string ApplicationInsights_InstrumentationKey { get => "97874bcf-701b-431d-b4e6-14c140f09fcc"; }
        public override string AppCenter_InstrumentationKey { get => "ed9034e2-7531-45c8-b7df-59a5558a6c76"; }
        public override bool Application_IsFirstRun { get; }
        public override int DefaultCurrencyId { get; }

        public override bool Application_Advanced
        {
            get
            {
                return true;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override T GetSetting<T>(string name, T defaultvalue) => defaultvalue;
        public override bool Application_Update { get; set; }
        public override void CheckForUpgrade() { }
    }
}
