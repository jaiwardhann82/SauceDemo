using Microsoft.Playwright;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace csharpMcp.Tests.Drivers;

public class PlaywrightDriver : IAsyncDisposable
{
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private Process? _chromeProcess;
    private string? _userDataDir;

    public async Task<IPage> InitializeAsync()
    {
        Console.WriteLine("Starting Playwright...");

        _playwright = await Playwright.CreateAsync();

        // Find an available local port.
        int port = GetFreePort();

        Console.WriteLine(
            $"Starting Google Chrome on CDP port {port}...");

        string chromePath =
            Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.ProgramFiles),
                "Google",
                "Chrome",
                "Application",
                "chrome.exe");

        if (!File.Exists(chromePath))
        {
            chromePath =
                Path.Combine(
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.ProgramFilesX86),
                    "Google",
                    "Chrome",
                    "Application",
                    "chrome.exe");
        }

        if (!File.Exists(chromePath))
        {
            throw new FileNotFoundException(
                "Google Chrome could not be found.",
                chromePath);
        }

        // Use a completely separate Chrome profile.
        // This prevents your normal Chrome session/cookies
        // from interfering with the test.
        _userDataDir = Path.Combine(
            Path.GetTempPath(),
            "DealCaptureChrome_" + Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(_userDataDir);

        var arguments =
            $"--remote-debugging-port={port} " +
            $"--user-data-dir=\"{_userDataDir}\" " +
            "--incognito " +
            "--new-window " +
            "about:blank";

        Console.WriteLine(
            "Launching Chrome in Incognito mode...");

        _chromeProcess = Process.Start(
            new ProcessStartInfo
            {
                FileName = chromePath,
                Arguments = arguments,
                UseShellExecute = true
            });

        if (_chromeProcess == null)
        {
            throw new InvalidOperationException(
                "Could not start Google Chrome.");
        }

        Console.WriteLine(
            "Chrome process started.");

        // Wait until Chrome exposes the CDP endpoint.
        string cdpUrl =
            $"http://127.0.0.1:{port}";

        Console.WriteLine(
            "Waiting for Chrome remote debugging...");

        await WaitForChromeAsync(
            cdpUrl,
            TimeSpan.FromSeconds(15));

        Console.WriteLine(
            "Connecting Playwright to Chrome...");

        _browser =
            await _playwright.Chromium.ConnectOverCDPAsync(
                cdpUrl);

        Console.WriteLine(
            "Playwright connected to Chrome.");

        var context =
            _browser.Contexts.FirstOrDefault();

        if (context == null)
        {
            throw new InvalidOperationException(
                "Chrome browser context was not found.");
        }

        var page =
            context.Pages.FirstOrDefault();

        if (page == null)
        {
            page = await context.NewPageAsync();
        }

        Console.WriteLine(
            "Playwright page connected.");

        return page;
    }

    private static int GetFreePort()
    {
        using var listener =
            new TcpListener(
                IPAddress.Loopback,
                0);

        listener.Start();

        return ((IPEndPoint)listener.LocalEndpoint)
            .Port;
    }

    private static async Task WaitForChromeAsync(
        string cdpUrl,
        TimeSpan timeout)
    {
        using var client = new HttpClient();

        var end =
            DateTime.UtcNow.Add(timeout);

        while (DateTime.UtcNow < end)
        {
            try
            {
                var response =
                    await client.GetAsync(
                        $"{cdpUrl}/json/version");

                if (response.IsSuccessStatusCode)
                {
                    return;
                }
            }
            catch
            {
                // Chrome is still starting.
            }

            await Task.Delay(250);
        }

        throw new TimeoutException(
            "Chrome remote debugging endpoint did not become available.");
    }

    public async ValueTask DisposeAsync()
    {
        Console.WriteLine(
            "Closing Playwright browser...");

        if (_browser != null)
        {
            try
            {
                await _browser.CloseAsync();
            }
            catch
            {
                // Chrome may already be closed.
            }
        }

        if (_chromeProcess != null &&
            !_chromeProcess.HasExited)
        {
            try
            {
                _chromeProcess.Kill(
                    entireProcessTree: true);
            }
            catch
            {
                // Ignore cleanup errors.
            }
        }

        _playwright?.Dispose();

        // Remove temporary Chrome profile.
        if (!string.IsNullOrWhiteSpace(_userDataDir) &&
            Directory.Exists(_userDataDir))
        {
            try
            {
                Directory.Delete(
                    _userDataDir,
                    recursive: true);
            }
            catch
            {
                // Ignore cleanup errors.
            }
        }

        Console.WriteLine(
            "Playwright browser closed.");
    }
}