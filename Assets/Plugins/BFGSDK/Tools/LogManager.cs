using System;
using UnityEngine;

namespace BFGUnityExample
{
	/// <summary>
	/// Log manager.
	///  - Controls output to the display.
	/// </summary>
	public class LogManager
	{
		private static LogManager Instance;
			
		public enum LogLevel
		{
			Suppress,
			Minimal,
			Default,
			Verbose,
		}

		public enum LoggingPriority
		{
			Low,       // Only output in Verbose mode
			Default,   // Standard Logging Level
			Critical,  // always output unless logging is suppressed
		}


		public LogLevel CurrentLogLevel { get; private set; }
			
		/// <summary>
		/// Initializes a new instance of the <see cref="BFGUnityExample.LogManager"/> class.
		/// </summary>
		public LogManager ()
		{			
			// Suppress output for Release builds
			CurrentLogLevel = Debug.isDebugBuild ? LogLevel.Default : LogLevel.Suppress;
		}

		/// <summary>
		/// Singleton
		/// </summary>
		/// <returns>The instance.</returns>
		public static LogManager GetInstance()
		{
			if( Instance==null)
			{
				Instance = new LogManager();
			}
			return Instance;
		}

		/// <summary>
		/// Outputs the 
		/// </summary>
		/// <param name="outputString">String to ouput.</param>
		/// <param name="priority"> The prioruty of this message..</param>
		public void DebugLogToConsole(string outputString, LoggingPriority priority = LoggingPriority.Default)
		{
			bool writeOutputString = false;

			// No string or output suppressed
			if(String.IsNullOrEmpty(outputString) || CurrentLogLevel.Equals(LogLevel.Suppress))
			{
				return;
			}

			// Output string IFF it's priority meets the current Log Level
			switch (priority) 
			{
			case LoggingPriority.Default:
				writeOutputString = CurrentLogLevel >= LogLevel.Default;
				break;

			case LoggingPriority.Low:
				writeOutputString = CurrentLogLevel >= LogLevel.Verbose;
				break;

			case LoggingPriority.Critical:
				writeOutputString = true;
				break;

			default:
				writeOutputString = true;
				break;
			}
				

			if (writeOutputString) 
			{
				Debug.Log (outputString);
			}
		}
	}
}

