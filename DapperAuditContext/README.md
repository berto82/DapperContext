# DapperAuditContext

A simple collections of functions and method for CRUD operation in Dapper for generic item with an integrated audit system.

## Version

### 2025-05-13
0.1.0 - Alpha version

## How to use
        
1. Add a reference to the DapperAuditContext project in your project.
2. Add a using statement to the DapperAuditContext namespace in your code file.
3. Create a new instance of the DapperAuditContext class, passing in your database connection string.
4. Use the methods provided by the DapperAuditContext class to perform CRUD operations on your database.
5. Dispose of the DapperAuditContext instance when you're done using it.
6. Optionally, you can use the DapperAuditContext class to perform other database operations, such as executing raw SQL queries or stored procedures.
7. You can use the Audit system to track changes to your data and generate audit logs.    

## Create a model
A simple data model

### VB.NET
```vb
Imports Dapper.Contrib.Extensions

Namespace Model

    <Table("Person")>
    <Audit>
    Public Class Person
        <Key>
        Public Property ID As Integer
        Public Property Name As String
        Public Property Surname As String
        <Audit(False)>
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

You can use Audit attribute on class or in single property to include or exclude trail, the default value is True, so is not necessary to specify a True value for all properties.

A simple use might look like this:

### VB.NET
```vb
Imports System

Module Program
    Sub Main(args As String())

        'Add new value into Person Table without audit trail
        Using ctx As New DapperContext

            Dim person As New Model.Person With {
                .Name = "John",
                .Surname = "Doe"
                .Address = "Street 23"
            }

            ctx.InsertOrUpdate(person)
        End Using

        'Add new value into Person Table with an automatic audit trail, just use DapperAuditContext instead of DapperContext
        Using ctx As New DapperAuditContext

            Dim person As New Model.Person With {
                .Name = "John",
                .Surname = "Doe"
                .Address = "Street 23"
            }

            ctx.InsertOrUpdate(person)

            'Changes a property after save
            person.Surname = "Black"

            'Saves the changes and adds a record in the control table with the difference between the previous save.
            ctx.InsertOrUpdate(person)

        End Using

    End Sub
End Module
```

### C#
```cs

internal static partial class Program
{
    public static void Main(string[] args)
    {

        // Add new value into Person Table without audit trail
        using (var ctx = new DapperContext())
        {

            var person = new Model.Person()
            {
                Name = "John",
                Surname = "Doe"
                Address = "Street 23"
            };

            ctx.InsertOrUpdate(person);
        }

        // Add new value into Person Table with an automatic audit trail, just use DapperAuditContext instead of DapperContext
        using (var ctx = new DapperAuditContext())
        {

            var person = new Model.Person()
            {
                Name = "John",
                Surname = "Doe"
                Address = "Street 23"
            };

            ctx.InsertOrUpdate(person);

            // Changes a property after save
            person.Surname = "Black";

            // Saves the changes and adds a record in the control table with the difference between the previous save.
            ctx.InsertOrUpdate(person);

        }

    }
}
```


## Dipendencies 

 - You have to install Dapper.Contrib and assign to a model the [Key] attribute on ID field

### Note

- The InsertOrUpdate method works only if the ‘Key’ attribute has been set to a field of type integer.
