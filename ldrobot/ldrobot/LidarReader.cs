using System.IO.Ports;
using System.Diagnostics;
using ldrobot;

/// <summary>
/// Represents a class for reading LIDAR data. Initializes the necessary components, opens a serial port connection,
/// reads LIDAR data, and handles the cleanup process upon stopping the reading operation.
/// </summary>
public class LidarReader
{
    #region Parameters

    /// <summary>
    /// The instance for communication with the LIDAR device.
    /// </summary>
    private SerialPort serialPort;

    /// <summary>
    /// The instance for analyzing and parsing LIDAR data packets.
    /// </summary>
    private LidarPacket LidarPacket;

    /// <summary>
    /// The length of the LIDAR data packet in bytes.
    /// </summary>
    private int PacketLen;

    /// <summary>
    /// The name of the serial port to establish a connection with the LIDAR device.
    /// </summary>
    private string PortName;

    /// <summary>
    /// The baud rate for the serial port connection with the LIDAR device.
    /// </summary>
    private int BaudRate;
    #endregion

    #region Public
    /// <summary>
    /// Initializes the objects,
    /// sets the parameters and starts the serial port connection.
    /// </summary>
    /// <param name="portName">The name of the serial port. </param>
    /// <param name="baudRate">The baud rate for the serial port connection.</param>
    public LidarReader(string portName, int baudRate)
    {
        LidarPacket = new LidarPacket();

        PacketLen = 47;
        PortName = portName;
        BaudRate = baudRate;
        StartSerialPort();
    }

    /// <summary>
    /// Starts reading LIDAR data from the serial port. Processes the received data.
    /// Analyzes the packet, and validates CRC (Cyclic Redundancy Check).
    /// </summary>
    public void StartReading()
    {
        byte[] buffer = new byte[PacketLen];
        var header = serialPort.ReadByte();

        if (header == 0x54)
        {
            buffer[0] = 0x54;
            for (int i = 1; i < PacketLen; i++)
            {
                buffer[i] = (byte)serialPort.ReadByte();
            }
            LidarPacket.AnalyzeLidarPacket(buffer);
        }
    }

    /// <summary>
    /// Stops the serial port connection when user presses enter key.
    /// </summary>
    public void StopReading()
    {
        serialPort.Close();
    }
    #endregion

    #region Private

    /// <summary>
    /// Starts the serial port connection with specified parameters.
    /// </summary>
    private void StartSerialPort()
    {
        Debug.WriteLine("Initializing LIDAR");
        try
        {
            serialPort = new SerialPort(PortName, BaudRate, Parity.None, 8, StopBits.One);
            serialPort.Open();
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error opening serial port: " + ex.Message);
        }
    }
    #endregion
}

