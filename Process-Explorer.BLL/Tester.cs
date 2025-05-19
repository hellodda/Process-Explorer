namespace Process_Explorer.BLL
{
    public static class Tester
    {
       public static void GetInfo()
       {
            var list = Native.ProcessManager.GetActiveProcesses().ToList();

            foreach (var process in list) {

                var info = process?.GetProcessInformation()!;

                if (info.PID != 0)
                {
                    Console.WriteLine($"Process ID: {info.PID}");
                    Console.WriteLine($"Process Name: {info.Name}");
                    Console.WriteLine($"Working Set: {info.WorkingSet}");
                    Console.WriteLine($"Private Bytes: {info.PrivateBytes}");
                    Console.WriteLine($"Company: {info.Company}");
                    Console.WriteLine($"Description: {info.Description}");
                }
            }
        }
    }
}
