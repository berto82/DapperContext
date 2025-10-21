Imports System.IO
Imports System.Reflection
Imports BertoSoftware.Context.Configuration
Imports Microsoft.Data.Sqlite

Namespace Context.Tools.Audit

    Public Class DapperAuditContextSQLite
        Inherits DapperAuditContext

        Public Sub New()

            Dim cnString As String = GetConnectionString()

            Dim cnStringBuilder As New SqliteConnectionStringBuilder(cnString)

            Me.Connection = New SqliteConnection(cnString)
            Me.Connection.ConnectionString = cnStringBuilder.ConnectionString

            Me.Connect()

            If AuditSettings.StoreLogMode = AuditStoreMode.Database Then
                Dim result = Query("SELECT 1 FROM sqlite_master WHERE type='table' AND name = @p0", New With {.p0 = AuditSettings.TableName})

                If result.Count = 0 Then
                    Dim scriptCreateDB As String = GetFromResources("AuditTable.sql")
                    Execute(scriptCreateDB)
                End If

            End If
        End Sub

        ''' <summary>
        ''' Check if the database exists.
        ''' </summary>
        ''' <param name="dbName">The name of the database to check.</param>
        ''' <returns>True if the database exists, otherwise False.</returns>
        ''' <remarks>Uses information_schema to check for the existence of the database.</remarks>
        Public Overrides Function DatabaseExist(dbName As String) As Boolean
            Return IO.File.Exists(CType(Me.Connection, SqliteConnection).DataSource)
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