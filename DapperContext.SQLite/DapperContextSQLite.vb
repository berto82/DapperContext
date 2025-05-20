Imports System.Data
Imports Dapper
Imports Microsoft.Data.Sqlite

Public Class DapperContextSQLite
    Inherits DapperContext

    Public Sub New()
        Dim cnString As String = GetConnectionString()

        Dim cnStringBuilder As New SqliteConnectionStringBuilder(cnString)

        Me.Connection = New SqliteConnection(cnString)
        Me.Connection.ConnectionString = cnStringBuilder.ConnectionString
        Me.Connection.Open()

    End Sub

    Public Overrides Function DatabaseExist(dbName As String) As Boolean

        Dim result As Boolean

        result = IO.File.Exists(CType(Me.Connection, SqliteConnection).DataSource)

        Return result

    End Function
End Class
