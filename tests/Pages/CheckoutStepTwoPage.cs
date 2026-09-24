using Microsoft.Playwright;

namespace csharpMcp.Tests.Pages;

public class CheckoutStepTwoPage
{
    private readonly IPage _page;

    public CheckoutStepTwoPage(IPage page)
    {
        _page = page;
    }

    public ILocator CartItems => _page.Locator("[data-test=\"inventory-item\"]");
    public ILocator FinishButton => _page.Locator("[data-test=\"finish\"]");

    public ILocator GetCartItemByName(string productName)
    {
        return _page.Locator("[data-test=\"inventory-item\"]").Filter(new() { HasText = productName });
    }

    public async Task<string> GetItemQuantityAsync(string productName)
    {
        var item = GetCartItemByName(productName);
        return (await item.Locator("[data-test=\"item-quantity\"]").InnerTextAsync()).Trim();
    }

    public async Task<string> GetItemDescriptionAsync(string productName)
    {
        var item = GetCartItemByName(productName);
        return (await item.Locator("[data-test=\"inventory-item-desc\"]").InnerTextAsync()).Trim();
    }

    public async Task ClickFinishAsync()
    {
        await FinishButton.ClickAsync();
    }
}
