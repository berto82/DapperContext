Public Interface IAuditConfigurationBuilder

    Function StoreMode(mode As AuditStoreMode) As IAuditConfigurationBuilder
    Function WithCustomLogPath(path As String) As IAuditConfigurationBuilder
    Function WithCustomLogFilename(filename As String) As IAuditConfigurationBuilder
    Function WithCustomTableName(tablename As String) As IAuditConfigurationBuilder
    Function Build() As AuditConfiguration

End Interface
