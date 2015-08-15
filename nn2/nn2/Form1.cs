using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
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

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Log("Loading training data...");

            List<double[]> trainQuestions = MNISTTools.AdjustImages(MNISTTools.ReadImages("mnist/train-images.idx3-ubyte"));
            List<double[]> trainAnswers = MNISTTools.AdjustLabels(MNISTTools.ReadLabels("mnist/train-labels.idx1-ubyte"));

            Log("Loading testing set..");

            List<double[]> testQuestions = MNISTTools.AdjustImages(MNISTTools.ReadImages("mnist/t10k-images.idx3-ubyte"));
            List<double[]> testAnswers = MNISTTools.AdjustLabels(MNISTTools.ReadLabels("mnist/t10k-labels.idx1-ubyte"));

            Log("Creating network..");

            // Parsing configuration
            string[] hiddenLayersConf = textBox_configuration.Text.Split(' ');

            int[] configuration = new int[hiddenLayersConf.Length + 2];

            configuration[0] = trainQuestions[0].Length;
            configuration[configuration.Length - 1] = trainAnswers[0].Length;

            for (int i = 0; i < hiddenLayersConf.Length; i++)
            {
                configuration[i + 1] = Convert.ToInt32(hiddenLayersConf[i]);
            }

            // Parsing speed coefficients
            double kBegin = double.Parse(textBox_kSettings.Text.Split(' ')[0], CultureInfo.InvariantCulture);
            double kDelta = double.Parse(textBox_kSettings.Text.Split(' ')[1], CultureInfo.InvariantCulture);
            double kEnd = double.Parse(textBox_kSettings.Text.Split(' ')[2], CultureInfo.InvariantCulture);

            Log("Speed: kBegin: " + kBegin + " kDelta: " + kDelta + " kEnd: " + kEnd);
            Log("Configuration: [" + string.Join(" ", configuration) + "]");

            NeuralNetwork nn = new NeuralNetwork(configuration);
            double k = kBegin;

            Log("Teaching network..");

            while (k >= kEnd)
            {
                TeachSet(trainQuestions, trainAnswers, k, nn);
                double globalError = nn.GlobalError(testQuestions, testAnswers);

                Log("Global error: " + globalError + " k: " + k);
                k -= kDelta;
            }

            Log("Testing network..");

            double correctAnswers = TestNetwork(trainQuestions, trainAnswers, nn);
            Log("Correct answers: " + correctAnswers);


            Log("Saving..");

            string fileName = textBox_outFileName.Text;
            nn.Save(fileName + ".bin");
            File.WriteAllLines(fileName + ".txt", new string[] { richTextBox1.Text });

            Log("ok");
        }

        private void TeachSet(List<double[]> inputs, List<double[]> outputs, double k, NeuralNetwork nn)
        {
            for (int i = 0; i < inputs.Count; i++)
            {
                nn.Learn(inputs[i], outputs[i], k);
            }
        }

        private double TestNetwork(List<double[]> trainQuestions, List<double[]> trainAnswers, NeuralNetwork nn)
        {
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

                if (trainAnswers[i][maxInd] == 1)
                {
                    correctAnswers++;
                }
            }

            return (double)correctAnswers / trainQuestions.Count;
        }

        private void Log(string s)
        {
            richTextBox1.AppendText("[" + DateTime.Now.ToString() + "] " + s + '\n');
            File.WriteAllLines("log.txt", new string[] { richTextBox1.Text });
            Application.DoEvents();
        }

    }
}
