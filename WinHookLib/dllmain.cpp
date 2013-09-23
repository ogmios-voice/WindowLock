// dllmain.cpp : Defines the entry point for the DLL application.
#include "stdafx.h"
#include "log.h"

HMODULE hInst;

BOOL APIENTRY DllMain(HMODULE hModule, DWORD ul_reason_for_call, LPVOID lpReserved) {
    switch (ul_reason_for_call) {
    case DLL_PROCESS_ATTACH: // called for every app the hook is injected into
        debug("DLL_PROCESS_ATTACH");
        hInst = hModule;
        break;
    case DLL_PROCESS_DETACH:
        debug("DLL_PROCESS_DETACH");
        break;
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
        break;
    }
    return TRUE;
}
