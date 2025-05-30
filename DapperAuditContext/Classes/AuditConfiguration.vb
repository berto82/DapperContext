﻿Namespace Context.Configuration
    Public Class AuditConfiguration
        Implements IAuditConfigurationBuilder

        Public ReadOnly Property StoreLogMode As AuditStoreMode
        Public ReadOnly Property Path As String
        Public ReadOnly Property FileName As String
        Public ReadOnly Property TableName As String

        Private Sub New()
            _StoreLogMode = AuditStoreMode.Database
            _TableName = "AuditTable"
            _Path = IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AuditLogs", Environment.UserName)
            _FileName = $"audit_{Date.Now:yyyyMMdd}.log"
        End Sub

        Public Shared Function CreateNew() As IAuditConfigurationBuilder
            Return New AuditConfiguration
        End Function

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

        Public Function WithCustomLogPath(path As String) As IAuditConfigurationBuilder Implements IAuditConfigurationBuilder.WithCustomLogPath
            _Path = path
            Return Me
        End Function

        Public Function WithCustomLogFilename(filename As String) As IAuditConfigurationBuilder Implements IAuditConfigurationBuilder.WithCustomLogFilename
            _FileName = filename
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
End Namespace