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

Partial Public Class AGNESEntity
    Inherits DbContext

    Public Sub New()
        MyBase.New("name=AGNESEntity")
    End Sub

    Protected Overrides Sub OnModelCreating(modelBuilder As DbModelBuilder)
        Throw New UnintentionalCodeFirstException()
    End Sub

    Public Overridable Property CashHandles() As DbSet(Of CashHandle)
    Public Overridable Property LOAs() As DbSet(Of LOA)
    Public Overridable Property Occurrences() As DbSet(Of Occurrence)

End Class
