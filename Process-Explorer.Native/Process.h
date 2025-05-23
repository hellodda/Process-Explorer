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


		~Process();
	};
}

