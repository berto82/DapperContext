Imports System

Module Program
    Sub Main(args As String())

        DapperAuditContext.Config = AuditConfigurationBuilder.Create.StoreMode(AuditConfiguration.AuditStoreMode.File).Build

        'Add new value into Person Table without audit trail
        Using ctx As New DapperContext

            Dim person As New Model.Person With {
                .Name = "John",
                .Surname = "Doe"
            }

            ctx.InsertOrUpdate(person)
        End Using



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

    End Sub
End Module
