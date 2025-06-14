Imports System.Data
Imports Dapper
Imports Microsoft.Data.Sqlite

Namespace Context.Tools

    Public Class DapperContextSQLite
        Inherits DapperContext

        Public Sub New()
            Dim cnString As String = GetConnectionString()

            Dim cnStringBuilder As New SqliteConnectionStringBuilder(cnString)

            Me.Connection = New SqliteConnection(cnString)
            Me.Connection.ConnectionString = cnStringBuilder.ConnectionString

            Me.Connect()

        End Sub

        ''' <summary>
        ''' Check if the database exists.
        ''' </summary>
        ''' <param name="dbName">The name of the database to check.</param>
        ''' <returns>True if the database exists, otherwise False.</returns>
        ''' <remarks>Uses information_schema to check for the existence of the database.</remarks>
        Public Overrides Function DatabaseExist(dbName As String) As Boolean

            Dim result As Boolean

            result = IO.File.Exists(CType(Me.Connection, SqliteConnection).DataSource)

            Return result

        End Function
    End Class
End Namespace