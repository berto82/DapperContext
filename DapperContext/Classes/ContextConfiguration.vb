Namespace Context.Configuration
    Public Class ContextConfiguration
        Implements IContextConfigurationBuilder

        Public ReadOnly Property SettingsMode As SettingFileMode
        Public ReadOnly Property CustomConfigurationFile As String
        Public ReadOnly Property ConnectionName As String
        Public ReadOnly Property ConnectionString As String
        Public ReadOnly Property EnableTransaction As Boolean

        Private Sub New()
            _SettingsMode = SettingFileMode.NETCore
            _CustomConfigurationFile = String.Empty
            _ConnectionName = "DefaultConnection"
            _ConnectionString = String.Empty
            _EnableTransaction = True
        End Sub

        ''' <summary>
        ''' Create a new default settings
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function CreateNew() As IContextConfigurationBuilder
            Return New ContextConfiguration
        End Function

        Public Function UseSettingsFileMode(mode As SettingFileMode) As IContextConfigurationBuilder Implements IContextConfigurationBuilder.UseSettingsFileMode
            _SettingsMode = mode
            Return Me
        End Function

        Public Function WithConfigurationFile(filename As String) As IContextConfigurationBuilder Implements IContextConfigurationBuilder.WithCustomConfigurationFile
            _CustomConfigurationFile = filename
            Return Me
        End Function

        Public Function WithConnectionName(name As String) As IContextConfigurationBuilder Implements IContextConfigurationBuilder.WithConnectionName
            _ConnectionName = name
            Return Me
        End Function

        Public Function WithCustomConnectionString(connectionstring As String) As IContextConfigurationBuilder Implements IContextConfigurationBuilder.WithCustomConnectionString
            _ConnectionString = connectionstring
            Return Me
        End Function

        Public Function DisableTransaction() As IContextConfigurationBuilder Implements IContextConfigurationBuilder.DisableTransaction
            _EnableTransaction = False
            Return Me
        End Function

        Public Function Build() As ContextConfiguration Implements IContextConfigurationBuilder.Build
            Return Me
        End Function

    End Class
End Namespace