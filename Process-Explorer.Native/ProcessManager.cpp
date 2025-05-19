#include "pch.h"  
#include "ProcessManager.h"  
#include "Process.h"  

using namespace System::Collections::Generic;


IEnumerable<Native::Process^>^ Native::ProcessManager::GetActiveProcesses()  
{  
   
   auto processes = gcnew List<Native::Process^>();  

   DWORD PIDs[1024], needed;  
   if (EnumProcesses(PIDs, sizeof(PIDs), &needed)) {  
       size_t count = needed / sizeof(DWORD);  
       for (size_t i = 0; i < count; ++i) {  
		   if (PIDs[i] == 0) continue;
           processes->Add(gcnew Native::Process(PIDs[i]));
       }  
   }  

   return processes;  
}
