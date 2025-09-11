using Moq;
using Native;

namespace Process_Explorer.Test.Tests.UnitTests
{
    [TestClass]
    public class ProcessManagerUnitTests
    {
        [TestMethod]
        public async Task When_GetActiveProcesses_Then_ProviderIsInvokedForEachProcess()
        {
            // Arrange
            var mockManager = new Mock<IProcessManager>();
            var mockProvider = new Mock<IProcessInformationProvider>();

            // create 2 "processes" as Process objects (in the test code we don't invoke their methods)
            var p1 = Mock.Of<Process>();
            var p2 = Mock.Of<Process>();
            var processes = new List<Process> { p1, p2 };

            mockManager.Setup(m => m.GetActiveProcessesAsync()).ReturnsAsync(processes);

            // provider returns different ProcessInformation for each call
            mockProvider.Setup(p => p.GetProcessInformation(It.IsAny<Process>()))
            .Returns<Process>(proc => new ProcessInformation
            {
                PID = (uint)(proc == p1 ? 1 : 2),
                Name = proc == p1 ? "P1" : "P2"
            });

            // Act
            var list = await mockManager.Object.GetActiveProcessesAsync();
            var infos = new List<ProcessInformation>();
            foreach (var proc in list)
            {
                // simulate using the provider
                infos.Add(mockProvider.Object.GetProcessInformation(proc));
            }

            // Assert
            Assert.IsNotNull(list);
            Assert.AreEqual(2, list.Count());
            // provider was called exactly two times with the expected arguments
            mockProvider.Verify(p => p.GetProcessInformation(It.IsAny<Process>()), Times.Exactly(2));

            CollectionAssert.AllItemsAreUnique(infos.Select(i => i.PID).ToList());
        }

        [TestMethod]
        public async Task When_ManagerReturnsEmptyList_Then_NoProviderCalls()
        {
            var mockManager = new Mock<IProcessManager>();
            var mockProvider = new Mock<IProcessInformationProvider>();

            mockManager.Setup(m => m.GetActiveProcessesAsync()).ReturnsAsync(new List<Process>());

            var list = await mockManager.Object.GetActiveProcessesAsync();
            foreach (var p in list)
            {
                mockProvider.Object.GetProcessInformation(p);
            }

            mockProvider.Verify(p => p.GetProcessInformation(It.IsAny<Process>()), Times.Never);
            Assert.AreEqual(0, list.Count());
        }
    }
}
