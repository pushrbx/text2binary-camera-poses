using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Text2BinaryCameraPoses
{
    internal class Program
    {
        [STAThread]
        private static int Main(string[] args)
        {
            var dialog = new OpenFileDialog
            {
                Multiselect = false,
                CheckFileExists = true
            };

            var filename = "";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                filename = dialog.SafeFileName;
            }
            else
            {
                Console.WriteLine("no file selected. exiting...");

                return 1;
            }

            if (!File.Exists(filename))
                return 1;

            Debug.Assert(filename != null, "filename != null");
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

            Console.WriteLine("Results has been written to output.dat in the current working directory.");
            return 0;
        }
    }

    internal class Pose
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
