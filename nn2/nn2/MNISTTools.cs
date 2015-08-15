using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nn2
{
    class MNISTTools
    {
        public static int[] ReadLabels(string fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);

            int magicNumber = br.ReadInt32();   // Ignore
            int labelsCount = ReverseBytes(br.ReadInt32());

            int[] result = new int[labelsCount];

            for (int i = 0; i < labelsCount; i++)
            {
                result[i] = br.ReadByte();
            }

            fs.Close();

            return result;
        }

        public static List<Bitmap> ReadImagesToBitmaps(string fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);

            int magicNumber = br.ReadInt32();   // Ignore
            int imagesCount = ReverseBytes(br.ReadInt32());
            int rowsCount = ReverseBytes(br.ReadInt32());
            int columnsCount = ReverseBytes(br.ReadInt32());

            List<Bitmap> result = new List<Bitmap>();

            for (int z = 0; z < imagesCount; z++)
            {
                Bitmap tmp = new Bitmap(columnsCount, rowsCount);

                for (int i = 0; i < rowsCount; i++)
                {
                    for (int j = 0; j < columnsCount; j++)
                    {
                        int c = br.ReadByte();
                        tmp.SetPixel(j, i, Color.FromArgb(c, c, c));
                    }
                }

                result.Add(tmp);
            }

            fs.Close();

            return result;
        }

        public static List<byte[]> ReadImages(string fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);

            int magicNumber = br.ReadInt32();   // Ignore
            int imagesCount = ReverseBytes(br.ReadInt32());
            int rowsCount = ReverseBytes(br.ReadInt32());
            int columnsCount = ReverseBytes(br.ReadInt32());

            List<byte[]> result = new List<byte[]>();

            for (int z = 0; z < imagesCount; z++)
            {
                byte[] tmp = new byte[columnsCount * rowsCount];

                for (int i = 0; i < rowsCount * columnsCount; i++)
                {
                    tmp[i] = br.ReadByte();
                }

                result.Add(tmp);
            }

            fs.Close();

            return result;
        }

        public static List<double[]> AdjustImages(List<byte[]> input)
        {
            List<double[]> result = new List<double[]>();

            foreach (byte[] rawTestInput in input)
            {
                double[] testInput = new double[rawTestInput.Length];

                for (int i = 0; i < rawTestInput.Length; i++)
                {
                    testInput[i] = (double)rawTestInput[i] / byte.MaxValue;
                }

                result.Add(testInput);
            }

            return result;
        }

        public static List<double[]> AdjustLabels(int[] input)
        {
            List<double[]> result = new List<double[]>();

            for (int i = 0; i < input.Length; i++)
            {
                double[] tmp = new double[10];
                tmp[input[i]] = 1;
                result.Add(tmp);
            }

            return result;
        }

        private static int ReverseBytes(int input)
        {
            byte[] bytes = BitConverter.GetBytes(input);
            Array.Reverse(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }
    }
}
