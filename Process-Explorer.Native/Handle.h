#pragma once

typedef void* HANDLE;

namespace Native
{
	public ref class Handle
	{
	private:
		HANDLE m_handle{ INVALID_HANDLE_VALUE };

	public:
		Handle();
		Handle(HANDLE handle);
		Handle(Handle^ other);
		Handle(Handle^% other);

		Handle^ operator=(Handle^ other);
		Handle^ operator=(Handle^% other);

		~Handle();
		!Handle();

		bool IsValid();

		HANDLE% Get();
		interior_ptr<HANDLE> GetPtr();

		HANDLE Detach();
		HANDLE Duplicate();

		void Close();
		System::String^ Address();

		explicit operator bool();
		operator HANDLE();
		
	};
}

