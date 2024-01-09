using System;
using System.Net;
using System.Net.Mime;
using System.Security.Policy;
using NetMQ;
using NetMQ.Sockets;

namespace ldrobot
{

    /// <summary>
    /// The LidarDataPublisher class creates a PublisherSocket using NetMQ and binds it to the specified endpoint.
    /// It can publish messages, including byte arrays, using the SendMessage method.
    /// </summary>
    public class LidarDataPublisher
    {
        #region Parameters
        private PublisherSocket Publisher;
        /// <summary>
        /// specified endpoint.
        /// </summary>
        private string Endpoint;
        #endregion

        #region Public

        /// <summary>
        /// Initializes the LidarDataPublisher class and binds it to the specified endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint to bind the PublisherSocket to.</param>
        public LidarDataPublisher(string endpoint)
        {
            Endpoint = endpoint;
            InitializePublisher();
        }
        #endregion

        #region Private
        /// <summary>
        /// Disposes of the PublisherSocket when the LidarDataPublisher instance is no longer needed.
        /// </summary>
        private void Dispose()
        {
            Publisher?.Dispose();
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Sends a message, including a byte array, through the PublisherSocket.
        /// </summary>
        /// <param name="message">The byte array message to be sent.</param>
        public void SendMessage(byte[] message)
        {

            if (Publisher != null)
            {
                Publisher.SendFrame(message);
            }
            else
            {
                Console.WriteLine("Publisher not initialized.");
            }
        }
        #endregion

        #region Private Functions
        /// <summary>
        /// Initializes the PublisherSocket by creating a new instance and binding it to the specified endpoint.
        /// </summary>
        private void InitializePublisher()
        {
            Publisher = new PublisherSocket();
            Publisher.Bind(Endpoint);
        }
        #endregion

    }
}
