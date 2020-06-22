Imports System.Runtime.InteropServices
Imports System.IO

Public Class Form1

    Private WithEvents ScreenBuf As ScreenBuffer

    Private Delegate Sub RenderDelegate(bg As Bitmap)
    Sub RenderDel(bg As Bitmap)
        PictureBox1.BackgroundImage = bg
        PictureBox1.Refresh()
    End Sub

    Private Sub ScreenBuf_RenderScreen(pSurface As Bitmap) Handles ScreenBuf.RenderScreen
        If PictureBox1.InvokeRequired Then
            PictureBox1.Invoke(New RenderDelegate(AddressOf RenderDel), pSurface)
        Else
            PictureBox1.BackgroundImage = pSurface
            PictureBox1.Refresh()
        End If
    End Sub

    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.

        'Init the screen buffer, need to figure out a size.
        ScreenBuf = New ScreenBuffer(1280, 720) 'Defaulted to 720p, pixel block size 24*24
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'Start up the procedure loop
    End Sub
    'Testing button
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        'ScreenBuf.DrawPixel(0, 0, PixelColors.DARK_GREEN)
        'ScreenBuf.Render()
        'ScreenBuf.DrawPixel(1, 0, PixelColors.DARK_GREEN)
        'ScreenBuf.Render()
        'ScreenBuf.DrawPixel(3, 6, PixelColors.DARK_GREEN)
        'ScreenBuf.Render()
        'ScreenBuf.DrawPixel(16, 2, PixelColors.DARK_GREEN)
        'ScreenBuf.Render()
        'ScreenBuf.DrawPixel(8, 72, PixelColors.DARK_GREEN)
        'ScreenBuf.Render()

        Dim cMap As New Map(16, 16)
        ScreenBuf.DrawSprite(cMap.x_location, cMap.y_location, cMap.MapRect)
        ScreenBuf.Render()
        'set player
        cMap.SetPlayerLocation(0, 0)
        ScreenBuf.DrawSprite(cMap.x_location, cMap.y_location, cMap.MapRect)
        ScreenBuf.Render()

        'update player location
        cMap.UpdatePlayer(4, 2)
        ScreenBuf.DrawSprite(cMap.x_location, cMap.y_location, cMap.MapRect)
        ScreenBuf.Render()
    End Sub

    <StructLayout(LayoutKind.Sequential, Pack:=1)>
    Public Structure SizeingData
        Public Width As UInteger
        Public Height As UInteger
        Public ReadOnly Property TotalSize()
            Get
                Return (Width * Height)
            End Get
        End Property
    End Structure

    <StructLayout(LayoutKind.Sequential, Pack:=1)>
    Public Structure PlayerData
        Public X As Double
        Public Y As Double
        Public A As Double
        Public FOV As Double
    End Structure

    <StructLayout(LayoutKind.Sequential, Pack:=1)>
    Public Structure ConsoleInfoData
        Public ScreenInf As SizeingData
        Public MapInf As SizeingData
        '
        Public Player As PlayerData
        '
        Public Running As Boolean
    End Structure

    Public InfoBag As ConsoleInfoData

    Private fDepth As Double = 16.0F
    Private m_Speed As Double = 5.0F

    'This is the maps default location
    Private Map_X_Loaction As Integer = 120

    Private Const FRAMERATE_LOCK As UInteger = (1000 / 32) 'Lock at 60 FPS / 60

    Private Sub Run()
        Dim CapTimer As New myTimer()

        While InfoBag.Running
            'Start Frame
            CapTimer.StartMe()
            'DoStuff

            '--------------------------.
            'Key Pressed Events        |
            '--------------------------|
            '                          |
            '--------------------------'





            'End Frame
            Debug.WriteLine("FPS:" & CapTimer.CalculateFPS().ToString())
            Dim frameticks As UInteger = CapTimer.GetDelta() 'should be using a floating point
            If frameticks < FRAMERATE_LOCK Then
                Threading.Thread.Sleep(FRAMERATE_LOCK - frameticks) 'The lock is set to 32fps
            End If
        End While
    End Sub

End Class
