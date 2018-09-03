Public Class AgnesBaseInput
    Inherits Grid
    Public Enum ShowFlare
        Show
        Hide
    End Enum

    Public Enum FontSz
        Small       '8pt font
        Standard    '12pt font
        Medium      '16pt font
        Large       '18pt font
        VeryLarge   '24pt font
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

    Public Sub New(ByVal FieldWidth As Integer, ByVal v As VerticalAlignment, ByVal h As HorizontalAlignment, FontSize As FontSz, Optional ta As TextAlignment = TextAlignment.Left, Optional ByVal s As String = "", Optional ByVal tw As TextWrapping = TextWrapping.Wrap)
        HorizontalAlignment = h
        VerticalAlignment = v
        Width = FieldWidth
        Dim errorflare As New Effects.BlurEffect With {.KernelType = Effects.KernelType.Gaussian, .Radius = 10,
            .RenderingBias = Effects.RenderingBias.Performance}
        Dim myrct As New Rectangle With {.Name = "ErrorRectangle", .Fill = Brushes.Red, .Opacity = 0.33, .StrokeThickness = 1,
            .Effect = errorflare, .Visibility = Visibility.Hidden}

        Dim mytxt As New TextBox With {.Name = "TextBox", .BorderBrush = Brushes.LightGray, .Text = s, .TextAlignment = ta,
            .TextWrapping = tw, .HorizontalAlignment = h, .VerticalAlignment = v}

        Select Case FontSize
            Case FontSz.Small
                Height = 24
                With mytxt
                    .Height = 16
                    .Width = FieldWidth - 8
                    .Margin = New Thickness(4, 4, 0, 0)
                    .FontSize = 8
                End With
            Case FontSz.Standard
                Height = 28
                With mytxt
                    .Height = 20
                    .Width = FieldWidth - 8
                    .Margin = New Thickness(4, 4, 0, 0)
                    .FontSize = 12
                End With
            Case FontSz.Medium
                Height = 34
                With mytxt
                    .Height = 26
                    .Width = FieldWidth - 8
                    .Margin = New Thickness(4, 4, 0, 0)
                    .FontSize = 16
                End With
            Case FontSz.Large
                Height = 36
                With mytxt
                    .Height = 28
                    .Width = FieldWidth - 8
                    .Margin = New Thickness(4, 4, 0, 0)
                    .FontSize = 18
                End With
            Case FontSz.VeryLarge
                Height = 44
                With mytxt
                    .Height = 36
                    .Width = FieldWidth - 8
                    .Margin = New Thickness(4, 4, 0, 0)
                    .FontSize = 24
                End With
        End Select

        With Children
            .Add(myrct)
            .Add(mytxt)
        End With
    End Sub


End Class
