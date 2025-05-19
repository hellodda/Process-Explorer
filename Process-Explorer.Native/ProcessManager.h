#pragma once

using namespace System;
using namespace System::Collections::Generic;

namespace Native
{
	ref class Process;

	public ref class ProcessManager abstract sealed
	{
	public:

		static IEnumerable<Process^>^ GetActiveProcesses();

	};
}

