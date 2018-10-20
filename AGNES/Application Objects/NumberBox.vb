Public Class NumberBox
    Inherits AgnesBaseInput
#Region "Properties"
    Private _posonly As Boolean
    Private _negonly As Boolean
    Public NumberOfDecimals As Byte
    Public Highlight As Boolean
    Private SystemChange As Boolean

    Private _pos As Boolean
    Public Property Positive As Boolean
        Get
            Return _pos
        End Get
        Set(value As Boolean)
            _pos = value
        End Set
    End Property

    Private _neg As Boolean
    Public Property Negative As Boolean
        Get
            Return _neg
        End Get
        Set(value As Boolean)
            _neg = value
        End Set
    End Property

    Private _amount As Double
    Public Property Amount As Double
        Get
            Return _amount
        End Get
        Set(value As Double)
            _amount = value
        End Set
    End Property

    Private _setamount As Double
    Public Property SetAmount As Double
        Get
            Return _setamount
        End Get
        Set(value As Double)
            _setamount = value
            Dim tb As TextBox = Children(1)
            tb.Text = FormatNumber(_setamount, NumberOfDecimals)
        End Set
    End Property

#End Region

#Region "Constructor"
    Public Sub New(FieldWidth As Integer, AllowPositive As Boolean, AllowNegative As Boolean, ForcePositive As Boolean, ForceNegative As Boolean, SelectAllUponEnteringField As Boolean, FontSize As AgnesBaseInput.FontSz, Optional ByVal Decimals As Byte = 0, Optional ByVal DefaultText As String = "$0.00")
        MyBase.New(FieldWidth, VerticalAlignment.Top, HorizontalAlignment.Left, FontSize, TextAlignment.Center, DefaultText, TextWrapping.NoWrap)
        Positive = AllowPositive
        Negative = AllowNegative
        _posonly = ForcePositive
        _negonly = ForceNegative
        NumberOfDecimals = Decimals
        Highlight = SelectAllUponEnteringField
        Dim t As TextBox = Children(1)
        AddHandler t.GotFocus, AddressOf EnterField
        AddHandler t.LostFocus, AddressOf ExitField
        AddHandler t.TextChanged, AddressOf ValidateText
        ' Field width for 8pt = 80
        '             for 12pt= 112
        '             for 16pt= 140
        '             for 18pt= 160
        '             for 24pt= 204
    End Sub

#End Region

#Region "Private Methods"
    Private Sub ValidateText(sender As Object, e As EventArgs)
        Dim t As TextBox = sender
        If SystemChange = True Then Exit Sub
        Try
            Dim cval As Double = FormatNumber(t.Text, NumberOfDecimals)
        Catch ex As Exception
            Flare = True
            Exit Sub
        End Try
        Flare = False
    End Sub

    Private Sub EnterField(sender As Object, e As EventArgs)
        Dim t As TextBox = sender
        SystemChange = True
        '// Remove negative parentheses, if present
        If InStr(1, t.Text, "(") > 0 Then
            t.Text = t.Text.Replace("(", "")
            t.Text = t.Text.Replace(")", "")
            t.Text = "-" & t.Text
        End If

        If Highlight = True Then
            t.SelectAll()
        Else
            t.CaretIndex = t.Text.Length
        End If
        SystemChange = False
    End Sub

    Private Sub ExitField(sender As Object, e As EventArgs)
        Dim t As TextBox = sender
        Try
            Dim cval As Double = FormatNumber(t.Text, NumberOfDecimals)
            If (Positive = False And cval > 0) Or (Negative = False And cval < 0) Then
                SystemChange = True
                Flare = True
            Else
                Flare = False
            End If
            t.Text = FormatNumber(cval, NumberOfDecimals)
            Amount = FormatNumber(cval, NumberOfDecimals)
            SystemChange = False
        Catch ex As Exception
            Flare = True
            Me.Focus()
        End Try

        Try
            Dim cval As Double = FormatNumber(t.Text)
            If (_posonly = True And cval < 0) Or (_negonly = True And cval > 0) Then
                SystemChange = True
                Flare = False
                t.Text = FormatNumber(-cval, NumberOfDecimals)
                Amount = FormatNumber(-cval, NumberOfDecimals)
                SystemChange = False
            End If
        Catch ex As Exception
            Flare = True
            Me.Focus()
        End Try

    End Sub

#End Region
End Class
