/**
 * @author  OmniSudo
 * @date    2023.02.14
 */

#pragma once

#ifdef _WIN32
#define MC_SHARED_API __declspec( dllexport )
#elifdef __linux__
#define MC_SHARED_API __attribute__((visibility("default")))
#else
#define MC_SHARED_API
#endif
