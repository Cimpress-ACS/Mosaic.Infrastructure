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
using System.ServiceModel;
using System.Security;
using System.Security.Permissions;

namespace VP.FF.PT.Common.Infrastructure.Wcf
{
#if(DEBUG)
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, MaxItemsInObjectGraph = 131072, IncludeExceptionDetailInFaults = true)]
#else
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, MaxItemsInObjectGraph = 131072, IncludeExceptionDetailInFaults = false)]
#endif
	public abstract class WcfServiceBase
	{
		public void Authorize()
		{
			string[] groups = null;
			Type serviceType = GetType();
			var configItem = WcfServiceConfiguration.GetServiceConfig(serviceType);

			if (null != configItem)
			{
				groups = configItem.Item.AuthorizedGroups.Split(',');
			}

			if (null != groups)
			{
				var pps = new PrincipalPermission[groups.Length];
				for (int i = 0; i < groups.Length; i++)
				{
					pps[i] = new PrincipalPermission(null, groups[i]);
				}

				PrincipalPermission pp = pps[0];
				if (groups.Length > 0)
				{
					for (int i = 1; i < groups.Length; i++)
					{
						pp = (PrincipalPermission)pp.Union(pps[i]);
					}
				}
				pp.Demand();
			}
			else
				throw new SecurityException("Group is null");
		}
	}
}
