using NUnit.Framework;
using UITests.Core;

namespace UITests;

public class PlatformSpecificSampleTest : BaseTest
{
	[Test]
	public void SampleTest()
	{
		App.GetScreenshot().SaveAsFile($"{nameof(SampleTest)}.png");
	}
}
