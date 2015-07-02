#ifndef PERCEPTRON_H
#define PERCEPTRON_H

#include <QVector>

class Perceptron
{
public:
    Perceptron(int inputsCount, int threshhold);
    ~Perceptron();

    int classify(const QVector<int> input);
    void teach(const QVector<int> input, int output);

    int threshhold() const;
    QVector<int> weights() const;

    int inputsCount() const;

private:
    QVector<int> weights_;
    int threshhold_;

    int inputsCount_;
};

#endif // PERCEPTRON_H
