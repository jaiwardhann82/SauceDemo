using Microsoft.Playwright;

namespace csharpMcp.Tests.Pages;

public class LoginPage
{
    private readonly IPage _page;

    public LoginPage(IPage page)
    {
        _page = page;
    }

    public ILocator UsernameInput => _page.Locator("[data-test=\"username\"]");
    public ILocator PasswordInput => _page.Locator("[data-test=\"password\"]");
    public ILocator LoginButton => _page.Locator("[data-test=\"login-button\"]");

    public async Task NavigateAsync(string url)
    {
        await _page.GotoAsync(url);
    }

    public async Task LoginAsync(string username, string password)
    {
        await UsernameInput.FillAsync(username);
        await PasswordInput.FillAsync(password);
        await LoginButton.ClickAsync();
    }
}
