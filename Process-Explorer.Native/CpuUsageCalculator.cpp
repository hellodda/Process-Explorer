#include "pch.h"
#include "Handle.h"
#include "CpuUsageCalculator.h"
#include "ProcessExplorer-Definitions.h"

void Native::CpuUsageCalculator::InitializeFileTimes()
{
    m_prevProcKernel = new FILETIME;
    m_prevProcUser = new FILETIME;
    m_prevSysKernel = new FILETIME;
    m_prevSysUser = new FILETIME;
}

void Native::CpuUsageCalculator::ReleaseFileTimes()
{
    if (m_prevProcKernel)
    {
        delete m_prevProcKernel;
        delete m_prevProcUser;
        delete m_prevSysKernel;
        delete m_prevSysUser;
    }
}

Native::CpuUsageCalculator::CpuUsageCalculator()
    : m_first(false) {}

Native::CpuUsageCalculator::~CpuUsageCalculator()
{
    ReleaseFileTimes();
}

Native::CpuUsageCalculator::!CpuUsageCalculator()
{
    this->~CpuUsageCalculator();
}

double Native::CpuUsageCalculator::GetCpuUsage(Handle^ handle)
{
    if (GetProcessId(handle) != 0 && !m_first)
        InitializeFileTimes();

    FILETIME sysIdle, sysKernel, sysUser;
    FILETIME procCreate, procExit, procKernel, procUser;

    GetSystemTimes(&sysIdle, &sysKernel, &sysUser);
    GetProcessTimes(handle, &procCreate, &procExit, &procKernel, &procUser);

    if (!m_first)
    {
        *m_prevSysKernel = sysKernel; *m_prevSysUser = sysUser;
        *m_prevProcKernel = procKernel; *m_prevProcUser = procUser;
        
        m_first = true;
        
        return 0.0;
    }

    ULONGLONG sys = FileTimeToULL(&sysKernel) - FileTimeToULL(m_prevSysKernel)
        + FileTimeToULL(&sysUser) - FileTimeToULL(m_prevSysUser);

    ULONGLONG proc = FileTimeToULL(&procKernel) - FileTimeToULL(m_prevProcKernel)
        + FileTimeToULL(&procUser) - FileTimeToULL(m_prevProcUser);

    *m_prevSysKernel = sysKernel; *m_prevSysUser = sysUser;
    *m_prevProcKernel = procKernel; *m_prevProcUser = procUser;

    return sys == 0 ? 0.0 : (double)proc / sys * 100.0;
}
