Public Class AuditConfiguration
    Implements IAuditConfigurationBuilder

    Public ReadOnly Property StoreLogMode As AuditStoreMode
    Public ReadOnly Property FilePath As String
    Public ReadOnly Property TableName As String

    Private Sub New()
        _StoreLogMode = AuditStoreMode.Database
        _TableName = "AuditTable"
        _FilePath = $"audit_{Date.UtcNow:yyyyMMdd}.log"
    End Sub

    Public Shared Function CreateNew() As IAuditConfigurationBuilder
        Return New AuditConfiguration
    End Function

    Public Function StoreMode(mode As AuditStoreMode) As IAuditConfigurationBuilder Implements IAuditConfigurationBuilder.StoreMode
        _StoreLogMode = mode
        Return Me
    End Function

    Public Function WithCustomLogFilename(path As String) As IAuditConfigurationBuilder Implements IAuditConfigurationBuilder.WithCustomLogFilename
        _FilePath = path
        Return Me
    End Function

    Public Function WithCustomTableName(tablename As String) As IAuditConfigurationBuilder Implements IAuditConfigurationBuilder.WithCustomTableName
        _TableName = tablename
        Return Me
    End Function

    Public Function Build() As AuditConfiguration Implements IAuditConfigurationBuilder.Build
        Return Me
    End Function

End Class
