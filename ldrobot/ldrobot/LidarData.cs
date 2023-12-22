using System.IO.Ports;
using System;
using System.Diagnostics;

/// <summary>
/// Represents the data structure for LIDAR data.
/// </summary>
public class LidarData
{
    private SerialPort serialPort;
    private bool isHeader;
    private List<byte> rawByteList;
    private LidarPacket currentLidarPacket;

    public LidarData(string portName, int baudRate)
    {
        Debug.WriteLine("Initializing LIDAR");

        serialPort = new SerialPort(portName, baudRate);
        serialPort.Open();
    }

    public void StartReading()
    {
        isHeader = false;
       // rawByteList.Clear();
        currentLidarPacket = null;
        serialPort.DataReceived += SerialPort_DataReceived;
    }

    public void StopReading()
    {
        serialPort.Close();
    }

    private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        byte[] buffer = new byte[serialPort.BytesToRead];
        serialPort.Read(buffer, 0, buffer.Length);

        ProcessLidarData(buffer);
    }
    private void ProcessLidarData(byte[] rawData)
    {
        foreach (byte dataByte in rawData)
        {
            if (isHeader)
            {
                if (dataByte == 0x54)
                {
                    if (currentLidarPacket != null)
                    {
                        currentLidarPacket = LidarPacket.FromByteArray(rawByteList.ToArray());
                        if (true)
                        {
                            Console.WriteLine(currentLidarPacket);
                        }
                    }
                    rawByteList.Clear();
                }
            }
            else
            {
                if (dataByte == 0x54)
                {
                    if (currentLidarPacket != null)
                    {
                        currentLidarPacket = LidarPacket.FromByteArray(rawByteList.ToArray());
                        if (true)
                        {
                            Console.WriteLine(currentLidarPacket);
                        }
                    }
                    rawByteList.Clear();
                }
            }

            rawByteList.Add(dataByte);
            AppendToFile("lidar.txt", BitConverter.ToString(new byte[] { dataByte }));
        }
    }

    private void AppendToFile(string fileName, string text)
    {
        using (StreamWriter sw = File.AppendText(fileName))
        {
            sw.WriteLine(text);
        }
    }

}
