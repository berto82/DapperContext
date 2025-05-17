Imports System

Module Program
    Sub Main(args As String())

        Try
            DapperContext.Settings = ContextConfiguration.CreateNew.UseSettingsFileMode(SettingFileMode.NET4x).WithConnectionName("pippo").Build
            DapperAuditContext.AuditSettings = AuditConfiguration.CreateNew.StoreMode(AuditStoreMode.File).Build

            Dim dbExist As Boolean = New DapperContext().DatabaseExist

            'Add new value into Person Table with an automatic audit trail, just use DapperAuditContext instead of DapperContext
            Using ctx As New DapperAuditContext


                Dim person As New Model.Person With {
                    .Name = "John",
                    .Surname = "Doe"
                }

                ctx.InsertOrUpdate(person)

                'Changes a property after save
                person.Surname = "Black"

                'Saves the changes and adds a record in the control table with the difference between the previous save.
                ctx.InsertOrUpdate(person)

            End Using

        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try





    End Sub
End Module
