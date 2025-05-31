#pragma once

namespace Native
{
	public ref class ProcessInformation
	{
	private:

		PPROCESS_MEMORY_COUNTERS_EX m_memoryCounters;

	public:

		ProcessInformation();
		~ProcessInformation();
		!ProcessInformation();

		property DWORD PID;
		property System::String^ Name;
		property DWORD WorkingSet
		{
			DWORD get()
			{
				return m_memoryCounters->WorkingSetSize;
			}
			void set(DWORD value)
			{
				m_memoryCounters->WorkingSetSize = value;
			}
		}
		property DWORD PrivateBytes
		{
			DWORD get()
			{
				return m_memoryCounters->PrivateUsage;
			}
			void set(DWORD value)
			{
				m_memoryCounters->PrivateUsage = value;
			}
		}
		property System::String^ Company;
		property System::String^ Description;
		property double CpuUsage;
	};
}

