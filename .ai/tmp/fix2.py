def rw(f, fn):
    b=open(f,'rb').read(); bom=b.startswith(b'\xef\xbb\xbf'); t=b.decode('utf-8-sig'); crlf='\r\n' in t
    t=fn(t.replace('\r\n','\n'))
    if crlf: t=t.replace('\n','\r\n')
    open(f,'wb').write((b'\xef\xbb\xbf' if bom else b'')+t.encode())
rw('Services/SettingsDrawerState.cs', lambda t: t.replace('''public sealed class SettingsDrawerState : BindableObject
{
''','''public sealed class SettingsDrawerState : BindableObject
{
	/// <summary>
	/// Gets the shared instance. Pages are constructed before the service provider is reachable,
	/// so the drawer and the shell find the state here rather than through the container.
	/// </summary>
	public static SettingsDrawerState Instance { get; } = new();

''',1))
rw('MauiProgram.cs', lambda t: t.replace('TryAddSingleton<SettingsDrawerState>();','TryAddSingleton(SettingsDrawerState.Instance);',1))
rw('ViewModels/ShellViewModel.cs', lambda t: t.replace('IPlatformApplication.Current?.Services.GetRequiredService<SettingsDrawerState>()','SettingsDrawerState.Instance',1))
def dr(t):
    t=t.replace('''		var services = IPlatformApplication.Current?.Services;
		if (services is null)
			return;

		_state = services.GetRequiredService<SettingsDrawerState>();
''','''		_state = SettingsDrawerState.Instance;
''',1)
    return t
rw('Views/Controls/SettingsDrawer.xaml.cs', dr)
