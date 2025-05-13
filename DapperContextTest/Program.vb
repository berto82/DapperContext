Imports System

Module Program
    Sub Main(args As String())

        Using ctx As New DapperAuditContext

            Dim person As New Model.Person With {
                .Name = "John",
                .Surname = "Doe"
            }

            ctx.InsertOrUpdate(person)


        End Using


    End Sub
End Module
