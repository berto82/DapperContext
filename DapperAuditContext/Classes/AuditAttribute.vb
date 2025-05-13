<AttributeUsage(AttributeTargets.Property Or AttributeTargets.Class, Inherited:=False, AllowMultiple:=False)>
Public Class AuditAttribute
    Inherits Attribute

    Public Sub New()
        _Include = True
    End Sub

    Public Sub New(include As Boolean)
        _Include = include
    End Sub

    Public ReadOnly Property Include As Boolean

End Class
