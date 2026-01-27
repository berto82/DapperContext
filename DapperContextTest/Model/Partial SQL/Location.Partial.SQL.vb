Imports BertoSoftware.Context.Tools.Audit
Imports Dapper.Contrib.Extensions

Namespace Model
    Partial Public Class Location

        <Computed>
        Public ReadOnly Property Person As IEnumerable(Of Person)
            Get
                Return New DapperAuditContextSqlServer().Query(Of Person)("select * from Person where LocationID = @p0", New With {.p0 = ID})
            End Get
        End Property

    End Class
End Namespace