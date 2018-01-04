﻿using Microsoft.Azure.Commands.RedisCache.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Commands.RedisCache
{
    public class Utility
    {
        public static void ValidateResourceGroupAndResourceName(string resourceGroupName, string name)
        {
            if (resourceGroupName != null && resourceGroupName.Contains("/"))
            {
                throw new ArgumentException(Resources.InvalidResourceGroupName);
            }

            if (name != null && (name.Contains("/") || name.Contains(".")))
            {
                throw new ArgumentException(Resources.InvalidRedisCacheName);
            }
        }

        public static string GetCacheNameFromRedisCacheId(string id)
        {
            //Id looks like this: "/subscriptions/<subscription id>/resourceGroups/<resource group name>/providers/Microsoft.Cache/Redis/<cache name>"
            string[] e = id.Split('/');
            if (e.Length != 9)
            {
                throw new ArgumentException(string.Format(Resources.InvalidRedisCacheId, id));
            }
            return e[8];
        }

        public static string GetResourceGroupNameFromRedisCacheId(string id)
        {
            //Id looks like this: "/subscriptions/<subscription id>/resourceGroups/<resource group name>/providers/Microsoft.Cache/Redis/<cache name>"
            string[] e = id.Split('/');
            if (e.Length != 9)
            {
                throw new ArgumentException(string.Format(Resources.InvalidRedisCacheId, id));
            }
            return e[4];
        }
    }
}
