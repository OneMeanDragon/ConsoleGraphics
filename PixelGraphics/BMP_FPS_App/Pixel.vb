
Imports System.Runtime.InteropServices


<StructLayout(LayoutKind.Explicit)>
Public Structure PixelUnion
    <FieldOffset(2)> Public r As Byte
    <FieldOffset(1)> Public g As Byte
    <FieldOffset(0)> Public b As Byte
    <FieldOffset(3)> Public a As Byte '
    <FieldOffset(0)> Public n As UInt32 ' = &HFF000000UI
    <FieldOffset(0)> Public Signed As Int32
End Structure 'I may have these fields in reverse order? (apparently not acording to c++)

Structure PixelColors
    Public Shared GREY As New Pixel(192, 192, 192)
    Public Shared DARK_GREY As New Pixel(128, 128, 128)
    Public Shared VERY_DARK_GREY As New Pixel(64, 64, 64)
    Public Shared RED As New Pixel(255, 0, 0)
    Public Shared DARK_RED As New Pixel(128, 0, 0)
    Public Shared VERY_DARK_RED As New Pixel(64, 0, 0)
    Public Shared YELLOW As New Pixel(255, 255, 0)
    Public Shared DARK_YELLOW As New Pixel(128, 128, 0)
    Public Shared VERY_DARK_YELLOW As New Pixel(64, 64, 0)
    Public Shared GREEN As New Pixel(0, 255, 0)
    Public Shared DARK_GREEN As New Pixel(0, 128, 0)
    Public Shared VERY_DARK_GREEN As New Pixel(0, 64, 0)
    Public Shared CYAN As New Pixel(0, 255, 255)
    Public Shared DARK_CYAN As New Pixel(0, 128, 128)
    Public Shared VERY_DARK_CYAN As New Pixel(0, 64, 64)
    Public Shared BLUE As New Pixel(0, 0, 255)
    Public Shared DARK_BLUE As New Pixel(0, 0, 128)
    Public Shared VERY_DARK_BLUE As New Pixel(0, 0, 64)
    Public Shared MAGENTA As New Pixel(255, 0, 255)
    Public Shared DARK_MAGENTA As New Pixel(128, 0, 128)
    Public Shared VERY_DARK_MAGENTA As New Pixel(64, 0, 64)
    Public Shared WHITE As New Pixel(255, 255, 255)
    Public Shared BLACK As New Pixel(0, 0, 0)
    Public Shared BLANK As New Pixel(0, 0, 0, 0)
End Structure

Public Class Pixel
    Public Enum enMode
        normal
        masking
        alpha
        custom
    End Enum
    Public Mode As enMode = enMode.normal
    Public m_Pixel As PixelUnion

    Public Sub New()
        m_Pixel.n = &HFF000000UI 'Default Black
    End Sub
    Public Sub New(ByVal r As Byte, ByVal g As Byte, ByVal b As Byte, ByVal Optional a As Byte = &HFFUI)
        '----------------------------------------------------.
        m_Pixel.r = r                                       '|
        m_Pixel.g = g                                       '|
        m_Pixel.b = b                                       '|
        m_Pixel.a = a                                       '|
        '---------------------------------------------------'|
        'm_Pixel.n = r or (g << 8) or (b << 16) or (a << 24)'|
        '----------------------------------------------------'
    End Sub
    Public Sub New(ByVal iValue As UInt32)
        m_Pixel.n = iValue
    End Sub

    Public Sub Copy(ByVal p As Pixel)
        Me.Mode = p.Mode
        Me.m_Pixel.a = p.m_Pixel.a
        Me.m_Pixel.r = p.m_Pixel.r
        Me.m_Pixel.g = p.m_Pixel.g
        Me.m_Pixel.b = p.m_Pixel.b
        'If Me.m_Pixel.n <> p.m_Pixel.n Then
        '    Debug.WriteLine("Failed to copy pixel.")
        'End If
    End Sub

    Public Shared Operator =(ByVal p1 As Pixel, ByVal p2 As Pixel) As Boolean
        Return (p1.m_Pixel.n = p2.m_Pixel.n)
    End Operator

    Public Shared Operator <>(ByVal p1 As Pixel, ByVal p2 As Pixel) As Boolean
        Return (p1.m_Pixel.n <> p2.m_Pixel.n)
    End Operator
End Class