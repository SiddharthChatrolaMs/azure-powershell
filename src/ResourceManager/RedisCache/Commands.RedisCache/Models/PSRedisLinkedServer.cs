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

namespace Microsoft.Azure.Commands.RedisCache.Models
{
    using System;
    using Management.Redis.Models;

    public class PSRedisLinkedServer
    {
        public string Name { get; set; }
        public string ResourceGroupName { get; set; }

        public string LinkedServerId { get; set; }
        public string Type { get; set; }
        public string ProvisioningState { get; set; }
        public string LinkedRedisCacheId { get; set; }
        public string LinkedRedisCacheLocation { get; set; }
        public string ServerRole { get; set; }
        

        public PSRedisLinkedServer() { }

        internal PSRedisLinkedServer(string resourceGroupName, string cacheName, RedisLinkedServerWithProperties redisLinkedServer)
        {
            Name = cacheName;
            ResourceGroupName = resourceGroupName;

            LinkedServerId = redisLinkedServer.Id;
            Type = redisLinkedServer.Type;
            ProvisioningState = redisLinkedServer.ProvisioningState;
            LinkedRedisCacheId = redisLinkedServer.LinkedRedisCacheId;
            LinkedRedisCacheLocation = redisLinkedServer.LinkedRedisCacheLocation;
            ServerRole = redisLinkedServer.ServerRole.ToString();
        }
    }
}