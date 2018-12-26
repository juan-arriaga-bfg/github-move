package com.bigfishgames.bfgunityandroid;

import com.bigfishgames.bfglib.bfgreporting.bfgReporting;
import com.bigfishgames.bfglib.bfgreporting.bfgRave;

public class bfgReportingUnityWrapper {

	public static String getRaveId() {
		return bfgRave.sharedInstance().currentRaveId();
	}
}
