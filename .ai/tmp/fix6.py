def rw(f, fn):
    b=open(f,'rb').read(); bom=b.startswith(b'\xef\xbb\xbf'); t=b.decode('utf-8-sig'); crlf='\r\n' in t
    t=fn(t.replace('\r\n','\n'))
    if crlf: t=t.replace('\n','\r\n')
    open(f,'wb').write((b'\xef\xbb\xbf' if bom else b'')+t.encode())
def sv(t):
    a=t.index('    private void UpdateShellContentTitles()')
    b=t.index('    #region IDisposable')
    new='''    private static readonly string[] TitleKeys = ["Dashboard", "CustomControls", "MauiControls", "SlideShow"];

    private void UpdateShellContentTitles()
    {
        if (_localizedStrings is null)
            return;

        // Each page is an implicit ShellItem > ShellSection > ShellContent chain; the flyout
        // reads the title at any of the three levels, so keep them in step.
        for (var i = 0; i < Items.Count && i < TitleKeys.Length; i++)
        {
            var title = _localizedStrings[TitleKeys[i]];
            var item = Items[i];
            item.Title = title;

            foreach (var section in item.Items)
            {
                section.Title = title;

                foreach (var content in section.Items)
                    content.Title = title;
            }
        }

        if (this.FindByName<NavigationItem>("HelpItem") is NavigationItem helpItem)
            helpItem.Text = _localizedStrings["Help"];
    }

'''
    return t[:a]+new+t[b:]
rw('Views/ShellView.xaml.cs',sv)
def dr(t):
    # drawer only refreshes the strings; ShellView listens and updates the titles
    a=t.index('				// Update shell titles and help item (if shell is visible)')
    b=t.index('			}\n\n			// Update FlowDirection')
    t=t[:a].rstrip('\t\n')+'\n'+t[b:]
    a=t.index('	private static void RefreshShellTitles'); b=t.index('	private void OnThemeChanged')
    return t[:a]+t[b:]
rw('Views/Controls/SettingsDrawer.xaml.cs',dr)
