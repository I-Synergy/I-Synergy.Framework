def rw(f, fn):
    b=open(f,'rb').read(); bom=b.startswith(b'\xef\xbb\xbf'); t=b.decode('utf-8-sig'); crlf='\r\n' in t
    t=fn(t.replace('\r\n','\n'))
    if crlf: t=t.replace('\n','\r\n')
    open(f,'wb').write((b'\xef\xbb\xbf' if bom else b'')+t.encode())
def strip_dbg(t): return '\n'.join(l for l in t.split('\n') if '//DBG' not in l)
def vm(t):
    t=strip_dbg(t)
    t=t.replace('    private readonly SettingsDrawerState? _settingsDrawerState;','    private readonly SettingsDrawerState _settingsDrawerState;',1)
    t=t.replace('''        ISettingsService settingsService,
        ILogger<ShellViewModel> logger)''','''        ISettingsService settingsService,
        SettingsDrawerState settingsDrawerState,
        ILogger<ShellViewModel> logger)''',1)
    t=t.replace('''    /// <param name="settingsService"></param>
    /// <param name="logger">The logger factory.</param>
    public ShellViewModel''','''    /// <param name="settingsService"></param>
    /// <param name="settingsDrawerState">The shared open/closed state of the settings drawer.</param>
    /// <param name="logger">The logger factory.</param>
    public ShellViewModel''',1)
    t=t.replace('_settingsDrawerState = SettingsDrawerState.Instance;','_settingsDrawerState = settingsDrawerState;',1)
    t=t.replace('_settingsDrawerState?.Open()','_settingsDrawerState.Open()')
    return t
rw('ViewModels/ShellViewModel.cs',vm)
rw('MauiProgram.cs', lambda t: t.replace('TryAddSingleton(SettingsDrawerState.Instance);','AddSingleton<SettingsDrawerState>();',1))
def st(t):
    a=t.index('	/// <summary>\n	/// Gets the shared instance.'); b=t.index('	private bool _isOpen')
    return t[:a]+t[b:]
rw('Services/SettingsDrawerState.cs',st)
def dr(t):
    t=strip_dbg(t)
    t=t.replace('''		// Subscribe right away: a drawer that starts collapsed may never raise Loaded, and
		// would then miss the request to open.
		EnsureState();
''','')
    t=t.replace('''		_state = SettingsDrawerState.Instance;
''','''		var services = IPlatformApplication.Current?.Services;
		if (services is null)
			return;

		_state = services.GetRequiredService<SettingsDrawerState>();
''',1)
    return t
rw('Views/Controls/SettingsDrawer.xaml.cs',dr)
