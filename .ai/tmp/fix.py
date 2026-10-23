for f,msg in (('ViewModels/ShellViewModel.cs','OpenSettingsAsync'),('Views/Controls/SettingsDrawer.xaml.cs','OnStateChanged')):
    b=open(f,'rb').read(); bom=b.startswith(b'\xef\xbb\xbf'); t=b.decode('utf-8-sig')
    nl='\r\n' if '\r\n' in t else '\n'
    out=[]
    for l in t.split(nl):
        if 'AppendAllText' in l and ('$"{DateTime' in l):
            continue
        if 'AppendAllText' in l:
            ind='        ' if f.endswith('ShellViewModel.cs') else '\t\t'
            l=ind+'System.IO.File.AppendAllText("D:/Projects/I-Synergy/I-Synergy.Framework/.ai/tmp/dbg.txt", "'+msg+' " + DateTime.Now.ToString("T") + System.Environment.NewLine); //DBG'
        out.append(l)
    open(f,'wb').write((b'\xef\xbb\xbf' if bom else b'')+nl.join(out).encode())
