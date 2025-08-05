#pragma once

namespace Native
{
	ref class Handle;
	ref class ProcessInformation;
	ref class CriticalSection;
	ref class CpuUsageCalculator;

	public ref class Process : System::IDisposable
	{
	private:
		Handle^ m_handle{ nullptr };
		CriticalSection^ m_cs;
		ProcessInformation^ m_info;
		CpuUsageCalculator^ m_usageCalculator;

		bool m_dataReceived{ false };
		bool m_firstTimeMeasured{ false };
	private:

		PROCESS_MEMORY_COUNTERS_EX GetProcessMemoryCounters();

	public:
		Process(DWORD pid);
		Process(Handle^ handle);

		ProcessInformation^ GetProcessInformation();

		System::String^ GetProcessName();
		System::String^ GetProcessDescription();
		System::String^ GetProcessCompany();
		System::String^ GetProcessFilePath();

		Handle^ GetHandle();

		void Terminate();
		

		~Process();
	};
}

