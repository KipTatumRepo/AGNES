Public Class AgnesBaseInput
    Inherits Grid
    Public Enum ShowFlare
        Show
        Hide
    End Enum
    Private _flare As Boolean
    Public Property Flare As Boolean
        Get
            Return _flare
        End Get
        Set(value As Boolean)
            _flare = value
            If value = False Then
                Me.Children(0).Visibility = Visibility.Hidden
                Me.Children(1).Opacity = 1
            Else
                Me.Children(0).Visibility = Visibility.Visible
                Me.Children(1).Opacity = 0.9
            End If

        End Set
    End Property

    Public Sub New(ByVal v As VerticalAlignment, ByVal h As HorizontalAlignment, Optional ByVal s As String = "", Optional ByVal tw As TextWrapping = TextWrapping.Wrap)

        With Me
            .HorizontalAlignment = Windows.HorizontalAlignment.Left
            .Height = 32
            .Width = 128
            .VerticalAlignment = VerticalAlignment.Top
        End With
        Dim errorflare As New Effects.BlurEffect With {.KernelType = Effects.KernelType.Gaussian, .Radius = 10,
            .RenderingBias = Effects.RenderingBias.Performance}
        Dim myrct As New Rectangle With {.Name = "ErrorRectangle", .Fill = Brushes.Red, .Opacity = 0.33, .StrokeThickness = 1,
            .Effect = errorflare, .Visibility = Visibility.Hidden}
        Dim mytxt As New TextBox With {.Name = "TextBox", .Height = 24, .Width = 120, .Margin = New Thickness(4, 4, 0, 0),
            .BorderBrush = Brushes.LightGray, .Text = s, .HorizontalAlignment = h, .VerticalAlignment = v, .TextWrapping = tw}

        With Children
            .Add(myrct)
            .Add(mytxt)
        End With
    End Sub

End Class
