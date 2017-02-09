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


using Amazon;

namespace Cimpress.ACS.AwsHarnesses
{
    public class RegionEndpointHelper
    {
        public static RegionEndpoint Parse(string regionEndpoint)
        {
            switch (regionEndpoint)
            {
                case "ap-northeast-1":
                    return RegionEndpoint.APNortheast1;
                case "ap-southeast-1":
                    return RegionEndpoint.APSoutheast1;
                case "ap-southeast-2":
                    return RegionEndpoint.APSoutheast2;
                case "eu-central-1":
                    return RegionEndpoint.EUCentral1;
                case "eu-west-1":
                    return RegionEndpoint.EUWest1;
                case "sa-east-1":
                    return RegionEndpoint.SAEast1;
                case "us-east-1":
                    return RegionEndpoint.USEast1;
                case "us-gov-west-1":
                    return RegionEndpoint.USGovCloudWest1;
                case "us-west-1":
                    return RegionEndpoint.USWest1;
                case "us-west-2":
                    return RegionEndpoint.USWest2;
                default:
                    throw new AwsQueueListenerException(string.Format("Region {0} does not exist", regionEndpoint));
            }
        }
    }
}
