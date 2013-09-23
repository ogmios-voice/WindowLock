#pragma once

#include "stdafx.h"

//#define LOG_DEBUG

#ifdef _DEBUG
    #define debug       _debug
#else
    #define debug(arg)  ((void)0)
#endif

inline void _debug(const char    *s) { OutputDebugStringA(s); }
inline void _debug(const wchar_t *s) { OutputDebugStringW(s); }
