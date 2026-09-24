using Microsoft.Playwright;

namespace csharpMcp.Tests.Pages;

public class InventoryPage
{
    private readonly IPage _page;

    public InventoryPage(IPage page)
    {
        _page = page;
    }

    public ILocator InventoryItems => _page.Locator("[data-test=\"inventory-item\"]");
    public ILocator ShoppingCartLink => _page.Locator("[data-test=\"shopping-cart-link\"]");

    public ILocator GetProductItem(string productName)
    {
        return _page.Locator("[data-test=\"inventory-item\"]").Filter(new() { HasText = productName });
    }

    public ILocator GetAddToCartButton(string productName)
    {
        var item = GetProductItem(productName);
        return item.Locator("button.btn_inventory");
    }

    public async Task AddProductToCartAsync(string productName)
    {
        var button = GetAddToCartButton(productName);
        await button.ClickAsync();
    }

    public async Task<string> GetButtonTextAsync(string productName)
    {
        var button = GetAddToCartButton(productName);
        return await button.InnerTextAsync();
    }

    public async Task ClickShoppingCartAsync()
    {
        await ShoppingCartLink.ClickAsync();
    }
}
