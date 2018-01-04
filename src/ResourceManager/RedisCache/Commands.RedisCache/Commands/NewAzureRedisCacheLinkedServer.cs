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
    using ResourceManager.Common.ArgumentCompleters;
    using System;
    using System.Management.Automation;
    using Rest.Azure;

    [Cmdlet(VerbsCommon.New, "AzureRmRedisCacheLinkedServer", SupportsShouldProcess = true), OutputType(typeof(PSRedisLinkedServer))]
    public class NewAzureRedisCacheLinkedServer : RedisCacheCmdletBase
    {
        [Parameter(ValueFromPipelineByPropertyName = true, Mandatory = false, HelpMessage = "Name of resource group in which cache exists.")]
        [ResourceGroupCompleter]
        public string ResourceGroupName { get; set; }

        [Parameter(ValueFromPipelineByPropertyName = true, Mandatory = true, HelpMessage = "Name of redis cache.")]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        [Parameter(ValueFromPipelineByPropertyName = true, Mandatory = true, HelpMessage = "Id of redis cache which should be linked to current cache.")]
        [ValidateNotNullOrEmpty]
        public string LinkedRedisCacheId { get; set; }

        [Parameter(ValueFromPipelineByPropertyName = true, Mandatory = true, HelpMessage = "Location of LinkedRedisCacheId.")]
        [LocationCompleter("Microsoft.Cache/Redis")]
        [ValidateNotNullOrEmpty]
        public string LinkedRedisCacheLocation { get; set; }

        [Parameter(ValueFromPipelineByPropertyName = true, Mandatory = true, HelpMessage = "Role of LinkedRedisCacheId in link.")]
        [ValidateSet("Primary","Secondary", IgnoreCase = true)]
        [ValidateNotNullOrEmpty]
        public string ServerRole { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "Do not ask for confirmation.")]
        public SwitchParameter Force { get; set; }

        public override void ExecuteCmdlet()
        {
            Utility.ValidateResourceGroupAndResourceName(ResourceGroupName, Name);
            ResourceGroupName = CacheClient.GetResourceGroupNameIfNotProvided(ResourceGroupName, Name);
            string linkedCacheName = Utility.GetCacheNameFromRedisCacheId(LinkedRedisCacheId);
            ReplicationRole replicationRole = (ReplicationRole)Enum.Parse(typeof(ReplicationRole), ServerRole, true);

            ConfirmAction(
                Force.IsPresent,
                string.Format(Resources.ShouldLinkRedisCache, linkedCacheName, Name),
                string.Format(Resources.LinkingRedisCache, linkedCacheName, Name),
                Name,
                () =>
                {
                    RedisLinkedServerWithProperties redisLinkedServer = CacheClient.SetLinkedServer(
                       resourceGroupName: ResourceGroupName,
                       cacheName: Name,
                       linkedCacheName: linkedCacheName,
                       linkedCacheId: LinkedRedisCacheId,
                       linkedCacheLocation: LinkedRedisCacheLocation,
                       serverRole: replicationRole);

                    if (redisLinkedServer == null)
                    {
                        throw new CloudException(string.Format(Resources.LinkedServerCreationFailed, linkedCacheName, Name));
                    }
                    WriteObject(new PSRedisLinkedServer(ResourceGroupName, Name, redisLinkedServer));
                }
            );
        }
    }
}