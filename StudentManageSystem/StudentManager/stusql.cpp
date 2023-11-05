#include "stusql.h"
#include <iostream>
#include <QSqlDatabase>
#include <QMessageBox>
#include <QtDebug>
#include <QSqlQuery>
#include <QCoreApplication>
#include <QSqlError>

stuSql * stuSql::ptrStuSql = nullptr;

// Constructor: Initializes the database connection
stuSql *stuSql::getInstance()
{
    if(nullptr == ptrStuSql){
        ptrStuSql = new stuSql;
    }
    return ptrStuSql;
}

stuSql::stuSql(QObject *parent): QObject{parent}
{
    init();
}

// Initialize database connection
void stuSql::init()
{
    // Check for available database drivers
    if(QSqlDatabase::drivers().isEmpty()){
        qDebug() << "No database driver found";
        return; // Early exit if no drivers
    }

    // Set up the SQLite database connection
    database = QSqlDatabase::addDatabase("QSQLITE");

    // Define the database name or the path
    QString dbName = "D:\\Portfolio\\Portfolio\\StudentManageSystem\\SQLite\\data.db";
    database.setDatabaseName(dbName);

    // Attempt to open the database and log any errors
    if(!database.open()){
        qDebug() << "Unable to open database";
        return; // Early exit on error
    }
}

// Get the total count of students from the database
quint32 stuSql::getStudentCount()
{
    // Create an SQL query object for the given database connection
    QSqlQuery sql(database);

    // Define the SQL select statement to get the count of students
    QString strSql = "select count('SystemID') from student";

    // Execute the SQL statement
    sql.exec(strSql);

    // Initialize the total student count to zero
    quint32 totalStudent = 0;

    // If there's a result from the query, fetch the count
    if(sql.next()){
        totalStudent = sql.value(0).toUInt();
    }

    // Return the total student count
    return totalStudent;
}

// Get students based on page and count (for pagination)
QList<StudentInfo> stuSql::getPageStudent(quint32 page, quint32 count)
{
    // Create an SQL query object for the given database connection
    QSqlQuery sql(database);

    // Construct the SQL select statement with placeholders for pagination
    // This will fetch students from the database, ordered by SystemID
    QString strsql = "select * from student order by SystemID limit ? offset ?";

    // Prepare the SQL statement for execution
    sql.prepare(strsql);

    // Bind values for LIMIT and OFFSET to the placeholders
    // This ensures the right amount of students are fetched, based on the page number
    sql.addBindValue(count);
    sql.addBindValue(page * count);

    // Execute the prepared SQL statement
    if (!sql.exec()) {
        // Handle the error, possibly throw an exception or log it
        qDebug() << "Error fetching students:" << sql.lastError().text();
        return QList<StudentInfo>();
    }

    // List to hold the fetched student information
    QList<StudentInfo> studentList;

    // Iterate over the returned rows from the query
    while (sql.next()) {
        // Populate the student info from the current row
        StudentInfo info;
        info.id = sql.value(0).toUInt();
        info.name = sql.value(1).toString();
        info.age = sql.value(2).toUInt();
        info.grade = sql.value(3).toUInt();
        info.year = sql.value(4).toUInt();
        info.studentID = sql.value(5).toUInt();
        info.phoneNum = sql.value(6).toString();
        info.major = sql.value(7).toString();

        // Append the student info to the list
        studentList.append(info);
    }

    // Return the populated list of students
    return studentList;
}

// Add student to the database
bool stuSql::addStudent(StudentInfo sInfo)
{
    // Create an SQL query object for the given database connection
    QSqlQuery sql(database);

    // Construct the SQL insert statement using placeholders
    // This prepares an SQL statement to insert a new student record with
    // provided information while setting the SystemID to be auto-incremented (null)
    QString strSql = "insert into student values(null, ?, ?, ?, ?, ?, ?, ?)";
    sql.prepare(strSql);

    // Bind values to the placeholders
    sql.addBindValue(sInfo.name);
    sql.addBindValue(sInfo.age);
    sql.addBindValue(sInfo.grade);
    sql.addBindValue(sInfo.year);
    sql.addBindValue(sInfo.studentID);
    sql.addBindValue(sInfo.phoneNum);
    sql.addBindValue(sInfo.major);

    // Execute the prepared SQL statement with the bound values
    // If the insertion succeeds, this method will return true; otherwise, it will return false
    if (!sql.exec()) {
        qDebug() << "Error adding student:" << sql.lastError().text();
        return false;
    }

    return true;
}

