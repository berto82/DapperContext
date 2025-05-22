Imports System.IO

Module Program

    Dim ctx As DapperContext

    Sub Main(args As String())

        Try
            'Enable or disable audit
            Dim enableAudit As Boolean = True

            'Configure context setting or leave default
            '***Uncomment this line if you want to configure settings
            'DapperContext.Settings = ContextConfiguration.CreateNew.UseSettingsFileMode(SettingFileMode.NETCore).Build

            'Configure audit setting or leave default
            '***Uncomment this line if you want to configure settings
            DapperAuditContext.AuditSettings = AuditConfiguration.CreateNew.StoreMode(AuditStoreMode.File).Build

            'SQL Server
            Console.WriteLine("SQL Server Data Example")
            DapperContext.Settings = ContextConfiguration.CreateNew.UseSettingsFileMode(SettingFileMode.NETCore).WithConnectionName("DefaultConnection").Build
            ConnectToSQLServer(enableAudit)
            ExecuteCRUDOperation()
            Console.WriteLine("---")

            'MySQL
            Console.WriteLine("MySQL Data Example")
            DapperContext.Settings = ContextConfiguration.CreateNew.UseSettingsFileMode(SettingFileMode.NETCore).WithConnectionName("MySqlConnection").Build
            ConnectToMySQL(enableAudit)
            ExecuteCRUDOperation()
            Console.WriteLine("---")

            'SQLite
            Console.WriteLine("SQLite Data Example")
            DapperContext.Settings = ContextConfiguration.CreateNew.UseSettingsFileMode(SettingFileMode.NETCore).WithConnectionName("SQLiteConnection").Build
            ConnectToSQLite(enableAudit)
            ExecuteCRUDOperation()
            Console.WriteLine("---")

            If enableAudit = True Then
                If DapperAuditContext.AuditSettings.StoreLogMode = AuditStoreMode.File Then
                    ReadLogFile()
                End If
            End If

        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try

    End Sub

    Public Sub ConnectToSQLServer(enableAudit As Boolean)
        If enableAudit = False Then
            ctx = New DapperContextSqlServer
        Else
            ctx = New DapperAuditContextSqlServer
        End If

    End Sub

    Public Sub ConnectToMySQL(enableAudit As Boolean)
        If enableAudit = False Then
            ctx = New DapperContextMySql
        Else
            ctx = New DapperAuditContextMySql
        End If
    End Sub

    Public Sub ConnectToSQLite(enableAudit As Boolean)

        If enableAudit = False Then
            ctx = New DapperContextSQLite
        Else
            ctx = New DapperAuditContextSQLite
        End If

    End Sub

    Private Sub ExecuteCRUDOperation()

        Dim insertedRecordID As Long = InsertRecord()

        GetRecordByID(insertedRecordID)

        GetAllRecords()

        UpdateRecordByID(insertedRecordID)

        DeleteRecordByID(insertedRecordID)

        DeleteAllRecords()

    End Sub

    Private Sub GetRecordByID(id As Object)
        'Get a single record
        Dim person As Model.Person = ctx.Get(Of Model.Person)(id)

        Console.WriteLine(String.Join(" | ", {person.ID, person.Name, person.Surname}))
    End Sub

    Private Sub GetAllRecords()
        'Get all record
        Dim lstPerson As List(Of Model.Person) = ctx.GetAll(Of Model.Person).ToList

        lstPerson.ForEach(Sub(x) Console.WriteLine(String.Join(" | ", {x.ID, x.Name, x.Surname})))
    End Sub

    Private Function InsertRecord() As Long
        'Create a record
        Dim person As New Model.Person With {
                .Name = "John",
                .Surname = "Doe"
            }

        Return ctx.InsertOrUpdate(person)

    End Function

    Public Sub UpdateRecordByID(id As Object)
        'Update a record
        Dim person As Model.Person = ctx.Get(Of Model.Person)(id)

        person.Surname = "Butt"

        ctx.InsertOrUpdate(person)

        Console.WriteLine(String.Join(" | ", {person.ID, person.Name, person.Surname}))
    End Sub

    Public Sub DeleteRecordByID(id As Long)
        'Delete a record
        Dim person As Model.Person = ctx.Get(Of Model.Person)(id)

        ctx.Delete(person)
    End Sub

    Public Sub DeleteAllRecords()
        'Delete all record
        ctx.DeleteAll(Of Model.Person)()
    End Sub

    Public Sub ReadLogFile()
        'Show audit record
        Dim logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AuditLogs")
        Dim logFile As String = Path.Combine(logDir, DapperAuditContext.AuditSettings.FileName)
        Dim auditLog As String = IO.File.ReadAllText(logFile)
        Console.Write(auditLog)

    End Sub


End Module
