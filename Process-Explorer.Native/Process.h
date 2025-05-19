#pragma once



namespace Native
{
	ref class Handle;

	public ref class Process
	{
	private:
		Handle^ m_handle = nullptr;
	public:

		Process(DWORD pid);
		Process(Handle^ handle);

		~Process();

	};
}

