#pragma once
typedef struct _SYSTEM_PROCESS_INFORMATION_EX* PSYSTEM_PROCESS_INFORMATION_EX;

namespace Native
{
	ref class ProcessInformationEx;
	ref class CriticalSection;
	ref class Handle;

	public ref class ProcessEx
	{
	private:

		Handle^ m_handle{ nullptr };

		CriticalSection^ m_cs;
		ProcessInformationEx^ m_info;

		bool m_dataReceived{ false };
		bool m_firstTimeMeasured{ false };

	private:

		void SaveProcessInformation(PSYSTEM_PROCESS_INFORMATION_EX pspi);

	public:

		ProcessEx(PSYSTEM_PROCESS_INFORMATION_EX pInfo);
		~ProcessEx();
		!ProcessEx();

		ProcessInformationEx^ GetProcessInformation();

		void UpdateProcessCpuUsage(PSYSTEM_PROCESS_INFORMATION_EX pspi);
		void UpdateProcessMainMetrics(PSYSTEM_PROCESS_INFORMATION_EX pspi);

		System::String^ GetProcessDescription();
		System::String^ GetProcessCompany();
		System::String^ GetProcessFilePath();

	};
}
