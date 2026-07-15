using System;

namespace TestSuite.Core;

public class CoreNavigationPage : NavigationPage
{
    public CoreNavigationPage()
    {
        this.PushAsync(new CorePage());
    }
}
