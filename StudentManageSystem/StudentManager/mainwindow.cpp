#include "mainwindow.h"
#include "ui_mainwindow.h"
#include <QKeyEvent>
#include <QFile>
#include <QCoreApplication>
#include <QMessageBox>
#include "stusql.h"

MainWindow::MainWindow(QWidget *parent)
    : QMainWindow(parent),
    ui(new Ui::MainWindow),
    ptrStuSql(nullptr),
    dlgAddStudent(nullptr)
{
    ui->setupUi(this);

    dlgAddStudent = new DialogAddStudent(this);
    pageLogin.show();
    auto f = [&](){
        this->show();
        ui->label_username->setText(stuSql::getInstance()->_username);
    };
    connect(&pageLogin, &Page_Login::sendLoginSuccess,this,f);

    ui->treeWidget->clear();
    ui->treeWidget->setColumnCount(1);

    //create the list of the name of the widget
    QStringList qStringList;
    qStringList << "Student Manage System"<<"Student Manager"<<"Admin Manager";

    //create the widgets
    QTreeWidgetItem *parentWidgetItem = new QTreeWidgetItem(ui->treeWidget);
    parentWidgetItem->setText(0, qStringList[0]);
    QTreeWidgetItem *firstWidgetItem = new QTreeWidgetItem(parentWidgetItem);
    firstWidgetItem->setText(0, qStringList[1]);
    QTreeWidgetItem *secondWidgetItem = new QTreeWidgetItem(parentWidgetItem);
    secondWidgetItem->setText(0, qStringList[2]);

    //make the other widget child of the parent widget
    parentWidgetItem->addChild(firstWidgetItem);
    parentWidgetItem->addChild(secondWidgetItem);

    //add the widget into the tree
    ui->treeWidget->addTopLevelItem(parentWidgetItem);

    //expand all the widget at default
    ui->treeWidget->expandAll();
    ui->stackedWidget->setCurrentWidget(0);

    updateTable();
}

MainWindow::~MainWindow()
{
    delete ui;
}


void MainWindow::on_button_exit_clicked()
{
    QSqlDatabase::removeDatabase("QSQLITE");
    delete ptrStuSql;
    delete dlgAddStudent;
    exit(0);
}

void MainWindow::on_pushButton_simulate_clicked()
{
    //create 1000 student
    for(int i = 1; i<=1000;i++){
        StudentInfo info;
        qint8 randomNum = (rand() % 5);
        info.name = QString("Student") + QString::number(ptrStuSql->getStudentCount()+1);
        info.age = randomNum+18;
        info.grade = randomNum+1;
        info.year = 2027-randomNum;
        info.studentID = rand()%100000000 +100000000;
        info.phoneNum = QString::number(rand()%10000000000+10000000000);

        QList<QString> majorList = {"Computer Science", "Game Design and Development", "Software Engineer","Computer Engineering","Biology","Chemistry","Mathematics"};
        info.major = majorList[rand()%majorList.size()];
        ptrStuSql->addStudent(info);
    }

    updateTable();

}

void MainWindow::on_pushButton_add_clicked()
{
    dlgAddStudent->setType(true);
    dlgAddStudent->exec();
    updateTable();
}

