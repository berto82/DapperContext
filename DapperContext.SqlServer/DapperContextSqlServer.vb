Imports System.Data
Imports Dapper
Imports Microsoft.Data.SqlClient

Public Class DapperContextSqlServer
    Inherits DapperContext

    Public Sub New()
        Try
            Dim cnString As String = GetConnectionString()

            Dim cnStringBuilder As New SqlConnectionStringBuilder(cnString)

            Me.Connection = New SqlConnection
            Me.Connection.ConnectionString = cnStringBuilder.ConnectionString
            Me.Connection.Open()

        Catch ex As Exception
            Throw
        End Try


    End Sub

    Public Overrides Function DatabaseExist(dbName As String) As Boolean

        Dim result As String

        If Settings.EnableTransaction = True Then
            Using transaction As IDbTransaction = Me.Connection.BeginTransaction
                Try
                    result = Me.Connection.Query(Of String)("SELECT name FROM master.sys.databases WHERE name = @p0", New With {.p0 = dbName}, transaction).FirstOrDefault
                    transaction.Commit()

                    Return result <> String.Empty
                Catch ex As Exception
                    transaction.Rollback()
                    Throw
                End Try
            End Using
        Else
            Try
                result = Me.Connection.Query(Of String)("SELECT name FROM master.sys.databases WHERE name = @p0", New With {.p0 = dbName}).FirstOrDefault
            Catch ex As Exception
                Throw
            End Try
        End If

        Return False

    End Function

End Class
