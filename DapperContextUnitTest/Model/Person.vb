Imports BertoSoftware.Context.Configuration
Imports Dapper.Contrib.Extensions

Namespace Model

    <Table("Person")>
    <Audit>
    Public Class Person
        <Key>
        Public Property ID As Integer
        Public Property Name As String
        Public Property Surname As String
        Public Property LocationID As Integer?

    End Class
End Namespace
