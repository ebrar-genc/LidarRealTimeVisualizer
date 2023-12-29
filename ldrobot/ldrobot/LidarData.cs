
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ldrobot
{
    class LidarData
    {
        public List<float>[] Steps;
        public List<(float Distance, float Intensity)>[] Data;

        private int Packet;
        public LidarData()
        {
            Steps = new List<float>[450];
            Data = new List<(float Distance, float Intensity)>[450];

            Packet = 0;
        }


        public void AddLists(List<float> steps, List<(float distance, float intensity)> data)
        {
            if (Steps[Packet] == null)
            {
                Steps[Packet] = new List<float>();
            }

            if (Data[Packet] == null)
            {
                Data[Packet] = new List<(float Distance, float Intensity)>();
            }
            Steps[Packet].AddRange(steps);
            Data[Packet].AddRange(data);

            Packet++;
            if (Packet == 449)
            {
                using (StreamWriter sw = new StreamWriter("correct.txt", true))
                {
                    sw.WriteLine("\ncorrect datas");
                    for (int i = 0; i < Data.Length; i++)
                    {
                        sw.WriteLine($"Packet {i + 1}:");

                        if (Data[i] != null)
                        {
                            foreach (var item in Data[i])
                            {
                                sw.WriteLine($"Distance: {item.Distance}, Intensity: {item.Intensity}");
                            }
                        }
                    }
                    Clear();

                }

            }
        }

        private void Clear()
        {
            for (int i = 0; i < 450; i++)
            {
                Steps[i] = null;
                Data[i] = null;
            }

            Steps = new List<float>[450];
            Data = new List<(float Distance, float Intensity)>[450];
            Packet = 0;

        }

    }
}
