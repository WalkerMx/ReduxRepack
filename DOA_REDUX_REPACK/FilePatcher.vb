Imports System.IO
Imports System.IO.Compression

Public Class FilePatcher

    Dim ArmorPatchList As New List(Of Patch)

    Public Structure Patch
        Public Property PatchesList As String()
        Public Property SourceFile As String
        Public Sub New(Patches As String(), Source As String)
            PatchesList = Patches
            SourceFile = Source
        End Sub
    End Structure

    Public Sub New()

        ArmorPatchList.AddRange({New Patch({"1950scasualvariantfemale.xd3", "1950scasualvariantfemaleold.xd3", "1950scasualoldfemale.xd3"}, "\meshes\doa\armor\1950stylecasual01\f\outfitf.nif"),
                            New Patch({"1950scasual02oldf.xd3", "1950scasual2variantfold.xd3"}, "\meshes\doa\armor\1950stylecasual02\outfitf.nif"),
                            New Patch({"1950ssuitdarkf.xd3"}, "\meshes\doa\armor\1950stylesuit\f\outfitf.nif"),
                            New Patch({"rivetcitysecurityuniformf.xd3", "taloncompanyf.xd3", "tenpennysecurityuniformf.xd3", "rileysrangersuniformf.xd3"}, "\meshes\doa\armor\combatarmor\f\outfitf.nif"),
                            New Patch({"rivetcityscientistf.xd3"}, "\meshes\doa\armor\doctorli\f\outfitf.nif"),
                            New Patch({"leopardprintlingerie.xd3"}, "\meshes\doa\armor\lingerie\outfitf.nif"),
                            New Patch({"redjumpsuitf.xd3", "redracerjumpsuitf.xd3"}, "\meshes\doa\armor\robcojumpsuit\outfitf.nif"),
                            New Patch({"vaultsuit108utilitydadf.xd3"}, "\meshes\doa\armor\vaultsuitutility\f\vaultsuitutilitydadf.nif"),
                            New Patch({"wastelanddoctor02f.xd3"}, "\meshes\doa\armor\wastelanddoctor01\outfitf.nif")})

    End Sub

    Public Sub PatchAllFiles(SourceFolder As String, DestFolder As String)
        Dim TempFolder As String = Path.GetTempPath & Path.GetRandomFileName
        Directory.CreateDirectory(TempFolder)
        File.WriteAllBytes(TempFolder & "\Delta.zip", My.Resources.Delta)
        ZipFile.ExtractToDirectory(TempFolder & "\Delta.zip", TempFolder)
        For Each PatchSet As Patch In ArmorPatchList
            For Each XDeltaPatch As String In PatchSet.PatchesList
                Dim Filename As String = Path.GetFileName(PatchSet.SourceFile)
                Dim NewFolderPath As String = Directory.GetParent(DestFolder & PatchSet.SourceFile).FullName & "\" & XDeltaPatch.Replace(".xd3", "")
                Directory.CreateDirectory(NewFolderPath)
                Form1.RunXDelta(Form1.Mode.Unpack, DestFolder & PatchSet.SourceFile, NewFolderPath & "\" & Filename, TempFolder & "\" & XDeltaPatch)
            Next
        Next

        Form1.RunXDelta(Form1.Mode.Unpack, SourceFolder & "\meshes\characters\_male\daughtersofares\eye\eyelefthumanfemale.nif", DestFolder & "\meshes\characters\_male\daughtersofares\eye\eyelefthumanfemale.nif", TempFolder & "\eyelefthumanfemale.xd3")
        Form1.RunXDelta(Form1.Mode.Unpack, SourceFolder & "\meshes\characters\_male\daughtersofares\eye\eyerighthumanfemale.nif", DestFolder & "\meshes\characters\_male\daughtersofares\eye\eyerighthumanfemale.nif", TempFolder & "\eyerighthumanfemale.xd3")
        Form1.RunXDelta(Form1.Mode.Unpack, SourceFolder & "\textures\characters\daughtersofares\eye\green.dds", DestFolder & "\textures\characters\daughtersofares\eye\green.dds", TempFolder & "\greenold-green.xd3")
        Form1.RunXDelta(Form1.Mode.Unpack, DestFolder & "\textures\characters\daughtersofares\eye\green.dds", DestFolder & "\textures\characters\daughtersofares\eye\blue.dds", TempFolder & "\green-blue.xd3")
        Form1.RunXDelta(Form1.Mode.Unpack, DestFolder & "\textures\characters\daughtersofares\eye\green.dds", DestFolder & "\textures\characters\daughtersofares\eye\indigo.dds", TempFolder & "\green-indigo.xd3")
        Form1.RunXDelta(Form1.Mode.Unpack, DestFolder & "\textures\characters\daughtersofares\eye\green.dds", DestFolder & "\textures\characters\daughtersofares\eye\orange.dds", TempFolder & "\green-orange.xd3")
        Form1.RunXDelta(Form1.Mode.Unpack, DestFolder & "\textures\characters\daughtersofares\eye\green.dds", DestFolder & "\textures\characters\daughtersofares\eye\pink.dds", TempFolder & "\green-pink.xd3")
        Form1.RunXDelta(Form1.Mode.Unpack, DestFolder & "\textures\characters\daughtersofares\eye\green.dds", DestFolder & "\textures\characters\daughtersofares\eye\red.dds", TempFolder & "\green-red.xd3")
        Form1.RunXDelta(Form1.Mode.Unpack, DestFolder & "\textures\characters\daughtersofares\eye\green.dds", DestFolder & "\textures\characters\daughtersofares\eye\violet.dds", TempFolder & "\green-violet.xd3")
        Form1.RunXDelta(Form1.Mode.Unpack, DestFolder & "\textures\characters\daughtersofares\eye\green.dds", DestFolder & "\textures\characters\daughtersofares\eye\white.dds", TempFolder & "\green-white.xd3")
        Form1.RunXDelta(Form1.Mode.Unpack, DestFolder & "\textures\characters\daughtersofares\eye\green.dds", DestFolder & "\textures\characters\daughtersofares\eye\yellow.dds", TempFolder & "\green-yellow.xd3")

        Directory.Delete(TempFolder, True)
    End Sub

End Class
