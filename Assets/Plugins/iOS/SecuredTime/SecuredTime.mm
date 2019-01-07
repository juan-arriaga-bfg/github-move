#import "SecuredTime.h"

int64_t SecuredTime_GetBootTimestamp() {
    struct timeval boottime;
    int mib[2] = {CTL_KERN, KERN_BOOTTIME};
    size_t size = sizeof(boottime);
    int rc = sysctl(mib, 2, &boottime, &size, NULL, 0);
    if (rc != 0) {
        return 0;
    }
    return (int64_t)boottime.tv_sec * 1000000 + (int64_t)boottime.tv_usec;
}

int64_t SecuredTime_GetUptime()
{
    int64_t before_now;
    int64_t after_now;
    struct timeval now;
    
    after_now = SecuredTime_GetBootTimestamp();
    do {
        before_now = after_now;
        gettimeofday(&now, NULL);
        after_now = SecuredTime_GetBootTimestamp();
    } while (after_now != before_now);
    
    return (int64_t)now.tv_sec * 1000000 + (int64_t)now.tv_usec - before_now;
}

int64_t SecuredTime_GetMonotonicTime() {
    int64_t msec = 0;
    
    if (@available(iOS 10.0, *)) {
       struct timespec monotime;
       clock_gettime(CLOCK_MONOTONIC_RAW, &monotime);
       
       msec = monotime.tv_sec;
    }
    else {
       msec = (int64_t)SecuredTime_GetUptime() / 1000.f;
    }

   return msec;   
}
