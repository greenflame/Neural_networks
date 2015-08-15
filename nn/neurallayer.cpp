#include "neurallayer.h"

NeuralLayer::NeuralLayer(int inputLength, int outputLength)
{
    setInputLength(inputLength);
    setOutputLength(outputLength);

    setThreshhold(0);
    setWeights(QVector<int>(inputLength_ * outputLength_));
}

NeuralLayer::~NeuralLayer()
{

}

QVector<int> NeuralLayer::classify(const QVector<int> &testInput)
{
    QVector<int> networkOutput;

    for (int i = 0; i < outputLength_; i++)
    {
        int currentOutput = 0;
        for (int j = 0; j < inputLength_; j++)
        {
            currentOutput += testInput[j] * weights_[index(j, i)];
        }
        currentOutput = currentOutput > 0 ? 1 : 0;  // act func
        networkOutput.append(currentOutput);
    }

    return networkOutput;
}

void NeuralLayer::teach(const QVector<int> &testInput, const QVector<int> &testOutput)
{
    QVector<int> networkOutput = classify(testInput);

    for (int i = 0; i < outputLength_; i++)
    {
        if (networkOutput[i] == testOutput[i])
        {
            continue;
        }
        else if (networkOutput[i] == 0)    //testOutput[i] == 1, add
        {
            for (int j = 0; j < inputLength_; j++)
            {
                weights_[index(j, i)] += testInput[j];
            }
        }
        else    // networkOutput == 1, testOutput == 0, substract
        {
            for (int j = 0; j < inputLength_; j++)
            {
                weights_[index(j, i)] -= testInput[j];
            }
        }
    }
}

int NeuralLayer::threshhold() const
{
    return threshhold_;
}

void NeuralLayer::setThreshhold(int threshhold)
{
    threshhold_ = threshhold;
}

QVector<int> NeuralLayer::weights() const
{
    return weights_;
}

void NeuralLayer::setWeights(const QVector<int> &weights)
{
    weights_ = weights;
}

int NeuralLayer::inputLength() const
{
    return inputLength_;
}

void NeuralLayer::setInputLength(int inputLength)
{
    inputLength_ = inputLength;
}

int NeuralLayer::outputLength() const
{
    return outputLength_;
}

void NeuralLayer::setOutputLength(int outputLength)
{
    outputLength_ = outputLength;
}

int NeuralLayer::index(int row, int column)
{
    return row * outputLength_ + column;
}
