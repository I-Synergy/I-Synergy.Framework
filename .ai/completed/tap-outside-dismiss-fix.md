# Tap-Outside Dismiss: Consistent Sibling-Overlay Pattern

Status: DONE
Started: 2026-08-15

## Goal

Apply a consistent "tap outside to dismiss" pattern across all MAUI dialog windows so that:
1. Tapping outside the dialog box cancels (dismisses) the dialog.
2. Tapping inside the dialog box does NOT bubble to the cancel handler (fixes tap-through bug, e.g. selecting a CollectionView item closing the selection dialog).

Pattern: outer `<Grid>` with two siblings — a full-screen transparent backdrop `<Grid>` carrying the `TapGestureRecognizer -> CancelCommand`, and the dialog `<ContentView>` declared after it (on top, so its taps don't bubble into the backdrop).

## Steps

- [x] SingleSelectionWindow.xaml — wrap ContentView in outer Grid + backdrop sibling
- [x] MultipleSelectionWindow.xaml — wrap ContentView in outer Grid + backdrop sibling
- [x] NoteWindow.xaml — convert wrapping Grid to sibling backdrop
- [x] ThemeWindow.xaml — convert wrapping Grid to sibling backdrop
- [x] LanguageWindow.xaml — convert wrapping Grid to sibling backdrop
- [x] Build ISynergy.Framework.UI.Maui (0 warnings / 0 errors)

## Notes

- The Note/Theme/Language windows currently use the broken wrapping pattern (`<Grid Background="Transparent">` containing the ContentView as a child); they must be converted to the sibling pattern too.
