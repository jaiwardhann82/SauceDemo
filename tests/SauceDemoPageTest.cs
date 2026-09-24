using System.Text.RegularExpressions;
using csharpMcp.Tests.Pages;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace csharpMcp.Tests;

[TestFixture]
public class SauceDemoPageTest : PageTest
{
    private LoginPage _loginPage = null!;
    private InventoryPage _inventoryPage = null!;
    private CartPage _cartPage = null!;
    private CheckoutStepOnePage _checkoutStepOnePage = null!;
    private CheckoutStepTwoPage _checkoutStepTwoPage = null!;
    private CheckoutCompletePage _checkoutCompletePage = null!;

    [SetUp]
    public void SetupPages()
    {
        _loginPage = new LoginPage(Page);
        _inventoryPage = new InventoryPage(Page);
        _cartPage = new CartPage(Page);
        _checkoutStepOnePage = new CheckoutStepOnePage(Page);
        _checkoutStepTwoPage = new CheckoutStepTwoPage(Page);
        _checkoutCompletePage = new CheckoutCompletePage(Page);
    }

    [Test]
    public async Task AddFiveProductsAndCheckoutSuccessfully()
    {
        // 1. Navigate to SauceDemo
        await _loginPage.NavigateAsync("https://www.saucedemo.com/?utm_source=chatgpt.com");

        // 2. Login
        await _loginPage.LoginAsync("standard_user", "secret_sauce");

        // 3. Select 5 products and check button changes to Remove
        var products = new List<(string Name, string Description, string Quantity)>
        {
            ("Sauce Labs Backpack", "carry.allTheThings() with the sleek, streamlined Sly Pack that melds uncompromising style with unequaled laptop and tablet protection.", "1"),
            ("Sauce Labs Bike Light", "A red light isn't the desired state in testing but it sure helps when riding your bike at night. Water-resistant with 3 lighting modes, 1 AAA battery included.", "1"),
            ("Sauce Labs Bolt T-Shirt", "Get your testing superhero on with the Sauce Labs bolt T-shirt. From American Apparel, 100% ringspun combed cotton, heather gray with red bolt.", "1"),
            ("Sauce Labs Fleece Jacket", "It's not every day that you come across a midweight quarter-zip fleece jacket capable of handling everything from a relaxing day outdoors to a busy day at the office.", "1"),
            ("Sauce Labs Onesie", "Rib snap infant onesie for the junior automation engineer in development. Reinforced 3-snap bottom closure, two-needle hemmed sleeved and bottom won't unravel.", "1")
        };

        foreach (var product in products)
        {
            var initialText = await _inventoryPage.GetButtonTextAsync(product.Name);
            Assert.That(initialText, Is.EqualTo("Add to cart"));

            await _inventoryPage.AddProductToCartAsync(product.Name);

            var updatedText = await _inventoryPage.GetButtonTextAsync(product.Name);
            Assert.That(updatedText, Is.EqualTo("Remove"));
        }

        // 4. Click cart
        await _inventoryPage.ClickShoppingCartAsync();

        // 5. Click checkout
        await _cartPage.ClickCheckoutAsync();

        // 6. Enter first name, last name, and zip code
        await _checkoutStepOnePage.FillInformationAsync("John", "Doe", "12345");
        await _checkoutStepOnePage.ClickContinueAsync();

        // 7. Verify quantity and description for all added products
        foreach (var product in products)
        {
            var actualQty = await _checkoutStepTwoPage.GetItemQuantityAsync(product.Name);
            var actualDesc = await _checkoutStepTwoPage.GetItemDescriptionAsync(product.Name);

            Assert.That(actualQty, Is.EqualTo(product.Quantity), $"Quantity mismatch for {product.Name}");
            Assert.That(actualDesc, Is.EqualTo(product.Description), $"Description mismatch for {product.Name}");
        }

        // 8. Click Finish
        await _checkoutStepTwoPage.ClickFinishAsync();

        // 9. Go back to home page
        await _checkoutCompletePage.ClickBackHomeAsync();

        // Verify we are back on inventory page
        await Expect(Page).ToHaveURLAsync(new Regex("inventory\\.html"));
    }
}
