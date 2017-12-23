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
    using System.Management.Automation;
    using Rest.Azure;

    [Cmdlet(VerbsCommon.New, "AzureRmRedisCacheFirewallRule", SupportsShouldProcess = true), OutputType(typeof(PSRedisFirewallRule))]
    public class NewAzureRedisCacheFirewallRule : RedisCacheCmdletBase
    {
        [Parameter(ValueFromPipelineByPropertyName = true, Mandatory = true, HelpMessage = "Name of resource group in which cache exists.")]
        [ResourceGroupCompleter]
        [ValidateNotNullOrEmpty]
        public string ResourceGroupName { get; set; }

        [Parameter(ValueFromPipelineByPropertyName = true, Mandatory = true, HelpMessage = "Name of redis cache.")]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; }

        [Parameter(ValueFromPipelineByPropertyName = true, Mandatory = true, HelpMessage = "Name of firewall rule.")]
        [ValidateNotNullOrEmpty]
        public string RuleName { get; set; }

        [Parameter(ValueFromPipelineByPropertyName = true, Mandatory = true, HelpMessage = "Starting IP address.")]
        [ValidateNotNullOrEmpty]
        public string StartIP { get; set; }

        [Parameter(ValueFromPipelineByPropertyName = true, Mandatory = true, HelpMessage = "Ending IP address.")]
        [ValidateNotNullOrEmpty]
        public string EndIP { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "Do not ask for confirmation.")]
        public SwitchParameter Force { get; set; }

        public override void ExecuteCmdlet()
        {
            Utility.ValidateResourceGroupAndResourceName(ResourceGroupName, Name);
            ConfirmAction(
                Force.IsPresent,
                string.Format(Resources.ShouldCreateFirewallRule, Name),
                string.Format(Resources.CreatingFirewallRule, Name),
                Name,
                () =>
                {
                    RedisFirewallRule redisFirewallRule = CacheClient.SetFirewallRule(
                        resourceGroupName: ResourceGroupName,
                        cacheName: Name,
                        ruleName: RuleName,
                        startIP: StartIP,
                        endIP: EndIP);

                    if (redisFirewallRule == null)
                    {
                        throw new CloudException(string.Format(Resources.FirewallRuleCreationFailed));
                    }
                    WriteObject(new PSRedisFirewallRule(ResourceGroupName, Name, redisFirewallRule));
                }
            );
        }
    }
}