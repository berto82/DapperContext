Public Interface IAuditConfigurationBuilder

    Function StoreMode(mode As AuditStoreMode) As IAuditConfigurationBuilder
    Function WithCustomLogFilename(path As String) As IAuditConfigurationBuilder
    Function WithCustomTableName(tablename As String) As IAuditConfigurationBuilder
    Function Build() As AuditConfiguration

End Interface
