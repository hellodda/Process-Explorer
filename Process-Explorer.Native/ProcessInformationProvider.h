#pragma once

namespace Native
{
	ref class ProcessInformation;
	ref class Process;

	public interface class IProcessInformationProvider
	{
		ProcessInformation^ GetProcessInformation(Process^ process);
	};

	public ref class ProcessInformationProvider : IProcessInformationProvider
	{ 
	public:
		ProcessInformation^ GetProcessInformation(Process^ process) override;
	};
}

