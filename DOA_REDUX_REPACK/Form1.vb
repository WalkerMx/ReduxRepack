Imports System.IO
Imports DOA_REDUX_REPACK.Form1.Mode

Public Class Form1

    Public Enum Mode
        Pack
        Unpack
    End Enum

    Dim Mapper As New FileMapper
    Dim Patcher As New FilePatcher
    Dim RootPath As String = Environment.CurrentDirectory

    Private Sub CompressCheckbox_CheckedChanged(sender As Object, e As EventArgs) Handles CompressCheckbox.CheckedChanged
        If CompressCheckbox.Checked = True Then
            If MessageBox.Show("Compression can cause stutter! Are you sure?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) = MsgBoxResult.No Then
                CompressCheckbox.Checked = False
            End If
        End If
    End Sub

    Private Sub StartButton_Click(sender As Object, e As EventArgs) Handles StartButton.Click

        Dim RequiredFiles As String() = {"BSArch.exe", "Daughters of Ares.bsa", "xdelta3.exe"}

        For Each FileName As String In RequiredFiles
            If Not File.Exists(RootPath & "\" & FileName) Then
                MsgBox(FileName & " is missing!", MsgBoxStyle.Exclamation, "Error")
                Exit Sub
            End If
        Next

        RepackFiles()

    End Sub

    Private Sub RepackFiles()
        StartButton.Enabled = False
        Dim SourceFolder As String = RootPath & "\TempSource"
        Dim DestFolder As String = RootPath & "\TempREDUX"
        Directory.CreateDirectory(SourceFolder)
        Directory.CreateDirectory(DestFolder)

        RunBSArch(Unpack, SourceFolder)

        ProgressBar1.Value = 20

        My.Computer.FileSystem.CopyDirectory(SourceFolder & "\meshes\characters", DestFolder & "\meshes\characters")
        My.Computer.FileSystem.CopyDirectory(SourceFolder & "\textures", DestFolder & "\textures")
        Dim SourceFiles = Directory.GetFiles(SourceFolder & "\meshes", "*.nif", SearchOption.AllDirectories)

        For Each MeshPath As String In SourceFiles
            If Mapper.GetFileMatch(MeshPath, DestFolder) IsNot Nothing Then
                My.Computer.FileSystem.CopyFile(MeshPath, Mapper.GetFileMatch(MeshPath, DestFolder))
            End If
        Next

        ProgressBar1.Value = 30

        File.Delete(DestFolder & "\meshes\characters\_male\daughtersofares\eye\eyelefthumanfemale.nif")
        File.Delete(DestFolder & "\meshes\characters\_male\daughtersofares\eye\eyerighthumanfemale.nif")
        Directory.Delete(DestFolder & "\textures\characters\daughtersofares\eye", True)
        Directory.CreateDirectory(DestFolder & "\textures\characters\daughtersofares\eye")
        File.Copy(SourceFolder & "\textures\characters\daughtersofares\eye\green_g.dds", DestFolder & "\textures\characters\daughtersofares\eye\eye_g.dds")
        File.Copy(SourceFolder & "\textures\characters\daughtersofares\eye\eye_n.dds", DestFolder & "\textures\characters\daughtersofares\eye\eye_n.dds")

        ProgressBar1.Value = 40

        For Each HairTex As String In {"01hair", "c13", "c24", "rd01", "rd11"}
            If File.Exists(DestFolder & "\textures\characters\daughtersofares\hair\" & HairTex & ".dds") Then
                File.Delete(DestFolder & "\textures\characters\daughtersofares\hair\" & HairTex & ".dds")
            End If
            If File.Exists(DestFolder & "\textures\characters\daughtersofares\hair\" & HairTex & "_hl.dds") Then
                File.Delete(DestFolder & "\textures\characters\daughtersofares\hair\" & HairTex & "_hl.dds")
            End If
            If File.Exists(DestFolder & "\textures\characters\daughtersofares\hair\" & HairTex & "_n.dds") Then
                File.Delete(DestFolder & "\textures\characters\daughtersofares\hair\" & HairTex & "_n.dds")
            End If
        Next

        ProgressBar1.Value = 50

        For Each HairMesh As String In {"c13", "c24", "hair3a", "hair3ahat", "hair3anohat", "rd01", "rd11"}
            If File.Exists(DestFolder & "\meshes\characters\_male\daughtersofares\hair\" & HairMesh & ".nif") Then
                File.Delete(DestFolder & "\meshes\characters\_male\daughtersofares\hair\" & HairMesh & ".nif")
            End If
            If File.Exists(DestFolder & "\meshes\characters\_male\daughtersofares\hair\" & HairMesh & ".egm") Then
                File.Delete(DestFolder & "\meshes\characters\_male\daughtersofares\hair\" & HairMesh & ".egm")
            End If
        Next

        ProgressBar1.Value = 60

        Patcher.PatchAllFiles(SourceFolder, DestFolder)

        For Each ThumbDb As String In Directory.GetFiles(DestFolder, "*thumbs.db", SearchOption.AllDirectories)
            File.Delete(ThumbDb)
        Next

        ProgressBar1.Value = 80

        RunBSArch(Pack, DestFolder)

        ProgressBar1.Value = 100

        Directory.Delete(SourceFolder, True)
        Directory.Delete(DestFolder, True)
        MsgBox("The operation completed successfully.", MsgBoxStyle.OkOnly, "Information") : ProgressBar1.Value = 0
        StartButton.Enabled = True
    End Sub

    Private Sub RunBSArch(PackMode As Mode, FolderPath As String)
        Dim Args As String = ""
        Select Case PackMode
            Case Pack
                Args = "pack " & Quote(FolderPath) & " " & Quote(RootPath & "\DoA-Redux.bsa") & " -fnv -mt" & IIf(CompressCheckbox.Checked = True, " -z", "")
            Case Unpack
                Args = "unpack " & Quote(RootPath & "\Daughters of Ares.bsa") & " " & Quote(FolderPath) & " -mt -q"
        End Select
        Dim BuildProcess As ProcessStartInfo = New ProcessStartInfo(RootPath & "\BSArch.exe", Args) With {.WorkingDirectory = RootPath, .RedirectStandardOutput = True, .RedirectStandardError = True, .UseShellExecute = False, .CreateNoWindow = True}
        Dim BatchProcess As Process = Process.Start(BuildProcess)
        BatchProcess.WaitForExit()
    End Sub

    Public Sub RunXDelta(PackMode As Mode, SourcePath As String, DestPath As String, XD3Path As String)
        Dim Args As String = ""
        Select Case PackMode
            Case Pack
                Args = "-A= -s " & Quote(SourcePath) & " " & Quote(DestPath) & " " & Quote(XD3Path)
            Case Unpack
                Args = "-d -s " & Quote(SourcePath) & " " & Quote(XD3Path) & " " & Quote(DestPath)
        End Select
        Dim BuildProcess As ProcessStartInfo = New ProcessStartInfo(RootPath & "\xdelta3.exe", Args) With {.WorkingDirectory = RootPath, .RedirectStandardOutput = True, .RedirectStandardError = True, .UseShellExecute = False, .CreateNoWindow = True}
        Dim BatchProcess As Process = Process.Start(BuildProcess)
        BatchProcess.WaitForExit()
    End Sub

    Public Function Quote(Source As String)
        Return """"c & Source & """"c
    End Function

End Class
