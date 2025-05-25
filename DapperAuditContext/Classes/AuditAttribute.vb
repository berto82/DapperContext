Namespace Context.Configuration
    ''' <summary>
    ''' Audit attribute is used to mark properties or classes that should be included in the audit trail.
    ''' </summary>
    ''' <remarks></remarks>
    <AttributeUsage(AttributeTargets.Property Or AttributeTargets.Class, Inherited:=False, AllowMultiple:=False)>
    Public Class AuditAttribute
        Inherits Attribute

        Public Sub New()
            _Include = True
        End Sub

        ''' <summary>
        ''' Constructor to set the Include property.
        ''' </summary>
        ''' <param name="include">If true, the property or class will be included in the audit trail.</param>
        ''' <remarks></remarks>     
        Public Sub New(include As Boolean)
            _Include = include
        End Sub

        Public ReadOnly Property Include As Boolean

    End Class
End Namespace