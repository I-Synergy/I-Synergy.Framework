f='ViewModels/ShellViewModel.cs'
t=open(f,encoding='utf-8',newline='').read()
a=t.index('    public override async Task InitializeFirstRunAsync()')
b=t.index('    /// <summary>', a)
new='''    public override async Task InitializeFirstRunAsync()
    {
        if (_settingsService.GlobalSettings is { IsFirstRun: true } globalSettings)
        {
            // First run: offer language, theme and accent in the settings drawer (applied live,
            // no restart), then remember that the introduction has been shown.
            _settingsDrawerState?.Open();

            globalSettings.IsFirstRun = false;
            await _settingsService.AddOrUpdateGlobalSettingsAsync(globalSettings);
        }
    }

'''
# find end of the old method: it ends before the next member after InitializeFirstRunAsync
import re
m=re.search(r'\n    (/// <summary>|protected override|public override)', t[a+10:])
end=a+10+m.start()+1
t=t[:a]+new+t[end:]
open(f,'w',encoding='utf-8',newline='').write(t)
