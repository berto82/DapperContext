Imports System.IO
Imports System.Reflection
Imports BertoSoftware.Context.Configuration
Imports FirebirdSql.Data.FirebirdClient

Namespace Context.Tools.Audit

    Public Class DapperAuditContextFirebird
        Inherits DapperAuditContext

        Public Sub New()

            Try
                Dim cnString As String = GetConnectionString()

                Dim cnStringBuilder As New FbConnectionStringBuilder(cnString)

                Me.Connection = New FbConnection(cnString)
                Me.Connection.ConnectionString = cnStringBuilder.ConnectionString

                Me.Connect()

                If AuditSettings.StoreLogMode = AuditStoreMode.Database Then
                    Dim result = Query($"SELECT * FROM RDB$RELATIONS WHERE RDB$RELATION_NAME = @p0", New With {.p0 = AuditSettings.TableName.ToUpper})

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
            Return IO.File.Exists(CType(Me.Connection, FbConnection).DataSource)
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