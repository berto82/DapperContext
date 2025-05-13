Imports System.Data
Imports System.Reflection
Imports Dapper
Imports Dapper.Contrib.Extensions
Imports Microsoft.Data.SqlClient
Imports Microsoft.Extensions.Configuration

Public Class DapperContext
    Implements IDisposable

    ''' <summary>
    ''' Create a new instance of DapperContext with the default connection string.
    ''' </summary>
    ''' <remarks></remarks>
    ''' <summary>
    ''' Create a new instance of DapperContext with the default connection string.
    ''' </summary>
    Public Sub New()
        Dim cnString As String = New ConfigurationBuilder().SetBasePath(IO.Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").
            Build.GetConnectionString("DefaultConnection")

        Me.Connection = New SqlConnection
        Me.Connection.ConnectionString = cnString
        Me.Connection.Open()
    End Sub

    ''' <summary>
    ''' Create a new instance of DapperContext with the specified connection string name.
    ''' </summary>
    ''' <param name="name"></param>
    ''' <remarks></remarks>
    ''' <summary>
    ''' Create a new instance of DapperContext with the specified connection string name.
    ''' </summary>
    ''' <param name="name"></param>
    Public Sub New(name As String)
        Dim cnString As String = New ConfigurationBuilder().SetBasePath(IO.Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").
            Build.GetConnectionString(name)

        Me.Connection = New SqlConnection
        Me.Connection.ConnectionString = cnString
        Me.Connection.Open()
    End Sub

    Public Property Connection As IDbConnection

    ''' <summary>
    ''' Get a single entity by its ID.
    ''' </summary>
    ''' <typeparam name="TEntity"></typeparam>
    ''' <param name="id"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' <summary>
    ''' Get a single entity by its ID.
    ''' </summary>
    ''' <typeparam name="TEntity"></typeparam>
    ''' <param name="id"></param>
    ''' <returns></returns>
    Public Overridable Function [Get](Of TEntity As Class)(id As Object) As TEntity

        Dim result As TEntity

        Using transaction As IDbTransaction = Me.Connection.BeginTransaction

            Try
                result = Me.Connection.Get(Of TEntity)(id, transaction)
                transaction.Commit()
            Catch ex As Exception
                transaction.Rollback()
                Throw
            End Try

        End Using

        Return result

    End Function

    ''' <summary>
    ''' Get a single entity by its ID.
    ''' </summary>
    ''' <typeparam name="TEntity"></typeparam>
    ''' <param name="id"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' <summary>
    ''' Get a single entity by its ID.
    ''' </summary>
    ''' <typeparam name="TEntity"></typeparam>
    ''' <param name="id"></param>
    ''' <returns></returns>
    Public Overridable Async Function GetAsync(Of TEntity As Class)(id As Object) As Task(Of TEntity)

        Dim result As TEntity

        Using transaction As IDbTransaction = Me.Connection.BeginTransaction

            Try
                result = Await Me.Connection.GetAsync(Of TEntity)(id, transaction)
                transaction.Commit()
            Catch ex As Exception
                transaction.Rollback()
                Throw
            End Try

        End Using

        Return result

    End Function

    ''' <summary>
    ''' Get all entities of a specific type.
    ''' </summary>
    ''' <typeparam name="TEntity"></typeparam>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' <summary>
    ''' Get all entities of a specific type.
    ''' </summary>
    ''' <typeparam name="TEntity"></typeparam>
    ''' <returns></returns>
    Public Overridable Function GetAll(Of TEntity As Class)() As IEnumerable(Of TEntity)

        Dim result As IEnumerable(Of TEntity)

        Using transaction As IDbTransaction = Me.Connection.BeginTransaction

            Try
                result = Me.Connection.GetAll(Of TEntity)(transaction)
                transaction.Commit()
            Catch ex As Exception
                transaction.Rollback()
                Throw
            End Try

        End Using

        Return result

    End Function

    ''' <summary>
    ''' Get all entities of a specific type.
    ''' </summary>
    ''' <typeparam name="TEntity"></typeparam>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' <summary>
    ''' Get all entities of a specific type.
    ''' </summary>
    ''' <typeparam name="TEntity"></typeparam>
    ''' <returns></returns>
    Public Overridable Async Function GetAllAsync(Of TEntity As Class)() As Task(Of IEnumerable(Of TEntity))

        Dim result As IEnumerable(Of TEntity)

        Using transaction As IDbTransaction = Me.Connection.BeginTransaction

            Try
                result = Await Me.Connection.GetAllAsync(Of TEntity)(transaction)
                transaction.Commit()
            Catch ex As Exception
                transaction.Rollback()
                Throw
            End Try

        End Using

        Return result

    End Function

    ''' <summary>
    ''' Insert or update an entity.
    ''' </summary>
    ''' <typeparam name="TEntity"></typeparam>
    ''' <param name="entity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' <summary>
    ''' Insert or update an entity.
    ''' </summary>
    ''' <typeparam name="TEntity"></typeparam>
    ''' <param name="entity"></param>
    ''' <returns></returns>
    Public Overridable Function InsertOrUpdate(Of TEntity As Class)(entity As TEntity) As Long

        Dim result As Long = Nothing

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

        Return result

    End Function

    ''' <summary>
    ''' Insert or update an entity.
    ''' </summary>
    ''' <typeparam name="TEntity"></typeparam>
    ''' <param name="entity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' <summary>
    ''' Insert or update an entity.
    ''' </summary>
    ''' <typeparam name="TEntity"></typeparam>
    ''' <param name="entity"></param>
    ''' <returns></returns>
    Public Overridable Async Function InsertOrUpdateAsync(Of TEntity As Class)(entity As TEntity) As Task(Of Long)

        Dim result As Long = Nothing

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

        Return result

    End Function

    ''' <summary>
    ''' Delete an entity.
    ''' </summary>
    ''' <typeparam name="TEntity"></typeparam>
    ''' <param name="entity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' <summary>
    ''' Delete an entity.
    ''' </summary>
    ''' <typeparam name="TEntity"></typeparam>
    ''' <param name="entity"></param>
    ''' <returns></returns>
    Public Overridable Function Delete(Of TEntity As Class)(entity As TEntity) As Boolean
        Dim result As Boolean

        Using transaction As IDbTransaction = Me.Connection.BeginTransaction

            Try
                result = Me.Connection.Delete(entity, transaction)
                transaction.Commit()
            Catch ex As Exception
                transaction.Rollback()
                Throw
            End Try

        End Using

        Return result

    End Function

    ''' <summary>
    ''' Delete an entity.
    ''' </summary>
    ''' <typeparam name="TEntity"></typeparam>
    ''' <param name="entity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' <summary>
    ''' Delete an entity.
    ''' </summary>
    ''' <typeparam name="TEntity"></typeparam>
    ''' <param name="entity"></param>
    ''' <returns></returns>
    Public Overridable Async Function DeleteAsync(Of TEntity As Class)(entity As TEntity) As Task(Of Boolean)
        Dim result As Boolean

        Using transaction As IDbTransaction = Me.Connection.BeginTransaction

            Try
                result = Await Me.Connection.DeleteAsync(entity, transaction)
                transaction.Commit()
            Catch ex As Exception
                transaction.Rollback()
                Throw
            End Try

        End Using

        Return result

    End Function

    ''' <summary>
    ''' Delete all entities of a specific type.
    ''' </summary>
    ''' <typeparam name="TEntity"></typeparam>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' <summary>
    ''' Delete all entities of a specific type.
    ''' </summary>
    ''' <typeparam name="TEntity"></typeparam>
    ''' <returns></returns>
    ''' </summary>
    ''' <typeparam name="TEntity"></typeparam>
    ''' <returns></returns>
    Public Overridable Function DeleteAll(Of TEntity As Class)() As Boolean

        Dim result As Boolean

        Using transaction As IDbTransaction = Me.Connection.BeginTransaction
            Try
                result = Me.Connection.DeleteAll(Of TEntity)(transaction)
                transaction.Commit()
            Catch ex As Exception
                transaction.Rollback()
                Throw
            End Try
        End Using

        Return result

    End Function

    ''' <summary>
    ''' Delete all entities of a specific type.
    ''' </summary>
    ''' <typeparam name="TEntity"></typeparam>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' <summary>
    ''' Delete all entities of a specific type.
    ''' </summary>
    ''' <typeparam name="TEntity"></typeparam>
    ''' <returns></returns>
    ''' </summary>
    ''' <typeparam name="TEntity"></typeparam>
    ''' <returns></returns>
    Public Overridable Async Function DeleteAllAsync(Of TEntity As Class)() As Task(Of Boolean)

        Dim result As Boolean

        Using transaction As IDbTransaction = Me.Connection.BeginTransaction
            Try
                result = Await Me.Connection.DeleteAllAsync(Of TEntity)(transaction)
                transaction.Commit()
            Catch ex As Exception
                transaction.Rollback()
                Throw
            End Try
        End Using

        Return result

    End Function

    ''' <summary>
    ''' Execute a SQL command.
    ''' </summary>
    ''' <param name="sql"></param>
    ''' <param name="param"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' <summary>
    ''' Execute a SQL command.
    ''' </summary>
    ''' <param name="sql"></param>
    ''' <param name="param"></param>
    ''' <returns></returns>
    Public Function Execute(sql As String, Optional param As Object = Nothing) As Integer
        Dim result As Integer

        Using transaction As IDbTransaction = Me.Connection.BeginTransaction
            Try
                result = Me.Connection.Execute(sql, param, transaction)
                transaction.Commit()
            Catch ex As Exception
                transaction.Rollback()
                Throw
            End Try
        End Using

        Return result

    End Function

    ''' <summary>
    ''' Execute a SQL command.
    ''' </summary>
    ''' <param name="sql"></param>
    ''' <param name="param"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' <summary>
    ''' Execute a SQL command.
    ''' </summary>
    ''' <param name="sql"></param>
    ''' <param name="param"></param>
    ''' <returns></returns>
    Public Async Function ExecuteAsync(sql As String, Optional param As Object = Nothing) As Task(Of Integer)
        Dim result As Integer

        Using transaction As IDbTransaction = Me.Connection.BeginTransaction
            Try
                result = Await Me.Connection.ExecuteAsync(sql, param, transaction)
                transaction.Commit()
            Catch ex As Exception
                transaction.Rollback()
                Throw
            End Try
        End Using

        Return result

    End Function

    ''' <summary>
    ''' Execute a SQL command and return a single value.
    ''' </summary>
    ''' <param name="sql"></param>
    ''' <param name="param"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' <summary>
    ''' Execute a SQL command and return a single value.
    ''' </summary>
    ''' <param name="sql"></param>
    ''' <param name="param"></param>
    ''' <returns></returns>
    Public Function ExecuteScalar(sql As String, Optional param As Object = Nothing) As Object
        Dim result As Object

        Using transaction As IDbTransaction = Me.Connection.BeginTransaction
            Try
                result = Me.Connection.ExecuteScalar(sql, param, transaction)
                transaction.Commit()
            Catch ex As Exception
                transaction.Rollback()
                Throw
            End Try
        End Using

        Return result

    End Function

    ''' <summary>
    ''' Execute a SQL command and return a single value.
    ''' </summary>
    ''' <param name="sql"></param>
    ''' <param name="param"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' <summary>
    ''' Execute a SQL command and return a single value.
    ''' </summary>
    ''' <param name="sql"></param>
    ''' <param name="param"></param>
    ''' <returns></returns>
    Public Async Function ExecuteScalarAsync(sql As String, Optional param As Object = Nothing) As Task(Of Object)
        Dim result As Object

        Using transaction As IDbTransaction = Me.Connection.BeginTransaction
            Try
                result = Await Me.Connection.ExecuteScalarAsync(sql, param, transaction)
                transaction.Commit()
            Catch ex As Exception
                transaction.Rollback()
                Throw
            End Try
        End Using

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
    ''' <summary>
    ''' Execute a SQL command and return a single value.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="sql"></param>
    ''' <param name="param"></param>
    ''' <returns></returns>
    Public Function ExecuteScalar(Of T)(sql As String, Optional param As Object = Nothing) As T

        Dim result As T

        Using transaction As IDbTransaction = Me.Connection.BeginTransaction
            Try
                result = Me.Connection.ExecuteScalar(Of T)(sql, param, transaction)
                transaction.Commit()
            Catch ex As Exception
                transaction.Rollback()
                Throw
            End Try
        End Using

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
    ''' <summary>
    ''' Execute a SQL command and return a single value.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="sql"></param>
    ''' <param name="param"></param>
    ''' <returns></returns>
    Public Async Function ExecuteScalarAsync(Of T)(sql As String, Optional param As Object = Nothing) As Task(Of T)

        Dim result As T

        Using transaction As IDbTransaction = Me.Connection.BeginTransaction
            Try
                result = Await Me.Connection.ExecuteScalarAsync(Of T)(sql, param, transaction)
                transaction.Commit()
            Catch ex As Exception
                transaction.Rollback()
                Throw
            End Try
        End Using

        Return result

    End Function

    ''' <summary>
    ''' Execute a SQL command and return a collection of objects.
    ''' </summary>
    ''' <param name="sql"></param>
    ''' <param name="param"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' <summary>
    ''' Execute a SQL command and return a collection of objects.
    ''' </summary>
    ''' <param name="sql"></param>
    ''' <param name="param"></param>
    ''' <returns></returns>
    Public Function Query(sql As String, Optional param As Object = Nothing) As IEnumerable(Of Object)

        Dim result As IEnumerable(Of Object)

        Using transaction As IDbTransaction = Me.Connection.BeginTransaction
            Try
                result = Me.Connection.Query(sql, param, transaction)
                transaction.Commit()
            Catch ex As Exception
                transaction.Rollback()
                Throw
            End Try
        End Using

        Return result

    End Function

    ''' <summary>
    ''' Execute a SQL command and return a collection of objects.
    ''' </summary>
    ''' <param name="sql"></param>
    ''' <param name="param"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' <summary>
    ''' Execute a SQL command and return a collection of objects.
    ''' </summary>
    ''' <param name="sql"></param>
    ''' <param name="param"></param>
    ''' <returns></returns>
    Public Async Function QueryAsync(sql As String, Optional param As Object = Nothing) As Task(Of IEnumerable(Of Object))

        Dim result As IEnumerable(Of Object)

        Using transaction As IDbTransaction = Me.Connection.BeginTransaction
            Try
                result = Await Me.Connection.QueryAsync(sql, param, transaction)
                transaction.Commit()
            Catch ex As Exception
                transaction.Rollback()
                Throw
            End Try
        End Using

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
    ''' <summary>
    ''' Execute a SQL command and return a collection of objects.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="sql"></param>
    ''' <param name="param"></param>
    ''' <returns></returns>
    Public Function Query(Of T As Class)(sql As String, Optional param As Object = Nothing) As IEnumerable(Of T)

        Dim result As IEnumerable(Of T)

        Using transaction As IDbTransaction = Me.Connection.BeginTransaction
            Try
                result = Me.Connection.Query(Of T)(sql, param, transaction)
                transaction.Commit()
            Catch ex As Exception
                transaction.Rollback()
                Throw
            End Try
        End Using

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
    ''' <summary>
    ''' Execute a SQL command and return a collection of objects.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="sql"></param>
    ''' <param name="param"></param>
    ''' <returns></returns>
    Public Async Function QueryAsync(Of T As Class)(sql As String, Optional param As Object = Nothing) As Task(Of IEnumerable(Of T))

        Dim result As IEnumerable(Of T)

        Using transaction As IDbTransaction = Me.Connection.BeginTransaction
            Try
                result = Await Me.Connection.QueryAsync(Of T)(sql, param, transaction)
                transaction.Commit()
            Catch ex As Exception
                transaction.Rollback()
                Throw
            End Try
        End Using

        Return result

    End Function

    ''' <summary>
    ''' Get the value of the key field of an entity.
    ''' </summary>
    ''' <typeparam name="TEntity"></typeparam>
    ''' <param name="entity"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ''' <summary>
    ''' Get the value of the key field of an entity.
    ''' </summary>
    ''' <typeparam name="TEntity"></typeparam>
    ''' <param name="entity"></param>
    ''' <returns></returns>
    Public Shared Function GetKeyFieldValue(Of TEntity As Class)(entity As TEntity) As Object

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