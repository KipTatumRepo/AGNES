Public Class Chef
    Inherits Image

#Region "Properties"
    Private _xpos As Double
    Public Property XPos As Double
        Get
            Return _xpos
        End Get
        Set(value As Double)
            _xpos = value
            SetLocation()
        End Set
    End Property

    Private _ypos As Double
    Public Property YPos As Double
        Get
            Return _ypos
        End Get
        Set(value As Double)
            _ypos = value
            SetLocation()
        End Set
    End Property

    Public Direction As Byte
    Public Feets As Byte

    Public CanvasTop As Double
    Public CanvasBottom As Double
    Public CanvasLeft As Double
    Public CanvasRight As Double

    Private ParentObj As Grid


#End Region

#Region "Constructor"
    Public Sub New(po As Grid)
        VerticalAlignment = VerticalAlignment.Top
        HorizontalAlignment = HorizontalAlignment.Left
        Source = New BitmapImage(New Uri("pack://application:,,,/Resources/RightChef1.png"))
        Margin = New Thickness(XPos, YPos, 0, 0)
    End Sub

#End Region

#Region "Public Methods"

#End Region

#Region "Private Methods"
    Private Sub SetLocation()
        Select Case Direction
            Case 3  'Move right
                Select Case ChefFeets
                    Case 0
                        Source = New BitmapImage(New Uri("pack://application:,,,/Resources/RightChef1.png"))
                    Case 1
                        Source = New BitmapImage(New Uri("pack://application:,,,/Resources/RightChef2.png"))
                    Case 2
                        Source = New BitmapImage(New Uri("pack://application:,,,/Resources/RightChef3.png"))
                End Select

            Case 7  ' Move left

                Select Case ChefFeets
                    Case 0
                        Source = New BitmapImage(New Uri("pack://application:,,,/Resources/LeftChef1.png"))
                    Case 1
                        Source = New BitmapImage(New Uri("pack://application:,,,/Resources/LeftChef2.png"))
                    Case 2
                        Source = New BitmapImage(New Uri("pack://application:,,,/Resources/LeftChef3.png"))
                End Select
        End Select
        ChefFeets += 1
        If ChefFeets > 2 Then ChefFeets = 0

        Margin = New Thickness(XPos, YPos, 0, 0)
    End Sub


#End Region

End Class
