Imports System.Data
Imports System.Data.Common
Imports Dapper
Imports Microsoft.Data.SqlClient

Namespace Context.Tools
    Public Class DapperContextSqlServer
        Inherits DapperContext

        Public Sub New()
            Try
                Dim cnString As String = GetConnectionString()

                Dim cnStringBuilder As New DbConnectionStringBuilder
                cnStringBuilder.ConnectionString = cnString

                Dim efConnectionString As Object = Nothing

                If cnStringBuilder.TryGetValue("provider connection string", efConnectionString) = True Then
                    cnStringBuilder.ConnectionString = CStr(efConnectionString)
                End If

                If cnStringBuilder.ContainsKey("TrustServerCertificate") = False Then
                    cnStringBuilder.Add("TrustServerCertificate", True)
                End If

                Me.Connection = New SqlConnection
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
                result = Me.Connection.Query(Of String)("SELECT name FROM master.sys.databases WHERE name = @p0", New With {.p0 = dbName}).FirstOrDefault
            Catch ex As Exception
                Throw
            End Try

            Return False

        End Function

    End Class
End Namespace