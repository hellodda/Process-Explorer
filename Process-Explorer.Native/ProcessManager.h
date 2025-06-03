#pragma once

using namespace System;
using namespace System::Collections::Generic;

namespace Native
{
	ref class Process;
	ref class ProcessEx;
	ref class ProcessInformation;

	public ref class ProcessManager abstract sealed
	{
	private:
		static ProcessManager();

		static System::Threading::Tasks::Task^ InitializeProcessesListAsync();
		static System::Threading::Tasks::Task^ NtInitializeProcessesListAsync();

		static bool m_first{ true };

		static System::Collections::Generic::Dictionary<DWORD, Process^>^ m_processes;
		static System::Collections::Generic::Dictionary<DWORD, ProcessEx^>^ m_processesEx;
	public:

		static System::Threading::Tasks::Task<IEnumerable<Process^>^>^ GetActiveProcessesAsync();
		static System::Threading::Tasks::Task<IEnumerable<ProcessEx^>^>^ NtGetActiveProcessesAsync();
	};
}

