#include "mainwindow.h"
#include "ui_mainwindow.h"

MainWindow::MainWindow(QWidget *parent) :
    QMainWindow(parent),
    ui(new Ui::MainWindow)
{
    ui->setupUi(this);
}

MainWindow::~MainWindow()
{
    delete ui;
}

void MainWindow::on_pushButton_test_clicked()
{
    ui->textEdit_output->append("Hello world!");

    int inputs[10][15] = {
        {1,1,1,1,0,1,1,0,1,1,0,1,1,1,1},// 0
        {0,0,1,0,1,0,1,0,1,0,0,1,0,0,1},// 1
        {1,1,1,0,0,1,1,1,1,1,0,0,1,1,1},// 2
        {1,1,1,0,0,1,1,1,1,0,0,1,1,1,1},// 3
        {1,0,1,1,0,1,1,1,1,0,0,1,0,0,1},// 4
        {1,1,1,1,0,0,1,1,1,0,0,1,1,1,1},// 5
        {1,1,1,1,0,0,1,1,1,1,0,1,1,1,1},// 6
        {1,1,1,0,0,1,0,1,0,1,0,0,1,0,0},// 7
        {1,1,1,1,0,1,1,1,1,1,0,1,1,1,1},// 8
        {1,1,1,1,0,1,1,1,1,0,0,1,1,1,1} // 9
    };

    int outputs[10][3] = {
        {1, 1, 0},
        {0, 1, 0},
        {1, 1, 1},
        {0, 1, 1},
        {1, 1, 0},
        {0, 0, 0},
        {1, 0, 1},
        {0, 0, 1},
        {1, 0, 0},
        {0, 0, 0}
    };

    NeuralLayer nl(15, 3);

    int iterations = 100;
    for (int z = 0; z < iterations; z++)
    {
        int errors = 0;
        for (int i = 0; i < 10; i++)
        {
            QVector<int> input;
            for (int j = 0; j < 15; j++)
            {
                input.append(inputs[i][j]);
            }

            QVector<int> output;
            for (int j = 0; j < 3; j++)
            {
                output.append(outputs[i][j]);
            }

            QVector<int> answer = nl.classify(input);

            if (!vecEqual(answer, output))
            {
                errors++;
            }

            ui->textEdit_output->append("Input: " + vecToStr(input) +
                    " Output: " + vecToStr(output) +
                    " Answer: " + vecToStr(answer));

            ui->textEdit_output->append("Weights before: " + vecToStr(nl.weights()));
            nl.teach(input, output);
            ui->textEdit_output->append("Weights after_: " + vecToStr(nl.weights()));
        }
        if (errors == 0)
        {
            ui->textEdit_output->append("Teaching complete in " + QString::number(z) + " iterations.");
            break;
        }
        if (z == iterations - 1)
        {
            ui->textEdit_output->append("Teaching not complete. Errors: " + QString::number(z) + ".");
        }
    }
}

void MainWindow::on_pushButton_mnist_clicked()
{
    QString s = QFileDialog::getOpenFileName(this, "images");

    QVector<QImage> i1 = MnistReader::readImages(s);
    QVector<QVector<int> > i2 = MnistReader::readImagesBytes(s);

    ui->textEdit_output->append(vecToStr(imgToBits(i1[0])));
    ui->textEdit_output->append(vecToStr(i2[0]));
}

void MainWindow::on_pushButton_start_clicked()
{
    ui->textEdit_output->append("Teach stage.");

    QVector<QImage> trainImages = MnistReader::readImages(QFileDialog::getOpenFileName(this, "Train images"));
    QVector<int> trainLabels = MnistReader::readLabels(QFileDialog::getOpenFileName(this, "Train labels"));

    ui->textEdit_output->append("Train tests loaded.");

    if (trainImages.length() != trainLabels.length())
    {
        ui->textEdit_output->append("Bad lengths.");
        return;
    }

    NeuralLayer neuralLayer(28 * 28 * 8, 10);

    for (int i = 0; i < trainImages.length(); i++)
    {
        neuralLayer.teach(imgToBits(trainImages.at(i)), numToBits(trainLabels.at(i)));
        if (i % 1000 ==  0)
        {
            ui->textEdit_output->append(tr("Training [%0 / %1] complete.").arg(i).arg(trainImages.length()));
            qApp->processEvents();
        }
    }

    ui->textEdit_output->append("Training complete.");
    ui->textEdit_output->append("Check stage.");

    QVector<QImage> checkImages = MnistReader::readImages(QFileDialog::getOpenFileName(this, "Test images"));
    QVector<int> checkLabels = MnistReader::readLabels(QFileDialog::getOpenFileName(this, "Test labels"));

    ui->textEdit_output->append("Check tests loaded");

    if (checkImages.length() != checkLabels.length())
    {
        ui->textEdit_output->append("Bad lengths.");
        return;
    }

    int errors = 0, succes = 0;

    for (int i = 0; i < checkImages.length(); i++)
    {
        QVector<int> networkOutput = neuralLayer.classify(imgToBits(checkImages.at(i)));

        if (vecEqual(numToBits(checkLabels.at(i)), networkOutput))
        {
            succes++;
        }
        else
        {
            errors++;
        }

        if (i % 1000 ==  0)
        {
            ui->textEdit_output->append(tr("%0 / %1 check completed").arg(i).arg(checkImages.length()));
            qApp->processEvents();
        }
    }

    ui->textEdit_output->append(tr("Testing complete."));
    ui->textEdit_output->append(tr("Succes: %0 errors: %1").arg(succes).arg(errors));
    ui->textEdit_output->append(vecToStr(neuralLayer.weights()));
}

bool MainWindow::vecEqual(QVector<int> v1, QVector<int> v2)
{
    if (v1.length() != v2.length())
    {
        return false;
    }

    for (int i = 0; i < v1.length(); i++)
    {
        if (v1.at(i) != v2.at(i))
        {
            return false;
        }
    }

    return true;
}

QString MainWindow::vecToStr(const QVector<int> &vector)
{
    if (vector.length() == 0)
    {
        return "[]";
    }

    QString result = "";
    for (int i = 0; i < vector.length(); i++)
    {
        result += ", " + QString::number(vector[i]);
    }
    return "[" + result.mid(2) + "]";
}

QVector<int> MainWindow::imgToBits(const QImage &image)
{
    QVector<int> result;

    for (int i = 0; i < image.height(); i++)
    {
        for (int j = 0; j < image.width(); j++)
        {
            quint8 value = QColor::fromRgb(image.pixel(j, i)).red();
            QString strRep =QString::number(value, 2);
            while (strRep.length() < 8)
            {
                strRep = '0' + strRep;
            }
            for (int k = 0; k < 8; k++)
            {
                result.append(QString(strRep[k]).toInt());
            }
        }
    }

    return result;
}

QVector<int> MainWindow::numToBits(int number)
{
    QVector<int> result;

    for (int i = 0; i < number; i++)
    {
        result.append(0);
    }

    result.append(1);

    while (result.length() < 10)
    {
        result.append(0);
    }

    return result;
}
