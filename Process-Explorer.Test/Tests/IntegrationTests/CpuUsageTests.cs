using Native;

namespace Process_Explorer.Test.Tests.IntegrationTests
{
    [TestClass]
    public class CpuUsageIntegrationTests
    {
        private IProcessManager _manager = default!;
        private IProcessInformationProvider _provider = default!;

        [TestInitialize]
        public void Init()
        {
            // Use real implementations for integration tests
            _manager = new ProcessManager();
            _provider = new ProcessInformationProvider();
        }

        /// <summary>
        /// Verifies that <see cref="ProcessInformation.CpuUsage"/> 
        /// stays within the valid [0,100] range when measured twice.
        ///
        /// Test flow:
        /// 1. Find a process we can safely query (prefer current process if possible).
        /// 2. Collect CPU usage once.
        /// 3. Wait ~600ms to allow CPU counters to update.
        /// 4. Collect CPU usage again.
        /// 5. Ensure the second measurement is within the expected range.
        ///
        /// Notes:
        /// - Some processes may be protected → test marks as inconclusive if info is inaccessible.
        /// - First CPU reading can be zero due to initialization — only second reading is checked strictly.
        /// </summary>
        [TestMethod]
        public async Task CpuUsage_IsWithinReasonableRange_AfterTwoMeasurements()
        {
            var list = await _manager.GetActiveProcessesAsync();
            var processes = list.ToList();

            // Prefer current process (higher chance of accessible CPU counters),
            // otherwise fall back to the first available process.
            var target = processes.FirstOrDefault(p =>
            {
                try
                {
                    var pid = TestTools.GetProcessIdFromProcessProxy(_provider, p);
                    return pid == System.Diagnostics.Process.GetCurrentProcess().Id;
                }
                catch
                {
                    return false; // skip inaccessible processes
                }
            }) ?? processes.FirstOrDefault()!;

            Assert.IsNotNull(target, "No process found to test CPU usage.");

            // First measurement
            if (!TestTools.TryGetProcessInformation(_provider, target, out var info1))
            {
                Assert.Inconclusive("Target process is not accessible for CPU measurement.");
                return;
            }

            // Wait at least half a second so CPU counters can update
            Thread.Sleep(600);

            // Second measurement
            if (!TestTools.TryGetProcessInformation(_provider, target, out var info2))
            {
                Assert.Inconclusive("Second measurement failed — target process may be protected.");
                return;
            }

            // Validate CPU usage range
            Assert.IsTrue(
                info2.CpuUsage >= 0.0 && info2.CpuUsage <= 100.0,
                $"CpuUsage out of range: {info2.CpuUsage}");
        }
    }
}