using GHelper;
using NLua;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace GHelper.Plugins
{
    public class FanPluginManager
    {
        private Lua _luaState;
        private string _activePlugin;
        private static string pluginsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins", "fans");

        public FanPluginManager()
        {
            _luaState = new Lua();
        }

        public List<string> DiscoverPlugins()
        {
            try
            {
                if (!Directory.Exists(pluginsPath))
                {
                    Debug.WriteLine($"Plugin directory not found: {pluginsPath}");
                    return new List<string>();
                }

                return Directory.GetFiles(pluginsPath, "*.lua")
                                .Select(Path.GetFileName)
                                .ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error discovering plugins: {ex.Message}");
                return new List<string>();
            }
        }

        public void SetActivePlugin(string pluginFileName)
        {
            try
            {
                string scriptPath = Path.Combine(pluginsPath, pluginFileName);
                if (File.Exists(scriptPath))
                {
                    string scriptContent = File.ReadAllText(scriptPath);
                    _luaState.DoString(scriptContent);
                    _activePlugin = pluginFileName;
                    Debug.WriteLine($"Successfully loaded plugin: {pluginFileName}");

                    // After loading, try to call the reset function if it exists
                    LuaFunction resetFunction = _luaState["reset"] as LuaFunction;
                    resetFunction?.Call();
                }
                else
                {
                    Debug.WriteLine($"Plugin file not found: {scriptPath}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading plugin {pluginFileName}: {ex.Message}");
                _activePlugin = null;
            }
        }

        public Dictionary<string, int> RunPlugin(Dictionary<string, float> sensorData)
        {
            if (string.IsNullOrEmpty(_activePlugin))
            {
                Debug.WriteLine("No active plugin is set.");
                return new Dictionary<string, int>();
            }

            try
            {
                LuaFunction updateFunction = _luaState["update"] as LuaFunction;
                if (updateFunction == null)
                {
                    Debug.WriteLine("Lua 'update' function not found in the active plugin.");
                    return new Dictionary<string, int>();
                }

                LuaTable sensorTable = (LuaTable)_luaState.DoString("return {}")[0];
                foreach (var kvp in sensorData)
                {
                    sensorTable[kvp.Key] = kvp.Value;
                }

                double dt = AppConfig.Get("sensor_timer", 1000) / 1000.0;
                var result = updateFunction.Call(sensorTable, dt);

                if (result != null && result.Length > 0 && result[0] is LuaTable resultTable)
                {
                    var fanSpeeds = new Dictionary<string, int>();
                    foreach (var key in resultTable.Keys)
                    {
                        var value = resultTable[key];
                        fanSpeeds[key.ToString()] = Convert.ToInt32(value);
                    }
                    return fanSpeeds;
                }
                else
                {
                    Debug.WriteLine("Lua 'update' function did not return a table.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error running plugin '{_activePlugin}': {ex.Message}");
            }

            return new Dictionary<string, int>();
        }
    }
}