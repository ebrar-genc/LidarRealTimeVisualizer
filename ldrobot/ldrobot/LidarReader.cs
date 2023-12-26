using System.IO.Ports;
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents the data structure for LIDAR data.
/// </summary>
public class LidarReader
{
    private SerialPort serialPort;

    public LidarReader(string portName, int baudRate)
    {
        Debug.WriteLine("Initializing LIDAR");

        serialPort = new SerialPort(portName, baudRate);
        serialPort.Open();
    }
    

    public void StartReading()
    {
        byte[] buffer = new byte[47];
        var header = serialPort.ReadByte();

        if (header == 0x54)
        {
            LidarPacket lidarPacket = new LidarPacket();

            buffer[0] = 0x54;
            for (int i = 1; i < 47; i++)
            {
                buffer[i] = (byte)serialPort.ReadByte();
            }
            lidarPacket = ParseLidarData(buffer, lidarPacket);
            AppendToFile(buffer);
            StepAnalysis(lidarPacket);
            Debug.WriteLine(lidarPacket.ToString());

        }
    }

    public void StopReading()
    {
        serialPort.Close();
    }

    public LidarPacket ParseLidarData(byte[] rawData, LidarPacket lidarPacket)
    {

        if (rawData != null && rawData.Length == 47)
        {
            lidarPacket.Header = rawData[0];
            lidarPacket.VerLen = rawData[1];

            lidarPacket.Speed = (ushort)((rawData[3] << 8) | rawData[2]);

            lidarPacket.StartAngle = (ushort)((rawData[5] << 8) | rawData[4]);

            // Extract Data (2 bytes distance, 1 byte intensity per measurement point)
            lidarPacket.Data = new List<(ushort Distance, byte Intensity)>();
            for (int i = 0; i < 36; i += 3)
            {
                ushort distance = (ushort)((rawData[i + 7] << 8) | rawData[i + 6]);
                byte intensity = rawData[i + 8];
                lidarPacket.Data.Add((distance, intensity)); //(mm)
            }

            lidarPacket.EndAngle = (ushort)((rawData[43] << 8) | rawData[42]);

            lidarPacket.Timestamp = (ushort)((rawData[45] << 8) | rawData[44]);

            lidarPacket.CrcCheck = (rawData[46]);
        }
        return lidarPacket;
    }

    private void StepAnalysis(LidarPacket lidarPacket)
    {
        float step;

        step = (lidarPacket.EndAngle - lidarPacket.StartAngle) / (11);

        Console.WriteLine(step.ToString());

    }

    private void AppendToFile(byte[] buffer)
    {
        using (StreamWriter sw = File.AppendText("readedLidar.txt"))
        {
            foreach (byte b in buffer)
            {
                sw.Write(b + " ");
            }
            sw.WriteLine();
        }
    }
}

