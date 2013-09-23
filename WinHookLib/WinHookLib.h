#pragma once

#include "stdafx.h"

extern HMODULE hInst;

#ifdef __cplusplus
extern "C" {
#endif
__declspec(dllexport) BOOL WINAPI SetHook(HWND hApp);
__declspec(dllexport) BOOL WINAPI Unhook();
#ifdef __cplusplus
}
#endif

LRESULT WINAPI Send(int code, WPARAM wparam, LPARAM lparam);
