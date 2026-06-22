Imports System.Data
Imports System.Runtime.CompilerServices

Namespace Extensions

    Module IDbConnectionsExtensions

        <Extension>
        Public Function OpenConnection(connection As IDbConnection) As IDbConnection
            If connection.State = ConnectionState.Closed Then
                connection.Open()
            End If

            Return connection
        End Function

        <Extension>
        Public Function CloseConnection(connection As IDbConnection) As IDbConnection
            If connection.State = ConnectionState.Open Then
                connection.Close()
            End If

            Return connection
        End Function

    End Module

End Namespace