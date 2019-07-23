using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using SFQuerable.Interface;
using System;
using TestStatefulService;

namespace StateExplorer
{
    public class RemotingHelper
    {
        public static IStatefulService GetStatefulServiceHandler(string appName, string serviceName, string partitionKey)
        {
            var serviceUri = $"fabric:/{appName}/{serviceName}";
            return ServiceProxy.Create<IStatefulService>(new Uri(serviceUri), new ServicePartitionKey(partitionKey));
        }

        public static IStatefulActorService GetActorServiceHandler(string appName, string serviceName, string partitionKey)
        {
            var serviceUri = $"fabric:/{appName}/{serviceName}";
            return ServiceProxy.Create<IStatefulActorService>(new Uri(serviceUri), new ServicePartitionKey(partitionKey));
        }
    }
}
