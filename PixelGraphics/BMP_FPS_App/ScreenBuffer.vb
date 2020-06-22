

Public Class ScreenBuffer

    Private Property RealWidth As Integer
    Private Property RealHeight As Integer

    Private Property Width As Integer
    Private Property Height As Integer
    Private Property PixelSize As Integer

    'The bitmap we will be drawing to
    Private Shared DrawSurface As Bitmap

    Event RenderScreen(ByVal pSurface As Bitmap)

    Public Sub New(w As Integer, h As Integer, Optional s As Integer = 1)
        PixelSize = s
        Width = w
        Height = h

        'create our DrawSurface
        DrawSurface = New Bitmap(Width, Height)

    End Sub

    Public Sub Render()
        RaiseEvent RenderScreen(DrawSurface)
    End Sub


    Public Sub DrawPixel(ByVal x As UInt32, ByVal y As UInt32, ByVal p As Pixel)
        If IsNothing(DrawSurface) Then
            DrawSurface = New Bitmap(Width, Height)
        End If

        '----------------------------------------------------------
        If PixelSize < 1 Then Return 'Throw an error at some point
        '----------------------------------------------------------

        If PixelSize = 1 Then
            DrawSurface.SetPixel(x, y, Color.FromArgb(p.m_Pixel.Signed))
        Else
            Dim temp_x As Integer = x * PixelSize
            Dim temp_y As Integer = y * PixelSize
            Dim tColor As Color = Color.FromArgb(p.m_Pixel.Signed)

            For xi As Integer = 0 To PixelSize - 1
                For yi As Integer = 0 To PixelSize - 1
                    DrawSurface.SetPixel(temp_x + xi, temp_y + yi, tColor)
                Next
            Next
        End If
    End Sub

    Private Sub FillRect(ByVal x As UInt32, ByVal y As UInt32, ByVal w As UInt32, ByVal h As UInt32, ByVal p As Pixel)
        Dim x2 As UInt32 = x + w
        Dim y2 As UInt32 = y + h

        If x <= 0 Then x = 0
        If x >= Width Then x = Width
        If y <= 0 Then y = 0
        If y >= Height Then y = Height

        If x2 <= 0 Then x2 = 0
        If x2 >= Width Then x2 = Width
        If y2 <= 0 Then y2 = 0
        If y2 >= Height Then y2 = Height

        For i As UInt32 = x To x2 - 1
            For j As UInt32 = y To y2 - 1
                DrawPixel(i, j, p)
            Next
        Next
    End Sub

    Private Sub DrawToScale(ByVal x As UInt32, ByVal y As UInt32, ByVal objSprite As Sprite, Optional ByVal scale As Single = 1.0F)
        Dim tempBMP As Bitmap
        If objSprite.Height > 0 AndAlso objSprite.Width > 0 Then
            If scale = 1.0F Then 'send to the normal draw were drawing at full scale
                DrawSprite(x, y, objSprite)
                Return
            End If
            Dim h As Integer = objSprite.Height
            Dim w As Integer = objSprite.Width
            tempBMP = New Bitmap(w, h)
        Else
            Return
        End If
        'Draw to the Temp BMP
        For i As Integer = 0 To tempBMP.Width - 1
            For j As Integer = 0 To tempBMP.Width - 1
                tempBMP.SetPixel(i, j, Color.FromArgb(objSprite.GetPixel(i, j).m_Pixel.Signed))
            Next
        Next
        'Resize the newly made bmp
        Dim resizedBMP As New Bitmap(tempBMP, (objSprite.Width * scale), (objSprite.Height * scale))
        ' Draw the new Image to the screen buffer
        If IsNothing(DrawSurface) Then
            DrawSurface = New Bitmap(Width, Height)
        End If

        For i As Int32 = 0 To resizedBMP.Width - 1
            For j As Int32 = 0 To resizedBMP.Height - 1
                DrawSurface.SetPixel(x + i, y + j, resizedBMP.GetPixel(i, j))
            Next
        Next

        'Dispose our shit
        tempBMP.Dispose()
        resizedBMP.Dispose()
    End Sub

    Private Sub DrawClear(ByVal p As Pixel)

        If IsNothing(DrawSurface) Then
            DrawSurface = New Bitmap(Width, Height)
        End If

        For i As Int32 = 0 To DrawSurface.Width - 1
            For j As Int32 = 0 To DrawSurface.Height - 1
                DrawSurface.SetPixel(i, j, Color.FromArgb(p.m_Pixel.Signed))
            Next
        Next

    End Sub

    Public Sub DrawSprite(ByVal x As Int32, ByVal y As Int32, ByVal ImageObj As Sprite)

        If IsNothing(DrawSurface) Then
            DrawSurface = New Bitmap(Width, Height)
        End If

        For i As Int32 = 0 To ImageObj.Width - 1
            For j As Int32 = 0 To ImageObj.Height - 1
                DrawSurface.SetPixel(x + i, y + j, Color.FromArgb(ImageObj.GetPixel(i, j).m_Pixel.Signed))
            Next
        Next

    End Sub
End Class
