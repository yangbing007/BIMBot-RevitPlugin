﻿using System;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace BimServerExchange.Runtime
{
	/// <summary>
	/// Class implements an ICN specific exception. It allows for additional caller data
	/// to be passed on in case information specific to the error must be passed on to the
	/// exception handler code.
	/// It also keeps track of the Method that generated the exception and the linenumber
	/// and filename. These can be accessed separately by the handler. The ToString() method
	/// generates an output with as much information as was set in the exception
	/// </summary>
	public class IcnException : Exception
	{
		#region fields
		private string _callerData;
		private int _severity;
		private string _methodName;
		private string _filePath;
		private int _lineNumber;
		private int _noLogger;
		#endregion fields

		#region properties
		public string CallerData
		{
			get { if (string.IsNullOrEmpty(_callerData)) return ""; return _callerData; }
			set { _callerData = value; if (null == _callerData) _callerData = ""; }
		}
		public int Severity
		{
			get { if (_severity < 1) return 1; if (_severity > 100) return 100; return _severity; }
			set { _severity = value; if (_severity < 1) _severity = 1; if (_severity > 100) _severity = 100; }
		}
		public string MethodName
		{
			get { if (string.IsNullOrEmpty(_methodName)) return "Unknown"; return _methodName; }
			set { _methodName = value; if (null == _methodName) _methodName = ""; }
		}
		public string FileName
		{
			get { if (string.IsNullOrEmpty(_filePath)) return ""; return _filePath; }
			set { _filePath = value; if (null == _filePath) _filePath = ""; }
		}
		public int LineNumber
		{
			get { if (_lineNumber < 0) return 0; return _lineNumber; }
			set { _lineNumber = value; if (_lineNumber < 0) _lineNumber = 0; }
		}
		public bool NoLogger
		{
			get { return _noLogger != 0; }
			private set { _noLogger = value ? 1 : 0; }
		}
		#endregion properties

		#region ctor, dtor
		/// <summary>
		/// Constructor, takes an exception message and a caller specific string
		/// </summary>
		/// <param name="mssg">Generic message (typically generated by the API framework)</param>
		/// <param name="severity">Severity between 1 and 100 of this exception (used for logging)</param>
		/// <param name="callerData">Additional information added by the function throwing the exception</param>
		/// <param name="mn">Automatically generated method name generating the exception. Do not supply manually</param>
		/// <param name="ln">Automatically generated linenumber where the exception was thrown. Do not supply manually</param>
		/// <param name="fp">Automatically generated file name with the source code generating the exception. Do not supply manually</param>
		public IcnException(string mssg, int severity, string callerData,
			[CallerMemberName] string mn = "", [CallerLineNumber] int ln = 0, [CallerFilePath] string fp = "")
			: base(mssg)
		{
			CallerData = callerData;
			Severity = severity;
			MethodName = mn;
			LineNumber = ln;
			FileName = fp;
			NoLogger = false;
		}

		/// <summary>
		/// Casting constructor
		/// </summary>
		/// <param name="ex">Standard windows exception. The inner context of that exception is not kept with the IcnException</param>
		/// <param name="severity">Severity between 1 and 100 of this exception (used for logging)</param>
		/// <param name="callerdata">Additional information added by the function (re)throwing the exception</param>
		/// <param name="mn">Automatically generated method name generating the exception. Do not supply manually</param>
		/// <param name="ln">Automatically generated linenumber where the exception was thrown. Do not supply manually</param>
		/// <param name="fp">Automatically generated file name with the source code generating the exception. Do not supply manually</param>
		public IcnException(Exception ex, int severity, string callerdata = "Generic windows exception",
			[CallerMemberName] string mn = "", [CallerLineNumber] int ln = 0, [CallerFilePath] string fp = "")
			: base(ex.Message)
		{
			CallerData = callerdata;
			Severity = severity;
			MethodName = mn;
			LineNumber = ln;
			FileName = fp;
			NoLogger = false;
		}
		#endregion ctor, dtor

		#region overloads
		/// <summary>
		/// Prints as much of the information in this exception as has been set by the function that threw it
		/// </summary>
		/// <returns>String with the exception definition</returns>
		public new string ToString()
		{
#if !DEBUG
			return Message;
#endif
			// if there is no method name only print the message and callerdata, if set
			if (string.IsNullOrEmpty(MethodName))
			{
				if (string.IsNullOrEmpty(CallerData))
				{
					return string.Format("{0}", Message);
				}
				return string.Format("{0}: {1}", Message, CallerData);
			}

			// if there is no filename, only print out method, linenumber and caller data if set
			if (string.IsNullOrEmpty(FileName))
			{
				if (string.IsNullOrEmpty(CallerData))
				{
					return string.Format("{0}() @{1}: {2}", MethodName, LineNumber, Message);
				}
				return string.Format("{0}() @{1}: {3} - {2}", MethodName, LineNumber, Message, CallerData);
			}

			// print out method name, line number, filename and caller data if set
			if (string.IsNullOrEmpty(CallerData))
			{
				return string.Format("{0}() in \"{2}\" @({1}): {3}", MethodName, LineNumber, FileName, Message);
			}
			return string.Format("{0}() in \"{2}\" @{1}: {4} - {3}", MethodName, LineNumber, FileName, Message, CallerData);
		}
#endregion overloads

		public void Display(string title)
		{
			if (string.IsNullOrEmpty(title)) title = "Exception in BIMserver Exchange";
			MessageBox.Show(ToString(), title);
		}
	}
}
