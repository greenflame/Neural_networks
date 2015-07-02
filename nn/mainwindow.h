#ifndef MAINWINDOW_H
#define MAINWINDOW_H

#include <QMainWindow>

#include <QList>
#include <QVector>
#include <QPair>
#include <QGenericMatrix>
#include <QFileDialog>
#include <QImage>
#include <QPicture>
#include <QColor>

#include <perceptron.h>
#include <mnistreader.h>

namespace Ui {
class MainWindow;
}

class MainWindow : public QMainWindow
{
    Q_OBJECT

public:
    explicit MainWindow(QWidget *parent = 0);
    ~MainWindow();

public slots:
    void on_pushButton_test_clicked();
    void on_pushButton_mnist_clicked();
    void on_pushButton_start_clicked();

private:
    Ui::MainWindow *ui;

    QString vecToStr(const QVector<int> &vector);
    QVector<int> imgToBits(const QImage &image);
};

#endif // MAINWINDOW_H
