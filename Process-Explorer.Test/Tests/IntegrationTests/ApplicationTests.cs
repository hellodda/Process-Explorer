using Native;

namespace Process_Explorer.Test.Tests.IntegrationTests
{
    [TestClass]
    public class ApplicationTests
    {
        /// <summary>
        /// Validates <see cref="Application.SetPrivileges"/> behavior 
        /// depending on whether the current process runs with administrator rights.
        /// 
        /// Expectations:
        /// - If running as administrator → no exception should be thrown.
        /// - If not administrator → either succeeds (environment-dependent) or throws 
        ///   <see cref="ApplicationException"/> mentioning "admin".
        /// </summary>
        [TestMethod]
        public async Task SetPrivileges_Behavior_IsAdminAware()
        {
            if (TestTools.IsAdministrator())
            {
                // Admin context → SetPrivileges must succeed without exceptions.
                await Task.Run(() => Application.SetPrivileges());
            }
            else
            {
                try
                {
                    // Non-admin context: method might still succeed, but if it fails,
                    // we expect a clear and informative exception message.
                    Application.SetPrivileges();

                    // If it succeeds while not admin → mark test as inconclusive
                    // because behavior can vary depending on environment and OS policies.
                    Assert.Inconclusive("SetPrivileges succeeded while not admin.");
                }
                catch (ApplicationException ex)
                {
                    // Verify exception message contains "admin" to ensure clarity.
                    StringAssert.Contains(ex.Message.ToLowerInvariant(), "admin");
                }
            }
        }

        /// <summary>
        /// Validates <see cref="Application.SetPriority"/> behavior 
        /// depending on whether the current process runs with administrator rights.
        /// 
        /// Expectations:
        /// - If running as administrator → no exception should be thrown.
        /// - If not administrator → either succeeds (unexpected but acceptable) or throws 
        ///   <see cref="ApplicationException"/> mentioning "admin".
        /// </summary>
        [TestMethod]
        public async Task SetPriority_Behavior_IsAdminAware()
        {
            if (TestTools.IsAdministrator())
            {
                // Admin context → SetPriority must succeed without exceptions.
                await Task.Run(() => Application.SetPriority());
            }
            else
            {
                try
                {
                    // Non-admin context: behavior can vary.
                    Application.SetPriority();

                    // Success in non-admin mode is possible but environment-specific,
                    // therefore mark the result as inconclusive.
                    Assert.Inconclusive("SetPriority succeeded while not admin.");
                }
                catch (ApplicationException ex)
                {
                    // Exception message should clearly mention "admin"
                    // so that the user understands why operation failed.
                    StringAssert.Contains(ex.Message.ToLowerInvariant(), "admin");
                }
            }
        }
    }
}