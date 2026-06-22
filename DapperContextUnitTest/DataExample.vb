Imports System.IO
Imports BertoSoftware.Context.Configuration
Imports BertoSoftware.Context.Tools
Imports BertoSoftware.Context.Tools.Audit

Public Class DataExample

    Dim ctx As DapperContext

    Enum ConnectionType
        SQLServer
        MySQL
        SQLite
        PostgreSQL
        Firebird
    End Enum

    Public Sub ConnectAndExecute(connectionType As ConnectionType, enableAudit As Boolean)

        Console.WriteLine($"Connecting to {connectionType.ToString()}")

        Select Case connectionType
            Case ConnectionType.SQLServer
                ConnectToSQLServer(enableAudit)
            Case ConnectionType.MySQL
                ConnectToMySQL(enableAudit)
            Case ConnectionType.PostgreSQL
                ConnectToPostgreSQL(enableAudit)
            Case ConnectionType.SQLite
                ConnectToSQLite(enableAudit)
            Case ConnectionType.Firebird
                ConnectToFirebird(enableAudit)
        End Select

        ExecuteCRUDOperation()

        Console.WriteLine("---")

    End Sub

    Private Sub ConnectToSQLServer(enableAudit As Boolean)

        DapperContext.Settings = ContextConfiguration.CreateNew.UseSettingsFileMode(SettingFileMode.NETCore).WithConnectionName("DefaultConnection").Build

        If enableAudit = False Then
            ctx = New DapperContextSqlServer
        Else
            ctx = New DapperAuditContextSqlServer
        End If

    End Sub

    Private Sub ConnectToMySQL(enableAudit As Boolean)

        DapperContext.Settings = ContextConfiguration.CreateNew.UseSettingsFileMode(SettingFileMode.NETCore).WithConnectionName("MySqlConnection").Build

        If enableAudit = False Then
            ctx = New DapperContextMySql
        Else
            ctx = New DapperAuditContextMySql
        End If
    End Sub

    Private Sub ConnectToSQLite(enableAudit As Boolean)

        DapperContext.Settings = ContextConfiguration.CreateNew.UseSettingsFileMode(SettingFileMode.NETCore).WithConnectionName("SQLiteConnection").Build

        If enableAudit = False Then
            ctx = New DapperContextSQLite
        Else
            ctx = New DapperAuditContextSQLite
        End If

    End Sub

    Private Sub ConnectToPostgreSQL(enableAudit As Boolean)

        DapperContext.Settings = ContextConfiguration.CreateNew.UseSettingsFileMode(SettingFileMode.NETCore).WithConnectionName("PostgreSQLConnection").Build

        If enableAudit = False Then
            ctx = New DapperContextPostgreSQL
        Else
            ctx = New DapperAuditContextPostgreSQL
        End If
    End Sub

    Private Sub ConnectToFirebird(enableAudit As Boolean)

        DapperContext.Settings = ContextConfiguration.CreateNew.UseSettingsFileMode(SettingFileMode.NETCore).WithConnectionName("FirebirdConnection").Build

        If enableAudit = False Then
            ctx = New DapperContextFirebird
        Else
            ctx = New DapperAuditContextFirebird
        End If
    End Sub


    Private Sub ExecuteCRUDOperation()

        Dim personID As Long = InsertPersonRecord()

        Dim person As Model.Person = GetRecordByID(personID)

        Console.WriteLine(String.Join(" | ", {person.ID, person.Name, person.Surname}))

        Dim lstPerson As List(Of Model.Person) = GetAllRecords()

        lstPerson.ForEach(Sub(x) Console.WriteLine(String.Join(" | ", {x.ID, x.Name, x.Surname})))

        UpdateRecord(person)

        Console.WriteLine(String.Join(" | ", {person.ID, person.Name, person.Surname}))

        DeleteRecord(person)

        DeleteAllRecords()

        ctx.Dispose()

    End Sub

    Private Function GetRecordByID(id As Object) As Model.Person
        'Get a single record
        Dim person As Model.Person = ctx.Get(Of Model.Person)(id)

        Return person

    End Function

    Private Function GetAllRecords() As List(Of Model.Person)
        'Get all record
        Dim lstPerson As List(Of Model.Person) = ctx.GetAll(Of Model.Person).ToList

        Return lstPerson

    End Function

    Private Function InsertLocationRecord() As Long
        'Create a record

        Dim location As New Model.Location With {
                .Name = "London"
            }

        Return ctx.InsertOrUpdate(location)

    End Function

    Private Function InsertPersonRecord() As Long
        'Create a record
        Dim person As New Model.Person With {
                .Name = "John",
                .Surname = "Doe"
            }
        Return ctx.InsertOrUpdate(person)

    End Function

    Private Function UpdateRecord(person As Model.Person) As Long

        'Update a record
        person.Surname = "Butt"

        Return ctx.InsertOrUpdate(person)

    End Function

    Private Function DeleteRecord(person As Model.Person) As Boolean
        'Delete a record
        Return ctx.Delete(person)
    End Function

    Private Function DeleteAllRecords() As Boolean
        'Delete all record
        Return ctx.DeleteAll(Of Model.Person)()
    End Function

    Public Sub ReadLogFile()
        'Show audit record
        Dim logFile As String = Path.Combine(DapperAuditContext.AuditSettings.Path, DapperAuditContext.AuditSettings.FileName)

        If IO.File.Exists(logFile) = False Then
            Console.WriteLine("Log file not found")
            Return
        End If

        Dim auditLog As String = IO.File.ReadAllText(logFile)
        Console.Write(auditLog)
    End Sub
End Class
