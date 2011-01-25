using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JollyBit.BS.Core.Utility;

namespace JollyBit.BS.Core.Networking
{
    public interface IConnection<TCLIENT>
    {
        /// <summary>
        /// This function sends a message to the client.
        /// </summary>
        /// <typeparam name="T">The type of message to send.</typeparam>
        /// <param name="message">The messafe to send.</param>
        void SendMessage<T>(T message);
        /// <summary>
        /// This event fires whenever a message is recived by a client.
        /// </summary>
        event EventHandler<EventArgs<object>> MessageReceived;
        /// <summary>
        /// This event fires whenever the client disconnects
        /// </summary>
        event EventHandler ConnectionEstablished;
        /// <summary>
        /// This event fires whenever the client connects. ClientConnectingOperations are preformed before
        /// this event is fired.
        /// </summary>
        event EventHandler ConnectionTerminated;
        /// <summary>
        /// The client associated with the connection.
        /// </summary>
        TCLIENT Client { get; set; }
    }
}