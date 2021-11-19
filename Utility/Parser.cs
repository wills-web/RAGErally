using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace RAGErally.Utility
{
    internal class Parser
    {
        /// <summary>
        /// Takes a string and converts it into a different object.
        /// </summary>
        /// <param name="obj">A string containing data to be converted into a different object type (bool, int, etc)</param>
        /// <returns>dynamic</returns>
        internal dynamic ParseStringToObject(String obj)
        {
            // Find what type the object is.
            Type type = FindTypes(obj); // Find what the object type is.
            Logger.Log(LogLevel.Trace, obj + " is type: " + type.ToString());

            // Now parse it based on type. Don't need to use TryParse as we have just done a pass of this.
            if (type == typeof(int)) { return int.Parse(obj); }
            else if (type == typeof(bool)) { return bool.Parse(obj); }
            else if (type == typeof(float)) { return float.Parse(obj); }
            else if (type == typeof(DateTime)) { return DateTime.Parse(obj); }
            else { return obj; } // String
        }

        /// <summary>
        /// Runs a string through various parsing attempts and returns the most likely Type.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal Type FindTypes(String obj)
        {
            // If we fail all these type checks than the type is probably string.
            bool success = false;

            // Integer
            int intResult = 0;
            success = int.TryParse(obj, out intResult);
            if (success) { return typeof(int); }

            // Bool
            bool boolResult = false;
            success = bool.TryParse(obj, out boolResult);
            if (success) { return typeof(bool); }

            // Float
            float floatResult = 0.0f;
            success = float.TryParse(obj, out floatResult);
            if (success) { return typeof(float); }

            // DateTime
            DateTime dateResult = DateTime.Now;
            success = DateTime.TryParse(obj, out dateResult);
            if (success) { return typeof(DateTime); }

            // String
            return typeof(string);
        }
    }
}
