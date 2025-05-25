Imports System.Data
Imports System.IO
Imports System.Reflection
Imports BertoSoftware.Context.Configuration
Imports Dapper
Imports MySqlConnector

Namespace Context.Tools.Audit

    Public Class DapperAuditContextMySql
        Inherits DapperAuditContext

        Public Sub New()

            Try
                Dim cnString As String = GetConnectionString()

                Dim cnStringBuilder As New MySqlConnectionStringBuilder(cnString)

                Me.Connection = New MySqlConnection(cnString)
                Me.Connection.ConnectionString = cnStringBuilder.ConnectionString

                Me.Connect()

                If AuditSettings.StoreLogMode = AuditStoreMode.Database Then
                    Dim result = Query($"SELECT * FROM information_schema.TABLES WHERE TABLE_SCHEMA = @p0 AND TABLE_NAME = @p1", New With {.p0 = cnStringBuilder.Database, .p1 = AuditSettings.TableName.ToLower})

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
                        result = Me.Connection.Query(Of String)("SELECT schema_name FROM infomrmation_schema.schemata WHERE schema_name = @p0", New With {.p0 = dbName}, transaction).FirstOrDefault
                        transaction.Commit()

                        Return result <> String.Empty
                    Catch ex As Exception
                        transaction.Rollback()
                        Throw
                    End Try
                End Using
            Else
                Try
                    result = Me.Connection.Query(Of String)("SELECT schema_name FROM infomrmation_schema.schemata WHERE schema_name = @p0", New With {.p0 = dbName}).FirstOrDefault
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