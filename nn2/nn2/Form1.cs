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
            double[,] Weights = new double[,] { { 1, 2 }, { 3, 4 }, { 5, 6 } };
            double[,] tw = new double[2, 3];


        }

        private void button2_Click(object sender, EventArgs e)
        {
            Log("loading tests data...");

            List<byte[]> rawTestInputs = MNISTTools.ReadImages("mnist/t10k-images.idx3-ubyte");
            int[] rawTestOutputs = MNISTTools.ReadLabels("mnist/t10k-labels.idx1-ubyte");

            Log("adjusting test data..");

            List<double[]> testInputs = MNISTTools.AdjustImages(MNISTTools.ReadImages("mnist/t10k-images.idx3-ubyte"));
            List<double[]> testOutputs = MNISTTools.AdjustLabels(MNISTTools.ReadLabels("mnist/t10k-labels.idx1-ubyte"));

            Log("creating check set..");
            Random r = new Random();
            List<double[]> csi = new List<double[]>();
            List<double[]> cso = new List<double[]>();
            for (int i = 0; i < 1000; i++)
            {
                csi.Add(testInputs[i]);
                cso.Add(testOutputs[i]);
            }

            Log("data loaded");

            //NeuralLayer nl = new NeuralLayer(testInputs[0].Length, testOutputs[0].Length);

            int hl = 300;
            NeuralNetwork nn = new NeuralNetwork(new int[] { testInputs[0].Length, hl, testOutputs[0].Length });

            double k = 0.9;
            double dk = 0.05;
            Log("dk: " + k + " hl: " + hl);
            Log(DateTime.Now.ToString());
            while (k > 0.1)
            {
                Log(nn.GlobalError(csi, cso).ToString());
                TeachSet(testInputs, testOutputs, 0.9, nn);
                Log("k: " + k.ToString());
                k -= dk;
            }
            Log(DateTime.Now.ToString());
            Log("testing network");

            int correctAnswers = 0;

            for (int i = 0; i < testInputs.Count; i++)
            {
                double[] nnOut = nn.Classify(testInputs[i]);

                int maxInd = 0;

                for (int j = 0; j < nnOut.Length; j++)
                {
                    if (nnOut[j] > nnOut[maxInd])
                    {
                        maxInd = j;
                    }
                }

                if (maxInd == rawTestOutputs[i])
                {
                    correctAnswers++;
                }
            }

            Log("correct answers: " + correctAnswers.ToString());

            Log(DateTime.Now.ToString());
            nn.Save("nn.txt");
            Log("saved");
            Log(DateTime.Now.ToString());
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
            richTextBox1.AppendText(s + '\n');
            Application.DoEvents();
        }

    }
}
