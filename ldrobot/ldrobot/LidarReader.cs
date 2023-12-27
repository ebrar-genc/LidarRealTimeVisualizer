using System.IO.Ports;
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.ComponentModel.DataAnnotations;
using ldrobot;
using System.Reflection.PortableExecutable;

/// <summary>
/// Represents the data structure for LIDAR data.
/// </summary>
public class LidarReader
{
    private SerialPort serialPort;
    private int PacketLen;
    private int CrcCheck;
    private int MeasuringPoint;

    public LidarReader(string portName, int baudRate)
    {
        PacketLen = 47;
        CrcCheck = 0;
        MeasuringPoint = 12;
        Debug.WriteLine("Initializing LIDAR");

        try
        {
            serialPort = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One);
            serialPort.Open();
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Error opening serial port: " + ex.Message);
        }

    }
    
    public void StartReading()
    {
        byte[] buffer = new byte[PacketLen];
        var header = serialPort.ReadByte();

        if (header == 0x54)
        {
            LidarPacket lidarPacket = new LidarPacket();
            buffer[0] = 0x54;
            for (int i = 1; i < PacketLen; i++)
            {
                buffer[i] = (byte)serialPort.ReadByte();
            }
            lidarPacket.ParseLidarData(buffer, MeasuringPoint);
            AppendToFileBuffer(buffer);
            lidarPacket.AppendToFilePacket();
            ValidateCrc(buffer);
            lidarPacket.CalculateAngles(MeasuringPoint);
        }
    }

    private void ValidateCrc(byte[] buffer)
    {
        LidarCrc lidarCrc = new LidarCrc();

        byte calculatedCrc = lidarCrc.CalculateCrc8(buffer, buffer.Length - 1);
        bool check = calculatedCrc == buffer[buffer.Length - 1];
        CrcCheck++;

        if (check)
        {
            if (CrcCheck == 10)
            {
                Debug.WriteLine("Data was received successfully");
                CrcCheck = 0;
            }
        }
        else
        {
            Debug.WriteLine("CRC Check Failed!");
        }
    }

    public void StopReading()
    {
        serialPort.Close();
    }

 

    private void AppendToFileBuffer(byte[] buffer)
    {
        using (StreamWriter sw = File.AppendText("lidarPacket.txt"))
        {
            foreach (byte b in buffer)
            {
                sw.Write(b.ToString("X2") + " ");
            }
            sw.WriteLine();
        }
    }

   
}

