#pragma once



namespace Native
{
	ref class Handle;
	ref class ProcessInformation;

	public ref class Process
	{
	private:
		Handle^ m_handle = nullptr;
	public:

		Process(DWORD pid);
		Process(Handle^ handle);

		ProcessInformation^ GetProcessInformation();


		~Process();

	};
}

