using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.Azure;
using Shared.Common.Resources;

namespace Shared.AzureCommon
{
    public class AzureSettingsReader
    {
        public string GetSetting(string key)
        {
            return CloudConfigurationManager.GetSetting(key);
        }

        public Dictionary<string, string> GetAll()
        {
            var result = new Dictionary<string, string>();

            try
            {
                foreach (var key in ConfigurationManager.AppSettings.AllKeys)
                    result.Add(key, ConfigurationManager.AppSettings[key]);
            }
            catch (Exception)
            {
                // ignored
            }

            return result;
        }

        public string GetSetting(string key, RunningEnvironmentEnum customEnvironment)
        {
            return CloudConfigurationManager.GetSetting(key);
        }

        public RunningEnvironmentEnum GetRunningEnvironment()
        {
            return (RunningEnvironmentEnum)Enum.Parse(typeof(RunningEnvironmentEnum), GetSetting("Environment"));
        }

        //public static IConfigurationReader GetConfigurationSettingsManager(RunningEnvironmentEnum environment)
        //{
        //    return GetConfigurationSettingsManager(ConfigurationManager.AppSettings[Configurationsetting.Common.AzureStorageAccountForAll], environment);
        //}

        //public static IConfigurationReader GetConfigurationSettingsManager(string connectionString, RunningEnvironmentEnum environment)
        //{
        //    var db = new SyncJsonTableStorageDb<Setting>(connectionString, Setting.ConfigurationDbName);
        //    var manager = new ConfigurationDb(db);

        //    manager.Initialize(environment);

        //    return manager;
        //}

        //public static ITestSettingsReader GetTestSettingsManager(TfsEnvironment environment)
        //{
        //    return GetTestSettingsManager(ConfigurationManager.AppSettings[Configurationsetting.Common.AzureStorageAccountForAll], environment);
        //}

        //public static ITestSettingsReader GetTestSettingsManager(string connectionString, TfsEnvironment environment)
        //{
        //    var db = new SyncJsonTableStorageDb<Setting>(connectionString, Setting.TestSettingsDbName);
        //    var manager = new TestSettingsDb(db);

        //    manager.Initialize(string.Empty, environment);

        //    return manager;
        //}
    }
}
