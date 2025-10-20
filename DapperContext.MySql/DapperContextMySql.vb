Imports System.Data
Imports Dapper
Imports MySqlConnector

Namespace Context.Tools
    Public Class DapperContextMySql
        Inherits DapperContext

        Public Sub New()

            Try
                Dim cnString As String = GetConnectionString()

                Dim cnStringBuilder As New MySqlConnectionStringBuilder(cnString)

                Me.Connection = New MySqlConnection(cnString)
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

            Dim result As String

            Try
                result = Me.Connection.Query(Of String)("SELECT schema_name FROM infomrmation_schema.schemata WHERE schema_name = @p0", New With {.p0 = dbName}).FirstOrDefault
            Catch ex As Exception
                Throw
            End Try


            Return False

        End Function
    End Class
End Namespace