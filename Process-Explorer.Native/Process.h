#pragma once

namespace Native
{
	ref class Handle;
	ref class ProcessInformation;
	ref class CriticalSection;

	public ref class Process
	{
	private:
		Handle^ m_handle{ nullptr };
		CriticalSection^ m_cs;
		ProcessInformation^ m_info;

		double m_cpuUsage{ 0.0 };

		FILETIME* m_prevSysKernelTime;
		FILETIME* m_prevSysUserTime;
		FILETIME* m_prevProcKernelTime;
		FILETIME* m_prevProcUserTime;

		bool m_firstTimeMeasured;
	private:

		void InitializeProcessTimes();
		void DeinitializeProcessTimes();

		PROCESS_MEMORY_COUNTERS_EX GetProcessMemoryCounters();
		void UpdateProcessCPUUsage();

	public:

		Process(DWORD pid);
		Process(Handle^ handle);

		ProcessInformation^ GetProcessInformation();

		System::String^ GetProcessName();
		System::String^ GetProcessDescription();
		System::String^ GetProcessCompany();

		Handle^ GetHandle();

		void Terminate();
		

		~Process();
	};
}

