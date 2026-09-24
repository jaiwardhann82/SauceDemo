using Microsoft.Playwright;

namespace csharpMcp.Tests.Pages;

public class CheckoutStepOnePage
{
    private readonly IPage _page;

    public CheckoutStepOnePage(IPage page)
    {
        _page = page;
    }

    public ILocator FirstNameInput => _page.Locator("[data-test=\"firstName\"]");
    public ILocator LastNameInput => _page.Locator("[data-test=\"lastName\"]");
    public ILocator PostalCodeInput => _page.Locator("[data-test=\"postalCode\"]");
    public ILocator ContinueButton => _page.Locator("[data-test=\"continue\"]");

    public async Task FillInformationAsync(string firstName, string lastName, string postalCode)
    {
        await FirstNameInput.FillAsync(firstName);
        await LastNameInput.FillAsync(lastName);
        await PostalCodeInput.FillAsync(postalCode);
    }

    public async Task ClickContinueAsync()
    {
        await ContinueButton.ClickAsync();
    }
}
