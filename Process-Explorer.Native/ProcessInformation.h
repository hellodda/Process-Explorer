#pragma once

namespace Native
{
	public ref class ProcessInformation
	{
	public:
		
		property DWORD PID;
		property DWORD PPID;
		property DWORD Priority;
		property DWORD SessionId;
		//property PROCESS_MEMORY_COUNTERS PrivateMemUsage;
		//property FILETIME CreationTime;
		//property FILETIME ExitTime;
		//property FILETIME KernelTime;
		//property FILETIME UserTime;
		property ULONG64 CycleTime;
		//property IO_COUNTERS IoCounters;
		property DWORD HandleCount;
		property DWORD GuiResources;
		property DWORD Heaps;
	};
}

