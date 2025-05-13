Imports System.IO
Imports System.Reflection
Imports KellermanSoftware.CompareNetObjects

''' <summary>
''' DapperAuditContext is a DapperContext that creates an audit trail of all changes to the database.
''' It uses the KellermanSoftware.CompareNetObjects library to compare objects and create a list of changes.
''' The audit trail is stored in the AuditTable table in the database.
''' </summary>
''' <remarks></remarks>
''' <summary>
''' DapperAuditContext is a DapperContext that creates an audit trail of all changes to the database.
''' It uses the KellermanSoftware.CompareNetObjects library to compare objects and create a list of changes.
''' The audit trail is stored in the AuditTable table in the database.
''' </summary>
Public Class DapperAuditContext
    Inherits DapperContext

    Public Enum AuditActionType
        Create
        Update
        Delete
    End Enum

    Public Sub New()
        Dim result = Query("SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE' AND TABLE_NAME='AuditTable'")

        If result.Count = 0 Then
            Dim scriptCreateDB As String = GetFromResources("AuditTable.sql")
            Execute(scriptCreateDB)
        End If
    End Sub

    ''' <summary>
    ''' Insert or Update the entity and create an audit trail.
    ''' </summary>
    ''' <typeparam name="TEntity"></typeparam>
    ''' <param name="entity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' <summary>
    ''' Insert or Update the entity and create an audit trail.
    ''' </summary>
    ''' <typeparam name="TEntity"></typeparam>
    ''' <param name="entity"></param>
    ''' <returns></returns>
    Public Overrides Function InsertOrUpdate(Of TEntity As Class)(entity As TEntity) As Long

        Dim result As Long
        Dim keyValue As Object = GetKeyFieldValue(entity)

        If CInt(keyValue) = 0 Then
            result = MyBase.InsertOrUpdate(entity)
            CreateAuditTrail(AuditActionType.Create, result, Activator.CreateInstance(Of TEntity), entity)
        Else
            Dim dbRec As TEntity = [Get](Of TEntity)(keyValue)
            result = MyBase.InsertOrUpdate(entity)
            CreateAuditTrail(AuditActionType.Update, CInt(keyValue), dbRec, entity)
        End If

        Return result

    End Function

    ''' <summary>
    ''' Insert or Update the entity and create an audit trail.
    ''' </summary>
    ''' <typeparam name="TEntity"></typeparam>
    ''' <param name="entity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' <summary>
    ''' Insert or Update the entity and create an audit trail.
    ''' </summary>
    ''' <typeparam name="TEntity"></typeparam>
    ''' <param name="entity"></param>
    ''' <returns></returns>
    Public Overrides Async Function InsertOrUpdateAsync(Of TEntity As Class)(entity As TEntity) As Task(Of Long)

        Dim result As Long
        Dim keyValue As Object = GetKeyFieldValue(entity)

        If CInt(keyValue) = 0 Then
            result = Await MyBase.InsertOrUpdateAsync(entity)
            CreateAuditTrail(AuditActionType.Create, result, Activator.CreateInstance(Of TEntity), entity)
        Else
            Dim dbRec As TEntity = Await GetAsync(Of TEntity)(keyValue)
            result = Await MyBase.InsertOrUpdateAsync(entity)
            CreateAuditTrail(AuditActionType.Update, CInt(keyValue), dbRec, entity)
        End If

        Return result

    End Function

    ''' <summary>
    ''' Delete the entity and create an audit trail.
    ''' </summary>
    ''' <typeparam name="TEntity"></typeparam>
    ''' <param name="entity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' <summary>
    ''' Delete the entity and create an audit trail.
    ''' </summary>
    ''' <typeparam name="TEntity"></typeparam>
    ''' <param name="entity"></param>
    ''' <returns></returns>
    Public Overrides Function Delete(Of TEntity As Class)(entity As TEntity) As Boolean

        Dim result As Boolean
        Dim keyValue As Object = GetKeyFieldValue(entity)
        Dim dbRec As TEntity = [Get](Of TEntity)(keyValue)
        result = MyBase.Delete(entity)

        CreateAuditTrail(AuditActionType.Delete, CInt(keyValue), dbRec, Activator.CreateInstance(Of TEntity))

        Return result

    End Function

    ''' <summary>
    ''' Delete the entity and create an audit trail.
    ''' </summary>
    ''' <typeparam name="TEntity"></typeparam>
    ''' <param name="entity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' <summary>
    ''' Delete the entity and create an audit trail.
    ''' </summary>
    ''' <typeparam name="TEntity"></typeparam>
    ''' <param name="entity"></param>
    ''' <returns></returns>
    Public Overrides Async Function DeleteAsync(Of TEntity As Class)(entity As TEntity) As Task(Of Boolean)

        Dim result As Boolean
        Dim keyValue As Object = GetKeyFieldValue(entity)
        Dim dbRec As TEntity = Await GetAsync(Of TEntity)(keyValue)
        result = Await MyBase.DeleteAsync(entity)

        CreateAuditTrail(AuditActionType.Delete, CInt(keyValue), dbRec, Activator.CreateInstance(Of TEntity))

        Return result

    End Function

    ''' <summary>
    ''' Delete all entities of the specified type and create an audit trail.
    ''' </summary>
    ''' <typeparam name="TEntity"></typeparam>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' <summary>
    ''' Delete all entities of the specified type and create an audit trail.
    ''' </summary>
    ''' <typeparam name="TEntity"></typeparam>
    ''' <returns></returns>
    Public Overrides Function DeleteAll(Of TEntity As Class)() As Boolean
        Dim result As Boolean

        Dim dbRec As IEnumerable(Of TEntity) = GetAll(Of TEntity)()
        result = MyBase.DeleteAll(Of TEntity)()

        dbRec.ToList.ForEach(Sub(x) CreateAuditTrail(AuditActionType.Delete, CInt(GetKeyFieldValue(x)), dbRec, Activator.CreateInstance(Of TEntity)))

        Return result

    End Function

    ''' <summary>
    ''' Delete all entities of the specified type and create an audit trail.
    ''' </summary>
    ''' <typeparam name="TEntity"></typeparam>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' <summary>
    ''' Delete all entities of the specified type and create an audit trail.
    ''' </summary>
    ''' <typeparam name="TEntity"></typeparam>
    ''' <returns></returns>
    Public Overrides Async Function DeleteAllAsync(Of TEntity As Class)() As Task(Of Boolean)
        Dim result As Boolean

        Dim dbRec As IEnumerable(Of TEntity) = Await GetAllAsync(Of TEntity)()
        result = Await MyBase.DeleteAllAsync(Of TEntity)()

        dbRec.ToList.ForEach(Sub(x) CreateAuditTrail(AuditActionType.Delete, CInt(GetKeyFieldValue(x)), dbRec, Activator.CreateInstance(Of TEntity)))

        Return result

    End Function

    Private Sub CreateAuditTrail(action As AuditActionType, keyFieldID As Long, oldObject As Object, newObject As Object)

        Dim compObjects As New CompareLogic
        compObjects.Config.IgnoreCollectionOrder = True
        compObjects.Config.MaxDifferences = 99

        Dim classAttr As AuditAttribute = CType(newObject.GetType().GetCustomAttributes(GetType(AuditAttribute), True), AuditAttribute()).FirstOrDefault

        If classAttr Is Nothing Then
            Return
        End If

        If classAttr.Include = False Then
            Return
        End If

        For Each prop As PropertyInfo In newObject.GetType().GetProperties
            Dim propAttr As AuditAttribute = CType(prop.GetCustomAttributes(GetType(AuditAttribute), True), AuditAttribute()).FirstOrDefault

            If propAttr IsNot Nothing Then
                If propAttr.Include = False Then
                    compObjects.Config.MembersToIgnore.Add(prop.Name)
                End If
            End If
        Next

        Dim compResult As ComparisonResult = compObjects.Compare(oldObject, newObject)
        Dim deltaList As New List(Of AuditDelta)

        For Each change As Difference In compResult.Differences
            Dim delta As New AuditDelta
            delta.FieldName = change.PropertyName
            delta.ValueBefore = change.Object1Value
            delta.ValueAfter = change.Object2Value
            deltaList.Add(delta)
        Next

        Dim audit As New AuditTable

        audit.ActionType = CInt(action)
        audit.DataModel = newObject.GetType.Name
        audit.DateTimeStamp = Date.Now
        audit.KeyFieldID = keyFieldID
        audit.ValueBefore = Text.Json.JsonSerializer.Serialize(oldObject)
        audit.ValueAfter = Text.Json.JsonSerializer.Serialize(newObject)
        audit.Changes = Text.Json.JsonSerializer.Serialize(deltaList)

        MyBase.InsertOrUpdate(audit)

    End Sub

    Private Function GetFromResources(resourceName As String) As String

        Using s As Stream = Assembly.GetExecutingAssembly.GetManifestResourceStream($"{[GetType].Namespace}.{resourceName}")
            Using reader As New StreamReader(s)
                Return reader.ReadToEnd
            End Using
        End Using


    End Function

End Class