void MainWindow::updateTable()
{
    //clear and reset the table
    ui->tableWidget->clear();
    QStringList headers;
    headers<<"Table ID"<<"System ID"<<"Name"<<"Age"<<"Grade"<<"Class"<<"Student ID"<<"Phone Number"<< "Major";
    ui->tableWidget->setHorizontalHeaderLabels(headers);

    //only allow select a single row
    ui->tableWidget->setSelectionMode(QAbstractItemView::SingleSelection);
    ui->tableWidget->setSelectionBehavior(QAbstractItemView::SelectRows);

    //don't allow user to edit from the table
    ui->tableWidget->setEditTriggers(QAbstractItemView::NoEditTriggers);

    //get the instance of the database
    ptrStuSql = stuSql::getInstance();

    //get the total number of student
    qint32 studentCount = ptrStuSql->getStudentCount();
    ui->label_studentCount->setText(QString("Total Student:%1").arg(studentCount));

    //get all the student from the database
    QList<StudentInfo> studentList = ptrStuSql->getPageStudent(0,studentCount);

    //write the data into the table
    ui->tableWidget->setRowCount(studentCount);
    for(int i = 0; i < studentCount; i++){
        ui->tableWidget->setItem(i,0,new QTableWidgetItem(QString::number(i+1)));
        ui->tableWidget->setItem(i,1,new QTableWidgetItem(QString::number(studentList[i].id)));
        ui->tableWidget->setItem(i,2,new QTableWidgetItem(studentList[i].name));
        ui->tableWidget->setItem(i,3,new QTableWidgetItem(QString::number(studentList[i].age)));
        ui->tableWidget->setItem(i,4,new QTableWidgetItem(QString::number(studentList[i].grade)));
        ui->tableWidget->setItem(i,5,new QTableWidgetItem(QString::number(studentList[i].year)));
        ui->tableWidget->setItem(i,6,new QTableWidgetItem(QString::number(studentList[i].studentID)));
        ui->tableWidget->setItem(i,7,new QTableWidgetItem(studentList[i].phoneNum));
        ui->tableWidget->setItem(i,8,new QTableWidgetItem(studentList[i].major));
    }
}

void MainWindow::on_pushButton_remove_clicked()
{
    int currentRow = ui->tableWidget->currentRow();
    if(currentRow>=0){
        int id = ui->tableWidget->item(currentRow,0)->data(0).toUInt();
        ptrStuSql->deleteStudent(id);
        updateTable();
        QMessageBox::information(nullptr,"Information","Deleted successfully");
    }
}

void MainWindow::on_pushButton_modify_clicked()
{
    StudentInfo sInfo;
    int currentRow = ui->tableWidget->currentRow();
    if(currentRow>=0){
        sInfo.id = ui->tableWidget->item(currentRow,1)->text().toUInt();
        sInfo.name = ui->tableWidget->item(currentRow,2)->text();
        sInfo.age = ui->tableWidget->item(currentRow,3)->text().toUInt();
        sInfo.grade = ui->tableWidget->item(currentRow,4)->text().toUInt();
        sInfo.year = ui->tableWidget->item(currentRow,5)->text().toUInt();
        sInfo.studentID = ui->tableWidget->item(currentRow,6)->text().toUInt();
        sInfo.phoneNum = ui->tableWidget->item(currentRow,7)->text();
        sInfo.major = ui->tableWidget->item(currentRow,8)->text();
        dlgAddStudent->setType(false,sInfo);
        dlgAddStudent->exec();
        updateTable();
    }

}

void MainWindow::on_pushButton_search_clicked()
{
    QString searchInput = ui->lineEdit_search->text();

    // Get the list of students that match the search criteria.
    QList<StudentInfo> studentList = ptrStuSql->searchStudentsByName(searchInput);
    if(studentList.count()==0){
        QMessageBox::information(nullptr, "Warning", "There are not student with the name:"+searchInput);
    }else{
        ui->tableWidget->setRowCount(studentList.count());
        for(int i = 0; i < studentList.count(); i++){
            ui->tableWidget->setItem(i,0,new QTableWidgetItem(QString::number(i+1)));
            ui->tableWidget->setItem(i,1,new QTableWidgetItem(QString::number(studentList[i].id)));
            ui->tableWidget->setItem(i,2,new QTableWidgetItem(studentList[i].name));
            ui->tableWidget->setItem(i,3,new QTableWidgetItem(QString::number(studentList[i].age)));
            ui->tableWidget->setItem(i,4,new QTableWidgetItem(QString::number(studentList[i].grade)));
            ui->tableWidget->setItem(i,5,new QTableWidgetItem(QString::number(studentList[i].year)));
            ui->tableWidget->setItem(i,6,new QTableWidgetItem(QString::number(studentList[i].studentID)));
            ui->tableWidget->setItem(i,7,new QTableWidgetItem(studentList[i].phoneNum));
            ui->tableWidget->setItem(i,8,new QTableWidgetItem(studentList[i].major));
        }
        ui->label_studentCount->setText(QString("Total Student:%1").arg(studentList.count()));
    }

}


void MainWindow::on_pushButton_clear_clicked()
{
    ptrStuSql->clearStudentTable();
    updateTable();
}

