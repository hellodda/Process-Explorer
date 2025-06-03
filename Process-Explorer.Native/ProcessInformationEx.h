#pragma once

namespace Native
{

	public ref class ProcessProcessorInformation
	{
	public:
		property System::DateTime LastCheckTime;
		property LONGLONG ProcessorTime;
	};

	public ref class ProcessInformationEx
	{
	public:

		ProcessInformationEx()
		{
			ProcessorInfo = gcnew ProcessProcessorInformation();
		}

		property DWORD PID;
		property DWORD PPID;
		property DWORD ThreadsCount;
		property DWORD HandlesCount;
		property System::String^ Name;
		property DWORD WorkingSet;
		property DWORD PrivateBytes;
		property System::String^ Company;
		property System::String^ Description;
		property System::String^ FilePath;
		property ProcessProcessorInformation^ ProcessorInfo;
		property double CpuUsage;
	};

}

