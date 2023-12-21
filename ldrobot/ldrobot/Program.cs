using System;
using System.IO;
using System.IO.Ports;

/// <summary>
/// Represents a simple program for receiving and analyzing LIDAR data from a serial port.
/// </summary>
class Program
{
    static SerialPort serialPort;

    static void Main()
    {
        string portName = "COM5";
        int baudRate = 230400;

        serialPort = new SerialPort(portName, baudRate);

        serialPort.Open();

        serialPort.DataReceived += SerialPort_DataReceived;

        Console.WriteLine("Waiting for LIDAR data. Press Enter to exit.");
        Console.ReadLine();

        serialPort.Close();
    }

    /// <summary>
    /// Event handler for the DataReceived event of the serial port.
    /// </summary>
    private static void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        byte[] buffer = new byte[serialPort.BytesToRead];
        serialPort.Read(buffer, 0, buffer.Length);

        LidarData lidarData = LidarData.FromRawData(buffer);

        Console.WriteLine(lidarData);

        AppendToFile("lidar.txt", BitConverter.ToString(buffer));
    }

    /// <summary>
    /// Appends text to a file.
    /// </summary>
    /// <param name="fileName">The name of the file to which text should be appended.</param>
    /// <param name="text">The text to append to the file.</param>
    private static void AppendToFile(string fileName, string text)
    {
        using (StreamWriter sw = File.AppendText(fileName))
        {
            sw.WriteLine(text);
        }
    }
}