Imports BertoSoftware.Context.Tools.Audit
Imports Dapper.Contrib.Extensions

Namespace Model
    Partial Public Class Person

        <Computed>
        Public ReadOnly Property Location As Location
            Get
                Return New DapperAuditContextSqlServer().Get(Of Location)(LocationID)
            End Get
        End Property

    End Class
End Namespace