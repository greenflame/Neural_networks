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
            RandomizeWeights(-1, 1);
        }

        public NeuralNetwork(string fileName)
        {
            Load(fileName);
        }

        private void CreateLayersFromConfiguration()
        {
            Layers = new NeuralLayer[Configuration.Length - 1];

            for (int i = 0; i < Layers.Length; i++)
            {
                Layers[i] = new NeuralLayer(Configuration[i], Configuration[i + 1]);
            }
        }

        private void RandomizeWeights(double minVal, double maxVal)
        {
            foreach (NeuralLayer layer in Layers)
            {
                layer.RandomizeWeights(minVal, maxVal);
            }
        }

        public double[] Classify(double[] inputs)
        {
            Layers[0].CalculateOutputs(inputs);

            for (int i = 1; i < Layers.Length; i++)
            {
                Layers[i].CalculateOutputs(Layers[i - 1].Outputs);
            }

            return Layers[Layers.Length - 1].Outputs;   // todo return copy?
        }

        public void Learn(double[] input, double[] output, double k)
        {
            Classify(input);

            Layers[Layers.Length - 1].CalculateDeltaBaseOnTarget(output);   // Last layer

            for (int i = Layers.Length - 1; i > 0; i--) // Hidden layers
            {
                Layers[i].CalculateDeltaForPrevLayer(Layers[i - 1].Delta);
                Layers[i].Learn(Layers[i - 1].Outputs, k);
            }

            Layers[0].Learn(input, k);   // First layer
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

            for (int z = 0; z < Layers.Length; z++)
            {
                for (int i = 0; i < Layers[z].Weights.GetLength(0); i++)
                {
                    for (int j = 0; j < Layers[z].Weights.GetLength(1); j++)
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

            for (int z = 0; z < Layers.Length; z++)
            {
                for (int i = 0; i < Layers[z].Weights.GetLength(0); i++)
                {
                    for (int j = 0; j < Layers[z].Weights.GetLength(1); j++)
                    {
                        Layers[z].Weights[i, j] = br.ReadDouble();
                    }
                }
            }

            br.Close();
        }

    }
}
