Namespace Context.Configuration
    Public Class AuditConfiguration
        Implements IAuditConfigurationBuilder

        Public ReadOnly Property StoreLogMode As AuditStoreMode
        Public ReadOnly Property Path As String
        Public ReadOnly Property FileName As String
        Public ReadOnly Property TableName As String
        Public Property UserResolver As Func(Of String)


        Private Sub New()
            _StoreLogMode = AuditStoreMode.Database
            _TableName = "AuditTable"
            _Path = IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AuditLogs", Environment.UserName)
            _FileName = $"audit_{Date.Now:yyyyMMdd}.log"
            _UserResolver = Function() $"{Environment.UserDomainName}\{Environment.UserName}"
        End Sub

        ''' <summary>
        ''' Initialize a new audit configuration settings
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function CreateNew() As IAuditConfigurationBuilder
            Return New AuditConfiguration
        End Function

        ''' <summary>
        ''' Set the mode of storing audit logs.
        ''' The default is Database mode, which will store logs in a database table.
        ''' Other options include TextFile and JSON for file-based storage.
        ''' </summary>
        ''' <param name="mode"></param>
        ''' <returns></returns>
        Public Function StoreMode(mode As AuditStoreMode) As IAuditConfigurationBuilder Implements IAuditConfigurationBuilder.StoreMode
            _StoreLogMode = mode

            Select Case _StoreLogMode
                Case AuditStoreMode.TextFile
                    _FileName = $"audit_{Date.Now:yyyyMMdd}.log"
                Case AuditStoreMode.JSON
                    _FileName = $"audit_{Date.Now:yyyyMMdd}.json"
            End Select

            Return Me
        End Function

        ''' <summary>
        ''' Set the path where audit logs will be stored.
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        Public Function WithCustomLogPath(path As String) As IAuditConfigurationBuilder Implements IAuditConfigurationBuilder.WithCustomLogPath
            _Path = path
            Return Me
        End Function

        ''' <summary>
        ''' Set the filename for the audit logs.
        ''' </summary>
        ''' <param name="filename"></param>
        ''' <returns></returns>
        Public Function WithCustomLogFilename(filename As String) As IAuditConfigurationBuilder Implements IAuditConfigurationBuilder.WithCustomLogFilename
            _FileName = filename
            Return Me
        End Function

        ''' <summary>
        ''' Set a custom table name for the audit logs when using database storage mode.
        ''' </summary>
        ''' <param name="tablename"></param>
        ''' <returns></returns>
        Public Function WithCustomTableName(tablename As String) As IAuditConfigurationBuilder Implements IAuditConfigurationBuilder.WithCustomTableName
            _TableName = tablename
            Return Me
        End Function

        ''' <summary>
        ''' Set a custom user resolver function to determine the user associated with each audit log entry.
        ''' By default, it uses the current Windows user.
        ''' </summary>
        ''' <param name="resolver">A function that returns the username as a string.</param>
        ''' <returns></returns>
        Public Function WithCustomUserResolver(resolver As Func(Of String)) As IAuditConfigurationBuilder Implements IAuditConfigurationBuilder.WithCustomUserResolver
            UserResolver = resolver
            Return Me
        End Function

        ''' <summary>
        ''' Build the audit configuration.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' This method finalizes the configuration and returns the AuditConfiguration instance.
        ''' </remarks>
        Public Function Build() As AuditConfiguration Implements IAuditConfigurationBuilder.Build
            Return Me
        End Function

    End Class
End Namespace