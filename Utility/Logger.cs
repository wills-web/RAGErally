using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Runtime;
using System.Text;
using Console = Colorful.Console;
using Colorful;

namespace RAGErally
{
    /// <summary>
    /// A logging level ennum to be used as part of the RAGErally.Logger static class. Exposed to this namespace for ease.
    /// </summary>
    internal enum LogLevel
    {
        Trace,  // Most highly detailed information to be used as a last resort by developers to understand complex issues.
        Debug,  // Information that makes sense to developers making and debugging application.
        Info,   // Information that makes sense to sysadmins and higher-level users.
        Warn,   // Potentially harmful situations of interest to end users or system managers that indicate potential problems.
        Error,  // Error events of considerable importance that will prevent normal program execution, but might still allow the application to continue running.
        Fatal,  // Very severe error events that might cause the application to terminate. Includes exception handling.
    }

    internal static class Logger
    {
        internal static LogLevel configLogLevel = LogLevel.Info; // Defaults to info, however should be loaded from server config 

        /// <summary>
        /// Logs the passed message at the passed logLevel. Some contents will be outputted to a log file, some to console as well, some may not be outputted at all depending on configuration.
        /// </summary>
        /// <param name="logLevel">Should be type Logger.LogLevel, which is an enum of various logging levels. Determines how verbose the various outputs should be.</param>
        /// <param name="message">The message to output.</param>
        internal static void Log(LogLevel logLevel, string message)
        {
            if (configLogLevel > logLevel) { return; } // The verbosity of this log is too great to be outputted.

            string calling = ""; // If we're handling an exception or an error, lets output the function from which this log request has been sent.
            if (logLevel == LogLevel.Fatal || logLevel == LogLevel.Error)
            {
                calling = " {# " + NameOfCallingClass() + " #}"; // We want to log the calling function to give us a hint of where its all gone wrong.
            }

            string messageFormat = "[{0} @ {1}][{2}] {3}{4}";
            Formatter[] contents = new Formatter[]
            {
                new Formatter("RAGErally", Color.Orange),
                new Formatter(DateTime.Now.ToString(), Color.White),
                new Formatter(logLevel.ToString(), GetLogColor(logLevel)),
                new Formatter(message, Color.LightGray),
                new Formatter(calling, Color.Red) // May be empty
            };

            Console.WriteLineFormatted(messageFormat, Color.White, contents);
        }

        /// <summary>
        /// Logs an exception. Makes the handling of any exceptions standardised.
        /// </summary>
        /// <param name="exception">Should be system type Exception, is broken down for logging in the Logger class.</param>
        /// <param name="message">An optional parameter to provide insight into the potential cause of an exception.</param>
        internal static void LogException(Exception exception, string message)
        {
            string logMessage = "";

            // Now we've formatted the exception, lets pass the logging responsibility on to Log().
            Log(LogLevel.Fatal, logMessage);
        }

        /// <summary>
        /// Determines the name of the class which has called the function in which this method is called. ex. If Class1 calls a function in Class2, and the function in Class2 calls this method, the name of Class1 will be returned.
        /// </summary>
        /// <returns>A string containing the name of the calling class.</returns>
        internal static string NameOfCallingClass()
        {
            string fullName;
            Type declaringType;
            int skipFrames = 2;
            do
            {
                MethodBase method = new StackFrame(skipFrames, false).GetMethod();
                declaringType = method.DeclaringType;
                if (declaringType == null)
                {
                    return method.Name;
                }
                skipFrames++;
                fullName = declaringType.FullName + "." + method.Name;
            }
            while (declaringType.Module.Name.Equals("mscorlib.dll", StringComparison.OrdinalIgnoreCase));

            return fullName;
        }

        /// <summary>
        /// Takes a LogLevel enum and returns the System.Drawing.Color enum that matches.
        /// </summary>
        /// <param name="logLevel">LogLevel enum</param>
        internal static Color GetLogColor(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    return Color.Gray;
                case LogLevel.Debug:
                    return Color.MediumSeaGreen;
                case LogLevel.Info:
                    return Color.DodgerBlue;
                case LogLevel.Warn:
                    return Color.Gold;
                case LogLevel.Error:
                    return Color.OrangeRed;
                case LogLevel.Fatal:
                    return Color.DarkMagenta;
                default:
                    return Color.Gray;
            }
        }


        /// <summary>
        /// Outputs an ASCII RAGErally logo to the console.
        /// </summary>
        internal static void LogLogo()
        {
            FigletFont fontHeader = FigletFont.Load(Main.ScriptPath + "\\Resources\\speed.flf");
            Figlet figletHeader = new Figlet(fontHeader);

            Console.WriteLine("");
            Console.WriteLine(figletHeader.ToAscii(" RAGErally"), ColorTranslator.FromHtml("#E9A212"));
            Console.WriteLine("                      A rally mini-game script written for RageMP.", ColorTranslator.FromHtml("#D8A847"));
            Console.WriteLine("                                   v0.1a | by Will_", ColorTranslator.FromHtml("#D8A847"));
            Console.WriteLine("");
        }
    }
}
