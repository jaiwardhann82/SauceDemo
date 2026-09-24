using csharpMcp.Tests.Pages;
using Microsoft.Playwright;
using NUnit.Framework;
using Reqnroll;

namespace csharpMcp.Tests.StepDefinitions;

[Binding]
public class SauceDemoSteps
{
    private readonly IPage _page;
    private readonly LoginPage _loginPage;
    private readonly InventoryPage _inventoryPage;
    private readonly CartPage _cartPage;
    private readonly CheckoutStepOnePage _checkoutStepOnePage;
    private readonly CheckoutStepTwoPage _checkoutStepTwoPage;
    private readonly CheckoutCompletePage _checkoutCompletePage;

    public SauceDemoSteps(IPage page)
    {
        _page = page;
        _loginPage = new LoginPage(page);
        _inventoryPage = new InventoryPage(page);
        _cartPage = new CartPage(page);
        _checkoutStepOnePage = new CheckoutStepOnePage(page);
        _checkoutStepTwoPage = new CheckoutStepTwoPage(page);
        _checkoutCompletePage = new CheckoutCompletePage(page);
    }

    [Given(@"I navigate to SauceDemo login page ""(.*)""")]
    public async Task GivenINavigateToSauceDemoLoginPage(string url)
    {
        await _loginPage.NavigateAsync(url);
    }

    [When(@"I log in with username ""(.*)"" and password ""(.*)""")]
    public async Task WhenILogInWithUsernameAndPassword(string username, string password)
    {
        await _loginPage.LoginAsync(username, password);
    }

    [When(@"I add the following 5 products to the cart and verify the button changes to ""(.*)"":")]
    public async Task WhenIAddTheFollowingProductsToTheCartAndVerifyTheButtonChangesTo(string expectedButtonCaption, DataTable table)
    {
        foreach (var row in table.Rows)
        {
            var productName = row["ProductName"];
            
            // Check initial button text is "Add to cart"
            var initialButtonText = await _inventoryPage.GetButtonTextAsync(productName);
            Assert.That(initialButtonText, Is.EqualTo("Add to cart"), $"Expected initial button for {productName} to be 'Add to cart'");

            // Click Add to cart
            await _inventoryPage.AddProductToCartAsync(productName);

            // Verify button caption has changed to "Remove"
            var updatedButtonText = await _inventoryPage.GetButtonTextAsync(productName);
            Assert.That(updatedButtonText, Is.EqualTo(expectedButtonCaption), $"Expected button caption for {productName} to change to '{expectedButtonCaption}'");
        }
    }

    [When(@"I click on the shopping cart")]
    public async Task WhenIClickOnTheShoppingCart()
    {
        await _inventoryPage.ClickShoppingCartAsync();
    }

    [When(@"I click on the checkout button")]
    public async Task WhenIClickOnTheCheckoutButton()
    {
        await _cartPage.ClickCheckoutAsync();
    }

    [When(@"I enter checkout information with first name ""(.*)"", last name ""(.*)"", and postal code ""(.*)""")]
    public async Task WhenIEnterCheckoutInformationWithFirstNameLastNameAndPostalCode(string firstName, string lastName, string postalCode)
    {
        await _checkoutStepOnePage.FillInformationAsync(firstName, lastName, postalCode);
    }

    [When(@"I click continue to proceed to checkout overview")]
    public async Task WhenIClickContinueToProceedToCheckoutOverview()
    {
        await _checkoutStepOnePage.ClickContinueAsync();
    }

    [Then(@"I verify the quantity and description for all added products:")]
    public async Task ThenIVerifyTheQuantityAndDescriptionForAllAddedProducts(DataTable table)
    {
        foreach (var row in table.Rows)
        {
            var productName = row["ProductName"];
            var expectedQuantity = row["Quantity"];
            var expectedDescription = row["Description"];

            var actualQuantity = await _checkoutStepTwoPage.GetItemQuantityAsync(productName);
            var actualDescription = await _checkoutStepTwoPage.GetItemDescriptionAsync(productName);

            Assert.That(actualQuantity, Is.EqualTo(expectedQuantity), $"Quantity mismatch for product {productName}");
            Assert.That(actualDescription, Is.EqualTo(expectedDescription), $"Description mismatch for product {productName}");
        }
    }

    [When(@"I click on the finish button")]
    public async Task WhenIClickOnTheFinishButton()
    {
        await _checkoutStepTwoPage.ClickFinishAsync();
    }

    [When(@"I click on the back home button")]
    public async Task WhenIClickOnTheBackHomeButton()
    {
        await _checkoutCompletePage.ClickBackHomeAsync();
    }

    [Then(@"I should be navigated back to the inventory home page")]
    public async Task ThenIShouldBeNavigatedBackToTheInventoryHomePage()
    {
        await Assertions.Expect(_page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex("inventory\\.html"));
    }
}
