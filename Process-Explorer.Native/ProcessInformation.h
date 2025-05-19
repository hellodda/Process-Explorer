#pragma once

namespace Native
{
	public ref class ProcessInformation
	{
	private:

		PPROCESS_MEMORY_COUNTERS_EX m_memoryCounters;

	public:

		ProcessInformation()
		{
			m_memoryCounters = new PROCESS_MEMORY_COUNTERS_EX();
			memset(m_memoryCounters, 0, sizeof(PROCESS_MEMORY_COUNTERS_EX));
		}
		~ProcessInformation()
		{
			if (m_memoryCounters)
			{
				delete m_memoryCounters;
				m_memoryCounters = nullptr;
			}
		}
		!ProcessInformation()
		{
			this->~ProcessInformation();
		}

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
	};
}

