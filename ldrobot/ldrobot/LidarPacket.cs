using ldrobot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

public class LidarPacket
{
    private byte Header; // 0x54
    private byte VerLen;// 3 bits packet type, 5 bits measurement points
    private ushort Speed; // degrees per second
    private float StartAngle; //degree
    private List<(ushort Distance, ushort Intensity)> Data;// (mm) Measurement data (2 bytes distance, 1 byte intensity per measurement point)
    private float EndAngle;// degree
    private ushort Timestamp;// millisecond
    private byte Crc;

    private int StepCheck;
    private int PacketNumber;
    private int IsData;
    public LidarPacket()
    {
        StepCheck = 0;
        PacketNumber = 0;
        IsData = 0;
    }

    public void AnalyzeLidarPacket(byte[] buffer, int measuringPoint)
    {
        if (buffer != null && buffer.Length == 47)
        {
            PacketNumber++;

            Header = buffer[0];
            VerLen = buffer[1];
            Crc = (buffer[46]);
            ParseLidarPacket(buffer, measuringPoint);
            CalStepAngle(measuringPoint);
            AppendToFilePacket();
        }
    }

    private void ParseLidarPacket(byte[] buffer, int measuringPoint)
    {
        Speed = (ushort)((buffer[3] << 8) | buffer[2]);
        StartAngle = (float)((buffer[5] << 8) | buffer[4]);

        // Extract Data (2 bytes distance, 1 byte intensity per measurement point)
        Data = new List<(ushort Distance, ushort Intensity)>();
        for (int i = 0; i < measuringPoint * 3; i += 3)
        {
            ushort distance = (ushort)((buffer[i + 7] << 8) | buffer[i + 6]);
            ushort intensity = buffer[i + 8];
            Data.Add((distance, intensity));
        }

        EndAngle = (float)((buffer[43] << 8) | buffer[42]);
        Timestamp = (ushort)((buffer[45] << 8) | buffer[44]);
    }


    public void CalStepAngle(int measuringPoint)
    {
        float step = (EndAngle - StartAngle) / (measuringPoint - 1);
        
        for (int i = 0; i < measuringPoint; i++)
        {
            float angle = StartAngle + step * i;
            if (i == 0 && angle == StartAngle || i == (measuringPoint - 1) && angle == EndAngle)
                StepCheck++;
        }

        IsDataCheck();
    }

    private void IsDataCheck()
    {
        if (StepCheck == 2)
        {
            StepCheck = 0;
            IsData++;
            if (PacketNumber == 10)
            {
                if (IsData == 10)
                {
                    Console.WriteLine("CORRECT DATAAAAAAAAAAAA");
                }
                else
                {
                    Console.WriteLine("incorrect data");
                }
                IsData = 0;

            }
        }
        else
        {
            Console.WriteLine("Failed packet: Step error!!");
        }
    }
    public void AppendToFilePacket()
    {
        using (StreamWriter sw = File.AppendText("lidarPacket.txt"))
        {
            string result = ("Header (byte): " + Header.ToString("X2") + " VerLen (byte): " + VerLen.ToString("X2") +
                " Speed degrees per second (byte): " + Speed.ToString("X2") + " StartAngle (ushort): " + StartAngle +
                " EndAngle (ushort): " + EndAngle + " Timestamp (ushort): " + Timestamp + " Crc (byte): " + Crc.ToString("X2"));

            sw.WriteLine(result);
            sw.WriteLine("Lidar Data:");
            foreach (var dataPoint in Data)
            {
                sw.WriteLine($"  Distance: {dataPoint.Distance} mm, Intensity: {dataPoint.Intensity}");
            }
            sw.WriteLine();
        }
        Clear();
    }

    private void Clear()
    {
        Header = 0;
        VerLen = 0;
        Speed = 0;
        StartAngle = 0;
        Data = null;
        EndAngle = 0;
        Timestamp = 0;
        Crc = 0;
        StepCheck = 0;
        if (PacketNumber == 10) 
        {
            //Console.WriteLine("10 packet was received");
            PacketNumber = 0;
        }
}
}

