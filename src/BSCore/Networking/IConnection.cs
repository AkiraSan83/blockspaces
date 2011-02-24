using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JollyBit.BS.Core.Utility;

namespace JollyBit.BS.Core.Networking
{
    public interface IClient
    {
    }

    public interface IConnection
    {
        /// <summary>
        /// This function sends a message to the client.
        /// </summary>
        /// <typeparam name="T">The type of message to send.</typeparam>
        /// <param name="message">The message to send.</param>
        void SendMessage<T>(T message);
        /// <summary>
        /// This event fires whenever a message is received by a client.
        /// </summary>
        event EventHandler<EventArgs<object>> MessageReceived;
        /// <summary>
        /// This event fires whenever the client disconnects
        /// </summary>
        event EventHandler ConnectionTerminated;
        /// <summary>
        /// This event fires whenever a connection is established. This event is fired before the connection
        /// is considered initialized. This event should be used to send client initialization data.
        /// </summary>
        event EventHandler ConnectionEstablished;
        /// <summary>
        /// This event fires when the connection is initialized. This occurs after the  
        /// ConnectionEstablished event fires.
        /// </summary>
        event EventHandler ConnectionInitialized;
        /// <summary>
        /// The client associated with the connection.
        /// </summary>
        IClient Client { get; set; }
    }
}