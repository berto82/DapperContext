Imports System.Data
Imports System.Reflection
Imports System.Xml
Imports Dapper
Imports Dapper.Contrib.Extensions
Imports Microsoft.Extensions.Configuration

#Region "DapperContext"

''' <summary>
''' DapperContext class for managing database connections and operations.
''' </summary>
''' <remarks></remarks>
Public MustInherit Class DapperContext
    Implements IDisposable

    ''' <summary>
    ''' A fluent configuration setting about this context. You have should use directly the method <c>ContextConfiguration.CreateNew()</c> to create a new settings, otherwhere will be used a default settings.
    ''' Remember to terminate with <c>Build()</c> to generate settings
    ''' </summary>
    ''' <returns></returns>
    Public Shared Property Settings As ContextConfiguration

    ''' <summary>
    ''' A connection database settings
    ''' </summary>
    ''' <returns></returns>
    Public Property Connection As IDbConnection

    Protected Friend Sub New()
        If _Settings Is Nothing Then
            _Settings = ContextConfiguration.CreateNew.Build
        End If
    End Sub

    Protected Friend Function GetConnectionString() As String

        Dim cnString As String = Nothing

        Try

            Select Case _Settings.SettingsMode
                Case SettingFileMode.NET4x
                    Dim doc As New XmlDocument

                    If _Settings.CustomConfigurationFile = String.Empty Then
                        doc.Load($"{IO.Directory.GetCurrentDirectory}\{Assembly.GetEntryAssembly.GetName.Name}.exe.config")
                    Else
                        doc.Load($"{IO.Directory.GetCurrentDirectory}\{_Settings.CustomConfigurationFile}")
                    End If

                    Dim cnStringNode As XmlNode = doc.SelectSingleNode("//connectionStrings")
                    Dim configSourceAttribute As XmlAttribute = cnStringNode.Attributes("configSource")

                    If configSourceAttribute Is Nothing Then
                        cnStringNode = cnStringNode.SelectSingleNode($"add[@name=""{_Settings.ConnectionName}""]/@connectionString")
                    Else
                        doc.Load($"{IO.Directory.GetCurrentDirectory}\{configSourceAttribute.Value}")
                        cnStringNode = doc.SelectSingleNode($"//connectionStrings/add[@name=""{_Settings.ConnectionName}""]/@connectionString")
                    End If

                    cnString = cnStringNode?.Value
                Case SettingFileMode.NETCore

                    If _Settings.CustomConfigurationFile = String.Empty Then
                        cnString = New ConfigurationBuilder().SetBasePath(IO.Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build.GetConnectionString(_Settings.ConnectionName)
                    Else
                        cnString = New ConfigurationBuilder().SetBasePath(IO.Directory.GetCurrentDirectory()).AddJsonFile(_Settings.CustomConfigurationFile).Build.GetConnectionString(_Settings.ConnectionName)
                    End If

            End Select
        Catch ex As Exception
            Throw
        End Try

        Return cnString

    End Function

    ''' <summary>
    ''' Check if the database exists
    ''' </summary>
    ''' <returns></returns>
    Public Function DatabaseExist() As Boolean
        Return DatabaseExist(Me.Connection.Database)
    End Function

    ''' <summary>
    '''  Check if the database exists
    ''' </summary>
    ''' <param name="dbName">A name of database to be check</param>
    ''' <returns></returns>
    Public Overridable Function DatabaseExist(dbName As String) As Boolean
        Return False
    End Function

    ''' <summary>
    ''' Get a single entity by its ID.
    ''' </summary>
    ''' <typeparam name="TEntity"></typeparam>
    ''' <param name="id"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overridable Function [Get](Of TEntity As Class)(id As Object) As TEntity

        Dim result As TEntity

        If _Settings.EnableTransaction = True Then
            Using transaction As IDbTransaction = Me.Connection.BeginTransaction
                Try
                    result = Me.Connection.Get(Of TEntity)(id, transaction)
                    transaction.Commit()
                Catch ex As Exception
                    transaction.Rollback()
                    Throw
                End Try

            End Using
        Else
            Try
                result = Me.Connection.Get(Of TEntity)(id)
            Catch ex As Exception
                Throw
            End Try
        End If


        Return result

    End Function

    ''' <summary>
    ''' Get a single entity by its ID.
    ''' </summary>
    ''' <typeparam name="TEntity"></typeparam>
    ''' <param name="id"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overridable Async Function GetAsync(Of TEntity As Class)(id As Object) As Task(Of TEntity)

        Dim result As TEntity

        If _Settings.EnableTransaction = True Then
            Using transaction As IDbTransaction = Me.Connection.BeginTransaction

                Try
                    result = Await Me.Connection.GetAsync(Of TEntity)(id, transaction)
                    transaction.Commit()
                Catch ex As Exception
                    transaction.Rollback()
                    Throw
                End Try

            End Using
        Else
            Try
                result = Await Me.Connection.GetAsync(Of TEntity)(id)
            Catch ex As Exception
                Throw
            End Try
        End If


        Return result

    End Function

    ''' <summary>
    ''' Get all entities of a specific type.
    ''' </summary>
    ''' <typeparam name="TEntity"></typeparam>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overridable Function GetAll(Of TEntity As Class)() As IEnumerable(Of TEntity)

        Dim result As IEnumerable(Of TEntity)

        If _Settings.EnableTransaction = True Then
            Using transaction As IDbTransaction = Me.Connection.BeginTransaction

                Try
                    result = Me.Connection.GetAll(Of TEntity)(transaction)
                    transaction.Commit()
                Catch ex As Exception
                    transaction.Rollback()
                    Throw
                End Try

            End Using
        Else
            Try
                result = Me.Connection.GetAll(Of TEntity)
            Catch ex As Exception
                Throw
            End Try
        End If

        Return result

    End Function

    ''' <summary>
    ''' Get all entities of a specific type.
    ''' </summary>
    ''' <typeparam name="TEntity"></typeparam>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overridable Async Function GetAllAsync(Of TEntity As Class)() As Task(Of IEnumerable(Of TEntity))

        Dim result As IEnumerable(Of TEntity)

        If _Settings.EnableTransaction = True Then
            Using transaction As IDbTransaction = Me.Connection.BeginTransaction

                Try
                    result = Await Me.Connection.GetAllAsync(Of TEntity)(transaction)
                    transaction.Commit()
                Catch ex As Exception
                    transaction.Rollback()
                    Throw
                End Try

            End Using
        Else
            Try
                result = Await Me.Connection.GetAllAsync(Of TEntity)
            Catch ex As Exception
                Throw
            End Try
        End If


        Return result

    End Function

    ''' <summary>
    ''' Insert or update an entity.
    ''' </summary>
    ''' <typeparam name="TEntity"></typeparam>
    ''' <param name="entity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overridable Function InsertOrUpdate(Of TEntity As Class)(entity As TEntity) As Long

        Dim result As Long = Nothing

        If _Settings.EnableTransaction = True Then
            Using transaction As IDbTransaction = Me.Connection.BeginTransaction

                Try
                    Dim isNew As Boolean = False

                    Dim entityType As Type = entity.GetType
                    Dim keyValue As Object = DapperContext.GetKeyFieldValue(entity)

                    If CInt(keyValue) = 0 Then
                        isNew = True
                    End If

                    If isNew = True Then
                        result = Me.Connection.Insert(entity, transaction)
                    Else
                        If Me.Connection.Update(entity, transaction) Then
                            result = CLng(keyValue)
                        End If
                    End If

                    transaction.Commit()

                Catch ex As Exception
                    transaction.Rollback()
                    Throw
                End Try

            End Using
        Else
            Try
                Dim isNew As Boolean = False

                Dim entityType As Type = entity.GetType
                Dim keyValue As Object = DapperContext.GetKeyFieldValue(entity)

                If CInt(keyValue) = 0 Then
                    isNew = True
                End If

                If isNew = True Then
                    result = Me.Connection.Insert(entity)
                Else
                    If Me.Connection.Update(entity) Then
                        result = CLng(keyValue)
                    End If
                End If
            Catch ex As Exception
                Throw
            End Try
        End If


        Return result

    End Function

    ''' <summary>
    ''' Insert or update an entity.
    ''' </summary>
    ''' <typeparam name="TEntity"></typeparam>
    ''' <param name="entity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overridable Async Function InsertOrUpdateAsync(Of TEntity As Class)(entity As TEntity) As Task(Of Long)

        Dim result As Long = Nothing

        If _Settings.EnableTransaction = True Then
            Using transaction As IDbTransaction = Me.Connection.BeginTransaction

                Try
                    Dim isNew As Boolean = False

                    Dim entityType As Type = entity.GetType
                    Dim keyValue As Object = DapperContext.GetKeyFieldValue(entity)

                    If CInt(keyValue) = 0 Then
                        isNew = True
                    End If

                    If isNew = True Then
                        result = Await Me.Connection.InsertAsync(entity, transaction)
                    Else
                        If Await Me.Connection.UpdateAsync(entity, transaction) Then
                            result = CLng(keyValue)
                        End If
                    End If

                    transaction.Commit()

                Catch ex As Exception
                    transaction.Rollback()
                    Throw
                End Try

            End Using
        Else
            Try
                Dim isNew As Boolean = False

                Dim entityType As Type = entity.GetType
                Dim keyValue As Object = DapperContext.GetKeyFieldValue(entity)

                If CInt(keyValue) = 0 Then
                    isNew = True
                End If

                If isNew = True Then
                    result = Await Me.Connection.InsertAsync(entity)
                Else
                    If Await Me.Connection.UpdateAsync(entity) Then
                        result = CLng(keyValue)
                    End If
                End If
            Catch ex As Exception
                Throw
            End Try
        End If


        Return result

    End Function

    ''' <summary>
    ''' Delete an entity.
    ''' </summary>
    ''' <typeparam name="TEntity"></typeparam>
    ''' <param name="entity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overridable Function Delete(Of TEntity As Class)(entity As TEntity) As Boolean
        Dim result As Boolean

        If _Settings.EnableTransaction = True Then
            Using transaction As IDbTransaction = Me.Connection.BeginTransaction
                Try
                    result = Me.Connection.Delete(entity, transaction)
                    transaction.Commit()
                Catch ex As Exception
                    transaction.Rollback()
                    Throw
                End Try
            End Using
        Else
            Try
                result = Me.Connection.Delete(entity)
            Catch ex As Exception
                Throw
            End Try
        End If


        Return result

    End Function

    ''' <summary>
    ''' Delete an entity.
    ''' </summary>
    ''' <typeparam name="TEntity"></typeparam>
    ''' <param name="entity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overridable Async Function DeleteAsync(Of TEntity As Class)(entity As TEntity) As Task(Of Boolean)
        Dim result As Boolean

        If _Settings.EnableTransaction = True Then
            Using transaction As IDbTransaction = Me.Connection.BeginTransaction

                Try
                    result = Await Me.Connection.DeleteAsync(entity, transaction)
                    transaction.Commit()
                Catch ex As Exception
                    transaction.Rollback()
                    Throw
                End Try

            End Using
        Else
            Try
                result = Await Me.Connection.DeleteAsync(entity)
            Catch ex As Exception
                Throw
            End Try
        End If


        Return result

    End Function

    ''' <summary>
    ''' Delete all entities of a specific type.
    ''' </summary>
    ''' <typeparam name="TEntity"></typeparam>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overridable Function DeleteAll(Of TEntity As Class)() As Boolean

        Dim result As Boolean

        If _Settings.EnableTransaction = True Then

            Using transaction As IDbTransaction = Me.Connection.BeginTransaction
                Try
                    result = Me.Connection.DeleteAll(Of TEntity)(transaction)
                    transaction.Commit()
                Catch ex As Exception
                    transaction.Rollback()
                    Throw
                End Try
            End Using
        Else
            Try
                result = Me.Connection.DeleteAll(Of TEntity)
            Catch ex As Exception
                Throw
            End Try
        End If


        Return result

    End Function

    ''' <summary>
    ''' Delete all entities of a specific type.
    ''' </summary>
    ''' <typeparam name="TEntity"></typeparam>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Overridable Async Function DeleteAllAsync(Of TEntity As Class)() As Task(Of Boolean)

        Dim result As Boolean

        If _Settings.EnableTransaction = True Then
            Using transaction As IDbTransaction = Me.Connection.BeginTransaction
                Try
                    result = Await Me.Connection.DeleteAllAsync(Of TEntity)(transaction)
                    transaction.Commit()
                Catch ex As Exception
                    transaction.Rollback()
                    Throw
                End Try
            End Using
        Else
            Try
                result = Await Me.Connection.DeleteAllAsync(Of TEntity)
            Catch ex As Exception
                Throw
            End Try
        End If


        Return result

    End Function

    ''' <summary>
    ''' Execute a SQL command.
    ''' </summary>
    ''' <param name="sql"></param>
    ''' <param name="param"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Execute(sql As String, Optional param As Object = Nothing) As Integer
        Dim result As Integer

        If _Settings.EnableTransaction = True Then
            Using transaction As IDbTransaction = Me.Connection.BeginTransaction
                Try
                    result = Me.Connection.Execute(sql, param, transaction)
                    transaction.Commit()
                Catch ex As Exception
                    transaction.Rollback()
                    Throw
                End Try
            End Using
        Else
            Try
                result = Me.Connection.Execute(sql, param)
            Catch ex As Exception
                Throw
            End Try
        End If


        Return result

    End Function

    ''' <summary>
    ''' Execute a SQL command.
    ''' </summary>
    ''' <param name="sql"></param>
    ''' <param name="param"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Async Function ExecuteAsync(sql As String, Optional param As Object = Nothing) As Task(Of Integer)
        Dim result As Integer

        If _Settings.EnableTransaction = True Then
            Using transaction As IDbTransaction = Me.Connection.BeginTransaction
                Try
                    result = Await Me.Connection.ExecuteAsync(sql, param, transaction)
                    transaction.Commit()
                Catch ex As Exception
                    transaction.Rollback()
                    Throw
                End Try
            End Using
        Else
            Try
                result = Await Me.Connection.ExecuteAsync(sql, param)
            Catch ex As Exception
                Throw
            End Try
        End If


        Return result

    End Function

    ''' <summary>
    ''' Execute a SQL command and return a single value.
    ''' </summary>
    ''' <param name="sql"></param>
    ''' <param name="param"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ExecuteScalar(sql As String, Optional param As Object = Nothing) As Object
        Dim result As Object

        If _Settings.EnableTransaction = True Then
            Using transaction As IDbTransaction = Me.Connection.BeginTransaction
                Try
                    result = Me.Connection.ExecuteScalar(sql, param, transaction)
                    transaction.Commit()
                Catch ex As Exception
                    transaction.Rollback()
                    Throw
                End Try
            End Using
        Else
            Try
                result = Me.Connection.ExecuteScalar(sql, param)
            Catch ex As Exception
                Throw
            End Try
        End If


        Return result

    End Function

    ''' <summary>
    ''' Execute a SQL command and return a single value.
    ''' </summary>
    ''' <param name="sql"></param>
    ''' <param name="param"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Async Function ExecuteScalarAsync(sql As String, Optional param As Object = Nothing) As Task(Of Object)
        Dim result As Object

        If _Settings.EnableTransaction = True Then
            Using transaction As IDbTransaction = Me.Connection.BeginTransaction
                Try
                    result = Await Me.Connection.ExecuteScalarAsync(sql, param, transaction)
                    transaction.Commit()
                Catch ex As Exception
                    transaction.Rollback()
                    Throw
                End Try
            End Using
        Else
            Try
                result = Await Me.Connection.ExecuteScalarAsync(sql, param)
            Catch ex As Exception
                Throw
            End Try
        End If


        Return result

    End Function

    ''' <summary>
    ''' Execute a SQL command and return a single value.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="sql"></param>
    ''' <param name="param"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ExecuteScalar(Of T)(sql As String, Optional param As Object = Nothing) As T

        Dim result As T

        If _Settings.EnableTransaction = True Then
            Using transaction As IDbTransaction = Me.Connection.BeginTransaction
                Try
                    result = Me.Connection.ExecuteScalar(Of T)(sql, param, transaction)
                    transaction.Commit()
                Catch ex As Exception
                    transaction.Rollback()
                    Throw
                End Try
            End Using
        Else
            Try
                result = Me.Connection.ExecuteScalar(Of T)(sql, param)
            Catch ex As Exception
                Throw
            End Try
        End If


        Return result

    End Function

    ''' <summary>
    ''' Execute a SQL command and return a single value.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="sql"></param>
    ''' <param name="param"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Async Function ExecuteScalarAsync(Of T)(sql As String, Optional param As Object = Nothing) As Task(Of T)

        Dim result As T

        If _Settings.EnableTransaction = True Then
            Using transaction As IDbTransaction = Me.Connection.BeginTransaction
                Try
                    result = Await Me.Connection.ExecuteScalarAsync(Of T)(sql, param, transaction)
                    transaction.Commit()
                Catch ex As Exception
                    transaction.Rollback()
                    Throw
                End Try
            End Using
        Else
            Try
                result = Await Me.Connection.ExecuteScalarAsync(Of T)(sql, param)
            Catch ex As Exception
                Throw
            End Try
        End If


        Return result

    End Function

    ''' <summary>
    ''' Execute a SQL command and return a collection of objects.
    ''' </summary>
    ''' <param name="sql"></param>
    ''' <param name="param"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Query(sql As String, Optional param As Object = Nothing) As IEnumerable(Of Object)

        Dim result As IEnumerable(Of Object)


        If _Settings.EnableTransaction = True Then
            Using transaction As IDbTransaction = Me.Connection.BeginTransaction
                Try
                    result = Me.Connection.Query(sql, param, transaction)
                    transaction.Commit()
                Catch ex As Exception
                    transaction.Rollback()
                    Throw
                End Try
            End Using
        Else
            Try
                result = Me.Connection.Query(sql, param)
            Catch ex As Exception
                Throw
            End Try
        End If


        Return result

    End Function

    ''' <summary>
    ''' Execute a SQL command and return a collection of objects.
    ''' </summary>
    ''' <param name="sql"></param>
    ''' <param name="param"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Async Function QueryAsync(sql As String, Optional param As Object = Nothing) As Task(Of IEnumerable(Of Object))

        Dim result As IEnumerable(Of Object)


        If _Settings.EnableTransaction = True Then
            Using transaction As IDbTransaction = Me.Connection.BeginTransaction
                Try
                    result = Await Me.Connection.QueryAsync(sql, param, transaction)
                    transaction.Commit()
                Catch ex As Exception
                    transaction.Rollback()
                    Throw
                End Try
            End Using
        Else
            Try
                result = Await Me.Connection.QueryAsync(sql, param)
            Catch ex As Exception
                Throw
            End Try
        End If


        Return result

    End Function

    ''' <summary>
    ''' Execute a SQL command and return a collection of objects.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="sql"></param>
    ''' <param name="param"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Query(Of T As Class)(sql As String, Optional param As Object = Nothing) As IEnumerable(Of T)

        Dim result As IEnumerable(Of T)


        If _Settings.EnableTransaction = True Then
            Using transaction As IDbTransaction = Me.Connection.BeginTransaction
                Try
                    result = Me.Connection.Query(Of T)(sql, param, transaction)
                    transaction.Commit()
                Catch ex As Exception
                    transaction.Rollback()
                    Throw
                End Try
            End Using
        Else
            Try
                result = Me.Connection.Query(Of T)(sql, param)
            Catch ex As Exception
                Throw
            End Try
        End If

        Return result

    End Function

    ''' <summary>
    ''' Execute a SQL command and return a collection of objects.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="sql"></param>
    ''' <param name="param"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Async Function QueryAsync(Of T As Class)(sql As String, Optional param As Object = Nothing) As Task(Of IEnumerable(Of T))

        Dim result As IEnumerable(Of T)

        If _Settings.EnableTransaction = True Then
            Using transaction As IDbTransaction = Me.Connection.BeginTransaction
                Try
                    result = Await Me.Connection.QueryAsync(Of T)(sql, param, transaction)
                    transaction.Commit()
                Catch ex As Exception
                    transaction.Rollback()
                    Throw
                End Try
            End Using
        Else
            Try
                result = Await Me.Connection.QueryAsync(Of T)(sql, param)
            Catch ex As Exception
                Throw
            End Try
        End If


        Return result

    End Function

    ''' <summary>
    ''' Get the value of the key field of an entity.
    ''' </summary>
    ''' <typeparam name="TEntity"></typeparam>
    ''' <param name="entity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetKeyFieldValue(Of TEntity As Class)(entity As TEntity) As Object

        If entity Is Nothing Then
            Throw New Exception("Entity was not found")
        End If

        Dim entityType As Type = entity.GetType
        Dim keyValue As Object = Nothing

        For Each prop As PropertyInfo In entityType.GetProperties
            Dim keyAttribute As Attribute = prop.GetCustomAttribute(Of KeyAttribute)()

            If keyAttribute IsNot Nothing Then
                keyValue = prop.GetValue(entity)
                Exit For
            End If
        Next

        Return keyValue

    End Function

#Region "Disposable"
    Private disposedValue As Boolean

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: eliminare lo stato gestito (oggetti gestiti)
                Me.Connection.Close()
                Me.Connection.Dispose()
            End If

            ' TODO: liberare risorse non gestite (oggetti non gestiti) ed eseguire l'override del finalizzatore
            ' TODO: impostare campi di grandi dimensioni su Null
            disposedValue = True
        End If
    End Sub

    ' ' TODO: eseguire l'override del finalizzatore solo se 'Dispose(disposing As Boolean)' contiene codice per liberare risorse non gestite
    ' Protected Overrides Sub Finalize()
    '     ' Non modificare questo codice. Inserire il codice di pulizia nel metodo 'Dispose(disposing As Boolean)'
    '     Dispose(disposing:=False)
    '     MyBase.Finalize()
    ' End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' Non modificare questo codice. Inserire il codice di pulizia nel metodo 'Dispose(disposing As Boolean)'
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
End Class
#End Region

#End Region