// Delete a student using SystemID
bool stuSql::deleteStudent(int systemID)
{
    // Create an SQL query object for the given database connection
    QSqlQuery sql(database);

    // Construct the SQL delete statement using a placeholder
    // This prepares an SQL statement to delete a student record using the SystemID
    QString strSql = "delete from student where SystemID = ?";
    sql.prepare(strSql);

    // Bind the SystemID value to the placeholder
    sql.addBindValue(systemID);

    // Execute the prepared SQL statement with the bound value
    // If the deletion succeeds, this method will return true; otherwise, it will return false
    if (!sql.exec()) {
        qDebug() << "Error deleting student:" << sql.lastError().text();
        return false;
    }

    return true;
}

// Clear all students from the table
bool stuSql::clearStudentTable()
{
    // Create an SQL query object for the given database connection
    QSqlQuery sql(database);

    // Construct the SQL delete statement
    // This prepares an SQL statement to delete all student records from the table
    QString strSql = "delete from student";
    sql.prepare(strSql);

    // Execute the prepared SQL statement
    // If the deletion succeeds, this method will return true; otherwise, it will return false
    if (!sql.exec()) {
        qDebug() << "Error clearing student table:" << sql.lastError().text();
        return false;
    }


    //Construct the SQL reset statement
    //This prepares an SQL stateemtn to set the system ID from the table
    strSql ="UPDATE SQLITE_SEQUENCE SET SEQ=0 WHERE NAME='student';";
    sql.prepare(strSql);

    // Execute the prepared SQL statement
    // If the deletion succeeds, this method will return true; otherwise, it will return false
    if (!sql.exec()) {
        qDebug() << "Error resetting student table:" << sql.lastError().text();
        return false;
    }

    return true;
}

// Update student information in the database
bool stuSql::UpdateStudentInfo(StudentInfo sInfo)
{
    // Create an SQL query object for the given database connection
    QSqlQuery sql(database);

    // Construct the SQL update statement with placeholders (?)
    // This prepares an SQL statement to update student records in the table
    // Using placeholders prevents SQL injection attacks by avoiding direct string concatenation
    QString strSql = "update student set Name = ?, Age = ?, Grade = ?, Class = ?, StudentID = ?, "
                     "PhoneNumber = ?, Major = ? where SystemID = ?";

    // Prepare the SQL statement for execution
    sql.prepare(strSql);

    // Bind the values from the StudentInfo object to the placeholders
    // The order of binding matches the order of placeholders in the SQL string
    sql.addBindValue(sInfo.name);
    sql.addBindValue(sInfo.age);
    sql.addBindValue(sInfo.grade);
    sql.addBindValue(sInfo.year);
    sql.addBindValue(sInfo.studentID);
    sql.addBindValue(sInfo.phoneNum);
    sql.addBindValue(sInfo.major);
    sql.addBindValue(sInfo.id);

    // Execute the prepared SQL statement with the bound values
    // If the update succeeds, this method will return true; otherwise, it will return false
    if (!sql.exec()) {
        qDebug() << "Error updating student info:" << sql.lastError().text();
        return false;
    }

    return true;
}

// Fetch all users from the database
QList<UserInfo> stuSql::getAllUser()
{
    // Initialize the list to store user information
    QList<UserInfo> list;

    // Create an SQL query object for the given database connection
    QSqlQuery sql(database);

    // Execute the SQL statement to fetch all users from the user table
    if (!sql.exec("select * from user")) {
        qDebug() << "Error fetching users:" << sql.lastError().text();
        return list;
    }

    // Iterate over the result set
    while (sql.next()) {
        // Create a new UserInfo object for each row
        UserInfo info;

        // Assuming specific column order, adjust these if needed
        info.username = sql.value(0).toString();  // Assuming username is the first column
        info.password = sql.value(1).toString();  // Assuming password is the second column
        info.aut = sql.value(2).toString();       // Assuming aut is the third column

        // Add the populated UserInfo object to the list
        list.push_back(info);
    }

    return list;
}

