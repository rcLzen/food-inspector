using Microsoft.UI.Xaml;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace FoodInspector.WinUI;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : MauiWinUIApplication
{
	/// <summary>
	/// Initializes the singleton application object.  This is the first line of authored code
	/// executed, and as such is the logical equivalent of main() or WinMain().
	/// </summary>
	public App()
	{
		// Initialize SQLite provider before any other code runs
		// This must happen early to avoid TypeInitializationException
		try
		{
			SQLitePCL.Batteries_V2.Init();
		}
		catch (Exception ex)
		{
			System.Diagnostics.Debug.WriteLine($"SQLite initialization warning: {ex.Message}");
			// Continue - some platforms may not need explicit init
		}

		this.InitializeComponent();

		// Set up global exception handling to capture inner exceptions
		this.UnhandledException += (sender, e) =>
		{
			System.Diagnostics.Debug.WriteLine($"Unhandled Exception: {e.Exception}");
			if (e.Exception is TypeInitializationException typeInitEx)
			{
				System.Diagnostics.Debug.WriteLine($"TypeName: {typeInitEx.TypeName}");
				System.Diagnostics.Debug.WriteLine($"Inner Exception: {typeInitEx.InnerException}");
			}
		};
	}

	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}

