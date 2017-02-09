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


using System.Configuration;

namespace VP.FF.PT.Common.Infrastructure.Wcf
{
    /*
    <wcfServices>
        <services serviceAddressPort="localhost:2981" >
            <add key="test1" 
                  endpointName="Test1EndPoint" 
                  port="2982"
                  hostType="Test1Service.MyService, Test1Service"
                  contractType="Test1Common.IMyService, Test1Common" />
            <add key="test2" 
                  endpointName="Test2EndPoint" 
                  hostType="Test2Service.MyService, Test2Service"
                  contractType="Test2Common.IMyService, Test2Common" />
        </services>
    </wcfServices>
    */
    public class WcfServiceConfigurationSection : ConfigurationSection
	{
		[ConfigurationProperty("services")]
		public WcfServiceConfigCollection Services
		{
			get
			{
				return ((WcfServiceConfigCollection)(base["services"]));
			}
		}

        [ConfigurationProperty("defaultPort", DefaultValue = "_", IsKey = false, IsRequired = true)]
        [StringValidator(InvalidCharacters = "~!@#$%^&()[]{}/;'\"|\\", MinLength = 1, MaxLength = 260)]
        public string DefaultPort
        {
            get
            { return (string)this["defaultPort"]; }
            set
            { this["defaultPort"] = value; }
        }
	}

	[ConfigurationCollectionAttribute(typeof(WcfServiceConfigElement))]
	public class WcfServiceConfigCollection : ConfigurationElementCollection
	{
		protected override ConfigurationElement CreateNewElement()
		{
			return new WcfServiceConfigElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((WcfServiceConfigElement)(element)).EndpointName;
		}

		public void Add(WcfServiceConfigElement element)
		{
			BaseAdd(element);
		}

		public void Remove(string key)
		{
			BaseRemove(key);
		}

		public void Clear()
		{
			BaseClear();
		}

		public WcfServiceConfigElement this[int idx]
		{
			get { return this[idx]; }
		}
	}

	public class WcfServiceConfigElement : ConfigurationElement
	{
		public WcfServiceConfigElement() 
        {}

		public WcfServiceConfigElement(string endpointName, string hostType, string contractType)
		{
			EndpointName = endpointName;
			HostTypeDeclaration = hostType;
			ContractTypeDeclaration = contractType;
		}

		[ConfigurationProperty("endpointName", DefaultValue = "_", IsKey = true, IsRequired = true)]
		[StringValidator(InvalidCharacters = "~!@#$%^&()[]{};'\"|\\", MinLength = 1, MaxLength = 260)]
		public string EndpointName
		{
			get
			{ return (string)this["endpointName"]; }
			set
			{ this["endpointName"] = value; }
		}

		[ConfigurationProperty("hostType", DefaultValue = "_", IsKey = false, IsRequired = true)]
		[StringValidator(InvalidCharacters = "~!@#$%^&()/|\\", MinLength = 1, MaxLength = 520)]
		public string HostTypeDeclaration
		{
			get
			{ return (string)this["hostType"]; }
			set
			{ this["hostType"] = value; }
		}

        [ConfigurationProperty("port", IsKey = false, IsRequired = false)]
        [StringValidator(InvalidCharacters = "~!@#$%^&()[]{}/;'\"|\\", MaxLength = 260)]
        public string Port
        {
            get
            { return (string)this["port"]; }
            set
            { this["port"] = value; }
        }

		public string HostTypeFullname
		{
			get
			{
				string[] parts = HostTypeDeclaration.Split(',');
				return parts[0].Trim();
			}
		}

		public string HostTypeAssembly
		{
			get
			{
				string[] parts = HostTypeDeclaration.Split(',');
				return (parts.Length > 1)
					? parts[1].Trim()
					: null;
			}
		}

		[ConfigurationProperty("contractType", DefaultValue = "_", IsKey = false, IsRequired = true)]
		[StringValidator(InvalidCharacters = "~!@#$%^&()/|\\", MinLength = 1, MaxLength = 520)]
		public string ContractTypeDeclaration
		{
			get
			{ return (string)this["contractType"]; }
			set
			{ this["contractType"] = value; }
		}

		public string ContractTypeFullname
		{
			get
			{
				string[] parts = ContractTypeDeclaration.Split(',');
				return parts[0].Trim();
			}
		}

		public string ContractTypeAssembly
		{
			get
			{
				string[] parts = ContractTypeDeclaration.Split(',');
				return (parts.Length > 1)
					? parts[1].Trim() 
					: null;
			}
		}
	}
}
