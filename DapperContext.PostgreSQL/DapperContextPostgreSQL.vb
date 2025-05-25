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

        Public Overrides Function DatabaseExist(dbName As String) As Boolean

            Dim result As String

            If Settings.EnableTransaction = True Then
                Using transaction As IDbTransaction = Me.Connection.BeginTransaction
                    Try
                        result = Me.Connection.Query(Of String)("select datname from pg_database where datname = @p0", New With {.p0 = dbName}, transaction).FirstOrDefault
                        transaction.Commit()

                        Return result <> String.Empty
                    Catch ex As Exception
                        transaction.Rollback()
                        Throw
                    End Try
                End Using
            Else
                Try
                    result = Me.Connection.Query(Of String)("select datname from pg_database where datname = @p0", New With {.p0 = dbName}).FirstOrDefault
                Catch ex As Exception
                    Throw
                End Try
            End If

            Return False

        End Function
    End Class
End Namespace