![image](https://i.ibb.co/MjNLPyQ/Banner-Dapper-Context.png)

> [!NOTE]
> 
> Documentation is still under development

# DapperContext

A simple collections of functions and method for CRUD operation in Dapper for generic item with or without an integrated audit system.

---

## Preface

This utility allows you to speed up CRUD operations with Dapper by writing less code.
The 'utility already integrates the connection system to various database providers such as:

- SQL Server
- MySQL / Maria DB
- Oracle
- SQLite

This utility arose from the need to avoid instantiating a connection context each time and perform all CRUD operations without having to write the database access each time; it should better facilitate operations with Dapper.

I also created another project derived from this one, which is called DapperAuditContext.
This project allows you to add attributes to model classes that allows you to track changes made to that specific table or specific table field.
The result can be saved automatically to a table within your own database, and the operation is automatic, or saved to a readable text file.

## Information about packages

To install the program simply download the nuget package(s) related to your needs.

There are several nuget packages available that you can install.
The packages are these:

- DapperContext
  
  - This is the core package that is mandatory for other and **will not work alone**

- DapperAuditContext
  
  - This is the core audit package that is mandatory only if you need to trail changes and **will not work alone**

- DapperContext.SqlServer ***(recommended package)***
  
  - This package allows you to connect to a Microsoft SQL Server and is dependent on the core package.
    With this package you can use all the functions available to you

- DapperAuditContext.SqlServer ***(recommended package)***
  
  - Same that DapperContext.SqlServer but with a integated audit system

## Installation

Install main core package via NuGet 

```powershell
PM > Install-Package BertoSoftware.DapperContext
```

Install sql server package via NuGet

```powershell
PM > Install-Package BertoSoftware.DapperContext.SqlServer
```

## Examples

Create a model class that respect the same on your database

### VB.NET

```vbnet
Imports Dapper.Contrib.Extensions

Namespace Model

    <Table("Person")> 
    Public Class Person
        <Key>
        Public Property ID As Integer
        Public Property Name As String
        Public Property Surname As String
        Public Property Address As String
    End Class

End Namespace
```

### C#

```cs
using Dapper.Contrib.Extensions;

namespace Model
{

    [Table("Person")]
    public partial class Person
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Address { get; set; }
    }
}
```
