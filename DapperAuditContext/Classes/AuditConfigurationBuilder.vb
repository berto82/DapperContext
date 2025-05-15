Imports BertoSoftware.AuditConfiguration

Public Class AuditConfigurationBuilder

    Private logStoreMode As AuditStoreMode
    Private logFilePath As String
    Private logTableName As String

    Private Sub New()
        logStoreMode = AuditStoreMode.Database
        logTableName = "AuditTable"
        logFilePath = $"audit_{Date.UtcNow:yyyyMMdd}.log"
    End Sub

    Public Shared Function Create() As AuditConfigurationBuilder
        Return New AuditConfigurationBuilder
    End Function

    Public Function StoreMode(mode As AuditStoreMode) As AuditConfigurationBuilder
        logStoreMode = mode
        Return Me
    End Function

    Public Function AddLogToFile(path As String) As AuditConfigurationBuilder
        logFilePath = path
        Return Me
    End Function

    Public Function AddLogToDatabase(tableName As String) As AuditConfigurationBuilder
        logTableName = tableName
        Return Me
    End Function

    Public Function Build() As AuditConfiguration
        Return New AuditConfiguration(logStoreMode, logFilePath, logTableName)
    End Function
End Class
