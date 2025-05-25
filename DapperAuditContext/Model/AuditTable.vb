Imports System.Text.Json.Serialization
Imports Dapper.Contrib.Extensions

Namespace Internal.Model

    <Table("AuditTable")>
    Friend Class AuditTable

        <Key>
        <JsonIgnore>
        Public Property ID As Integer
        Public Property Username As String
        Public Property KeyFieldID As Long
        Public Property ActionType As Integer
        Public Property DateTimeStamp As Date
        Public Property DataModel As String
        Public Property Changes As Object
        Public Property ValueBefore As Object
        Public Property ValueAfter As Object

    End Class
End Namespace