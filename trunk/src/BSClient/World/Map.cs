using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JollyBit.BS.Core.World;
using Ninject;
using JollyBit.BS.Core.Networking;
using JollyBit.BS.Core.Networking.Messages;
using Ninject.Extensions.Logging;

namespace JollyBit.BS.Client.World
{
    public class Map : MapBase, IMap
    {
        public Map(IKernel kernel, IConnection connection, ILoggerFactory logFactory)
            : base(kernel, logFactory)
        {
            connection.MessageReceived += new EventHandler<Core.Utility.EventArgs<object>>(connection_MessageReceived);
            _logger.Info("Map started.");
        }

        void connection_MessageReceived(object sender, Core.Utility.EventArgs<object> e)
        {
            if (e.Data is ChunkMessage)
            {
                
                ChunkMessage m = (ChunkMessage)e.Data;
                IChunk chunk = _kernel.Get<IChunk>();
                chunk.FillFromMessage(m);
                this[m.Location] = chunk;
            }
        }
    }
}
