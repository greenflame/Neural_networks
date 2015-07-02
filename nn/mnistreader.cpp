#include "mnistreader.h"

MnistReader::MnistReader()
{

}

MnistReader::~MnistReader()
{

}

QVector<QImage> MnistReader::readImages(const QString &fileName)
{
    QFile file(fileName);
    file.open(QIODevice::ReadOnly);
    QDataStream in(&file);

    quint32 magicNumber, imagesCount, height, width;
    in >> magicNumber >> imagesCount >> height >> width;

    QVector<QImage> images;

    for (int z = 0; z < (int)imagesCount; z++)
    {
        QImage image(width, height, QImage::Format_RGB32);

        for (int i = 0; i < (int)height; i++)
        {
            for (int j = 0; j < (int)width; j++)
            {
                quint8 data;
                in >> data;
                image.setPixel(j, i, qRgb(data, data, data));
            }
        }

        images.append(image);
    }

    return images;
}

QVector<int> MnistReader::readLabels(const QString &fileName)
{
    QFile file(fileName);
    file.open(QIODevice::ReadOnly);
    QDataStream in(&file);

    quint32 magicNumber, labelsCount;
    in >> magicNumber >> labelsCount;

    QVector<int> labels;

    for (int i = 0; i < (int)labelsCount; i++)
    {
        quint8 data;
        in >> data;
        labels.append(data);
    }

    return labels;
}

