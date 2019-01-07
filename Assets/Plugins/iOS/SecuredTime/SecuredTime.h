#pragma once

#include <sys/sysctl.h>

extern "C" {
	int64_t SecuredTime_GetMonotonicTime();
}
