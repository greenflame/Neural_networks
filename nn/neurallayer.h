#ifndef NEURALLAYER_H
#define NEURALLAYER_H

#include <QVector>

class NeuralLayer
{
public:
    NeuralLayer(int inputLength, int outputLength);
    ~NeuralLayer();

    QVector<int> classify(const QVector<int> &testInput);
    void teach(const QVector<int> &testInput, const QVector<int> &testOutput);

    int threshhold() const;
    void setThreshhold(int threshhold);

    QVector<int> weights() const;
    void setWeights(const QVector<int> &weights);

    int inputLength() const;
    void setInputLength(int inputLength);

    int outputLength() const;
    void setOutputLength(int outputLength);

private:
    int threshhold_;
    QVector<int> weights_;

    int inputLength_;
    int outputLength_;

    int index(int row, int column); // row - input, column - output
};

#endif // NEURALLAYER_H
