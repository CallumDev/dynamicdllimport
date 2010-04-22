// asmproject.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"


extern "C" __declspec(dllexport) int add(int a, int b)
{
	int c;
	__asm
	{
		mov eax, a;
		add eax, b;
		mov c, eax;
	}
	return c;
}

extern "C" __declspec(dllexport) char* hello(char* s)
{
	return s;
}

extern "C" __declspec(dllexport) void nothing(void)
{
	printf("nothing to do.");
}
