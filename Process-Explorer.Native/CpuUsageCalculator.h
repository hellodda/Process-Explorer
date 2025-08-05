#pragma once

namespace Native
{
    ref class Handle;

    public ref class CpuUsageCalculator : System::IDisposable
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
        
        double GetCpuUsage(Handle^ handle);
    };
}


