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
    private LidarPacket LidarPacket;
    private LidarCrcCheck LidarCrcCheck;

    private int PacketLen;
    private int MeasuringPoint;
    private string PortName;
    private int BaudRate;

    public LidarReader(string portName, int baudRate)
    {
        LidarPacket = new LidarPacket();
        LidarCrcCheck = new LidarCrcCheck();
        PacketLen = 47;
        MeasuringPoint = 12;
        PortName = portName;
        BaudRate = baudRate;

        StartSerialPort();
        
    }
    
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
            AppendToFileBuffer(buffer);
            LidarPacket.AnalyzeLidarPacket(buffer, MeasuringPoint);
            LidarCrcCheck.ValidateCrc(buffer, buffer.Length - 1);
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

