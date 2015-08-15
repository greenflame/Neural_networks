#ifndef MNISTREADER_H
#define MNISTREADER_H

#include <QVector>
#include <QImage>
#include <QString>
#include <QFile>

class MnistReader
{
public:
    MnistReader();
    ~MnistReader();

    static QVector<QImage> readImages(const QString &fileName);
    static QVector<QVector<int> > readImagesBytes(const QString &fileName);
    static QVector<int> readLabels(const QString &fileName);
};

#endif // MNISTREADER_H
