#pragma once

namespace Native
{
	ref class Handle;
	ref class ProcessInformation;
	ref class CriticalSection;

	public ref class Process
	{
	private:
		Handle^ m_handle = nullptr;
		CriticalSection^ m_cs;
	public:

		Process(DWORD pid);
		Process(Handle^ handle);

		ProcessInformation^ GetProcessInformation();

		System::String^ GetProcessName();
		System::String^ GetProcessDescription();
		System::String^ GetProcessCompany();

		double GetProcessCPUUsage();

		PROCESS_MEMORY_COUNTERS_EX GetProcessMemoryCounters();

		~Process();
	};
}

