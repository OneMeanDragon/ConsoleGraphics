Public Class Map
    Private Const M_map As String = "" _
                                    & "################" _
                                    & "#..............#" _
                                    & "#.......########" _
                                    & "#..............#" _
                                    & "#......##......#" _
                                    & "#......##......#" _
                                    & "#..............#" _
                                    & "###............#" _
                                    & "##.............#" _
                                    & "#.......####.###" _
                                    & "#.......#......#" _
                                    & "#.......#......#" _
                                    & "#..............#" _
                                    & "#.......########" _
                                    & "#..............#" _
                                    & "################"

    Private Property MapData As String
    Private Property Width As Integer
    Private Property Height As Integer

    Public Property MapRect As Sprite

    Public Property x_location As Integer
    Public Property y_location As Integer

    Private Property BlockSize As Integer

    Private Property PlayerCurrentX As Integer
    Private Property PlayerCurrentY As Integer
    Private Property PlayerCurrentChar As String

    Private Sub UpdatePixelAt(x As Integer, y As Integer, strOld As String)
        Dim temp_x As Integer = x * BlockSize
        Dim temp_y As Integer = y * BlockSize
        Dim tmp_c As Pixel = GetColorBlock(strOld)

        For xi As Integer = 0 To BlockSize - 1
            For yi As Integer = 0 To BlockSize - 1
                MapRect.SetPixel(temp_x + xi, temp_y + yi, tmp_c)
            Next
        Next
    End Sub

    Public Sub SetPlayerLocation(x As Integer, y As Integer)
        PlayerCurrentX = x
        PlayerCurrentY = y
        PlayerCurrentChar = MapData.Substring(PlayerCurrentX + (PlayerCurrentY * Height), 1)
        UpdatePixelAt(PlayerCurrentX, PlayerCurrentY, "P")
    End Sub

    Public Sub UpdatePlayer(LocX As Integer, LocY As Integer)
        If (PlayerCurrentX = LocX) AndAlso (PlayerCurrentY = LocY) Then Return 'Nothing to update its the same location
        'Update the previous block pixel
        UpdatePixelAt(PlayerCurrentX, PlayerCurrentY, PlayerCurrentChar)
        'Set the new block pixel
        PlayerCurrentX = LocX
        PlayerCurrentY = LocY
        PlayerCurrentChar = MapData.Substring(PlayerCurrentX + (PlayerCurrentY * Height), 1)
        UpdatePixelAt(PlayerCurrentX, PlayerCurrentY, "P")
    End Sub

    Private Function GetColorBlock(ByVal strChar As String) As Pixel
        Select Case strChar
            Case "#" 'Wall
                Return PixelColors.VERY_DARK_BLUE
            Case "." 'Floor
                Return PixelColors.BLANK ' just testing else use white or black? well see
            Case "P", "p" 'Player
                Return PixelColors.DARK_GREEN
            Case Else
                Return PixelColors.BLACK
        End Select
    End Function

    Private Sub DrawMapObject() ' Initial rect draw
        Dim temp_x As Integer
        Dim temp_y As Integer

        For i As Integer = 0 To MapData.Length() - 1
            temp_x = (i Mod Width) * BlockSize
            temp_y = (i \ Height) * BlockSize
            For x As Integer = 0 To BlockSize - 1
                For y As Integer = 0 To BlockSize - 1
                    MapRect.SetPixel(temp_x + x, temp_y + y, GetColorBlock(MapData.Substring(i, 1)))
                Next
            Next
        Next
    End Sub

    Private Sub Init()
        x_location = 0
        y_location = 0

        MapRect = New Sprite(Width * BlockSize, Height * BlockSize)

        'Draw the map into the sprite rect
        DrawMapObject()
    End Sub
    Public Sub New(strMapData As String, w As Integer, h As Integer, Optional SquareSize As Integer = 4)
        Width = w
        Height = h
        '------------------------------------------------------------------------
        If strMapData.Length() < (w * h) Then Return 'get fucked, throw an error
        '------------------------------------------------------------------------
        MapData = strMapData
        BlockSize = SquareSize

        Init()
    End Sub

    Public Sub New(w As Integer, h As Integer, Optional SquareSize As Integer = 6) 'Default map
        Width = w
        Height = h
        MapData = M_map
        BlockSize = SquareSize

        Init()
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

End Class
