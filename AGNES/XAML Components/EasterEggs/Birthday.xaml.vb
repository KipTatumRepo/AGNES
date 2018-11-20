Imports System.Windows.Threading
Public Class Birthday

#Region "Properties"
    Dim dt As DispatcherTimer = New DispatcherTimer()
    Dim dt2 As DispatcherTimer = New DispatcherTimer()

#End Region

#Region "Constructor"
    Public Sub New()
        InitializeComponent()
        AddHandler dt.Tick, AddressOf CloseDown
        dt.Interval = New TimeSpan(0, 0, 5)
        dt.Start()
    End Sub

#End Region

#Region "Public Methods"
    Public Sub CloseDown(ByVal sender As Object, ByVal e As EventArgs)
        CommandManager.InvalidateRequerySuggested()
        dt.Stop()
        Close()
    End Sub

#End Region

End Class
