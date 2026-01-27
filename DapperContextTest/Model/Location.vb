Imports BertoSoftware.Context.Configuration
Imports Dapper.Contrib.Extensions

Namespace Model

    <Table("Location")>
    <Audit>
    Public Class Location
        <Key>
        Public Property ID As Integer
        Public Property Name As String

    End Class
End Namespace
