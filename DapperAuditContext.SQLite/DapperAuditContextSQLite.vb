Imports System.IO
Imports System.Reflection
Imports Microsoft.Data.Sqlite

Public Class DapperAuditContextSQLite
    Inherits DapperAuditContext

    Public Sub New()

        Dim cnString As String = GetConnectionString()

        Dim cnStringBuilder As New SqliteConnectionStringBuilder(cnString)

        Me.Connection = New SqliteConnection(cnString)
        Me.Connection.ConnectionString = cnStringBuilder.ConnectionString
        Me.Connection.Open()

        If AuditSettings.StoreLogMode = AuditStoreMode.Database Then
            Dim result = Query("SELECT 1 FROM sqlite_master WHERE type='table' AND name='AuditTable'")

            If result.Count = 0 Then
                Dim scriptCreateDB As String = GetFromResources("AuditTable.sql")
                Execute(scriptCreateDB)
            End If

        End If
    End Sub

    Public Overrides Function DatabaseExist(dbName As String) As Boolean

        Dim result As Boolean

        result = IO.File.Exists(CType(Me.Connection, SqliteConnection).DataSource)

        Return result

    End Function


    Private Function GetFromResources(resourceName As String) As String
        Using s As Stream = Assembly.GetExecutingAssembly.GetManifestResourceStream($"{[GetType].Namespace}.{resourceName}")
            Using reader As New StreamReader(s)
                Return reader.ReadToEnd
            End Using
        End Using
    End Function
End Class
