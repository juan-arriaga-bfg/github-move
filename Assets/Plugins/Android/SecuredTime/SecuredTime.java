package com.neskinsoft.securedtime;

public class SecuredTime {
	public static long GetMonotonicTime {
		return android.os.SystemClock.elapsedRealtime();
	}
}