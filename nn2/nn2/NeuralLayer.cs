using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nn2
{
    class NeuralLayer
    {
        public double[] Outputs { get; set; }
        public double[] Delta { get; set; }

        public double[,] Weights { get; }

        public NeuralLayer(int inputsCount, int outputsCount)
        {
            Outputs = new double[outputsCount];
            Delta = new double[outputsCount];

            Weights = new double[inputsCount, outputsCount];    // Row - input; column - output
        }

        public void CalculateOutputs(double[] inputs)
        {
            MultiplyHVectorMatrix(inputs, Outputs, Weights);

            for (int i = 0; i < Outputs.Length; i++)
            {
                Outputs[i] = 1 / (1 + Math.Exp(-Outputs[i]));
            }
        }

        public void Learn(double[] inputs, double k)    // Uses delta
        {
            for (int i = 0; i < Outputs.Length; i++)
            {
                for (int j = 0; j < inputs.Length; j++)
                {
                    Weights[j, i] = Weights[j, i] + k * Delta[i] * inputs[j] * Outputs[i] * (1 - Outputs[i]);
                }
            }
        }

        public void CalculateDeltaBaseOnTarget(double[] target)
        {
            for (int i = 0; i < Outputs.Length; i++)
            {
                Delta[i] = target[i] - Outputs[i];
            }
        }

        public void CalculateDeltaForPrevLayer(double[] prevDelta)
        {
            MultiplyHVectorTrMatrix(Delta, prevDelta, Weights);
        }

        public void RandomizeWeights(double minVal, double maxVal)
        {
            Random r = new Random();

            for (int i = 0; i < Weights.GetLength(0); i++)
            {
                for (int j = 0; j < Weights.GetLength(1); j++)
                {
                    Weights[i, j] = minVal + r.NextDouble() * (maxVal - minVal);
                }
            }
        }

        private static void MultiplyHVectorMatrix(double[] inputVector, double[] outputVector, double[,] matrix)
        {
            for (int i = 0; i < outputVector.Length; i++)
            {
                outputVector[i] = 0;
                for (int j = 0; j < inputVector.Length; j++)
                {
                    outputVector[i] += inputVector[j] * matrix[j, i];
                }
            }
        }

        private static void MultiplyHVectorTrMatrix(double[] inputVector, double[] outputVector, double[,] matrix)
        {
            for (int i = 0; i < outputVector.Length; i++)
            {
                outputVector[i] = 0;
                for (int j = 0; j < inputVector.Length; j++)
                {
                    outputVector[i] += inputVector[j] * matrix[i, j];
                }
            }
        }

    }
}
