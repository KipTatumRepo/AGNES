Public Class CurrencyBox
    Inherits AgnesBaseInput
    Private _credit As Boolean
    Public Property Credit As Boolean
        Get
            Return _credit
        End Get
        Set(value As Boolean)
            _credit = value
        End Set
    End Property

    Private _debit As Boolean
    Public Property Debit As Boolean
        Get
            Return _debit
        End Get
        Set(value As Boolean)
            _debit = value
        End Set
    End Property

    Private _amount As Double
    Private _setamount As Double
    Public Property Amount As Double
        Get
            Return _amount
        End Get
        Set(value As Double)
            _amount = value
        End Set
    End Property
    Public Property SetAmount As Double
        Get
            Return _setamount
        End Get
        Set(value As Double)
            _setamount = value
            Dim tb As TextBox = Children(1)
            tb.Text = FormatCurrency(_setamount, 2)
        End Set
    End Property
    Private _debitonly As Boolean
    Private _creditonly As Boolean
    Public Highlight As Boolean
    Private SystemChange As Boolean


    Public Sub New(FieldWidth As Integer, AllowCredit As Boolean, AllowDebit As Boolean, ForceCredit As Boolean, ForceDebit As Boolean, SelectAllUponEnteringField As Boolean, FontSize As AgnesBaseInput.FontSz, Optional ByVal DefaultText As String = "$0.00")
        MyBase.New(FieldWidth, VerticalAlignment.Top, HorizontalAlignment.Left, FontSize, TextAlignment.Right, DefaultText, TextWrapping.NoWrap)
        Credit = AllowCredit
        Debit = AllowDebit
        _debitonly = ForceDebit
        _creditonly = ForceCredit
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

    Private Sub ValidateText(sender As Object, e As EventArgs)
        Dim t As TextBox = sender
        If SystemChange = True Then Exit Sub
        Try
            Dim cval As Double = FormatCurrency(t.Text, 2)
        Catch ex As Exception
            Flare = True
            Exit Sub
        End Try
        Flare = False
    End Sub

    Private Sub EnterField(sender As Object, e As EventArgs)
        Dim t As TextBox = sender
        SystemChange = True
        '// Remove currency symbol, if present  
        t.Text = t.Text.Replace("$", "")

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
            Dim cval As Double = FormatNumber(t.Text, 2)
            If (Debit = False And cval > 0) Or (Credit = False And cval < 0) Then
                SystemChange = True
                Flare = True
            Else
                Flare = False
            End If
            t.Text = FormatCurrency(cval, 2)
            Amount = FormatNumber(cval, 2)
            SystemChange = False
        Catch ex As Exception
            Flare = True
            Me.Focus()
        End Try

        Try
            Dim cval As Double = FormatNumber(t.Text, 2)
            If (_debitonly = True And cval < 0) Or (_creditonly = True And cval > 0) Then
                SystemChange = True
                Flare = False
                t.Text = FormatCurrency(-cval, 2)
                Amount = FormatNumber(-cval, 2)
                SystemChange = False
            End If
        Catch ex As Exception
            Flare = True
            Me.Focus()
        End Try

    End Sub

End Class
