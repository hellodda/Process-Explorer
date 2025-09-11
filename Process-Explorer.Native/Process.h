#pragma once
#include "CpuUsageCalculator.h"

namespace Native
{
	ref class Handle;

	public ref class Process : System::IDisposable
	{
	private:
		Handle^ m_handle{ nullptr };
		ICpuUsageCalculator^ m_usageCalculator;
	public:
		Process();
		Process(DWORD pid);
		Process(Handle^ handle);

		System::String^ GetProcessName();
		System::String^ GetProcessDescription();
		System::String^ GetProcessCompany();
		System::String^ GetProcessFilePath();
		double GetCpuUsage();
		Handle^ GetHandle();

		void Terminate();

		PROCESS_MEMORY_COUNTERS_EX GetProcessMemoryCounters();

		~Process();
	};
}

