using Microsoft.BizTalk.SSOClient.Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace BizTalkComponents.OrchestrationUtils
{
    /// <summary>
    ///     Contains helper methods for using SSO as a config store.
    ///     Slightly modified from
    ///     http://www.getcodesamples.com/src/2B2085E1/C4A921B1
    /// </summary>
    public static class SSOConfigHelper
    {
        /// <summary>
        ///     Can be set to anything
        /// </summary>
        private static string _identifierGuid = "ConfigProperties";

        /// <summary>
        ///     Read method helps get configuration data
        /// </summary>
        /// <param name="appName">The name of the affiliate application to represent the configuration container to access</param>
        /// <param name="propName">The property name to read</param>
        /// <returns>
        ///     The value of the property stored in the given affiliate application of this component.
        /// </returns>
        public static string Read(string appName, string propName)
        {
            try
            {
                var ssoConfigStore = new SSOConfigStore();
                var configurationPropertyBag = new ConfigurationPropertyBag();
                // ReSharper disable once SuspiciousTypeConversion.Global
                ((ISSOConfigStore) ssoConfigStore).GetConfigInfo(appName, _identifierGuid, SSOFlag.SSO_FLAG_RUNTIME,
                    configurationPropertyBag);
                object ptrVar;
                configurationPropertyBag.Read(propName, out ptrVar, 0);
                return ptrVar.ToString();
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                throw;
            }
        }
    }

    // The code above uses this propertybag
    public class ConfigurationPropertyBag : IPropertyBag
    {
        /// <summary>
        ///     The properties
        /// </summary>
        private readonly IDictionary<string, object> _properties;

        /// <summary>
        ///     Initializes a new instance of the ConfigurationPropertyBag class
        /// </summary>
        internal ConfigurationPropertyBag()
        {
            _properties = new Dictionary<string, object>();
        }

        /// <summary>
        ///     Searches for property key
        /// </summary>
        /// <param name="key">key of kv pair</param>
        /// <returns>true if key found</returns>
        public bool Contains(string key)
        {
            return _properties.ContainsKey(key);
        }

        /// <summary>
        ///     Removes property
        /// </summary>
        /// <param name="key">key of kv pair</param>
        public void Remove(string key)
        {
            _properties.Remove(key);
        }

        #region IPropertyBag Members

        /// <summary>
        ///     Implements IPropertyBag read
        /// </summary>
        /// <param name="propName">IPropertyBag propName</param>
        /// <param name="ptrVar">IPropertyBag ptrVar</param>
        /// <param name="errorLog">IPropertyBag errLog</param>
        public void Read(string propName, out object ptrVar, int errorLog)
        {
            _properties.TryGetValue(propName, out ptrVar);
        }

        /// <summary>
        ///     Implements IPropertyBag write
        /// </summary>
        /// <param name="propName">IPropertyBag propName</param>
        /// <param name="ptrVar">IPropertyBag ptrVar</param>
        public void Write(string propName, ref object ptrVar)
        {
            _properties.Add(propName, ptrVar);
        }

        #endregion IPropertyBag Members
    }
}