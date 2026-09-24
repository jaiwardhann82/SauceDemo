using Microsoft.Playwright;

namespace csharpMcp.Tests.Pages;

public class CartPage
{
    private readonly IPage _page;

    public CartPage(IPage page)
    {
        _page = page;
    }

    public ILocator CartItems => _page.Locator("[data-test=\"inventory-item\"]");
    public ILocator CheckoutButton => _page.Locator("[data-test=\"checkout\"]");

    public async Task ClickCheckoutAsync()
    {
        await CheckoutButton.ClickAsync();
    }
}
