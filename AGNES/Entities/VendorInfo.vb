'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated from a template.
'
'     Manual changes to this file may cause unexpected behavior in your application.
'     Manual changes to this file will be overwritten if the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Imports System
Imports System.Collections.Generic

Partial Public Class VendorInfo
    Public Property PID As Long
    Public Property Name As String
    Public Property Invoice As String
    Public Property Supplier As Nullable(Of Long)
    Public Property ProductClassId As Nullable(Of Integer)
    Public Property StoreId As Nullable(Of Long)
    Public Property VendorType As Short
    Public Property Active As Boolean
    Public Property FoodType As Nullable(Of Integer)
    Public Property FoodSubType As Nullable(Of Integer)
    Public Property CAMType As Nullable(Of Short)
    Public Property CAMStart As Nullable(Of Date)
    Public Property CAMAmount As Nullable(Of Decimal)
    Public Property KPIType As Nullable(Of Short)
    Public Property KPIStart As Nullable(Of Date)
    Public Property KPIAmount As Nullable(Of Decimal)
    Public Property RequiresHood As Nullable(Of Boolean)
    Public Property InsuranceExpiration As Nullable(Of Date)
    Public Property ContractExpiration As Nullable(Of Date)
    Public Property MaximumDailyCafes As Nullable(Of Short)

End Class
