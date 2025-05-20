Public Interface IContextConfigurationBuilder

    ''' <summary>
    ''' Select which configuration file will be used.
    ''' </summary>
    ''' <remarks><b>Default value:</b> NET4x</remarks>
    ''' <param name="mode">Select which configuration file will be used</param>
    ''' <returns></returns>
    Function UseSettingsFileMode(mode As SettingFileMode) As IContextConfigurationBuilder

    ''' <summary>
    ''' Select the custom configuration file, also specifying the type of configuration to be used with the <b>UseSettingsFileMode</b> method.
    ''' </summary>
    ''' <example>
    ''' <code>
    ''' DapperContext.Settings = ContextConfiguration.CreateNew.UseSettingsFileMode(SettingFileMode.NET4x).WithCustomConfigurationFile(myfile.config).Build
    '''  
    ''' </code>
    ''' </example>
    ''' <param name="filename">A custom configuration file name</param>
    ''' <returns></returns>
    Function WithCustomConfigurationFile(filename As String) As IContextConfigurationBuilder

    ''' <summary>
    ''' Specify a custom connection name
    ''' </summary>
    ''' <param name="name">A connection name</param>
    ''' <returns></returns>
    Function WithConnectionName(name As String) As IContextConfigurationBuilder

    ''' <summary>
    ''' Disable transaction on Dapper
    ''' </summary>
    ''' <returns></returns>
    Function DisableTransaction() As IContextConfigurationBuilder

    ''' <summary>
    ''' Build configuration settings
    ''' </summary>
    ''' <returns></returns>
    Function Build() As ContextConfiguration

End Interface
