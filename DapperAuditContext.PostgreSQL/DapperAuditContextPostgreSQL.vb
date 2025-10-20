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

        ''' <summary>
        ''' Check if the database exists.
        ''' </summary>
        ''' <param name="dbName">The name of the database to check.</param>
        ''' <returns>True if the database exists, otherwise False.</returns>
        ''' <remarks>Uses information_schema to check for the existence of the database.</remarks>
        Public Overrides Function DatabaseExist(dbName As String) As Boolean

            Dim result As String

            Try
                result = Me.Connection.Query(Of String)("select datname from pg_database where datname = @p0", New With {.p0 = dbName}).FirstOrDefault
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