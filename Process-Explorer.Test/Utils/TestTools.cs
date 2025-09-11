using Native;
using System.Security.Principal;

internal static class TestTools
{
    public static bool IsAdministrator()
    {
        using (var identity = WindowsIdentity.GetCurrent())
        {
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }

    public static bool TryGetProcessInformation(
        IProcessInformationProvider provider,
        Process proc,
        out ProcessInformation info)
    {
        info = null;
        if (proc == null) return false;

        try
        {
            info = provider.GetProcessInformation(proc);
            return info != null;
        }
        catch
        {
            return false;
        }
    }

    public static int GetProcessIdFromProcessProxy(
        IProcessInformationProvider provider,
        Process p)
    {
        var info = provider.GetProcessInformation(p);
        return (int)info.PID;
    }
}