using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject;
using Ninject.Activation;
using JollyBit.BS.Core.Utility;
using Ninject.Parameters;
using Ninject.Extensions.Logging;

namespace JollyBit.BS.Core.World.Actors
{
    public class ActorProvider : Provider<IActor>
    {
        private int currentActorId = 1;
        private IDictionary<int, IActor> _actors = new DictionaryWithWeaklyReferencedKey<int, IActor>();
        private readonly ILogger _logger;
        public ActorProvider(ILogger logger)
        {
            _logger = logger;
        }
        protected override IActor CreateInstance(IContext context)
        {
            int? actorId = null;
            IActor actor = null;
            //Check for ActorId parameter
            {
                IParameter parameter = context.Parameters.FirstOrDefault(parm => parm.Name.ToLower() == "actorid");
                if (parameter != null) 
                    actorId = parameter.GetValue(context, context.Request.Target) as int?;
            }
            //If actorId not specified get next available actorId
            if (actorId == null)
            {
                actorId = currentActorId;
                currentActorId++;
            }
            //If actor not in _actors dictionary create a new actor
            if (!_actors.TryGetValue(actorId.Value, out actor))
            {
                actor = new Actor(actorId.Value);
                _actors.Add(actorId.Value, actor);
                _logger.Debug("Created actor. ActorId={0}", actorId.Value);
            }
            //we have an actor ref so return it
            return actor;
        }

        private class Actor : IActor
        {
            public Actor(int actorId) { ActorId = actorId; }
            public int ActorId { get; private set;}
        }
    }
}
