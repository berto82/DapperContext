Imports BertoSoftware.Context.Configuration
Imports BertoSoftware.Context.Tools.Audit
Imports Xunit

Namespace DapperContextUnitTest
    Public Class UnitTest1

        Public Sub New()
            'Configure context setting or leave default
            '***Uncomment this line if you want to configure settings
            'DapperContext.Settings = ContextConfiguration.CreateNew.UseSettingsFileMode(SettingFileMode.NETCore).Build
            'Configure audit setting or leave default
            '***Uncomment this line if you want to configure settings
            DapperAuditContext.AuditSettings = AuditConfiguration.CreateNew.StoreMode(AuditStoreMode.Database).Build
        End Sub

        <Fact>
        Sub TestDataSQLServer()

            Try
                Dim dataExample As New DataExample
                dataExample.ConnectAndExecute(DataExample.ConnectionType.SQLServer, True)

                If DapperAuditContext.AuditSettings.StoreLogMode <> AuditStoreMode.Database Then
                    dataExample.ReadLogFile()
                End If

                Assert.True(True)

            Catch ex As Exception
                Assert.False(True, ex.Message)
            End Try

        End Sub

        <Fact>
        Sub TestDataMySQL()
            Try
                Dim dataExample As New DataExample
                dataExample.ConnectAndExecute(DataExample.ConnectionType.MySQL, True)

                If DapperAuditContext.AuditSettings.StoreLogMode <> AuditStoreMode.Database Then
                    dataExample.ReadLogFile()
                End If

                Assert.True(True)

            Catch ex As Exception
                Assert.False(True, ex.Message)
            End Try
        End Sub


        <Fact>
        Sub TestDataSQLite()
            Try
                Dim dataExample As New DataExample
                dataExample.ConnectAndExecute(DataExample.ConnectionType.SQLite, True)

                If DapperAuditContext.AuditSettings.StoreLogMode <> AuditStoreMode.Database Then
                    dataExample.ReadLogFile()
                End If

                Assert.True(True)

            Catch ex As Exception
                Assert.False(True, ex.Message)
            End Try
        End Sub

        <Fact>
        Sub TestDataPostgreSQL()
            Try
                Dim dataExample As New DataExample
                dataExample.ConnectAndExecute(DataExample.ConnectionType.PostgreSQL, True)

                If DapperAuditContext.AuditSettings.StoreLogMode <> AuditStoreMode.Database Then
                    dataExample.ReadLogFile()
                End If

                Assert.True(True)

            Catch ex As Exception
                Assert.False(True, ex.Message)
            End Try
        End Sub

        <Fact>
        Sub TestDataFirebird()
            Try
                Dim dataExample As New DataExample
                dataExample.ConnectAndExecute(DataExample.ConnectionType.Firebird, True)

                If DapperAuditContext.AuditSettings.StoreLogMode <> AuditStoreMode.Database Then
                    dataExample.ReadLogFile()
                End If

                Assert.True(True)

            Catch ex As Exception
                Assert.False(True, ex.Message)
            End Try
        End Sub

    End Class
End Namespace

