using NetMQ;
using NetMQ.Sockets;
using System.Diagnostics;

namespace WpfApp2
{
    /// <summary>
    /// LidarDataSubscriber class subscribes to a PublisherSocket using NetMQ and receives messages.
    /// </summary>
    public class LidarDataSubscriber
    {
        #region Parameters
        private SubscriberSocket Subscriber;
        private LidarDataParser Parser;
        /// <summary>
        /// specified endpoint.
        /// </summary>
        private string Endpoint;
        #endregion

        #region Public

        /// <summary>
        /// Initializes the LidarDataSubscriber class and connects it to the specified endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint to connect the SubscriberSocket to.</param>
        public LidarDataSubscriber(string endpoint)
        {
            Endpoint = endpoint;
            Parser = new LidarDataParser();
            InitializeSubscriber();
        }

        #endregion

        #region Private

        /// <summary>
        /// Disposes of the SubscriberSocket when the LidarDataSubscriber instance is no longer needed.
        /// </summary>
        private void Dispose()
        {
            Subscriber?.Dispose();
        }

        #endregion

        #region Public Functions

        /// <summary>
        /// Listens for messages from the PublisherSocket.
        /// </summary>
        public async Task<Tuple<double[], double[]>> ListenForMessages()
        {
            while (true)
            {

                if (Subscriber != null)
                {
                    byte[] message = Subscriber.ReceiveFrameBytes();
                    var parsedData = Parser.ParseData(message);
                    return parsedData;
                }
                else
                {
                    Debug.WriteLine("Subscriber not initialized.");
                }

                await Task.Delay(100);
            }
        }

        #endregion

        #region Private Functions

        /// <summary>
        /// Initializes the SubscriberSocket by creating a new instance and connecting it to the specified endpoint.
        /// </summary>
        private void InitializeSubscriber()
        {
            Subscriber = new SubscriberSocket();
            Subscriber.Connect(Endpoint);
            Subscriber.SubscribeToAnyTopic();
            Debug.WriteLine("Subscriber Started!");
        }

        #endregion
    }
}
