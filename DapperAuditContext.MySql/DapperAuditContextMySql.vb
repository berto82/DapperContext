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

        ''' <summary>
        ''' Check if the database exists.
        ''' </summary>
        ''' <param name="dbName">The name of the database to check.</param>
        ''' <returns>True if the database exists, otherwise False.</returns>
        ''' <remarks>Uses information_schema to check for the existence of the database.</remarks>
        Public Overrides Function DatabaseExist(dbName As String) As Boolean
            Try
                Return Me.Connection.Query(Of String)("SELECT schema_name FROM infomrmation_schema.schemata WHERE schema_name = @p0", New With {.p0 = dbName}).Count > 0
            Catch ex As Exception
                Throw
            End Try

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