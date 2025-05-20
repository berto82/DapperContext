![ ](https://i.imgur.com/uDCuPh1.png)

> NOTE: Documentation is still under development

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

- **DapperContext**
  
  - This is the core package that is mandatory for other and **will not work alone**

- **DapperAuditContext**
  
  - This is the core audit package that is mandatory only if you need to trail changes and **will not work alone**

- **DapperContext.SqlServer**
  
  - This package allows you to connect to a Microsoft SQL Server and is dependent on the core package.
    With this package you can use all the functions available to you

- **DapperAuditContext.SqlServer** 
  
  - Same that DapperContext.SqlServer but with a integated audit system

- **DapperContext.MySql**
  
  - This package allows you to connect to a MySQL / MariaDB and is dependent on the core package.
    With this package you can use all the functions available to you

- **DapperAuditContext.MySql**
  
  - Same that DapperContext.MySql but with a integated audit system

## Getting started

### Providers

#### SQL Server

Install sql server package via NuGet

```powershell
PM > Install-Package BertoSoftware.DapperContext.SqlServer
```

#### MySQL

```powershell
PM > Install-Package BertoSoftware.DapperContext.MySql
```

This package will install also all dependecies regard main core package and specified database connection

### Configuration

In the main program you can define a settings globally for the project

#### VB.NET

```vbnet
   DapperContext.Settings = ContextConfiguration.CreateNew.
         UseSettingsFileMode(SettingFileMode.NET4x).
         WithConnectionName("MyConnection").
         WithCustomConfigurationFile("app1.config").
         DisableTransaction.
         Build()
```

#### C#

```csharp
   DapperContext.Settings = ContextConfiguration.CreateNew.
         UseSettingsFileMode(SettingFileMode.NET4x).
         WithConnectionName("MyConnection").
         WithCustomConfigurationFile("app1.config").
         DisableTransaction.
         Build();
```

You can configure different configuration parameters such as:

```vbnet
.UseSettingsFileMode(SettingsFileMode.Net4x)
```

With `.UseSettingsFileMode` you can select which configuration file should be loaded to look for the connection string.

- `SettingFileMode.NET4x` load a file *"app.config" *and retrive appropriate connection string.

- `SettingFileMode.NETCore` load a file *"appsettings.json"* and retrive a connection string

With `.WithConnectionName("MyConnection")` will search a name of connection string you provide into parameter, if this settings is omitted will search a connection string with name *"DefaultConnection"*

With `.WithCustomConfigurationFile("app1.config")` will search a file settings you provide into parameter, if this settings is omitted will search a default file like *"app.config"* or *"appsettings.json"*

With `.DisableTransaction` will disable automatic SQL transaction

You can terminate settings with `.Build()` method

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

### Console

#### VB.NET

```vbnet
Imports BertoSoftware

Module Program
  Sub Main(args As String())
 
     DapperContext.Settings = ContextConfiguration.CreateNew().UseSettingsFileMode(SettingFileMode.NetCore).Build()
 
    'Create a record
    Using ctx As New DapperContextSqlServer

        Dim person As New Model.Person With {
            .Name = "John",
            .Surname = "Doe"
        }

        ctx.InsertOrUpdate(person)

    End Using

    'Get a single record
    Using ctx As New DapperContextSqlServer

        Dim person As Model.Person = ctx.Get(Of Model.Person)(1)

        Console.WriteLine(String.Join(" | ", {person.ID, person.Name, person.Surname}))

    End Using

    'Get all record
    Using ctx As New DapperContextSqlServer

        Dim lstPerson As List(Of Model.Person) = ctx.GetAll(Of Model.Person).ToList

        lstPerson.ForEach(Sub(x) Console.WriteLine(String.Join(" | ", {x.ID, x.Name, x.Surname})))

    End Using

    'Update a record
    Using ctx As New DapperContextSqlServer

        Dim person As Model.Person = ctx.Get(Of Model.Person)(1)

        person.Surname = "Butt"

        ctx.InsertOrUpdate(person)

        Console.WriteLine(String.Join(" | ", {person.ID, person.Name, person.Surname}))

    End Using

    'Delete a record
    Using ctx As New DapperContextSqlServer

        Dim person As Model.Person = ctx.Get(Of Model.Person)(1)

        ctx.Delete(person)
    End Using

    'Delete all record
    Using ctx As New DapperContextSqlServer
        ctx.DeleteAll(Of Model.Person)()
    End Using
 
 End Sub
End Module
```

#### C#

```csharp
using BertoSoftware;
using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        DapperContext.Settings = ContextConfiguration.CreateNew().UseSettingsFileMode(SettingFileMode.NetCore).Build();

        // Create a record
        using (var ctx = new DapperContextSqlServer())
        {
            var person = new Model.Person
            {
                Name = "John",
                Surname = "Doe"
            };

            ctx.InsertOrUpdate(person);
        }

        // Get a single record
        using (var ctx = new DapperContextSqlServer())
        {
            var person = ctx.Get<Model.Person>(1);
            Console.WriteLine(string.Join(" | ", new[] { person.ID, person.Name, person.Surname }));
        }

        // Get all records
        using (var ctx = new DapperContextSqlServer())
        {
            var lstPerson = ctx.GetAll<Model.Person>().ToList();
            lstPerson.ForEach(x => Console.WriteLine(string.Join(" | ", new[] { x.ID, x.Name, x.Surname })));
        }

        // Update a record
        using (var ctx = new DapperContextSqlServer())
        {
            var person = ctx.Get<Model.Person>(1);
            person.Surname = "Butt";
            ctx.InsertOrUpdate(person);
            Console.WriteLine(string.Join(" | ", new[] { person.ID, person.Name, person.Surname }));
        }

        // Delete a record
        using (var ctx = new DapperContextSqlServer())
        {
            var person = ctx.Get<Model.Person>(1);
            ctx.Delete(person);
        }

        // Delete all records
        using (var ctx = new DapperContextSqlServer())
        {
            ctx.DeleteAll<Model.Person>();
        }
    }
}


```

This example is provided with `DapperContext.SQLServer` installed package but you can change the class `DapperContextSqlServer` with your appropriate.

The classes avaiabile are these:

| Package name                          | Classes                              |
|:------------------------------------- |:------------------------------------ |
| BertoSoftware.DapperContext.SqlServer | BertoSoftware.DapperContextSqlServer |
| BertoSoftware.DapperContext.MySql     | BertoSoftware.DapperContextMySql     |

---

# DapperAuditContext

> Documentation soon avaiable
