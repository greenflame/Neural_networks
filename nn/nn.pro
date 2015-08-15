#-------------------------------------------------
#
# Project created by QtCreator 2015-06-26T20:57:55
#
#-------------------------------------------------

QT       += core gui

greaterThan(QT_MAJOR_VERSION, 4): QT += widgets

TARGET = nn
TEMPLATE = app


SOURCES += main.cpp\
        mainwindow.cpp \
    perceptron.cpp \
    mnistreader.cpp \
    neurallayer.cpp

HEADERS  += mainwindow.h \
    perceptron.h \
    mnistreader.h \
    neurallayer.h

FORMS    += mainwindow.ui
