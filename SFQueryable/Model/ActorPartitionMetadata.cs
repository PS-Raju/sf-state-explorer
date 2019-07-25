using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFQuerable.Model
{
    public class ActorPartitionMetadata
    {
        public int count;

        public List<ActorId> actors;
    }
}
