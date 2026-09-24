using Microsoft.Playwright;

namespace csharpMcp.Tests.Pages;

public class CheckoutCompletePage
{
    private readonly IPage _page;

    public CheckoutCompletePage(IPage page)
    {
        _page = page;
    }

    public ILocator CompleteHeader => _page.Locator("[data-test=\"complete-header\"]");
    public ILocator BackHomeButton => _page.Locator("[data-test=\"back-to-products\"]");

    public async Task ClickBackHomeAsync()
    {
        await BackHomeButton.ClickAsync();
    }
}
