// WinHookLib.cpp : Defines the exported functions for the DLL application.
#include "stdafx.h"
#include "WinHookLib.h"
#include "log.h"

#define MSG_WIN_POS_LOCK  WM_USER

#pragma data_seg("HOOKSEC") // shared memory between dll instances
HWND hDest = NULL;
#pragma data_seg()
#pragma comment(linker, "/section:HOOKSEC,rws")

HHOOK hhook = NULL;

/// <summary>
/// Note: Whether a windows was resized/moved can only be polled
/// TODO: check: CBTProc callback function: HCBT_MOVESIZE = A window is about to be moved or sized.
/// </summary>
/// <param name="code"></param>
/// <param name="wparam"></param>
/// <param name="lparam"></param>
/// <returns></returns>
/// <see cref="http://msdn.microsoft.com/en-us/library/windows/desktop/ms644977(v=vs.85).aspx"/>
/// <see cref="http://stackoverflow.com/questions/5204717/hooking-win32-windows-creation-resize-querying-sizes"/>
LRESULT WINAPI shellproc(int code, WPARAM wparam, LPARAM lparam) {
    if(hDest != NULL) {
        switch(code) {
        case HSHELL_WINDOWCREATED:
        case HSHELL_WINDOWDESTROYED: // destroy is triggered while the window is still visible => WindowLock/Refresh will not be able to update the list of windows
        //case HSHELL_WINDOWREPLACED:
            Send(code, wparam, lparam);
            break;
        }
    }
    return CallNextHookEx(0, code, wparam, lparam);
}

/// <see cref="http://msdn.microsoft.com/en-us/library/windows/desktop/ms644931(v=vs.85).aspx">Message Constants: WM_USER</see>
LRESULT WINAPI Send(int code, WPARAM wparam, LPARAM lparam) {
    if(hDest != NULL) {
        debug("SendMessage");
        return SendMessage(hDest, MSG_WIN_POS_LOCK, wparam, code);
    }
    return 0;
}

extern "C" __declspec(dllexport)
BOOL WINAPI SetHook(HWND hApp) {
    debug("SetHook");
    hDest = hApp; // shared between all dll instances
    if(hhook == NULL) {
        debug("SetWindowsHookEx");
        hhook = SetWindowsHookEx(WH_SHELL, shellproc, hInst, 0);
    }
    return hhook != NULL;
}

extern "C" __declspec(dllexport)
BOOL WINAPI Unhook() {
    debug("Unhook");
    if(hhook != NULL) {
        debug("UnhookWindowsHookEx");
        UnhookWindowsHookEx(hhook);
        hhook = NULL;
    }
    hDest = NULL;
    return hhook == NULL;
}
