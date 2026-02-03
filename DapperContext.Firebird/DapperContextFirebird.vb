Imports BertoSoftware.Context.Tools
Imports FirebirdSql.Data.FirebirdClient

Namespace Context.Tools
    Public Class DapperContextFirebird
        Inherits DapperContext

        Public Sub New()
            Dim cnString As String = GetConnectionString()

            Dim cnStringBuilder As New FbConnectionStringBuilder(cnString)

            Me.Connection = New FbConnection(cnString)
            Me.Connection.ConnectionString = cnStringBuilder.ConnectionString

            Me.Connect()

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
    End Class
End Namespace