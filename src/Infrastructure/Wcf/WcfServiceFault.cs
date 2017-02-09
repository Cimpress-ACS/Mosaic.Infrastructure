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
using System.Runtime.Serialization;

namespace VP.FF.PT.Common.Infrastructure.Wcf
{
	[DataContract]
	public class WcfServiceFault
	{
		public WcfServiceFault Inner { get; set; }

		[DataMember]
		public string Message { get; set; }

		[DataMember]
		public string Source { get; set; }

		[DataMember]
		public string Target { get; set; }

		public override string ToString()
		{
		    if (null == Inner)
				return string.Format("Target: {0} / Source: {1} / Message: {2}", Target, Source, Message);
		    
            return string.Format("Target: {0} / Source: {1} / Message: {2}{3}/ Inner: {4}", Target, Source, Message, Environment.NewLine, Inner.ToString());
		}
	}

	public static class WcfServiceFaultFactory
	{
		public static WcfServiceFault CreateWcfServiceFault(Exception ex)
		{
			var fault = new WcfServiceFault { Message = ex.Message, Source = ex.Source, Target = ex.TargetSite.ToString() };
			if (null != ex.InnerException)
			{
				WcfServiceFault wrapper = fault;
				Exception te = ex.InnerException;
				while (null != te)
				{
					wrapper.Inner = new WcfServiceFault { Message = te.Message, Source = te.Source, Target = te.TargetSite.ToString() };
					te = te.InnerException;
					wrapper = wrapper.Inner;
				}
			}
			return fault;
		}
	}
}
