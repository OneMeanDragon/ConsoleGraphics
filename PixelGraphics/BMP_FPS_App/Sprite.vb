

#Const OVERDRAW = 0
Public Class Sprite 'Rect
    Public Property Width() As UInt32
    Public Property Height() As UInt32
    Public Enum enuMode
        normal
        periodic
    End Enum
    Private modeSample As enuMode

    Public pColData() As Pixel

    Public Sub New(ByVal arWidth As UInt32, ByVal arHeight As UInt32)
        modeSample = enuMode.normal
        Width = arWidth
        Height = arHeight
        ReDim pColData((Width * Height) - 1)
        For i As UInt32 = 0 To ((Width * Height) - 1) 'VB
            pColData(i) = New Pixel()
        Next
    End Sub
    Protected Overrides Sub Finalize()
        'build pixel destructor
        MyBase.Finalize()
    End Sub


#If (OVERDRAW >= 1) Then
        Public Shared nOverdrawCount As UInt32
#End If
    Public Function SetPixel(ByVal x As Int32, ByVal y As Int32, ByVal p As Pixel) As Boolean
#If (OVERDRAW >= 1) Then
            nOverdrawCount += 1
#End If
        '
        If x >= 0 AndAlso x < Width AndAlso y >= 0 AndAlso y < Height Then
            pColData(y * Width + x).Copy(p)
            Return True
        End If
        Return False
    End Function

    Public Function GetPixel(ByVal x As Int32, ByVal y As Int32) As Pixel
        If modeSample = enuMode.normal Then
            If x >= 0 AndAlso x < Width AndAlso y >= 0 AndAlso y < Height Then
                Return pColData(y * Width + x)
            Else
                Return New Pixel(0, 0, 0, 0)
            End If
        Else
            Return pColData(Math.Abs(y Mod Height) * Width + Math.Abs(x Mod Width))
        End If
    End Function
End Class