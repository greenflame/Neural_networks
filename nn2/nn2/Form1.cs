using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace nn2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Log("Loading training data...");

            List<byte[]> rawTrainQuestions = MNISTTools.ReadImages("mnist/t10k-images.idx3-ubyte");
            int[] rawTrainAnswers = MNISTTools.ReadLabels("mnist/t10k-labels.idx1-ubyte");

            Log("Adjusting training data..");

            List<double[]> trainQuestions = MNISTTools.AdjustImages(MNISTTools.ReadImages("mnist/t10k-images.idx3-ubyte"));
            List<double[]> trainAnswers = MNISTTools.AdjustLabels(MNISTTools.ReadLabels("mnist/t10k-labels.idx1-ubyte"));

            Log("Loading network..");

            NeuralNetwork nn = new NeuralNetwork("nn.txt");

            Log("Testing network..");

            int correctAnswers = 0;

            for (int i = 0; i < trainQuestions.Count; i++)
            {
                double[] nnOut = nn.Classify(trainQuestions[i]);

                int maxInd = 0;

                for (int j = 0; j < nnOut.Length; j++)
                {
                    if (nnOut[j] > nnOut[maxInd])
                    {
                        maxInd = j;
                    }
                }

                if (maxInd == rawTrainAnswers[i])
                {
                    correctAnswers++;
                }
            }

            Log("correct answers: " + correctAnswers.ToString() + " / " + trainQuestions.Count);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Log("Loading training data...");

            List<byte[]> rawTrainQuestions = MNISTTools.ReadImages("mnist/t10k-images.idx3-ubyte");
            int[] rawTrainAnswers = MNISTTools.ReadLabels("mnist/t10k-labels.idx1-ubyte");

            Log("Adjusting training data..");

            List<double[]> trainQuestions = MNISTTools.AdjustImages(MNISTTools.ReadImages("mnist/t10k-images.idx3-ubyte"));
            List<double[]> trainAnswers = MNISTTools.AdjustLabels(MNISTTools.ReadLabels("mnist/t10k-labels.idx1-ubyte"));

            Log("Creating testing set..");

            List<double[]> testQuestions = new List<double[]>();
            List<double[]> testAnswers = new List<double[]>();
            for (int i = 0; i < 1000; i++)
            {
                testQuestions.Add(trainQuestions[i]);
                testAnswers.Add(trainAnswers[i]);
            }

            Log("Creating network..");

            int hiddenUnits = 20;
            double kBegin = 0.9;
            double kDelta = 0.05;
            double kEnd = 0.1;

            Log("kBegin: " + kBegin + " kDelta: " + kDelta + " kEnd: " + kEnd + " hidden units: " + hiddenUnits);

            NeuralNetwork nn = new NeuralNetwork(new int[] { trainQuestions[0].Length, hiddenUnits, trainAnswers[0].Length });
            double k = kBegin;

            Log("Teaching network..");

            while (k > kEnd)
            {
                TeachSet(trainQuestions, trainAnswers, k, nn);
                double globalError = nn.GlobalError(testQuestions, testAnswers);

                Log("Global error: " + globalError + " k: " + k);
                k -= kDelta;
            }

            Log("Testing network..");

            int correctAnswers = 0;

            for (int i = 0; i < trainQuestions.Count; i++)
            {
                double[] nnOut = nn.Classify(trainQuestions[i]);

                int maxInd = 0;

                for (int j = 0; j < nnOut.Length; j++)
                {
                    if (nnOut[j] > nnOut[maxInd])
                    {
                        maxInd = j;
                    }
                }

                if (maxInd == rawTrainAnswers[i])
                {
                    correctAnswers++;
                }
            }

            Log("correct answers: " + correctAnswers.ToString() + " / " + trainQuestions.Count);


            Log("Saving..");
            nn.Save("nn.txt");
            Log("ok");
        }

        private void TeachSet(List<double[]> inputs, List<double[]> outputs, double k, NeuralNetwork nn)
        {
            for (int i = 0; i < inputs.Count; i++)
            {
                nn.Learn(inputs[i], outputs[i], k);
            }
        }

        private void Log(string s)
        {
            richTextBox1.AppendText("[" + DateTime.Now.ToString() + "] " + s + '\n');
            Application.DoEvents();
        }

    }
}
