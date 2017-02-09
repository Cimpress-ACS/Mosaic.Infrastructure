/* Copyright 2017 Cimpress

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License. */


using System;
using System.Collections.Generic;
using System.Configuration;

namespace VP.FF.PT.Common.Infrastructure.Wcf
{
    public class ServiceConfig
    {
        public string Port { get; set; }
        public Type HostType { get; set; }
        public Type ContractType { get; set; }
        public WcfServiceConfigElement Item { get; set; }
    }

    public static class WcfServiceConfiguration
	{
        static readonly object Lock = new object();
        static WcfServiceConfigurationSection _wcfServices;
        static readonly Dictionary<Type, ServiceConfig> WcfServiceConfigByType = new Dictionary<Type, ServiceConfig>();
        static readonly List<ServiceConfig> WcfServiceConfigs = new List<ServiceConfig>();

	    public static List<ServiceConfig> GetServiceConfigList()
	    {
	        lock (Lock)
	        {
	            if (_wcfServices == null) 
	                InitializeConfig();

	            return WcfServiceConfigs;
	        }
	    }

	    static void InitializeConfig()
	    {
	        _wcfServices = (WcfServiceConfigurationSection)ConfigurationManager.GetSection("wcfServices");

	        foreach (WcfServiceConfigElement item in _wcfServices.Services)
	        {
	            Type hostType = Type.GetType(item.HostTypeDeclaration);
	            Type contractType = Type.GetType(item.ContractTypeDeclaration);
	            var configItem = new ServiceConfig { Item = item, HostType = hostType, ContractType = contractType };
				
	            if (hostType != null && !WcfServiceConfigByType.ContainsKey(hostType))
	                WcfServiceConfigByType.Add(hostType, configItem);

	            if (contractType != null && !WcfServiceConfigByType.ContainsKey(contractType))
	                WcfServiceConfigByType.Add(contractType, configItem);

                if (!string.IsNullOrEmpty(item.Port))
                    configItem.Port = item.Port;
                else
                    configItem.Port = _wcfServices.DefaultPort;

                WcfServiceConfigs.Add(configItem);
	        }
	    }

	    public static ServiceConfig GetServiceConfig(Type type)
		{
			lock (Lock)
			{
				if (_wcfServices == null) 
                    InitializeConfig();

				if (WcfServiceConfigByType.ContainsKey(type))
					return WcfServiceConfigByType[type];
			    
                return null;
			}
		}

	    public static string GetAddressPort()
	    {
            _wcfServices = (WcfServiceConfigurationSection)ConfigurationManager.GetSection("wcfServices");
	        return _wcfServices.DefaultPort;
	    }
	}
}
