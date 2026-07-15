using Microsoft.Extensions.DependencyInjection;
using TestSuite.Core;

namespace TestSuite;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new CoreNavigationPage());
	}
}