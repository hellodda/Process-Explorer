using Native;
using System.Security.Principal;

namespace Process_Explorer.Test
{
    [TestClass]
    public class IntegrationTests
    {
        private IProcessManager _manager = default!;
        private IProcessInformationProvider _provider = default!;

        [TestInitialize]
        public void Init()
        {
            // Use the real implementations for integration tests
            _manager = new ProcessManager();
            _provider = new ProcessInformationProvider();
        }

        private static bool IsAdministrator()
        {
            using (var identity = WindowsIdentity.GetCurrent())
            {
                var principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        /// <summary>
        /// Try to obtain ProcessInformation for the given process.
        /// Returns true if we got a non-null information object, false otherwise.
        /// Any exception from native code is swallowed and considered "not accessible".
        /// </summary>
        private bool TryGetProcessInformation(Process proc, out ProcessInformation info)
        {
            info = null;
            if (proc == null) return false;

            try
            {
                info = _provider.GetProcessInformation(proc);
                return info != null;
            }
            catch (Exception)
            {
                // Native/WinAPI may throw for protected processes — treat as "inaccessible".
                return false;
            }
        }

        [TestMethod]
        public async Task SetPrivileges_Behavior_IsAdminAware()
        {
            // This test verifies Application.SetPrivileges behavior both when running as admin and not.
            if (IsAdministrator())
            {
                // When running as admin, SetPrivileges should not throw.
                await Task.Run(() => Application.SetPrivileges());
            }
            else
            {
                // When not admin, SetPrivileges may either:
                //  - succeed (unexpected but acceptable), or
                //  - throw ApplicationException with a message that explains admin is required.
                try
                {
                    Application.SetPrivileges();
                    // succeed — mark test inconclusive because behavior varies across environments
                    Assert.Inconclusive("SetPrivileges succeeded while test not running as administrator. If you expect strict admin-only behavior, run tests as admin.");
                }
                catch (ApplicationException ex)
                {
                    StringAssert.Contains(ex.Message.ToLowerInvariant(), "admin", "Expected exception message to mention 'admin' when privileges can't be set.");
                }
            }
        }

        [TestMethod]
        public async Task SetPriority_Behavior_IsAdminAware()
        {
            // Same approach as SetPrivileges: verify it either succeeds (admin) or fails with informative message.
            if (IsAdministrator())
            {
                await Task.Run(() => Application.SetPriority());
            }
            else
            {
                try
                {
                    Application.SetPriority();
                    Assert.Inconclusive("SetPriority succeeded while test not running as administrator. Run as admin for strict verification.");
                }
                catch (ApplicationException ex)
                {
                    StringAssert.Contains(ex.Message.ToLowerInvariant(), "admin", "Expected exception message to mention 'admin' when priority cannot be set.");
                }
            }
        }

        [TestMethod]
        public async Task GetAllProcesses_ReturnsEnumerable_And_AtLeastOneAccessible()
        {
            var list = await _manager.GetActiveProcessesAsync();

            Assert.IsNotNull(list, "GetActiveProcessesAsync must not return null.");

            var processes = list.ToList();
            Assert.IsTrue(processes.Count > 0, "Expected at least one process to be returned by the OS.");

            // Try to get ProcessInformation for each process and count how many are accessible.
            var accessibleInfos = new List<ProcessInformation>();
            foreach (var p in processes)
            {
                if (TryGetProcessInformation(p, out var info))
                    accessibleInfos.Add(info);
            }

            // At least one process should be accessible without admin (e.g. current test runner process).
            Assert.IsTrue(accessibleInfos.Count >= 1, "No accessible processes found. Try running tests with elevated privileges.");

            // Basic sanity checks on the first accessible info
            var first = accessibleInfos.First();
            Assert.IsTrue(first.PID != 0, "PID must be non-zero.");
            Assert.IsFalse(string.IsNullOrEmpty(first.Name), "Process name should not be empty.");
            Assert.IsTrue(first.WorkingSet >= 0, "WorkingSet should be non-negative.");
        }

        [TestMethod]
        public async Task ProcessInformationProvider_ProducesUniquePIDs_ForAccessibleProcesses()
        {
            var list = await _manager.GetActiveProcessesAsync();
            var processes = list.ToList();

            var infos = new List<ProcessInformation>();
            foreach (var p in processes)
            {
                if (TryGetProcessInformation(p, out var info))
                    infos.Add(info);
            }

            Assert.IsTrue(infos.Count > 0, "No accessible ProcessInformation instances found.");

            var pidSet = new HashSet<uint>(infos.Select(i => i.PID));
            Assert.AreEqual(pidSet.Count, infos.Count, "Accessible ProcessInformation entries must have unique PIDs.");
        }

        [TestMethod]
        public async Task CpuUsage_IsWithinReasonableRange_AfterTwoMeasurements()
        {
            // Find a process that we can query twice (preferably our current process)
            var list = await _manager.GetActiveProcessesAsync();
            var processes = list.ToList();

            // Prefer the current process (if present) to increase chance of accessible CPU counters
            Process target = processes.FirstOrDefault(p =>
            {
                try
                {
                    var pid = GetProcessIdFromProcessProxy(p);
                    return pid == System.Diagnostics.Process.GetCurrentProcess().Id;
                }
                catch { return false; }
            }) ?? processes.FirstOrDefault()!;

            Assert.IsNotNull(target, "No process found to measure CPU on.");

            // Try first retrieval
            if (!TryGetProcessInformation(target, out var info1))
            {
                Assert.Inconclusive("Target process is not accessible for ProcessInformation. Run tests as admin for full coverage.");
                return;
            }

            // Wait a bit to allow the CPU calculator to have different measurements.
            Thread.Sleep(600); // 600 ms — enough for a second measurement

            if (!TryGetProcessInformation(target, out var info2))
            {
                Assert.Inconclusive("Second measurement failed — target process may be protected.");
                return;
            }

            // CpuUsage may be 0 on the very first overall measurement (calculator behavior).
            // We ensure it's within [0, 100] range for the second reading.
            Assert.IsTrue(info2.CpuUsage >= 0.0 && info2.CpuUsage <= 100.0, $"CpuUsage out of range: {info2.CpuUsage}");
        }

        /// <summary>
        /// Helper to extract PID from a Process instance.
        /// We must call the underlying method(s) that your Process wrapper exposes.
        /// If Process doesn't expose PID directly, adjust this helper (for example, call GetProcessInformation()).
        /// </summary>
        private int GetProcessIdFromProcessProxy(Process p)
        {
            // Prefer direct method if exists — but safe fallback to provider
            try
            {
                var info = _provider.GetProcessInformation(p);
                return (int)info.PID;
            }
            catch
            {
                // If provider fails, try alternative (if your Process exposes GetHandle/GetId etc.)
                // Throw to indicate we couldn't get an id
                throw;
            }
        }
    }
}
