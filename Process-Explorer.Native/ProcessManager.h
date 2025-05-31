#pragma once

using namespace System;
using namespace System::Collections::Generic;

namespace Native
{
	ref class Process;
	
	public ref class ProcessManager abstract sealed
	{
	private:
		static ProcessManager();

		static System::Threading::Tasks::Task^ InitializeProcessesListAsync();

		static System::Collections::Generic::Dictionary<DWORD, Process^>^ m_processes;
	public:

		static System::Threading::Tasks::Task<IEnumerable<Process^>^>^ GetActiveProcessesAsync();

	};
}

