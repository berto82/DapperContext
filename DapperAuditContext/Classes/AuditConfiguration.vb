Public Class AuditConfiguration

    Enum AuditStoreMode
        Database
        File
    End Enum

    Public ReadOnly Property StoreMode As AuditStoreMode
    Public ReadOnly Property FilePath As String
    Public ReadOnly Property TableName As String

    Friend Sub New(logStoreMode As AuditStoreMode, logfilePath As String, logTableName As String)
        _StoreMode = logStoreMode
        _FilePath = logfilePath
        _TableName = logTableName
    End Sub

End Class
