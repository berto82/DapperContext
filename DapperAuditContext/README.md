# Welcome to DapperContext!

A simple collections of functions and method for CRUD operation in Dapper for generic item with an integrated audit system.

## How to use
        
        1. Add a reference to the DapperAuditContext project in your project.
        2. Add a using statement to the DapperAuditContext namespace in your code file.
        3. Create a new instance of the DapperAuditContext class, passing in your database connection string.
        4. Use the methods provided by the DapperAuditContext class to perform CRUD operations on your database.
        5. Dispose of the DapperAuditContext instance when you're done using it.
        6. Optionally, you can use the DapperAuditContext class to perform other database operations, such as executing raw SQL queries or stored procedures.
        ## Audit system
        7. You can use the Audit system to track changes to your data and generate audit logs.
            

## Create a model
Create a model like this:

    'VB
    Import Dapper.Contrib.Extesions
    
    <Table("Person")>
    Public Class Person
    <Key>
    Public Property ID As Integer
    Public Property Name As String
    Public Property Surname As String
    
    //C#
    using Dapper.Contrib.Extensions;
    
    [Table("Person")]
    public class Person
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
    }
 

## Dipendencies 

 - You have to install Dapper.Contrib and assign to a model the [Key] attribute on ID field
