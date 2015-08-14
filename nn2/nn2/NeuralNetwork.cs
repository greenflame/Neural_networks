using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nn2
{
    class NeuralNetwork
    {
        public int[] Configuration { get; set; }
        private NeuralLayer[] Layers { get; set; }

        public NeuralNetwork(int[] configuration)
        {
            Configuration = configuration;
            CreateLayersFromConfiguration();
        }

        public NeuralNetwork(string fileName)
        {
            Load(fileName);
        }

        private void CreateLayersFromConfiguration()
        {
            Layers = new NeuralLayer[Configuration.Length];

            for (int i = 0; i < Configuration.Length; i++)
            {
                if (i == 0)
                {
                    Layers[i] = new NeuralLayer(0, Configuration[i]);   // First layer only for outputs
                }
                else
                {
                    Layers[i] = new NeuralLayer(Configuration[i - 1], Configuration[i]);
                    Layers[i].PrevLayer = Layers[i - 1];    // Linking layers
                }
            }
        }

        public double[] Classify(double[] input)
        {
            Layers[0].Outputs = input;

            for (int i = 1; i < Layers.Length; i++)
            {
                Layers[i].Classify();
            }

            return Layers[Layers.Length - 1].Outputs;   // todo return copy?
        }

        public void Learn(double[] input, double[] output, double k)
        {
            Classify(input);

            Layers[Layers.Length - 1].CalculateDeltaBaseOnTarget(output);

            for (int i = Layers.Length - 1; i > 0; i--)
            {
                if (i != 1)
                {
                    Layers[i].CalculateDeltaForPrevLayer();
                }

                Layers[i].LearnByDelta(k);
            }
        }

        public double LocalError(double[] input, double[] target)
        {
            double[] output = Classify(input);

            double error = 0;

            for (int i = 0; i < output.Length; i++)
            {
                error += Math.Pow(output[i] - target[i], 2);
            }

            error = Math.Pow(error / output.Length, 0.5);
            return error;
        }

        public double GlobalError(List<double[]> inputs, List<double[]> targets)
        {
            double error = 0;

            for (int i = 0; i < inputs.Count; i++)
            {
                double localErr = LocalError(inputs[i], targets[i]);
                error += Math.Pow(localErr, 2);
            }

            error = Math.Pow(error / targets.Count, 0.5);
            return error;
        }

        public void Save(string fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);

            bw.Write(Configuration.Length);

            for (int i = 0; i < Configuration.Length; i++)
            {
                bw.Write(Configuration[i]);
            }

            for (int z = 1; z < Layers.Length; z++)
            {
                for (int i = 0; i < Layers[i].Weights.GetLength(0); i++)
                {
                    for (int j = 0; j < Layers[i].Weights.GetLength(1); j++)
                    {
                        bw.Write(Layers[z].Weights[i, j]);
                    }
                }
            }

            bw.Close();
        }

        public void Load(string fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);

            int configurationLength = br.ReadInt32();
            Configuration = new int[configurationLength];

            for (int i = 0; i < Configuration.Length; i++)
            {
                Configuration[i] = br.ReadInt32();
            }

            CreateLayersFromConfiguration();

            for (int z = 1; z < Layers.Length; z++)
            {
                for (int i = 0; i < Layers[i].Weights.GetLength(0); i++)
                {
                    for (int j = 0; j < Layers[i].Weights.GetLength(1); j++)
                    {
                        Layers[z].Weights[i, j] = br.ReadDouble();
                    }
                }
            }

            br.Close();
        }

    }
}
