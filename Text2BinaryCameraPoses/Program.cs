using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Text2BinaryCameraPoses
{
    class Program
    {
        static int Main(string[] args)
        {
            var filename = @"z:\ply\SCENE_PRIOR_GREATCENTRAL_RUN1_1_plyFiles\Pose\AbsolutePoses.txt";

            if (!File.Exists(filename))
                return 1;

            var lines = File.ReadAllLines(filename);
            var poses = new List<Pose>();
            foreach (var line in lines)
            {
                if (line.Contains("#"))
                    continue;

                var fields = line.Split(',');
                for (var i = 0; i < fields.Length; i++)
                {
                    fields[i] = fields[i].Trim();
                }

                var pose = new Pose
                {
                    TimeStamp = long.Parse(fields[0]),
                    X = double.Parse(fields[1]),
                    Y = double.Parse(fields[2]),
                    Z = double.Parse(fields[3]),
                    Roll = double.Parse(fields[4]),
                    Pitch = double.Parse(fields[5]),
                    Yaw = double.Parse(fields[6])
                };

                poses.Add(pose);
            }

            using (var outFile = File.Open("output.dat", FileMode.CreateNew))
            using (var bw = new EndianBinaryWriter(EndianBitConverter.Big, outFile))
            {
                bw.Write(poses.Count); // 4 bytes
                foreach (var pose in poses)
                {
                    bw.Write(pose.TimeStamp); // 8 bytes
                    bw.Write(pose.X); // 8 bytes
                    bw.Write(pose.Y); // 8 bytes
                    bw.Write(pose.Z); // 8 bytes
                    bw.Write(pose.Roll); // 8 bytes
                    bw.Write(pose.Pitch); // 8 bytes
                    bw.Write(pose.Yaw); // 8 bytes
                    bw.Write(0xffe2); // 2 byte
                }
                bw.Write(0xffe0); // 4 bytes - end of file
                bw.Flush();
            }

            return 0;
        }
    }

    class Pose
    {
        public long TimeStamp { get; set; }

        public double X { get; set; }

        public double Y { get; set; }

        public double Z { get; set; }

        public double Roll { get; set; }

        public double Pitch { get; set; }

        public double Yaw { get; set; }
    }
}
