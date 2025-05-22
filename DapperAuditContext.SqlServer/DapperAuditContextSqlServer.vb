Imports System.Data
Imports System.Data.Common
Imports System.IO
Imports System.Reflection
Imports Dapper
Imports Microsoft.Data.SqlClient

Public Class DapperAuditContextSqlServer
    Inherits DapperAuditContext

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


            If AuditSettings.StoreLogMode = AuditStoreMode.Database Then
                Dim result = Query("SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE' AND TABLE_NAME='AuditTable'")

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

    Private Function GetFromResources(resourceName As String) As String
        Using s As Stream = Assembly.GetExecutingAssembly.GetManifestResourceStream($"{[GetType].Namespace}.{resourceName}")
            Using reader As New StreamReader(s)
                Return reader.ReadToEnd
            End Using
        End Using
    End Function
End Class
