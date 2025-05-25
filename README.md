<img src="https://i.imgur.com/uDCuPh1.png" title="" alt=" " data-align="center">

<img title="" src="https://img.shields.io/github/license/berto82/DapperContext.svg" alt="GitHub License" data-align="inline"> <img title="" src="https://img.shields.io/nuget/vpre/BertoSoftware.DapperContext.svg" alt="NuGet Version" data-align="inline"> ![NuGet Downloads](https://img.shields.io/nuget/dt/BertoSoftware.DapperContext.svg) ![GitHub Release Date](https://img.shields.io/github/release-date-pre/berto82/DapperContext.svg) ![GitHub last commit (branch)](https://img.shields.io/github/last-commit/berto82/DapperContext/master.svg)

---

# Preface

This utility allows you to speed up CRUD operations with Dapper by writing less code.
The 'utility already integrates the connection system to various database providers such as:

- SQL Server
- MySQL / Maria DB
- PostgreSQL
- SQLite

This utility arose from the need to avoid instantiating a connection context each time and perform all CRUD operations without having to write the database access each time; it should better facilitate operations with Dapper.

# Information about packages

To install the program simply download the nuget package(s) related to your needs.

There are several nuget packages available that you can install.
The packages are these:

- **DapperContext**
  
  - This is the core package that is mandatory for other and **will not work alone**

- **DapperContext.SqlServer**
  
  - This package allows you to connect to a Microsoft SQL Server and is dependent on the core package.
    With this package you can use all the functions available to you

- **DapperContext.MySql**
  
  - This package allows you to connect to a MySQL / MariaDB and is dependent on the core package.
    With this package you can use all the functions available to you

- **DapperContext.PostgreSQL**
  
  - This package allows you to connect to a PostgreSQL and is dependent on the core package.
    With this package you can use all the functions available to you

- **DapperContext.SQLite**
  
  - This package allows you to connect to a SQLite and is dependent on the core package.
    With this package you can use all the functions available to you

# Getting started

## Database providers

### SQL Server

Install sql server package via NuGet

```powershell
PM > Install-Package BertoSoftware.DapperContext.SqlServer
```

### MySQL

```powershell
PM > Install-Package BertoSoftware.DapperContext.MySql
```

### SQLite

```powershell
PM > Install-Package BertoSoftware.DapperContext.SQLite
```

### PostgreSQL

```powershell
PM > Install-Package BertoSoftware.DapperContext.PostgreSQL
```

This packages will install also all dependecies regard main core package and specified database connection

## Configuration

In the main program you can define a settings globally for the project

### VB.NET

```vbnet
Imports BertoSoftware.Context.Configuration

DapperContext.Settings = ContextConfiguration.CreateNew.
         UseSettingsFileMode(SettingFileMode.NET4x).
         WithConnectionName("MyConnection").
         WithCustomConfigurationFile("app1.config").
         WithCustomConnectionString("MyConnectionString")   
         DisableTransaction.
         Build()
```

### C#

```csharp
using BertoSoftware.Context.Configuration;

DapperContext.Settings = ContextConfiguration.CreateNew().
         UseSettingsFileMode(SettingFileMode.NET4x).
         WithConnectionName("MyConnection").
         WithCustomConfigurationFile("app1.config").
         WithCustomConnectionString("MyConnectionString") 
         DisableTransaction().
         Build();
```

You can configure different configuration parameters such as:

```vbnet
.UseSettingsFileMode(SettingsFileMode.Net4x)
```

With `.UseSettingsFileMode` you can select which configuration file should be loaded to look for the connection string.

- `SettingFileMode.NET4x` load a file *"app.config"* and retrive appropriate connection string.

- `SettingFileMode.NETCore` load a file *"appsettings.json"* and retrive a connection string

With `.WithConnectionName("MyConnection")` will search a name of connection string you provide into parameter, if this settings is omitted will search a connection string with name *"DefaultConnection"*

With `.WithCustomConfigurationFile("app1.config")` will search a file settings you provide into parameter, if this settings is omitted will search a default file like *"app.config"* or *"appsettings.json"*

With `.WithCustomConnectionString("MyConnectionString")` will use a custom connection you provide into parameter and this will be a priority on other parameter

With `.DisableTransaction` will disable automatic SQL transaction

You can terminate settings with `.Build()` method

## Examples

Create a model class that respect the same on your database

### Database Model

#### VB.NET

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

#### C#

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

### Application

You can configure a global setting once time into your start module program, like a Console app, Windows Forms, ASP.NET etc.

Otherwhere a default settings will be loaded

In this example, will take a look what how do a connection with SQL Server:

On package manager you have to install `BertoSoftware.DapperContext.SqlServer` to connect a SQL Server instance.

You can use a local variable with a simple declaration or you can use a using statement for automatic dispose item.

#### VB.NET

```vbnet
Imports BertoSoftware.Context.Configuration

Module Program
  Sub Main(args As String())

    'Configure context setting or leave default
    '***Uncomment this line if you want to configure settings
    DapperContext.Settings = ContextConfiguration.CreateNew.UseSettingsFileMode(SettingFileMode.NETCore).Build

  End Sub
End Module
```

```vbnet
Imports BertoSoftware.Context.Tools

Dim ctx As New DapperContextSqlServer

'Your code

ctx.Dispose()


Using ctx As New DapperContextSqlServer
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
using BertoSoftware.Configuration;

class Program
{
    static void Main(string[] args)
    {
        //Configure context setting or leave default
        //***Uncomment this line if you want to configure settings
        DapperContext.Settings = ContextConfiguration.CreateNew().UseSettingsFileMode(SettingFileMode.NetCore).Build();

    }
}
```

```csharp
using BertoSoftware.Context.Tools

var ctx = new DapperContextSqlServer;

//your code

ctx.Dispose();

using (var ctx = new DapperContextSqlServer())
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

# Other packages

This example is provided with `DapperContext.SQLServer` installed package but you can change the class `DapperContextSqlServer` with your appropriate.

The classes avaiabile are these:

| Package name                           | Classes                               | NuGet                                                                                                                                                                                    |
|:-------------------------------------- |:------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| BertoSoftware.DapperContext.SqlServer  | BertoSoftware.DapperContextSqlServer  | ![NuGet Downloads](https://img.shields.io/nuget/dt/BertoSoftware.DapperContext.SqlServer.svg?link=https%3A%2F%2Fwww.nuget.org%2Fpackages%2FBertoSoftware.DapperAuditContext.SqlServer)   |
| BertoSoftware.DapperContext.MySql      | BertoSoftware.DapperContextMySql      | ![NuGet Downloads](https://img.shields.io/nuget/dt/BertoSoftware.DapperContext.MySql.svg?link=https%3A%2F%2Fwww.nuget.org%2Fpackages%2FBertoSoftware.DapperAuditContext.MySql)           |
| BertoSoftware.DapperContext.SQLite     | BertoSoftware.DapperContextSQLite     | ![NuGet Downloads](https://img.shields.io/nuget/dt/BertoSoftware.DapperContext.SQLite.svg?link=https%3A%2F%2Fwww.nuget.org%2Fpackages%2FBertoSoftware.DapperAuditContext.SQLite)         |
| BertoSoftware.DapperContext.PostgreSQL | BertoSoftware.DapperContextPostgreSQL | ![NuGet Downloads](https://img.shields.io/nuget/dt/BertoSoftware.DapperContext.PostgreSQL.svg?link=https%3A%2F%2Fwww.nuget.org%2Fpackages%2FBertoSoftware.DapperAuditContext.PostgreSQL) |

# Feedback

Please let me a feedback about your opinion, some issues or some missing feature to implement in future, I'll be happy to hear you.
