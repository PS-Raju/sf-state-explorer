using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Remoting.V2.FabricTransport.Client;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting.V2;
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
            return ServiceProxy.Create<IStatefulService>(new Uri(serviceUri), new ServicePartitionKey(long.Parse(partitionKey)));
        }

        public static IStatefulActorService GetActorServiceHandler(string appName, string serviceName, string partitionKey)
        {
            var serviceUri = $"fabric:/{appName}/{serviceName}";
            ActorProxyFactory actorProxyFactory = new ActorProxyFactory();
            return actorProxyFactory.CreateActorServiceProxy<IStatefulActorService>(new Uri(serviceUri), long.Parse(partitionKey));
        }
    }
}
