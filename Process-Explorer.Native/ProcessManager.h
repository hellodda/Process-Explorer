#pragma once

using namespace System;
using namespace System::Collections::Generic;

namespace Native
{
	ref class Process;

	public interface class IProcessManager
	{
		System::Threading::Tasks::Task<IEnumerable<Process^>^>^ GetActiveProcessesAsync();
	};
	
	public ref class ProcessManager : IProcessManager
	{
	private:
		System::Threading::Tasks::Task^ InitializeProcessesListAsync();

		System::Collections::Generic::Dictionary<DWORD, Process^>^ m_processes;
	public:

		ProcessManager();

		System::Threading::Tasks::Task<IEnumerable<Process^>^>^ GetActiveProcessesAsync() override;
	};
}

