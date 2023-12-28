﻿using System.IO.Ports;
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.ComponentModel.DataAnnotations;
using ldrobot;
using System.Reflection.PortableExecutable;

/// <summary>
/// Represents a class for reading LIDAR data. Initializes the necessary components, opens a serial port connection,
/// reads LIDAR data, and handles the cleanup process upon stopping the reading operation.
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

    /// <summary>
    /// Initializes the objects,
    /// sets the parameters and starts the serial port connection.
    /// </summary>
    /// <param name="portName">The name of the serial port. </param>
    /// <param name="baudRate">The baud rate for the serial port connection.</param>
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
            AppendToFileBuffer(buffer);
            LidarPacket.AnalyzeLidarPacket(buffer, MeasuringPoint);
            LidarCrcCheck.ValidateCrc(buffer, buffer.Length - 1);
        }
    }

    /// <summary>
    /// Stops the serial port connection when user presses enter key.
    /// </summary>
    public void StopReading()
    {
        serialPort.Close();
    }

    /// <summary>
    /// Appends the provided byte array to a file in hexadecimal format.
    /// </summary>
    /// <param name="buffer"> The byte array is the package content. </param>
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

