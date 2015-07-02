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

    int outputs[10] = {1,0,1,0,1,0,1,0,1,0};

    Perceptron perceptron(15, 0);

    int iterations = 10;
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
            int output = outputs[i];

            int answer = perceptron.classify(input);

            if (answer != output)
            {
                errors++;
            }

            ui->textEdit_output->append("Input: " + vecToStr(input) +
                    " Output: " + QString::number(output) +
                    " Answer: " + QString::number(answer));

            ui->textEdit_output->append("Weights before: " + vecToStr(perceptron.weights()));
            perceptron.teach(input, output);
            ui->textEdit_output->append("Weights after_: " + vecToStr(perceptron.weights()));
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
    QString imagesFileName = QFileDialog::getOpenFileName(this, "Images");
    QVector<QImage> images = MnistReader::readImages(imagesFileName);
    ui->label->setPixmap(QPixmap::fromImage(images[0]));
//    ui->textEdit_output->append(QString::number(images.length()));
    ui->textEdit_output->append(vecToStr(imgToBits(images[0])));

//    QString labelsFileName = QFileDialog::getOpenFileName(this, "Labels");
    //    ui->textEdit_output->append(vecToStr(MnistReader::readLabels(labelsFileName)));
}

void MainWindow::on_pushButton_start_clicked()
{
    ui->textEdit_output->append("Teach stage");

    QString trainImagesFileName = QFileDialog::getOpenFileName(this, "Train images");
    QVector<QImage> images = MnistReader::readImages(trainImagesFileName);

    ui->textEdit_output->append("Images loaded");

    QString trainLabelsFileName = QFileDialog::getOpenFileName(this, "Train labels");
    QVector<int> labels = MnistReader::readLabels(trainLabelsFileName);

    ui->textEdit_output->append("Labels loaded");

    Perceptron perceptron(28 * 28 * 8, 0);

    if (images.length() != labels.length())
    {
        ui->textEdit_output->append("Bad lengths");
        return;
    }

    for (int i = 0; i < images.length(); i++)
    {
        int answer = labels.at(i) == 5 ? 1 : 0;
        perceptron.teach(imgToBits(images.at(i)), answer);
        if (i % 1000 ==  0)
        {
            ui->textEdit_output->append(tr("%0 / %1 completed").arg(i + 1).arg(images.length()));
            qApp->processEvents();
        }
    }

    ui->textEdit_output->append("Check stage");

    QString checkImagesFileName = QFileDialog::getOpenFileName(this, "Train images");
    QVector<QImage> checkImages = MnistReader::readImages(checkImagesFileName);

    ui->textEdit_output->append("Images loaded");

    QString checkLabelsFileName = QFileDialog::getOpenFileName(this, "Train labels");
    QVector<int> checkLabels = MnistReader::readLabels(checkLabelsFileName);

    ui->textEdit_output->append("Labels loaded");


    if (checkImages.length() != checkLabels.length())
    {
        ui->textEdit_output->append("Bad lengths");
        return;
    }

    int errors = 0, succes = 0;

    for (int i = 0; i < checkImages.length(); i++)
    {
        int answer = checkLabels.at(i) == 5 ? 1 : 0;
        int out = perceptron.classify(imgToBits(checkImages.at(i)));

        if (answer == out)
        {
            succes++;
        }
        else
        {
            errors++;
        }

        if (i % 1000 ==  0)
        {
            ui->textEdit_output->append(tr("%0 / %1 check completed").arg(i + 1).arg(checkImages.length()));
            qApp->processEvents();
        }
    }

    ui->textEdit_output->append(tr("succes: %0 errors: %1").arg(succes).arg(errors));
    ui->textEdit_output->append(vecToStr(perceptron.weights()));
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
