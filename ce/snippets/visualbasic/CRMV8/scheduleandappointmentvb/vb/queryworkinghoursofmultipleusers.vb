' <snippetqueryworkinghoursofmultipleusers>


Imports System.ServiceModel
Imports System.ServiceModel.Description

' These namespaces are found in the Microsoft.Xrm.Sdk.dll assembly
' found in the SDK\bin folder.
Imports Microsoft.Xrm.Sdk
Imports Microsoft.Xrm.Sdk.Client
Imports Microsoft.Xrm.Sdk.Query
Imports Microsoft.Xrm.Sdk.Discovery
Imports Microsoft.Xrm.Sdk.Messages

' This namespace is found in Microsoft.Crm.Sdk.Proxy.dll assembly
' found in the SDK\bin folder.
Imports Microsoft.Crm.Sdk.Messages

Namespace Microsoft.Crm.Sdk.Samples
	''' <summary>
    ''' This sample shows how to retrieve the working hours of multiple users.
	''' </summary>
    Public Class QueryWorkingHoursOfMultipleUsers
#Region "Class Level Members"

        ''' <summary>
        ''' Stores the organization service proxy.
        ''' </summary>
        Private _serviceProxy As OrganizationServiceProxy

        ' Define the IDs needed for this sample.
        Private _currentUserId As Guid
        Private _otherUserId As Guid

#End Region ' Class Level Members

#Region "How To Sample Code"
        ''' <summary>
        ''' Create and configure the organization service proxy.
        ''' Retrieve the working hours of multiple users.        
        ''' Optionally delete any entity records that were created for this sample.
        ''' </summary>
        ''' <param name="serverConfig">Contains server connection information.</param>        
        ''' <param name="promptforDelete">When True, the user will be prompted to delete all
        ''' created entities.</param>
        Public Sub Run(ByVal serverConfig As ServerConnection.Configuration,
                       ByVal promptForDelete As Boolean)
            Try

                ' Connect to the Organization service. 
                ' The using statement assures that the service proxy will be properly disposed.
                _serviceProxy = ServerConnection.GetOrganizationProxy(serverConfig)
                Using _serviceProxy
                    ' This statement is required to enable early-bound type support.
                    _serviceProxy.EnableProxyTypes()

                    ' Call the method to create any data that this sample requires.
                    CreateRequiredRecords()


                    ' Retrieve the working hours of the current and the other user.                                              
                    Dim scheduleRequest As New QueryMultipleSchedulesRequest()
                    scheduleRequest.ResourceIds = New Guid(1) {}
                    scheduleRequest.ResourceIds(0) = _currentUserId
                    scheduleRequest.ResourceIds(1) = _otherUserId
                    scheduleRequest.Start = Date.Now
                    scheduleRequest.End = Date.Today.AddDays(7)
                    scheduleRequest.TimeCodes = New TimeCode() {TimeCode.Available}

                    Dim scheduleResponse As QueryMultipleSchedulesResponse =
                        CType(_serviceProxy.Execute(scheduleRequest), 
                            QueryMultipleSchedulesResponse)

                    ' Verify if some data is returned for the availability of the users
                    If scheduleResponse.TimeInfos.Length > 0 Then
                        Console.WriteLine(
                            "Successfully queried the working hours of multiple users.")
                    End If
                End Using
                ' Catch any service fault exceptions that Microsoft Dynamics CRM throws.
            Catch fe As FaultException(Of Microsoft.Xrm.Sdk.OrganizationServiceFault)
                ' You can handle an exception here or pass it back to the calling method.
                Throw
            End Try
        End Sub

        ''' <summary>
        ''' This method creates any entity records that this sample requires.        
        ''' </summary>
        Public Sub CreateRequiredRecords()
            ' Get the current user's information.
            Dim userRequest As New WhoAmIRequest()
            Dim userResponse As WhoAmIResponse =
                CType(_serviceProxy.Execute(userRequest), WhoAmIResponse)
            _currentUserId = userResponse.UserId

            ' Create another user, Kevin Cook.
            _otherUserId = SystemUserProvider.RetrieveSalesManager(_serviceProxy)
        End Sub

#End Region ' How To Sample Code

#Region "Main"
        ''' <summary>
        ''' Standard Main() method used by most SDK samples.
        ''' </summary>
        ''' <param name="args"></param>
        Public Shared Sub Main(ByVal args() As String)
            Try
                ' Obtain the target organization's Web address and client logon 
                ' credentials from the user.
                Dim serverConnect As New ServerConnection()
                Dim config As ServerConnection.Configuration = serverConnect.GetServerConfiguration()

                Dim app As New QueryWorkingHoursOfMultipleUsers()
                app.Run(config, True)

            Catch ex As FaultException(Of Microsoft.Xrm.Sdk.OrganizationServiceFault)
                Console.WriteLine("The application terminated with an error.")
                Console.WriteLine("Timestamp: {0}", ex.Detail.Timestamp)
                Console.WriteLine("Code: {0}", ex.Detail.ErrorCode)
                Console.WriteLine("Message: {0}", ex.Detail.Message)
                Console.WriteLine("Plugin Trace: {0}", ex.Detail.TraceText)
                Console.WriteLine("Inner Fault: {0}",
                                  If(Nothing Is ex.Detail.InnerFault, "No Inner Fault", "Has Inner Fault"))
            Catch ex As TimeoutException
                Console.WriteLine("The application terminated with an error.")
                Console.WriteLine("Message: {0}", ex.Message)
                Console.WriteLine("Stack Trace: {0}", ex.StackTrace)
                Console.WriteLine("Inner Fault: {0}",
                                  If(Nothing Is ex.InnerException.Message, "No Inner Fault", ex.InnerException.Message))
            Catch ex As Exception
                Console.WriteLine("The application terminated with an error.")
                Console.WriteLine(ex.Message)

                ' Display the details of the inner exception.
                If ex.InnerException IsNot Nothing Then
                    Console.WriteLine(ex.InnerException.Message)

                    Dim fe As FaultException(Of Microsoft.Xrm.Sdk.OrganizationServiceFault) =
                        TryCast(ex.InnerException, 
                            FaultException(Of Microsoft.Xrm.Sdk.OrganizationServiceFault))
                    If fe IsNot Nothing Then
                        Console.WriteLine("Timestamp: {0}", fe.Detail.Timestamp)
                        Console.WriteLine("Code: {0}", fe.Detail.ErrorCode)
                        Console.WriteLine("Message: {0}", fe.Detail.Message)
                        Console.WriteLine("Plugin Trace: {0}", fe.Detail.TraceText)
                        Console.WriteLine("Inner Fault: {0}",
                                          If(Nothing Is fe.Detail.InnerFault, "No Inner Fault", "Has Inner Fault"))
                    End If
                End If
                ' Additional exceptions to catch: SecurityTokenValidationException, ExpiredSecurityTokenException,
                ' SecurityAccessDeniedException, MessageSecurityException, and SecurityNegotiationException.

            Finally

                Console.WriteLine("Press <Enter> to exit.")
                Console.ReadLine()
            End Try

        End Sub
#End Region ' Main
    End Class
End Namespace

' </snippetqueryworkinghoursofmultipleusers>