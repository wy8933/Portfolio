#ifndef DIALOGADDSTUDENT_H
#define DIALOGADDSTUDENT_H

#include <QWidget>
#include <QDialog>
#include "stusql.h"

namespace Ui {
class DialogAddStudent;
}

class DialogAddStudent : public QDialog
{
    Q_OBJECT

public:
    explicit DialogAddStudent(QWidget *parent = nullptr);
    ~DialogAddStudent();
    void setType(bool isAdd,StudentInfo sInfo = {});

private slots:
    void on_pushButton_save_clicked();

    void on_pushButton_cancel_clicked();

private:
    Ui::DialogAddStudent *ui;
    bool isAdd;
    StudentInfo sInfo;
};

#endif // DIALOGADDSTUDENT_H
