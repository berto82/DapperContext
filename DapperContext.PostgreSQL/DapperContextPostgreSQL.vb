Imports System.Data
Imports Dapper
Imports Npgsql

Namespace Context.Tools.Audit

    Public Class DapperContextPostgreSQL
        Inherits DapperContext

        Public Sub New()
            Try
                Dim cnString As String = GetConnectionString()

                Dim cnStringBuilder As New NpgsqlConnectionStringBuilder(cnString)
                cnStringBuilder.ConnectionString = cnString

                Me.Connection = New NpgsqlConnection
                Me.Connection.ConnectionString = cnStringBuilder.ConnectionString

                Me.Connect()

            Catch ex As Exception
                Throw
            End Try

        End Sub

        ''' <summary>
        ''' Check if the database exists.
        ''' </summary>
        ''' <param name="dbName">The name of the database to check.</param>
        ''' <returns>True if the database exists, otherwise False.</returns>
        ''' <remarks>Uses information_schema to check for the existence of the database.</remarks>
        Public Overrides Function DatabaseExist(dbName As String) As Boolean
            Try
                Return Me.Connection.Query(Of String)("select datname from pg_database where datname = @p0", New With {.p0 = dbName}).Count > 0
            Catch ex As Exception
                Throw
            End Try

            Return False

        End Function
    End Class
End Namespace