Imports System.Data
Imports System.IO
Imports System.Reflection
Imports BertoSoftware.Context.Configuration
Imports Dapper
Imports Npgsql

Namespace Context.Tools.Audit
    Public Class DapperAuditContextPostgreSQL
        Inherits DapperAuditContext

        Public Sub New()
            Try
                Dim cnString As String = GetConnectionString()

                Dim cnStringBuilder As New NpgsqlConnectionStringBuilder(cnString)
                cnStringBuilder.ConnectionString = cnString

                Me.Connection = New NpgsqlConnection
                Me.Connection.ConnectionString = cnStringBuilder.ConnectionString

                Me.Connect()

                If AuditSettings.StoreLogMode = AuditStoreMode.Database Then
                    Dim result = Query($"SELECT * FROM pg_tables WHERE schemaname = 'public' AND tablename = @p0", New With {.p0 = AuditSettings.TableName})

                    If result.Count = 0 Then
                        Dim scriptCreateDB As String = GetFromResources("AuditTable.sql")
                        Execute(scriptCreateDB)
                    End If

                End If


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

        Private Function GetFromResources(resourceName As String) As String
            Using s As Stream = Assembly.GetExecutingAssembly.GetManifestResourceStream($"{[GetType].Namespace.Split("."c)(0)}.{resourceName}")
                Using reader As New StreamReader(s)
                    Return reader.ReadToEnd
                End Using
            End Using
        End Function


    End Class
End Namespace