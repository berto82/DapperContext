Imports System.IO
Imports System.Reflection
Imports System.Text
Imports BertoSoftware.Context.Configuration
Imports BertoSoftware.Internal.Model
Imports Dapper.Contrib.Extensions
Imports KellermanSoftware.CompareNetObjects

Namespace Context.Tools.Audit
    ''' <summary>
    ''' DapperAuditContext is a DapperContext that creates an audit trail of all changes to the database.
    ''' It uses the KellermanSoftware.CompareNetObjects library to compare objects and create a list of changes.
    ''' The audit trail is stored in the AuditTable table in the database.
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class DapperAuditContext
        Inherits DapperContext

        Public Enum AuditActionType
            Create
            Update
            Delete
        End Enum

        ''' <summary>
        ''' AuditSettings is the configuration for the audit trail.
        ''' It is used to determine how the audit trail is stored and where it is stored.
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared Property AuditSettings As AuditConfiguration
        ''' <summary>
        ''' DisableAudit is a flag that can be set to true to disable the audit trail.
        ''' This is useful for performance reasons when the audit trail is not needed.
        ''' </summary>
        ''' <remarks></remarks>     
        Public Shared Property DisableAudit As Boolean

        Protected Friend Sub New()

            If AuditSettings Is Nothing Then
                AuditSettings = AuditConfiguration.CreateNew.StoreMode(AuditStoreMode.TextFile).Build
            End If

        End Sub

        ''' <summary>
        ''' Insert or Update the entity and create an audit trail.
        ''' </summary>
        ''' <typeparam name="TEntity"></typeparam>
        ''' <param name="entity"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function InsertOrUpdate(Of TEntity As Class)(entity As TEntity) As Object

            Dim result As Object
            Dim keyValue As Object = GetKeyFieldValue(entity)

            If CLng(keyValue) = 0 Then
                result = MyBase.InsertOrUpdate(entity)
                CreateAuditTrail(AuditActionType.Create, CLng(result), Activator.CreateInstance(Of TEntity), entity)
            Else
                Dim dbRec As TEntity = [Get](Of TEntity)(keyValue)
                result = MyBase.InsertOrUpdate(entity)
                CreateAuditTrail(AuditActionType.Update, CLng(keyValue), dbRec, entity)
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
        Public Overrides Async Function InsertOrUpdateAsync(Of TEntity As Class)(entity As TEntity) As Task(Of Object)

            Dim result As Object
            Dim keyValue As Object = GetKeyFieldValue(entity)

            If CInt(keyValue) = 0 Then
                result = Await MyBase.InsertOrUpdateAsync(entity)
                CreateAuditTrail(AuditActionType.Create, CLng(result), Activator.CreateInstance(Of TEntity), entity)
            Else
                Dim dbRec As TEntity = Await GetAsync(Of TEntity)(keyValue)
                result = Await MyBase.InsertOrUpdateAsync(entity)
                CreateAuditTrail(AuditActionType.Update, CLng(keyValue), dbRec, entity)
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
        Public Overrides Async Function DeleteAllAsync(Of TEntity As Class)() As Task(Of Boolean)
            Dim result As Boolean

            Dim dbRec As IEnumerable(Of TEntity) = Await GetAllAsync(Of TEntity)()
            result = Await MyBase.DeleteAllAsync(Of TEntity)()

            dbRec.ToList.ForEach(Sub(x) CreateAuditTrail(AuditActionType.Delete, CInt(GetKeyFieldValue(x)), dbRec, Activator.CreateInstance(Of TEntity)))

            Return result

        End Function

        Private Sub CreateAuditTrail(action As AuditActionType, keyFieldID As Long, oldObject As Object, newObject As Object)

            If _DisableAudit = True Then
                Return
            End If

            Dim jsonOptions As New Json.JsonSerializerOptions With {
                .IgnoreReadOnlyProperties = True,
                .IgnoreReadOnlyFields = True,
                .ReferenceHandler = Json.Serialization.ReferenceHandler.IgnoreCycles
            }

            Dim compObjects As New CompareLogic
            compObjects.Config.IgnoreCollectionOrder = True
            compObjects.Config.MaxDifferences = 99

            Dim auditAttr As AuditAttribute = newObject.GetType().GetCustomAttribute(Of AuditAttribute)

            If auditAttr Is Nothing Then
                Return
            End If

            If auditAttr.Include = False Then
                Return
            End If

            For Each prop As PropertyInfo In newObject.GetType().GetProperties
                Dim propAttr As AuditAttribute = prop.GetCustomAttribute(Of AuditAttribute)
                Dim computedAttr As ComputedAttribute = prop.GetCustomAttribute(Of ComputedAttribute)

                If propAttr IsNot Nothing Then
                    If Not propAttr.Include Then
                        compObjects.Config.MembersToIgnore.Add(prop.Name)
                    End If
                End If

                If computedAttr IsNot Nothing Then
                    compObjects.Config.MembersToIgnore.Add(prop.Name)
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
            audit.Username = GetCurrentUserName()
            audit.DataModel = newObject.GetType.Name
            audit.DateTimeStamp = Date.Now
            audit.KeyFieldID = keyFieldID

            If AuditSettings.StoreLogMode <> AuditStoreMode.JSON Then
                audit.ValueBefore = Text.Json.JsonSerializer.Serialize(oldObject, jsonOptions)
                audit.ValueAfter = Text.Json.JsonSerializer.Serialize(newObject, jsonOptions)
                audit.Changes = Text.Json.JsonSerializer.Serialize(deltaList, jsonOptions)
            Else
                audit.ValueBefore = oldObject
                audit.ValueAfter = newObject
                audit.Changes = deltaList
            End If

            Select Case AuditSettings.StoreLogMode
                Case AuditStoreMode.Database
                    MyBase.InsertOrUpdate(audit)
                Case AuditStoreMode.TextFile
                    Dim logDir As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AuditLogs", Environment.UserName)

                    If AuditSettings.Path <> logDir Then
                        logDir = Path.Combine(AuditSettings.Path, Environment.UserName)
                    End If

                    If Not Directory.Exists(logDir) Then Directory.CreateDirectory(logDir)

                    Dim fullPath = Path.Combine(logDir, AuditSettings.FileName)

                    Using fs As New IO.FileStream(fullPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite)
                        Using sw As New IO.StreamWriter(fs, Encoding.Default) With {.AutoFlush = True}
                            sw.WriteLine("-----")
                            sw.WriteLine($"Timestamp: {audit.DateTimeStamp:O}")
                            sw.WriteLine($"User:      {audit.Username}")
                            sw.WriteLine($"Table:     {audit.DataModel}")
                            sw.WriteLine($"Action:    {CType(audit.ActionType, AuditActionType)}")
                            sw.WriteLine($"Keys:      {audit.KeyFieldID}")
                            If Not String.IsNullOrEmpty(CStr(audit.ValueBefore)) Then sw.WriteLine($"Old:       {audit.ValueBefore}")
                            If Not String.IsNullOrEmpty(CStr(audit.ValueAfter)) Then sw.WriteLine($"New:       {audit.ValueAfter}")
                            If Not String.IsNullOrEmpty(CStr(audit.Changes)) Then sw.WriteLine($"Changes:    {audit.Changes}")
                        End Using
                    End Using
                Case AuditStoreMode.JSON
                    Dim logDir As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AuditLogs", Environment.UserName)

                    If AuditSettings.Path <> logDir Then
                        logDir = Path.Combine(AuditSettings.Path, Environment.UserName)
                    End If

                    If Not Directory.Exists(logDir) Then Directory.CreateDirectory(logDir)

                    Dim fullPath = Path.Combine(logDir, AuditSettings.FileName)

                    Dim lstAudit As List(Of AuditTable)

                    If IO.File.Exists(fullPath) = False Then
                        lstAudit = New List(Of AuditTable)
                    Else
                        lstAudit = Json.JsonSerializer.Deserialize(Of List(Of AuditTable))(IO.File.ReadAllText(fullPath, Encoding.UTF8))
                    End If

                    lstAudit.Add(audit)

                    Dim resultString As String = Json.JsonSerializer.Serialize(lstAudit, jsonOptions)

                    IO.File.WriteAllText(fullPath, resultString)

            End Select

        End Sub

        Private Function GetCurrentUserName() As String
            Return $"{Environment.UserDomainName}\{Environment.UserName}"
        End Function

    End Class
End Namespace