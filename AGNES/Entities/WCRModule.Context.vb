﻿'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated from a template.
'
'     Manual changes to this file may cause unexpected behavior in your application.
'     Manual changes to this file will be overwritten if the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Imports System
Imports System.Data.Entity
Imports System.Data.Entity.Infrastructure

Partial Public Class WCREntities
    Inherits DbContext

    Public Sub New()
        MyBase.New("name=WCREntities")
    End Sub

    Protected Overrides Sub OnModelCreating(modelBuilder As DbModelBuilder)
        Throw New UnintentionalCodeFirstException()
    End Sub

    Public Overridable Property Tender_GL_Mapping() As DbSet(Of Tender_GL_Mapping)
    Public Overridable Property TenderID_TenderType_Mapping() As DbSet(Of TenderID_TenderType_Mapping)
    Public Overridable Property VendorInfoes() As DbSet(Of VendorInfo)
    Public Overridable Property CAMWithholdingTypes() As DbSet(Of CAMWithholdingType)
    Public Overridable Property KPIWithholdingTypes() As DbSet(Of KPIWithholdingType)
    Public Overridable Property ReceivedCAMChecks() As DbSet(Of ReceivedCAMCheck)

End Class
