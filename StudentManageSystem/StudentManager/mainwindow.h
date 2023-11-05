#ifndef MAINWINDOW_H
#define MAINWINDOW_H

#include <QMainWindow>
#include "page_login.h"
#include "stuSql.h"
#include <dialogaddstudent.h>

QT_BEGIN_NAMESPACE
namespace Ui { class MainWindow; }
QT_END_NAMESPACE

class MainWindow : public QMainWindow
{
    Q_OBJECT

public:
    MainWindow(QWidget *parent = nullptr);
    ~MainWindow();
private slots:
    void on_button_exit_clicked();


    void on_pushButton_simulate_clicked();

    void on_pushButton_add_clicked();

    void on_pushButton_remove_clicked();

    void on_pushButton_modify_clicked();
    void on_pushButton_search_clicked();
    void on_pushButton_clear_clicked();
private:
    Ui::MainWindow *ui;
    Page_Login pageLogin;
    stuSql *ptrStuSql;
    DialogAddStudent *dlgAddStudent;

    void updateTable();
};
#endif // MAINWINDOW_H
