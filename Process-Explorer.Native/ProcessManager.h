#pragma once

using namespace System;
using namespace System::Collections::Generic;

namespace Native
{
	ref class Process;
	ref class CriticalSection;

	public ref class ProcessManager abstract sealed
	{
	private:
		static CriticalSection^ m_cs{ nullptr };

		static ProcessManager();

		static void InitializeProcessesList();

		static System::Collections::Generic::Dictionary<DWORD, Process^>^ m_processes;
	public:

		static IEnumerable<Process^>^ GetActiveProcesses();

	};
}

