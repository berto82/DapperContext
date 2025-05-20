Imports System
Imports System.IO

Module Program
    Sub Main(args As String())

        Try
            DapperContext.Settings = ContextConfiguration.CreateNew.UseSettingsFileMode(SettingFileMode.NET4x).WithConnectionName("DocutechEntities").Build

            'SQL Server
            ExampleNotAuditing()
            ''      ExampleWithAuditing()

            'My SQL
            ' ExampleNotAudtingMySql()

            ExampleWithAuditingMySql()

        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try

    End Sub

    Sub ExampleNotAuditing()

        'Configure context setting or leave default
        '***Uncomment this line if you want to configure settings
        'DapperContext.Settings = ContextConfiguration.CreateNew.UseSettingsFileMode(SettingFileMode.NETCore).Build

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

    Sub ExampleWithAuditing()

        'Configure context audit setting or leave default
        '***Uncomment this line if you want to configure settings
        ' DapperAuditContext.AuditSettings = AuditConfiguration.CreateNew.StoreMode(AuditStoreMode.File).Build

        'Create a record
        Using ctx As New DapperAuditContextSqlServer

            Dim person As New Model.Person With {
                .Name = "John",
                .Surname = "Doe"
            }

            ctx.InsertOrUpdate(person)

        End Using

        'Get a single record
        Using ctx As New DapperAuditContextSqlServer

            Dim person As Model.Person = ctx.Get(Of Model.Person)(1)

            Console.WriteLine(String.Join(" | ", {person.ID, person.Name, person.Surname}))

        End Using

        'Get all record
        Using ctx As New DapperAuditContextSqlServer

            Dim lstPerson As List(Of Model.Person) = ctx.GetAll(Of Model.Person).ToList

            lstPerson.ForEach(Sub(x) Console.WriteLine(String.Join(" | ", {x.ID, x.Name, x.Surname})))

        End Using

        'Update a record
        Using ctx As New DapperAuditContextSqlServer

            Dim person As Model.Person = ctx.Get(Of Model.Person)(1)

            person.Surname = $"{person.Surname}_{Date.Now.Ticks}"

            ctx.InsertOrUpdate(person)

            Console.WriteLine(String.Join(" | ", {person.ID, person.Name, person.Surname}))

        End Using

        'Delete a record
        Using ctx As New DapperAuditContextSqlServer

            Dim person As Model.Person = ctx.Get(Of Model.Person)(1)

            ctx.Delete(person)
        End Using

        'Delete all record
        Using ctx As New DapperAuditContextSqlServer
            ctx.DeleteAll(Of Model.Person)()
        End Using

        'Show audit record
        Dim logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AuditLogs")
        Dim logFile As String = Path.Combine(logDir, DapperAuditContext.AuditSettings.FilePath)
        Dim auditLog As String = IO.File.ReadAllText(logFile)
        Console.Write(auditLog)

    End Sub

    Sub ExampleNotAudtingMySql()

        'Configure context setting or leave default
        '***Uncomment this line if you want to configure settings
        'DapperContext.Settings = ContextConfiguration.CreateNew.UseSettingsFileMode(SettingFileMode.NETCore).Build

        DapperContext.Settings = ContextConfiguration.CreateNew.UseSettingsFileMode(SettingFileMode.NETCore).WithConnectionName("MySqlConnection").Build

        'Create a record
        Using ctx As New DapperContextMySql

            Dim person As New Model.Person With {
                .Name = "John",
                .Surname = "Doe"
            }

            ctx.InsertOrUpdate(person)

        End Using

        'Get a single record
        Using ctx As New DapperContextMySql

            Dim person As Model.Person = ctx.Get(Of Model.Person)(1)

            Console.WriteLine(String.Join(" | ", {person.ID, person.Name, person.Surname}))

        End Using

        'Get all record
        Using ctx As New DapperContextMySql

            Dim lstPerson As List(Of Model.Person) = ctx.GetAll(Of Model.Person).ToList

            lstPerson.ForEach(Sub(x) Console.WriteLine(String.Join(" | ", {x.ID, x.Name, x.Surname})))

        End Using

        'Update a record
        Using ctx As New DapperContextMySql

            Dim person As Model.Person = ctx.Get(Of Model.Person)(1)

            person.Surname = "Butt"

            ctx.InsertOrUpdate(person)

            Console.WriteLine(String.Join(" | ", {person.ID, person.Name, person.Surname}))

        End Using

        'Delete a record
        Using ctx As New DapperContextMySql

            Dim person As Model.Person = ctx.Get(Of Model.Person)(1)

            ctx.Delete(person)
        End Using

        'Delete all record
        Using ctx As New DapperContextMySql
            ctx.DeleteAll(Of Model.Person)()
        End Using

    End Sub

    Sub ExampleWithAuditingMySql()

        'Configure context setting or leave default
        '***Uncomment this line if you want to configure settings
        'DapperContext.Settings = ContextConfiguration.CreateNew.UseSettingsFileMode(SettingFileMode.NETCore).Build

        DapperContext.Settings = ContextConfiguration.CreateNew.UseSettingsFileMode(SettingFileMode.NETCore).WithConnectionName("MySqlConnection").Build
        DapperAuditContext.AuditSettings = AuditConfiguration.CreateNew.StoreMode(AuditStoreMode.Database).Build

        'Create a record
        Using ctx As New DapperAuditContextMySql

            Dim person As New Model.Person With {
                .Name = "John",
                .Surname = "Doe"
            }

            ctx.InsertOrUpdate(person)

        End Using

        'Get a single record
        Using ctx As New DapperAuditContextMySql

            Dim person As Model.Person = ctx.Get(Of Model.Person)(1)

            Console.WriteLine(String.Join(" | ", {person.ID, person.Name, person.Surname}))

        End Using

        'Get all record
        Using ctx As New DapperAuditContextMySql

            Dim lstPerson As List(Of Model.Person) = ctx.GetAll(Of Model.Person).ToList

            lstPerson.ForEach(Sub(x) Console.WriteLine(String.Join(" | ", {x.ID, x.Name, x.Surname})))

        End Using

        'Update a record
        Using ctx As New DapperAuditContextMySql

            Dim person As Model.Person = ctx.Get(Of Model.Person)(1)

            person.Surname = "Butt"

            ctx.InsertOrUpdate(person)

            Console.WriteLine(String.Join(" | ", {person.ID, person.Name, person.Surname}))

        End Using

        'Delete a record
        Using ctx As New DapperAuditContextMySql

            Dim person As Model.Person = ctx.Get(Of Model.Person)(1)

            ctx.Delete(person)
        End Using

        'Delete all record
        Using ctx As New DapperAuditContextMySql
            ctx.DeleteAll(Of Model.Person)()
        End Using

    End Sub


End Module
