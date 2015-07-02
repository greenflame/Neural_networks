#include "perceptron.h"

Perceptron::Perceptron(int inputsCount, int threshhold)
{
    threshhold_ = threshhold;
    inputsCount_ = inputsCount;

    weights_ = QVector<int>(inputsCount_);
}

Perceptron::~Perceptron()
{

}

int Perceptron::classify(const QVector<int> input)
{
    int summ = 0;

    for (int i = 0; i < inputsCount_; i++)
    {
        summ += input[i] * weights_[i];
    }

    return summ > threshhold_ ? 1 : 0;
}

void Perceptron::teach(const QVector<int> input, int output)
{
    int answer = classify(input);

    if (answer == output)
    {
        return;
    }
    else if (answer == 0)   // Need 1
    {
        for (int i = 0; i < inputsCount_; i++)
        {
            weights_[i] += input[i];
        }
    }
    else if  (answer == 1)
    {
        for (int i = 0; i < inputsCount_; i++)
        {
            weights_[i] -= input[i];
        }
    }
}

QVector<int> Perceptron::weights() const
{
    return weights_;
}

int Perceptron::threshhold() const
{
    return threshhold_;
}

int Perceptron::inputsCount() const
{
    return inputsCount_;
}
