#ifndef STUSQL_H
#define STUSQL_H

#include <QObject>
#include <QSqlDatabase>

struct StudentInfo{
    int id;
    QString name;
    quint8 age;
    quint16 grade;
    quint16 year;
    quint32 studentID;
    QString phoneNum;
    QString major;
};
struct UserInfo{
    QString username;
    QString password;
    QString aut;

};

class stuSql : public QObject
{
    Q_OBJECT
public:
    static stuSql *ptrStuSql;

    static stuSql *getInstance();

    QSqlDatabase database;

    QString _username;
    explicit stuSql(QObject *parent = nullptr);

    void init();

    //find the number of students
    quint32 getStudentCount();

    //search the student at page x for count number of student, start at page 0
    QList<StudentInfo> getPageStudent(quint32 page, quint32 count);

    //add student
    bool addStudent(StudentInfo sInfo);

    //remove student
    bool deleteStudent(int systemID);

    //clear the chart of student
    bool clearStudentTable();

    //modify student
    bool UpdateStudentInfo(StudentInfo sInfo);

    //search for student
    QList<StudentInfo> searchStudentsByName(const QString& name);

    //search all user
    QList<UserInfo> getAllUser();

    //search if user exist
    bool isExist(QString username);

    //modify user authorization
    bool updateUser(UserInfo uInfo);

    //add a user
    bool addUser(UserInfo uInfo);

    //remove a user
    bool deleteUser(QString username);

    bool checkLoginInfo(QString username, QString password);
};

#endif // STUSQL_H
