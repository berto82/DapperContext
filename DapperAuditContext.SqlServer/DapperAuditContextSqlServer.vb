Imports System.IO
Imports System.Reflection
Imports Microsoft.Data.SqlClient

Public Class DapperAuditContextSqlServer
    Inherits DapperAuditContext

    Public Sub New()

        Try
            Dim cnString As String = GetConnectionString()

            Dim cnStringBuilder As New SqlConnectionStringBuilder(cnString)

            Me.Connection = New SqlConnection
            Me.Connection.ConnectionString = cnStringBuilder.ConnectionString
            Me.Connection.Open()

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

    Private Function GetFromResources(resourceName As String) As String
        Using s As Stream = Assembly.GetExecutingAssembly.GetManifestResourceStream($"{[GetType].Namespace}.{resourceName}")
            Using reader As New StreamReader(s)
                Return reader.ReadToEnd
            End Using
        End Using
    End Function
End Class
