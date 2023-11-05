#include "page_login.h"
#include "ui_page_login.h"
#include "stusql.h"

#include <QMessageBox>
Page_Login::Page_Login(QWidget *parent) :
    QWidget(parent),
    ui(new Ui::Page_Login)
{
    ui->setupUi(this);
}

Page_Login::~Page_Login()
{
    delete ui;
}

void Page_Login::on_btn_login_clicked()
{

    //check login
    QString username = ui->lineEdit_userName->text();
    QString password = ui->lineEdit_password->text();

    if(stuSql::getInstance()->checkLoginInfo(username,password)){
        stuSql::getInstance()->_username = username;
        emit sendLoginSuccess();
    }else{
        QMessageBox::information(nullptr,"Warning","Either your password or username is incorrect");
    }

}

//exit the software
void Page_Login::on_btn_exit_clicked()
{
    exit(0);
}



