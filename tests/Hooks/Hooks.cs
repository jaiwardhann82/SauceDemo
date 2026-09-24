using csharpMcp.Tests.Drivers;
using Microsoft.Playwright;
using Reqnroll;
using Reqnroll.BoDi;

namespace csharpMcp.Tests.Hooks;

[Binding]
public class Hooks
{
    private readonly IObjectContainer _container;

    private PlaywrightDriver? _driver;

    public Hooks(IObjectContainer container)
    {
        _container = container;
    }

    [BeforeScenario]
    public async Task BeforeScenario()
    {
        Console.WriteLine(
            "==========================================");

        Console.WriteLine(
            "Starting Deal Capture test");

        Console.WriteLine(
            "==========================================");

        // Start Playwright and Chrome.
        // Authentication credentials are NOT passed here.
        _driver = new PlaywrightDriver();

        var page = await _driver.InitializeAsync();

        _container.RegisterInstanceAs(_driver);
        _container.RegisterInstanceAs(page);
    }

    [AfterScenario]
    public async Task AfterScenario()
    {
        if (_driver != null)
        {
            await _driver.DisposeAsync();
        }
    }
}