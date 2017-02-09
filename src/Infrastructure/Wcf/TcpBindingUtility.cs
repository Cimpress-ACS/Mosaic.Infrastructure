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

namespace VP.FF.PT.Common.Infrastructure.Wcf
{
	public class TcpBindingUtility
	{
		/// <summary>
		/// Client: maximum connections to be pooled for reuse.
		/// Server: maximum connections allowed to be pending dispatch.
		/// </summary>
		public static Int32 MaxConnections { get; private set; }

		/// <summary>
		/// The default is 65,536 bytes.
		/// </summary>
		public static Int64 MaxReceivedMessageSize { get; private set; }

		/// <summary>
		/// The default is 65,536 bytes
		/// </summary>
		public static Int64 MaxBufferPoolSize { get; private set; }

		public static Int32 MaxArrayLength { get; private set; }
		public static Int32 MaxBytesPerRead { get; private set; }
		public static Int32 MaxDepth { get; private set; }

		public static TimeSpan OpenTimeout { get; private set; }
		public static TimeSpan CloseTimeout { get; private set; }
		public static TimeSpan ReceiveTimeout { get; private set; }
		public static TimeSpan SendTimeout { get; private set; }


		/// <summary>
		/// Static initializer for default values in each build configuration.
		/// </summary>
		static TcpBindingUtility()
		{
#if(DEBUG)
            /*
            MaxConnections = 10;
            MaxReceivedMessageSize = 4194304;			// 4MB The default is 65,536 bytes
            MaxBufferPoolSize = 1048576;				// 1MB default is 65,536 bytes
            OpenTimeout = new TimeSpan(0, 20, 0);
            CloseTimeout = new TimeSpan(0, 20, 0);
            ReceiveTimeout = new TimeSpan(0, 20, 0);
            SendTimeout = new TimeSpan(0, 20, 0);
            MaxArrayLength = 524288;					// 512KB default is 16,384 bytes
            MaxBytesPerRead = 16384;					// 16KB default is 4,096 bytes
            MaxDepth = 96;								// 96 default is 32
             */

            MaxConnections = 10;
            MaxReceivedMessageSize = 2147483647;			
            MaxBufferPoolSize = 2147483647;					
            OpenTimeout = new TimeSpan(0, 50, 0);
            CloseTimeout = new TimeSpan(0, 50, 0);
            ReceiveTimeout = new TimeSpan(0, 20, 0);
            SendTimeout = new TimeSpan(0, 20, 0);
            MaxArrayLength = 2147483647;					
            MaxBytesPerRead = 2147483647;					
            MaxDepth = 2147483647;							
#else
            /*
			MaxConnections = 10;
			MaxReceivedMessageSize = 4194304;				// 4MB The default is 65,536 bytes
			MaxBufferPoolSize = 1048576;					// 1MB default is 65,536 bytes
			OpenTimeout = new TimeSpan(0, 0, 30);
			CloseTimeout = new TimeSpan(0, 0, 30);
			ReceiveTimeout = new TimeSpan(0, 0, 6);
			SendTimeout = new TimeSpan(0, 0, 6);
			MaxArrayLength = 524288;					// 512KB default is 16,384 bytes
			MaxBytesPerRead = 16384;					// 16KB default is 4,096 bytes
			MaxDepth = 96;								// 96 default is 32
            */

            MaxConnections = 10;
            MaxReceivedMessageSize = 2147483647;			
            MaxBufferPoolSize = 2147483647;					
            OpenTimeout = new TimeSpan(0, 50, 0);
            CloseTimeout = new TimeSpan(0, 50, 0);
            ReceiveTimeout = new TimeSpan(0, 20, 0);
            SendTimeout = new TimeSpan(0, 20, 0);
            MaxArrayLength = 2147483647;					
            MaxBytesPerRead = 2147483647;					
            MaxDepth = 2147483647;		
#endif
        }


		public static NetTcpBinding CreateNetTcpBinding()
		{
			//NetTcpBinding tcpBinding = new NetTcpBinding(SecurityMode.Transport, false);
            var tcpBinding = new NetTcpBinding(SecurityMode.None, false);

			//tcpBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
			//tcpBinding.Security.Transport.ProtectionLevel = ProtectionLevel.EncryptAndSign;
            tcpBinding.Security.Mode = SecurityMode.None;

			tcpBinding.MaxConnections = MaxConnections;
			tcpBinding.MaxReceivedMessageSize = MaxReceivedMessageSize;
			tcpBinding.MaxBufferPoolSize = MaxBufferPoolSize;
		    
            tcpBinding.ReaderQuotas.MaxNameTableCharCount = 2147483647;
		    tcpBinding.ReaderQuotas.MaxStringContentLength = 2147483647;

			tcpBinding.ReaderQuotas.MaxArrayLength = MaxArrayLength;
			tcpBinding.ReaderQuotas.MaxBytesPerRead = MaxBytesPerRead;
			tcpBinding.ReaderQuotas.MaxDepth = MaxDepth;
			
			//not allowed by partially trusted 
			//tcpBinding.MaxBufferSize = 262144;							//256KB default is 65,536 bytes

			tcpBinding.OpenTimeout = OpenTimeout;
			tcpBinding.CloseTimeout = CloseTimeout;
			tcpBinding.ReceiveTimeout = ReceiveTimeout;
			tcpBinding.SendTimeout = SendTimeout;
		    tcpBinding.PortSharingEnabled = false;

			return tcpBinding;
		}

		public static EndpointAddress CreateEndpointAddress(string serviceAddress)
		{
			return new EndpointAddress(string.Format("net.tcp://{0}", serviceAddress));
		}
	}
}
