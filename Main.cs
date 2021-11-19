using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using GTANetworkAPI;

namespace RAGErally
{
    public class Main : Script
    {
        internal static Dictionary<String, dynamic> Config = new Dictionary<string, dynamic>();
        internal static String ScriptPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        internal static String ScriptName = "RAGErally";

        /// <summary>
        /// Called on resource start. All start-up logic should be called from here.
        /// </summary>
        [ServerEvent(Event.ResourceStart)]
        public void onResourceStarted()
        {
            // We're going to nestle the startup procedure in an async function so that we can wait a few seconds before starting.
            // This is because the resource sometimes starts before the settings have chance to be loaded into the server :/
            Task.Factory.StartNew(async () =>
            {
                // Delay by three seconds - should give us enough time for everything to be loaded in the background.
                await Task.Delay(3000);

                // Announce the startup!
                Logger.LogLogo();
                Logger.Log(LogLevel.Info, "RAGErally is starting!");


                Logger.Log(LogLevel.Info, "Loading configuration...");
                LoadConfiguration();

                // Output some debugging information
                Logger.Log(LogLevel.Trace, "Debugging information");
                Logger.Log(LogLevel.Trace, "Assembly Path: " + ScriptPath);

                // Announce loading complete and finish function.
                Logger.Log(LogLevel.Info, "RAGErally has finished start-up!");
            });
        }

        /// <summary>
        /// A function to load the configuration from file into memory and to set itself in the correct places.
        /// </summary>
        public void LoadConfiguration()
        {
            RefreshConfiguration(); // Reload the config from file before doing anything.

            // Some values require setting from here, others are manually loaded from their respective class locations.
            if (Enum.IsDefined(typeof(LogLevel), Config["LogLevel"])) { Logger.configLogLevel = (LogLevel)Config["LogLevel"]; } // LogLevel
        }

        /// <summary>
        /// Loads the settings from the file into the Config dictionary.
        /// </summary>
        private void RefreshConfiguration()
        {
            ///
            /// Trace level logging will not output in this function, as the default level is Info and we have not yet loaded the config in yet.
            /// Manually change the default logging level in Logger to Trace to see logging output from this function.
            ///

            Config.Clear(); // Empty the contents of the config dictionary.

            List<string> failed = new List<string>(); // So we can keep track of any failed config options (such as duplicate keys)).
            int loaded = 0; // So we can keep track of how many have loaded.

            // Load the settings array and initialise new Parser.
            string[] settings = NAPI.Resource.GetResourceSettings(ScriptName);
            Utility.Parser parser = new Utility.Parser();

            // All settings are loaded as strings (reee), so we'll run them through a parser to see if they should be something else (bool, int, etc)
            Logger.Log(LogLevel.Trace, "Detected " + settings.Length.ToString() + " settings, iterating now...");
            foreach (string settingKey in settings) // Iterates through the setting keys.
            {
                if (Config.ContainsKey(settingKey)) { failed.Add(settingKey); continue; } // Checks to see if a setting with the same name has already been loaded.                
                string settingValue = NAPI.Resource.GetResourceSetting<string>(ScriptName, settingKey); // Get the setting value from the key array we're cycling through.
                Config.Add(settingKey, parser.ParseStringToObject(settingValue)); // Parse the string to the appropriate object and add to the dictionary.
                Logger.Log(LogLevel.Trace, "Added " + settingKey + " (" + settingValue + ") to config dictionary.");
                loaded++; // Increment the loaded counter so we can differentiate between settings in the file and settings actually loaded.
            }

            Logger.Log(LogLevel.Debug, "Loaded " + loaded.ToString() + " settings from configuration.");
            if (failed.Count != 0)
            { // If any settings have failed to load, lets warn the user.
                string failedString = ""; // Provide the user with a list of the broken strings to make their (and my) fuckups easier to find.
                foreach (string fail in failed) { failedString = failedString + fail + ", "; }

                Logger.Log(LogLevel.Warn, failed.Count.ToString() + " settings failed to load from configuration (" + failedString + "), this may mean there are duplicate setting keys and could cause errors!");
            }

            return;
        }
    }
}
