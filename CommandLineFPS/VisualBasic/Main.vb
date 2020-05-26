Imports System.Runtime.InteropServices
Imports System.IO

Module Module1
    Public Class SortedDepthArray
        Private Destructing As Boolean = False
        Private Const m_MaxArraySize As Integer = 4

        <StructLayout(LayoutKind.Sequential, Pack:=1)>
        Public Structure DepthArrayItem
            Public Value1 As Double
            Public Value2 As Double
        End Structure

        Public Data(m_MaxArraySize) As DepthArrayItem

        'moveable item for the sorting
        Private m_Move As DepthArrayItem

        Private m_count As Integer
        Private ReadOnly Property Count() As Integer
            Get
                Return m_count
            End Get
        End Property
        Private Sub IncrementCount(ByVal Optional decrement As Boolean = False)
            If decrement Then
                m_count -= 1
            Else
                m_count += 1
            End If
        End Sub

        Private Sub InitClassVars()
            m_count = 0
        End Sub

        Public Sub New()
            InitClassVars()
        End Sub

        Public Function SortLowest() As Boolean
            For i As Integer = 0 To m_MaxArraySize - 1

                If i = 4 Then
                    Return True
                End If

                For y As Integer = i + 1 To m_MaxArraySize - 1
                    If Data(i).Value1 <= Data(y).Value1 Then '<= since they can manage to be the same
                        'good
                    Else
                        m_Move = Data(y)
                        Data(y) = Data(i)
                        Data(i) = m_Move
                        y = i + 1
                    End If
                Next

            Next
            Return True '
        End Function

        Public Sub Insert(v1 As Double, v2 As Double)
            Dim drIn As New DepthArrayItem
            If v1 < -0 Then
                Debug.WriteLine("negative value inserted.")
            End If
            drIn.Value1 = v1
            drIn.Value2 = v2
            Dim index As Integer = Me.Count
            'this should nolonger be needed now..
            'If index = 0 OrElse Data Is Nothing Then ReDim Data(m_MaxArraySize - 1) ' : m_count = 0
            Data(index) = drIn

            'increment the counter
            IncrementCount()
        End Sub

        Protected Overrides Sub Finalize()
            If Destructing Then Return
            If Not Destructing Then Destructing = True
            Data = Nothing
            m_count = 0
            MyBase.Finalize()
        End Sub
    End Class

    Public Class ScreenBufferData
        Private buffer() As Char
        Private m_size As Integer

        Public Sub New(ByVal buffer_size As Integer)
            m_size = buffer_size - 1
            ReDim buffer(m_size)
        End Sub

        Public ReadOnly Property Length() As Integer
            Get
                Return buffer.Length
            End Get
        End Property

        Public Property Data(ByVal index As Integer) As Char
            Get
                Return buffer(index)
            End Get
            Set(value As Char)
                buffer(index) = value
            End Set
        End Property

        Public ReadOnly Property Data() As String
            Get
                Return buffer ' New String(buffer)
            End Get
        End Property
    End Class

    Public Class ConsoleWriter
        Private m_Writer As StreamWriter

        Public Sub New(ByVal SizeInfo As SizeingData)
            Console.SetWindowSize(SizeInfo.Width, SizeInfo.Height) '+2 = \r\n
            Console.SetBufferSize(SizeInfo.Width, SizeInfo.Height) '+2 = \r\n (if using line feed add to the width)
            Console.OutputEncoding = System.Text.Encoding.UTF8

            m_Writer = New StreamWriter(Console.OpenStandardOutput())
            m_Writer.AutoFlush = True
        End Sub

        Protected Overrides Sub Finalize()
            m_Writer.Close()
            m_Writer.Dispose()

            MyBase.Finalize()
        End Sub

        Public Sub Add(ByVal strIn As String)
            Console.SetCursorPosition(0, 0) 'do not clear just overwrite the buffer
            m_Writer.Write(strIn)
        End Sub
    End Class

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
    Private i_ScreenIndexDefault As Integer = 120 'second line @ 0, console dosent like printing the very top line.

    'Screen buffer
    Private Screen As ScreenBufferData
    Private ConWrite As ConsoleWriter

    Private Const FRAMERATE_LOCK As UInteger = (1000 / 30) 'Lock at 60 FPS / 60

    Private Function InitData() As Boolean
        InfoBag.ScreenInf.Height = 40
        InfoBag.ScreenInf.Width = 120

        Screen = New ScreenBufferData(InfoBag.ScreenInf.TotalSize)
        ConWrite = New ConsoleWriter(InfoBag.ScreenInf)

        InfoBag.MapInf.Height = 16
        InfoBag.MapInf.Width = 16

        InfoBag.Player.X = 2.0F
        InfoBag.Player.Y = 2.0F
        InfoBag.Player.A = 0.0F
        InfoBag.Player.FOV = 3.14159F / 4.0F

        InfoBag.Running = True

        Return True
    End Function


    Sub Main()
        If InitData() = False Then
            Return 'failed to initalize
        End If

        Dim input_value As Integer = 0

        Dim TimePart1 As Long = System.Environment.TickCount
        Dim TimePart2 As Long = System.Environment.TickCount


        Dim map As String = ""
        map &= "################"
        map &= "#..............#"
        map &= "#.......########"
        map &= "#..............#"
        map &= "#......##......#"
        map &= "#......##......#"
        map &= "#..............#"
        map &= "###............#"
        map &= "##.............#"
        map &= "#.......####.###"
        map &= "#.......#......#"
        map &= "#.......#......#"
        map &= "#..............#"
        map &= "#.......########"
        map &= "#..............#"
        map &= "################"

        Dim CapTimer As New mTimer.myTimer()
        Dim firstrun As Boolean = True
        While (InfoBag.Running)
            CapTimer.StartMe() 'Testing FPS timer

            TimePart2 = System.Environment.TickCount
            Dim TimeElapsed As Double = (TimePart2 - TimePart1) 'gettickcount equiv
            TimePart1 = TimePart2

            If Console.KeyAvailable Then 'get our key clicks
                input_value = Console.ReadKey(True).Key '######################### This will sleep the application until a key is pushed
                Dim tmpPos As Integer = (InfoBag.Player.X * InfoBag.MapInf.Width) + InfoBag.Player.Y
                Select Case input_value
                    Case ConsoleKey.Escape 'Kill the application
                        InfoBag.Running = False
                        Exit While
                    Case ConsoleKey.A
                        InfoBag.Player.A -= (0.1F) '* TimeElapsed
                    Case ConsoleKey.D
                        InfoBag.Player.A += (0.1F) '* TimeElapsed
                    Case ConsoleKey.W
                        InfoBag.Player.X += Math.Sin(InfoBag.Player.A) * 0.1F '* TimeElapsed
                        InfoBag.Player.Y += Math.Cos(InfoBag.Player.A) * 0.1F '* TimeElapsed
                        If (map((InfoBag.Player.X * InfoBag.MapInf.Width) + InfoBag.Player.Y) = "#") Then
                            InfoBag.Player.X -= Math.Sin(InfoBag.Player.A) * 0.1F '* TimeElapsed
                            InfoBag.Player.Y -= Math.Cos(InfoBag.Player.A) * 0.1F '* TimeElapsed
                        End If
                    Case ConsoleKey.S
                        InfoBag.Player.X -= Math.Sin(InfoBag.Player.A) * 0.1F '* TimeElapsed
                        InfoBag.Player.Y -= Math.Cos(InfoBag.Player.A) * 0.1F '* TimeElapsed
                        If (map((InfoBag.Player.X * InfoBag.MapInf.Width) + InfoBag.Player.Y) = "#") Then
                            InfoBag.Player.X += Math.Sin(InfoBag.Player.A) * 0.1F '* TimeElapsed
                            InfoBag.Player.Y += Math.Cos(InfoBag.Player.A) * 0.1F '* TimeElapsed
                        End If
                    Case Else
                        'do nothing
                End Select
            End If

            For x As Integer = 0 To InfoBag.ScreenInf.Width - 1
                Dim fRayAngle As Double = (InfoBag.Player.A - InfoBag.Player.FOV / 2.0F) + (x / InfoBag.ScreenInf.Width) * InfoBag.Player.FOV
                Dim fDistanceToWall As Double = 0

                Dim fEyeX As Double = Math.Sin(fRayAngle)
                Dim fEyeY As Double = Math.Cos(fRayAngle)

                Dim bHitWall As Boolean = False
                Dim bBoundry As Boolean = False
                While Not bHitWall AndAlso fDistanceToWall < fDepth

                    fDistanceToWall += 0.1F

                    Dim nTestX As Integer = (InfoBag.Player.X + fEyeX * fDistanceToWall)
                    Dim nTestY As Integer = (InfoBag.Player.Y + fEyeY * fDistanceToWall)

                    If (nTestX < 0) OrElse (nTestX >= InfoBag.MapInf.Width) OrElse (nTestY < 0) OrElse (nTestY >= InfoBag.MapInf.Height) Then
                        bHitWall = True
                        fDistanceToWall = fDepth
                    Else
                        If map((nTestY * InfoBag.MapInf.Width) + nTestX) = "#" Then
                            bHitWall = True

                            Dim p As New SortedDepthArray()
                            For tx As Integer = 0 To 1
                                For ty As Integer = 0 To 1
                                    Dim vy As Double = nTestY + ty - InfoBag.Player.Y
                                    Dim vx As Double = nTestX + tx - InfoBag.Player.X
                                    Dim d As Double = Math.Sqrt((vx * vx) + (vy * vy))
                                    Dim dot As Double = ((fEyeX * vx) / d) + ((fEyeY * vy) / d)
                                    p.Insert(d, dot)
                                Next
                            Next
                            p.SortLowest()

                            Dim fBound As Double = 0.01F

                            If (Math.Acos(p.Data(0).Value2) < fBound) Then bBoundry = True
                            If (Math.Acos(p.Data(1).Value2) < fBound) Then bBoundry = True
                            If (Math.Acos(p.Data(2).Value2) < fBound) Then bBoundry = True

                            p = Nothing
                        End If
                    End If
                End While

                Dim nCeiling As Integer = (InfoBag.ScreenInf.Height / 2.0) - (InfoBag.ScreenInf.Height / fDistanceToWall)
                Dim nFloor As Integer = InfoBag.ScreenInf.Height - nCeiling

                Dim n_Shade As Char = " "
                If fDistanceToWall <= fDepth / 4.0F Then
                    n_Shade = ChrW(&H2588) 'Full block
                ElseIf fDistanceToWall < fDepth / 3.0F Then
                    n_Shade = ChrW(&H2593) 'Dark
                ElseIf fDistanceToWall < fDepth / 2.0F Then
                    n_Shade = ChrW(&H2592) 'Medium
                ElseIf fDistanceToWall < fDepth Then
                    n_Shade = ChrW(&H2591) 'Light
                Else
                    n_Shade = " "
                End If

                If (bBoundry) Then
                    n_Shade = "I" 'i believe vb's math isnt catching all these boundry values
                End If

                Dim tmpStr As String = ""
                For y As Integer = 0 To InfoBag.ScreenInf.Height - 1
                    Dim index_value As Integer = (y * InfoBag.ScreenInf.Width) + x
                    If y < nCeiling Then
                        Screen.Data(index_value) = " "
                    ElseIf (y > nCeiling AndAlso y <= nFloor) Then
                        Screen.Data(index_value) = n_Shade
                    Else
                        Dim f_shade As Char
                        Dim b As Double = 1.0F - ((y - InfoBag.ScreenInf.Height / 2.0F) / (InfoBag.ScreenInf.Height / 2.0F))
                        If b < 0.25 Then
                            f_shade = "#"
                        ElseIf (b < 0.5) Then
                            f_shade = "X"
                        ElseIf (b < 0.75) Then
                            f_shade = "."
                        ElseIf (b < 0.9) Then
                            f_shade = "-"
                        Else
                            f_shade = " "
                        End If

                        Screen.Data(index_value) = f_shade
                    End If
                Next
            Next

            'sanity check.
            If screen.Length <> (InfoBag.ScreenInf.TotalSize) Then
                'should print an error message (screen text is corrupt or some shit like that)
                Exit While
            End If

            ConWrite.Add(Screen.Data())

            Console.Title = "CommandLineFPS FPS:" & CapTimer.CalculateFPS().ToString()

            Dim frameticks As UInteger = CapTimer.GetDelta()
            If frameticks < FRAMERATE_LOCK Then
                Threading.Thread.Sleep(FRAMERATE_LOCK - frameticks)
            End If
        End While
    End Sub

End Module
