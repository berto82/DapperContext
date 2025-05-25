![k](https://imgur.com/0wKgRot.png)

![GithHub License](https://img.shields.io/github/license/berto82/DapperContext.svg) ![NuGet Version](https://img.shields.io/nuget/v/BertoSoftware.DapperAuditContext.svg) ![NuGet Downloads](https://img.shields.io/nuget/dt/BertoSoftware.DapperAuditContext.svg) ![GitHub Release Date](https://img.shields.io/github/release-date-pre/berto82/DapperContext.svg) ![GitHub last commit](https://img.shields.io/github/last-commit/berto82/DapperContext.svg)

---

# Preface

This utility allows you to trail changes on your database

You can track an entire table, or a single field on table you decide to trail

The result audit can be saved into an automatic table into your database that you track, or a text plain file, or a JSON file

You can implement audit system on these provider, just downloading the specific package

- SQL Server
- MySQL / Maria DB
- PostgreSQL
- SQLite

# Information about packages

- **DapperAuditContext**
  
  - This is the core audit package that is mandatory only if you need to trail changes and **will not work alone**

- **DapperAuditContext.SqlServer**
  
  - This package allows you to connect to a Microsoft SQL Server and is dependent on the core package.
    This package enable audit system and you can use all the functions available to you 
    
    This package depend on **DapperContext** core package

- **DapperAuditContext.MySql**
  
  - This package allows you to connect to a MySQL / MariaDB and is dependent on the core package.
    This package enable audit system and you can use all the functions available to you
    
    This package depend on **DapperContext** core package

- **DapperAuditContext.PostgreSQL**
  
  - This package allows you to connect to a PostgreSQL and is dependent on the core package.
    This package enable audit system and you can use all the functions available to you
    
    This package depend on **DapperContext** core package

- **DapperAuditContext.SQLite**
  
  - This package allows you to connect to a SQLIte and is dependent on the core package.
    This package enable audit system and you can use all the functions available to you
    
    This package depend on **DapperContext** core package

# Getting started

## Database providers

### SQL Server

Install sql server package via NuGet

```powershell
PM > Install-Package BertoSoftware.DapperAuditContext.SqlServer
```

### MySQL

```powershell
PM > Install-Package BertoSoftware.DapperAuditContext.MySql
```

### SQLite

```powershell
PM > Install-Package BertoSoftware.DapperAuditContext.SQLite
```

### PostgreSQL

```powershell
PM > Install-Package BertoSoftware.DapperAuditContext.PostgreSQL
```

This packages will install also all dependecies regard main core package and specified database connection

## Configuration

In the main program you can define a settings globally for the project

VB.NET

```vbnet
Imports BertoSoftware.Context.Configuration
Imports BertoSoftware.Context.Tools.Audit

DapperAuditContext.AuditSettings = AuditConfiguration.CreateNew.
         StoreMode(AuditStoreMode.Database).
         WithCustomTableName("MyAuditTable").
         WithCustomLogPath("MyPathFolder").
         WithCustomLogFileName("myAuditFile.txt").  
         Build()
```

### C#

```csharp
using BertoSoftware.Context.Configuration;
using BertoSoftware.Context.Tools.Audit

DapperAuditContext.AuditSettings = AuditConfiguration.CreateNew().
         StoreMode(AuditStoreMode.Database).
         WithCustomTableName("MyAuditTable").
         WithCustomLogPath("MyPathFolder").
         WithCustomLogFileName("myAuditFile.txt").  
         Build()
```

You can configure different configuration parameters such as:

```vbnet
.StoreMode(AuditStoreMode.Database)
```

With `.StoreMode` you can select where to save the generated audit trail.

- `AuditStoreMode.Database` on a table into database.
  
  `SettingFileMode.TextFile` into a plain text file.
  
  `SettingFileMode.JSON` into a JSON file that you can use this like a database.

With `.WithCustomTableName("MyAuditTable")` will save a table with name you provide into parameter, if this settings is omitted, the default table name is **AuditTable**

With `.WithCustomLogPath("MyPathFolder")` you can specify a custom path where you want to save logs, if this settings is omitted, the default folder is the same on your application and will create a new folder with name **AuditLogs**

With `.WithCustomLogFileName("myAuditFile.txt")` you can specify a custom log filename, if this settings is omitted, the default filename format is **$"audit_{Date.Now:yyyyMMdd}.log"**

You can terminate settings with `.Build()` method

> NOTE FOR WINDOWS FORMS / WPF USERS
> 
> If you decide to save a log into file, a folder of each user inside a log path, will created automatically. Es. (AuditLogs\John)
> 
> This is usefull where your application is started from a network folder and logs are separated for each users

## Examples

Create a model class that respect the same on your database

### Database Model

#### VB.NET

```vbnet
Imports BertoSoftware.Context.Configuration
Imports Dapper.Contrib.Extensions

Namespace Model

    <Table("Person")>
    <Audit>
    Public Class Person
        <Key>
        Public Property ID As Integer
        Public Property Name As String
        Public Property Surname As String
        <Audit(false)>
        Public Property Address As String
    End Class

End Namespace
```

#### C#

```cs
using BertoSoftware.Context.Configuration;
using Dapper.Contrib.Extensions;

namespace Model
{

    [Table("Person")]
    [Audit]
    public partial class Person
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        [Audit(false)]
        public string Address { get; set; }
    }

}
```

### Application

You can configure a global setting once time into your start module program, like a Console app, Windows Forms, ASP.NET etc.

Otherwhere a default settings will be loaded

In this example, will take a look what how do a connection with SQL Server:

On package manager you have to install `BertoSoftware.DapperAuditContext.SqlServer` to connect a SQL Server instance.

You can use a local variable with a simple declaration or you can use a using statement for automatic dispose item.

#### VB.NET

```vbnet
Imports BertoSoftware.Context.Configuration

Module Program
  Sub Main(args As String())

  'Configure audit setting or leave default
  '***Uncomment this line if you want to configure settings
  DapperAuditContext.AuditSettings = AuditConfiguration.CreateNew.StoreMode(AuditStoreMode.Database).Build

 End Sub
End Module
```

```vbnet
Imports BertoSoftware.Context.Tools

Dim ctx As New DapperAuditContextSqlServer

'Your code

ctx.Dispose()


Using ctx As New DapperAuditContextSqlServer
'Your code    
End Using
```

In following examples, declaration will be omitted

##### Insert a record

```vbnet
Private Function InsertRecord() As Long
    'Create a record
    Dim person As New Model.Person With {
              .Name = "John",
              .Surname = "Doe"
          }

    Return CLng(ctx.InsertOrUpdate(person))

End Function
```

##### Get a single record

```vbnet
Private Function GetRecordByID(id As Object) As Model.Person
    'Get a single record
    Dim person As Model.Person = ctx.Get(Of Model.Person)(id)

    Return person

End Function
```

##### Get all record

```vbnet
Private Function GetAllRecords() As List(Of Model.Person)
     'Get all record
     Dim lstPerson As List(Of Model.Person) = ctx.GetAll(Of Model.Person).ToList

     Return lstPerson

End Function
```

##### Update a record

```vbnet
Public Function UpdateRecord(person As Model.Person) As Boolean
    'Update a record
     person.Surname = "Butt"

     Return CBool(ctx.InsertOrUpdate(person))

End Function
```

##### Delete single record

```vbnet
Public Function DeleteRecord(person As Model.Person) As Boolean
    'Delete a record
     Return ctx.Delete(person)
End Function
```

##### Delete all record

```vbnet
Public Function DeleteAllRecords() As Boolean
    'Delete all record
     Return ctx.DeleteAll(Of Model.Person)()
End Function
```

#### C#

```csharp
using BertoSoftware.Context.Configuration;

internal static partial class Program
{
    public static void Main(string[] args)
    {

        // Configure audit setting or leave default
        // ***Uncomment this line if you want to configure settings
        DapperAuditContext.AuditSettings = AuditConfiguration.CreateNew.StoreMode(AuditStoreMode.Database).Build;

    }
}
```

```csharp
using BertoSoftware.Context.Tools

var ctx = new DapperAuditContextSqlServer;

//your code

ctx.Dispose();

using (var ctx = new DapperAuditContextSqlServer())
{
//your code    
}
```

In following examples, declaration will be omitted

##### Insert a record

```csharp
private long InsertRecord()
{
    // Create a record
    var person = new Model.Person()
    {
        Name = "John",
        Surname = "Doe"
    };

    return (long)ctx.InsertOrUpdate(person);

}
```

##### Get a single record

```csharp
private Model.Person GetRecordByID(object id)
{
    // Get a single record
    Model.Person person = ctx.Get<Model.Person>(id);

    return person;

}
```

##### Get all record

```csharp
private List<Model.Person> GetAllRecords()
{
    // Get all record
    List<Model.Person> lstPerson = ctx.GetAll<Model.Person>.ToList;

    return lstPerson;

}
```

##### Update a record

```csharp
public bool UpdateRecord(Model.Person person)
{
    // Update a record
    person.Surname = "Butt";

    return (bool)ctx.InsertOrUpdate(person);

}
```

##### Delete single record

```csharp
public bool DeleteRecord(Model.Person person)
{
    // Delete a record
    return ctx.Delete(person);
}
```

##### Delete all record

```csharp
public bool DeleteAllRecords()
{
    // Delete all record
    return ctx.DeleteAll<Model.Person>();
}
```

### Audit result

> **IMPORTANT**
> 
> Only a tracked entity with `<Audit>`attribute will taked to trail changes.
> 
> A object (class or fields) with `<Audit(false)>` will not taked to trail.   

#### Table

The created audit table contains follow information:

| Field         | Type          | Description                                                                               |
| ------------- | ------------- | ----------------------------------------------------------------------------------------- |
| ID            | int           | Primary key of audit table                                                                |
| Username      | nvarchar(255) | Name and domain of current logged user in format DOMAIN\USERNAME                          |
| KeyFieldID    | int           | Primary key of entity you trail                                                           |
| ActionType    | int (Enum)    | The type of action trailed.<br/><br/>**Create = 0**<br/>**Update = 1**<br/>**Delete = 2** |
| DateTimeStamp | datetime      | Timestamp of trailed entity                                                               |
| DataModel     | nvarchar(255) | Name of entity trailed                                                                    |
| Changes       | nvarchar(MAX) | JSON of fields changed                                                                    |
| ValueBefore   | nvarchar(MAX) | JSON of entity before changes                                                             |
| ValueAfter    | nvarchar(MAX) | JSON of entity after changes                                                              |

#### Text plain format

In a text plain format output can be in follow manner:

```
-----
Timestamp: 2025-05-24T23:47:24.5298578+02:00
User:      BERTO-PC\rfacc
Table:     Person
Action:    Update
Keys:      21
Old:       {"ID":21,"Name":"John","Surname":"Doe"}
New:       {"ID":21,"Name":"John","Surname":"Butt"}
Changes:    [{"FieldName":"Surname","ValueBefore":"Doe","ValueAfter":"Butt"}
```

Old, New and Changes line are always a JSON string format

#### JSON FORMAT

In a JSON format, output can be in follow manner:

```json
[
  {
        "Username": "BERTO-PC\\rfacc",
        "KeyFieldID": 20,
        "ActionType": 1,
        "DateTimeStamp": "2025-05-24T23:45:09.283554+02:00",
        "DataModel": "Person",
        "Changes": [
            {
                "FieldName": "Surname",
                "ValueBefore": "Doe",
                "ValueAfter": "Butt"
            }
        ],
        "ValueBefore": {
            "ID": 20,
            "Name": "John",
            "Surname": "Doe"
        },
        "ValueAfter": {
            "ID": 20,
            "Name": "John",
            "Surname": "Butt"
        }
    }
  ]
```

### Other packages

This example is provided with `DapperAuditContext.SQLServer` installed package but you can change the class `DapperAuditContextSqlServer` with your appropriate.

The classes avaiabile are these:

| Package name                                | Classes                                    | NuGet                                                                                                                                                                                         |
| ------------------------------------------- | ------------------------------------------ | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| BertoSoftware.DapperAuditContext.SqlServer  | BertoSoftware.DapperAuditContextSqlServer  | ![NuGet Downloads](https://img.shields.io/nuget/dt/BertoSoftware.DapperAuditContext.SqlServer.svg)                                                                                            |
| BertoSoftware.DapperAuditContext.MySql      | BertoSoftware.DapperAuditContextMySql      | ![NuGet Downloads](https://img.shields.io/nuget/dt/BertoSoftware.DapperAuditContext.MySql.svg?link=https%3A%2F%2Fwww.nuget.org%2Fpackages%2FBertoSoftware.DapperAuditContext.MySql)           |
| BertoSoftware.DapperAuditContext.SQLite     | BertoSoftware.DapperAuditContextSQLite     | ![NuGet Downloads](https://img.shields.io/nuget/dt/BertoSoftware.DapperAuditContext.SQLite.svg?link=https%3A%2F%2Fwww.nuget.org%2Fpackages%2FBertoSoftware.DapperAuditContext.SQLite)         |
| BertoSoftware.DapperAuditContext.PostgreSQL | BertoSoftware.DapperAuditContextPostgreSQL | ![NuGet Downloads](https://img.shields.io/nuget/dt/BertoSoftware.DapperAuditContext.PostgreSQL.svg?link=https%3A%2F%2Fwww.nuget.org%2Fpackages%2FBertoSoftware.DapperAuditContext.PostgreSQL) |

# Feedback

Please let me a feedback about your opinion, some issues or some missing feature to implement in future, I'll be happy to hear you.
