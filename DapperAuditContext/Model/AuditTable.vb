Imports Dapper.Contrib.Extensions

<Table("AuditTable")>
Public Class AuditTable

    <Key>
    Public Property ID As Integer
    Public Property Username As String
    Public Property KeyFieldID As Long
    Public Property ActionType As Integer
    Public Property DateTimeStamp As Date
    Public Property DataModel As String
    Public Property Changes As String
    Public Property ValueBefore As String
    Public Property ValueAfter As String

End Class