// Check if a user exists in the database using the username
bool stuSql::isExist(QString username)
{
    // Create an SQL query object for the given database connection
    QSqlQuery sql(database);

    // Define the SQL SELECT statement with a placeholder (?)
    // Using placeholders prevents SQL injection attacks
    QString strSql = "SELECT * FROM user WHERE Username = ?";

    // Prepare the SQL statement for execution
    sql.prepare(strSql);

    // Bind the username value to the placeholder
    sql.addBindValue(username);

    // Execute the prepared SQL statement with the bound value
    if (!sql.exec()) {
        qDebug() << "Query execution error:" << sql.lastError().text();
        return false;
    }

    return sql.next();
}

QList<StudentInfo> stuSql::searchStudentsByName(const QString& name) {
    QList<StudentInfo> students;

    QSqlQuery sql;
    sql.prepare("SELECT * FROM student WHERE LOWER(name) LIKE LOWER(:name)");
    sql.bindValue(":name", "%" + name.toLower() + "%");

    if (sql.exec()) {
        while (sql.next()) {
            StudentInfo info;
            info.id = sql.value(0).toUInt();
            info.name = sql.value(1).toString();
            info.age = sql.value(2).toUInt();
            info.grade = sql.value(3).toUInt();
            info.year = sql.value(4).toUInt();
            info.studentID = sql.value(5).toUInt();
            info.phoneNum = sql.value(6).toString();
            info.major = sql.value(7).toString();
            students.append(info);
        }
    } else {
        qDebug() << "Search error:" << sql.lastError().text();
    }

    return students;
}

// Update user details in the database
bool stuSql::updateUser(UserInfo uInfo)
{
    // Create an SQL query object for the given database connection
    QSqlQuery sql(database);

    // Define the SQL update statement with placeholders (?)
    // Using placeholders prevents SQL injection attacks
    QString strSql = "update user set Password = ?, Auth = ? where Username = ?";

    // Prepare the SQL statement for execution
    sql.prepare(strSql);

    // Bind the values from the uInfo object to the placeholders
    sql.addBindValue(uInfo.password);
    sql.addBindValue(uInfo.aut);
    sql.addBindValue(uInfo.username);

    // Execute the prepared SQL statement with the bound values
    if (!sql.exec()) {
        qDebug() << "Update failed:" << sql.lastError().text();
        return false;
    }

    return true;
}

// Add a new user to the database
bool stuSql::addUser(UserInfo uInfo)
{
    // Create an SQL query object for the given database connection
    QSqlQuery sql(database);

    // Define the SQL insert statement with placeholders (?)
    // Using placeholders prevents SQL injection attacks
    QString strSql = "insert into user values(?, ?, ?)";

    // Prepare the SQL statement for execution
    sql.prepare(strSql);

    // Bind the values from the uInfo object to the placeholders
    sql.addBindValue(uInfo.username);
    sql.addBindValue(uInfo.password);
    sql.addBindValue(uInfo.aut);

    // Execute the prepared SQL statement with the bound values
    return sql.exec();
}

// Delete user from the database using the username
bool stuSql::deleteUser(QString username)
{
    // Create an SQL query object for the given database connection
    QSqlQuery sql(database);

    // Define the SQL delete statement with a named placeholder
    // Using placeholders prevents SQL injection attacks
    QString strSql = "delete from user where Username = :username";

    // Prepare the SQL statement for execution
    sql.prepare(strSql);

    // Bind the username value to the named placeholder
    sql.bindValue(":username", username);

    // Execute the prepared SQL statement with the bound value
    return sql.exec();
}

bool stuSql::checkLoginInfo(QString username, QString password)
{
    QSqlQuery sql(database);
    sql.prepare("SELECT * FROM user WHERE Username = :username AND Password = :password");
    sql.bindValue(":username", username);
    sql.bindValue(":password", password);

    if(sql.exec()) {
        if(sql.next()) {
            // User with given username and password found.
            return true;
        }
    } else {
        qDebug() << "Login error:" << sql.lastError().text();
    }
    return false;

}
