Imports System.IO
Imports BertoSoftware.Context.Configuration
Imports BertoSoftware.Context.Tools
Imports BertoSoftware.Context.Tools.Audit

Module Program

    Dim ctx As DapperContext

    Enum ConnectionType
        SQLServer
        MySQL
        SQLite
        PostgreSQL
        Firebird
    End Enum


    Sub Main(args As String())

        Try
            'Enable or disable audit
            Dim enableAudit As Boolean = True

            'Configure context setting or leave default
            '***Uncomment this line if you want to configure settings
            'DapperContext.Settings = ContextConfiguration.CreateNew.UseSettingsFileMode(SettingFileMode.NETCore).Build

            'Configure audit setting or leave default
            '***Uncomment this line if you want to configure settings
            DapperAuditContext.AuditSettings = AuditConfiguration.CreateNew.StoreMode(AuditStoreMode.Database).Build

            'SQL Server
            Connect(ConnectionType.SQLServer, enableAudit)

            'MySQL
            Connect(ConnectionType.MySQL, enableAudit)

            'SQLite
            Connect(ConnectionType.SQLite, enableAudit)

            'PostgreSQL
            Connect(ConnectionType.PostgreSQL, enableAudit)

            'Firebird
            Connect(ConnectionType.Firebird, enableAudit)

            If enableAudit = True Then
                If DapperAuditContext.AuditSettings.StoreLogMode <> AuditStoreMode.Database Then
                    ReadLogFile()
                End If
            End If

        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try

    End Sub

    Private Sub Connect(connectionType As ConnectionType, enableAudit As Boolean)

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

    Public Sub ConnectToSQLServer(enableAudit As Boolean)

        DapperContext.Settings = ContextConfiguration.CreateNew.UseSettingsFileMode(SettingFileMode.NETCore).WithConnectionName("DefaultConnection").Build

        If enableAudit = False Then
            ctx = New DapperContextSqlServer
        Else
            ctx = New DapperAuditContextSqlServer
        End If

    End Sub

    Public Sub ConnectToMySQL(enableAudit As Boolean)

        DapperContext.Settings = ContextConfiguration.CreateNew.UseSettingsFileMode(SettingFileMode.NETCore).WithConnectionName("MySqlConnection").Build

        If enableAudit = False Then
            ctx = New DapperContextMySql
        Else
            ctx = New DapperAuditContextMySql
        End If
    End Sub

    Public Sub ConnectToSQLite(enableAudit As Boolean)

        DapperContext.Settings = ContextConfiguration.CreateNew.UseSettingsFileMode(SettingFileMode.NETCore).WithConnectionName("SQLiteConnection").Build

        If enableAudit = False Then
            ctx = New DapperContextSQLite
        Else
            ctx = New DapperAuditContextSQLite
        End If

    End Sub

    Public Sub ConnectToPostgreSQL(enableAudit As Boolean)

        DapperContext.Settings = ContextConfiguration.CreateNew.UseSettingsFileMode(SettingFileMode.NETCore).WithConnectionName("PostgreSQLConnection").Build

        If enableAudit = False Then
            ctx = New DapperContextPostgreSQL
        Else
            ctx = New DapperAuditContextPostgreSQL
        End If
    End Sub

    Public Sub ConnectToFirebird(enableAudit As Boolean)

        DapperContext.Settings = ContextConfiguration.CreateNew.UseSettingsFileMode(SettingFileMode.NETCore).WithConnectionName("FirebirdConnection").Build

        If enableAudit = False Then
            ctx = New DapperContextFirebird
        Else
            ctx = New DapperAuditContextFirebird
        End If
    End Sub

    Private Sub ExecuteCRUDOperation()

        Dim inserterPersonID As Long = InsertPersonRecord()

        Dim person As Model.Person = GetRecordByID(inserterPersonID)

        Console.WriteLine(String.Join(" | ", {person.ID, person.Name, person.Surname}))

        Dim lstPerson As List(Of Model.Person) = GetAllRecords()

        lstPerson.ForEach(Sub(x) Console.WriteLine(String.Join(" | ", {x.ID, x.Name, x.Surname})))

        UpdateRecord(person)

        Console.WriteLine(String.Join(" | ", {person.ID, person.Name, person.Surname}))

        DeleteRecord(person)

        DeleteAllRecords()

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

        Return CLng(ctx.InsertOrUpdate(location))

    End Function

    Private Function InsertPersonRecord() As Long
        'Create a record
        Dim person As New Model.Person With {
                .Name = "John",
                .Surname = "Doe"
            }
        Return CLng(ctx.InsertOrUpdate(person))

    End Function

    Public Function UpdateRecord(person As Model.Person) As Boolean

        'Update a record
        person.Surname = "Butt"

        Return CBool(ctx.InsertOrUpdate(person))

    End Function

    Public Function DeleteRecord(person As Model.Person) As Boolean
        'Delete a record
        Return ctx.Delete(person)
    End Function

    Public Function DeleteAllRecords() As Boolean
        'Delete all record
        Return ctx.DeleteAll(Of Model.Person)()
    End Function

    Public Sub ReadLogFile()
        'Show audit record
        Dim logFile As String = Path.Combine(DapperAuditContext.AuditSettings.Path, DapperAuditContext.AuditSettings.FileName)
        Dim auditLog As String = IO.File.ReadAllText(logFile)
        Console.Write(auditLog)

    End Sub


End Module
