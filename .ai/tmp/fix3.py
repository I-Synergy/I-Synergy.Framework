import re
for f in ('InfoView','CustomControlsView','MauiControlsView','SlideShowView'):
    p=f'Views/{f}.xaml'
    b=open(p,'rb').read(); bom=b.startswith(b'\xef\xbb\xbf'); t=b.decode('utf-8-sig')
    t=t.replace('xmlns:views="clr-namespace:Sample.Views"','xmlns:controls="clr-namespace:Sample.Views.Controls"').replace('<views:SettingsDrawer />','<controls:SettingsDrawer />')
    open(p,'wb').write((b'\xef\xbb\xbf' if bom else b'')+t.encode())
