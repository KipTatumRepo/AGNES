Imports AGNES

Public Class MoveableElement

#Region "Properties"
    Private _inputElement As IInputElement = Nothing
    Public Property InputElement() As IInputElement
        Get
            Return Me._inputElement
        End Get

        Set(ByVal value As IInputElement)
            Me._inputElement = value
        End Set
    End Property

    Private _x As Double
    Public Property X() As Double
        Get
            Return Me._x
        End Get

        Set(ByVal value As Double)
            Me._x = value
        End Set
    End Property

    Private _y As Double = 0
    Public Property Y() As Double
        Get
            Return Me._y
        End Get

        Set(ByVal value As Double)
            Me._y = value
        End Set
    End Property

    Private _isDragging As Boolean = False
    Public Property IsDragging() As Boolean
        Get
            Return Me._isDragging
        End Get

        Set(ByVal value As Boolean)
            Me._isDragging = value
        End Set
    End Property

#End Region

#Region "Constructor"
    Public Sub New()
    End Sub

#End Region


End Class
