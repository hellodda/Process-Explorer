#pragma once

namespace Native
{
	public ref class MessageBox abstract sealed
	{
	public:
		static void Show(System::String^ message);
	};
}

