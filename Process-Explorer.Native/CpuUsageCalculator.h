#pragma once

namespace Native
{
    ref class Handle;

    public interface class ICpuUsageCalculator
    {
        double GetCpuUsage(Handle^ handle);
    };

    public ref class CpuUsageCalculator : ICpuUsageCalculator, System::IDisposable
	{
    private:
        PFILETIME m_prevSysKernel, m_prevSysUser;
        PFILETIME m_prevProcKernel, m_prevProcUser;

        bool m_first;
    private:

        void InitializeFileTimes();
        void ReleaseFileTimes();

    public:
        CpuUsageCalculator();
        ~CpuUsageCalculator();
        !CpuUsageCalculator();
        
        double GetCpuUsage(Handle^ handle) override;
    };
}


