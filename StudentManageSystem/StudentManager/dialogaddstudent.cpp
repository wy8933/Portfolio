#include "dialogaddstudent.h"
#include "ui_dialogaddstudent.h"

#include <QMessageBox>
DialogAddStudent::DialogAddStudent(QWidget *parent) :
    QDialog(parent),
    ui(new Ui::DialogAddStudent)
{
    ui->setupUi(this);
}

DialogAddStudent::~DialogAddStudent()
{
    delete ui;
}

void DialogAddStudent::setType(bool isAdd, StudentInfo sInfo)
{
    this->isAdd = isAdd;
    this->sInfo = sInfo;

    ui->lineEdit_name->setText(sInfo.name);
    ui->spinBox_age->setValue(sInfo.age);
    ui->lineEdit_grade->setText(QString::number(sInfo.grade));
    ui->lineEdit_class->setText(QString::number(sInfo.year));
    ui->lineEdit_studentID->setText(QString::number(sInfo.studentID));
    ui->lineEdit_phoneNum->setText(sInfo.phoneNum);
    ui->lineEdit_major->setText(sInfo.major);
}

void DialogAddStudent::on_pushButton_save_clicked()
{

    //read the input from the window enter by the user

    sInfo.name = ui->lineEdit_name->text();
    sInfo.age = ui->spinBox_age->value();
    sInfo.grade = ui->lineEdit_grade->text().toInt();
    sInfo.year = ui->lineEdit_class->text().toInt();
    sInfo.studentID = ui->lineEdit_studentID->text().toInt();
    sInfo.phoneNum = ui->lineEdit_phoneNum->text();
    sInfo.major = ui->lineEdit_major->text();

    if(isAdd){
        stuSql::getInstance()->addStudent(sInfo);
    }
    else{
        stuSql::getInstance()->UpdateStudentInfo(sInfo);
    }

    QMessageBox::information(nullptr,"Information","Save successfully");
    this->hide();
}


void DialogAddStudent::on_pushButton_cancel_clicked()
{
    this->hide();
}

