using System.Diagnostics;
using System.Runtime.InteropServices;

namespace csharpMcp.Tests.Drivers;

public static class NativeAuthenticationDialog
{
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(
        IntPtr hWnd);

    public static async Task SignInAsync(
        string username,
        string password)
    {
        Console.WriteLine(
            "Waiting for Chrome authentication dialog...");

        // Chrome needs time to display the native
        // authentication dialog.
        await Task.Delay(1500);

        var chromeWindow =
            GetForegroundWindow();

        if (chromeWindow == IntPtr.Zero)
        {
            throw new InvalidOperationException(
                "Could not identify the active Chrome window.");
        }

        SetForegroundWindow(chromeWindow);

        await Task.Delay(500);

        Console.WriteLine(
            $"Entering username: {username}");

        SendKeys(username);

        await Task.Delay(300);

        // Username -> Password
        SendKeys("{TAB}");

        await Task.Delay(300);

        Console.WriteLine(
            "Entering password.");

        SendKeys(password);

        await Task.Delay(300);

        // Password -> Sign in
        SendKeys("{TAB}");

        await Task.Delay(300);

        SendKeys("{ENTER}");

        Console.WriteLine(
            "Submitted Chrome authentication dialog.");

        await Task.Delay(1500);
    }

    private static void SendKeys(
        string keys)
    {
        var escaped =
            keys.Replace("'", "''");

        var psi =
            new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments =
                    $"-NoProfile -Command \"Add-Type -AssemblyName System.Windows.Forms; [System.Windows.Forms.SendKeys]::SendWait('{escaped}')\"",
                UseShellExecute = false,
                CreateNoWindow = true
            };

        using var process =
            Process.Start(psi);

        process?.WaitForExit();
    }
}