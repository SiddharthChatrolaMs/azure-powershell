// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

namespace Microsoft.Azure.Commands.RedisCache
{
    using Microsoft.Azure.Commands.RedisCache.Models;
    using Microsoft.Azure.Commands.RedisCache.Properties;
    using Microsoft.Azure.Management.Redis.Models;
    using System.Collections.Generic;
    using System.Management.Automation;
    using Rest.Azure;
    using ResourceManager.Common.ArgumentCompleters;

    [Cmdlet(VerbsCommon.Get, "AzureRmRedisCacheLinkedServer"), OutputType(typeof(List<PSRedisLinkedServer>))]
    public class GetAzureRedisCacheLinkedServer : RedisCacheCmdletBase
    {
        [Parameter(ValueFromPipelineByPropertyName = true, Mandatory = true, HelpMessage = "Name of resource group in which cache exists.")]
        [ResourceGroupCompleter]
        [ValidateNotNullOrEmpty]
        public string ResourceGroupName { get; set; }

        [Parameter(ValueFromPipelineByPropertyName = true, Mandatory = true, HelpMessage = "Name of redis cache.")]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        [Parameter(ValueFromPipelineByPropertyName = true, Mandatory = false, HelpMessage = "Id of linked cache.")]
        public string LinkedRedisCacheId { get; set; }

        public override void ExecuteCmdlet()
        {
            Utility.ValidateResourceGroupAndResourceName(ResourceGroupName, Name);
            if (!string.IsNullOrEmpty(LinkedRedisCacheId))
            {
                string linkedCacheName = Utility.GetCacheNameFromLinkedRedisCacheId(LinkedRedisCacheId);
                RedisLinkedServerWithProperties redisLinkedServer = CacheClient.GetLinkedServer(
                    resourceGroupName: ResourceGroupName,
                    cacheName: Name,
                    linkedCacheName: linkedCacheName);

                if (redisLinkedServer == null)
                {
                    throw new CloudException(string.Format(Resources.LinkedServerNotFound, Name, linkedCacheName));
                }
                WriteObject(new PSRedisLinkedServer(ResourceGroupName, Name, redisLinkedServer));
            }
            else
            {
                IPage<RedisLinkedServerWithProperties> response = CacheClient.ListLinkedServer(ResourceGroupName, Name);
                List<PSRedisLinkedServer> list = new List<PSRedisLinkedServer>();
                foreach (RedisLinkedServerWithProperties redisLinkedServer in response)
                {
                    list.Add(new PSRedisLinkedServer(ResourceGroupName, Name, redisLinkedServer));
                }

                while (!string.IsNullOrEmpty(response.NextPageLink))
                {
                    response = CacheClient.ListLinkedServer(response.NextPageLink);
                    foreach (RedisLinkedServerWithProperties redisLinkedServer in response)
                    {
                        list.Add(new PSRedisLinkedServer(ResourceGroupName, Name, redisLinkedServer));
                    }
                }

                WriteObject(list, true);
            }
        }
    }
}