# DapperContext

A simple collections of functions and method for CRUD operation in Dapper for generic item.

## Version

### 2025-05-16
0.2.0 - Alpha version

- No changes

### 2025-05-13
0.1.0 - Alpha version

## How to use
        
1. Add a reference to the DapperContext project in your project.
2. Add a using statement to the DapperContext namespace in your code file.
3. Create a new instance of the DapperContext class, passing in your database connection string.
4. Use the methods provided by the DapperContext class to perform CRUD operations on your database.
5. Dispose of the DapperContext instance when you're done using it.
6. Optionally, you can use the DapperContext class to perform other database operations, such as executing raw SQL queries or stored procedures. 

## Create a model
A simple data model

### VB.NET
```vb
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

A simple use might look like this:

### VB.NET
```vb
Imports System

Module Program
    Sub Main(args As String())

        'Add new value into Person Table
        Using ctx As New DapperContext

            Dim person As New Model.Person With {
                .Name = "John",
                .Surname = "Doe"
                .Address = "Street 23"
            }

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

        // Add new value into Person Table
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

    }
}
```


## Dipendencies 

 - You have to install Dapper.Contrib and assign to a model the [Key] attribute on ID field

### Note

- The InsertOrUpdate method works only if the ‘Key’ attribute has been set to a field of type integer.
