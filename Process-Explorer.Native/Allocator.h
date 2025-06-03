#pragma once

namespace Native
{
	ref class Allocator abstract sealed
	{
	public:

		template<typename T>
		static T* Allocate(size_t capacity);

		static void Deallocate(void* buffer);

	};


	template<typename T>
	inline T* Allocator::Allocate(size_t capacity)
	{
		return (T*)VirtualAlloc(nullptr, capacity, MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);
	}
}

