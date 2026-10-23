namespace Sample.Services;

/// <summary>
/// Shares the open/closed state of the settings drawer between the shell (which opens it
/// from the footer) and the page that hosts the drawer overlay. A single instance is registered
/// in the container, so every page shows the same drawer.
/// </summary>
public sealed class SettingsDrawerState : BindableObject
{
	private bool _isOpen;

	/// <summary>
	/// Gets or sets a value indicating whether the drawer is showing.
	/// </summary>
	public bool IsOpen
	{
		get => _isOpen;
		set
		{
			if (_isOpen == value)
				return;

			_isOpen = value;
			OnPropertyChanged();
		}
	}

	/// <summary>
	/// Opens the drawer.
	/// </summary>
	public void Open() => IsOpen = true;

	/// <summary>
	/// Closes the drawer.
	/// </summary>
	public void Close() => IsOpen = false;
}
