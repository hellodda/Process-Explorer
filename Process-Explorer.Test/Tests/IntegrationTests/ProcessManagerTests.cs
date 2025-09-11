using Native;

namespace Process_Explorer.Test.Tests.IntegrationTests
{
    [TestClass]
    public class ProcessManagerIntegrationTests
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

        /// <summary>
        /// Ensures that <see cref="IProcessManager.GetActiveProcessesAsync"/> 
        /// returns a non-empty list and that at least one process is accessible
        /// via <see cref="IProcessInformationProvider"/>.
        /// 
        /// Additionally verifies basic sanity checks on the first accessible process:
        /// - PID must be non-zero
        /// - Name must not be empty
        /// - WorkingSet must be non-negative
        /// </summary>
        [TestMethod]
        public async Task GetAllProcesses_ReturnsEnumerable_And_AtLeastOneAccessible()
        {
            var list = await _manager.GetActiveProcessesAsync();
            Assert.IsNotNull(list, "Process list must not be null.");

            var processes = list.ToList();
            Assert.IsTrue(processes.Count > 0, "Expected at least one process from the OS.");

            // Try to obtain process information for each process
            var accessibleInfos = new List<ProcessInformation>();
            foreach (var p in processes)
            {
                if (TestTools.TryGetProcessInformation(_provider, p, out var info))
                    accessibleInfos.Add(info);
            }

            // At least one process should be accessible (e.g. current process)
            Assert.IsTrue(accessibleInfos.Count >= 1, "No accessible processes found.");

            // Basic sanity checks on first accessible process
            var first = accessibleInfos.First();
            Assert.IsTrue(first.PID != 0, "PID must be non-zero.");
            Assert.IsFalse(string.IsNullOrEmpty(first.Name), "Process name must not be empty.");
            Assert.IsTrue(first.WorkingSet >= 0, "WorkingSet must be non-negative.");
        }

        /// <summary>
        /// Ensures that <see cref="IProcessInformationProvider"/> 
        /// produces <see cref="ProcessInformation"/> objects with unique PIDs.
        /// 
        /// This test guards against provider bugs that could duplicate or reuse PID values.
        /// </summary>
        [TestMethod]
        public async Task ProcessInformationProvider_ProducesUniquePIDs()
        {
            var list = await _manager.GetActiveProcessesAsync();
            var processes = list.ToList();

            // Collect information for all accessible processes
            var infos = new List<ProcessInformation>();
            foreach (var p in processes)
            {
                if (TestTools.TryGetProcessInformation(_provider, p, out var info))
                    infos.Add(info);
            }

            Assert.IsTrue(infos.Count > 0, "No accessible ProcessInformation instances found.");

            // Check uniqueness of PIDs
            var pidSet = new HashSet<uint>(infos.Select(i => i.PID));
            Assert.AreEqual(pidSet.Count, infos.Count, "PIDs must be unique across accessible processes.");
        }
    }
}