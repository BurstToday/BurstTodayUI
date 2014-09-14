Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.IO.Compression

Public Class Form1
    Dim WithEvents wc As System.Net.WebClient
    Dim WithEvents wc2 As System.Net.WebClient
    Dim Version As String = "v1.0"
    Dim WalletDL As Integer = 0
    Dim MinerDL As Integer = 0

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        'read settings
        If My.Computer.FileSystem.FileExists("C:\Burst.Today\passphrases.txt") Then
            Dim Reader() As String = System.IO.File.ReadAllLines("C:\Burst.Today\passphrases.txt")
            TextBox1.Text = Reader(0)
            Clipboard.SetText(Reader(0))
        End If

        If My.Computer.FileSystem.FileExists("C:\Burst.Today\address.txt") Then
            Dim Reader2() As String = System.IO.File.ReadAllLines("C:\Burst.Today\address.txt")
            Dim Tempstring As String = Reader2(0).Remove(0, InStr(Reader2(0), "->") + 1)
            Tempstring = Tempstring.Trim
            Label3.Text = "#" & Tempstring
        Else
            TextBox1.ReadOnly = False
        End If

        If My.Computer.FileSystem.FileExists("C:\Burst.Today\Installs.txt") Then
            Dim Reader3() As String = System.IO.File.ReadAllLines("C:\Burst.Today\Installs.txt")
            Dim Reader3Length As Integer = Reader3.Length
            Dim n As Integer = 0
            While n < Reader3Length
                CheckedListBox1.Items.Add(Reader3(n))
                n = n + 1
            End While
        Else
            TextBox1.ReadOnly = False
        End If



        Me.CenterToScreen()
        Me.Text = Me.Text & " - " & Version
        Button3.PerformClick()


        Dim allDrives() As DriveInfo = DriveInfo.GetDrives()
        Dim d As DriveInfo
        For Each d In allDrives
            If d.IsReady = True Then
                If d.AvailableFreeSpace > 0 Then
                    ComboBox2.Items.Add(d.Name & " - " & Math.Round(d.AvailableFreeSpace * (10 ^ -9), 0) & " GB available")

                End If

            End If
        Next

        ComboBox2.SelectedIndex = 0

        Dim coreCount As Integer = System.Environment.ProcessorCount
        Dim Loopz As Integer = 1
        While Loopz < coreCount + 1
            ComboBox1.Items.Add(Loopz)
            Loopz = Loopz + 1
        End While
        ComboBox1.SelectedIndex = 0





    End Sub


    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click

        '----------------------NOTICE OF FEE-------------------------
        Dim result As Integer = MessageBox.Show("Burst.Today Implements a 5% fee to cover the cost of development." & vbCrLf & vbCrLf & "Because of you are an early adopter, this fee is waived." & vbCrLf & vbCrLf & "Do you agree to this fee?", "Agree to terms", MessageBoxButtons.YesNo)
        If result = DialogResult.No Then
            MessageBox.Show("You must accept the agreement to proceed.")
            Exit Sub

        ElseIf result = DialogResult.Yes Then
            '  MessageBox.Show("Yes pressed")
        End If
        '----------------------END NOTICE OF FEE-------------------------

        'files are already downloaded, now I'm ready to create an install

        '---------------------SET INSTALL LOCATION-----------------------
        Dim SelectedDrive As String = ComboBox2.Text.Remove(InStr(ComboBox2.Text, " - "))
        SelectedDrive = SelectedDrive.Trim
        'Create a subfolder if there is not one, Crowetic says this will work
        If My.Computer.FileSystem.DirectoryExists(SelectedDrive & "Burst.Today.Install\") Then
        Else
            My.Computer.FileSystem.CreateDirectory(SelectedDrive & "Burst.Today.Install\")
        End If
        'Set the selected spot to the locoation here. 
        SelectedDrive = SelectedDrive & "Burst.Today.Install\"
        '---------------------END INSTALL LOCATION-----------------------

        '-------------------------------------COPY UPDATED FILES TO INSTALL LOCATION
        'Miner
        My.Computer.FileSystem.CopyDirectory("C:\Burst.Today\Miner\pocminer-master", SelectedDrive, True)
        'Wallet
        My.Computer.FileSystem.CopyDirectory("C:\Burst.Today\Wallet\burstcoin-master\", SelectedDrive & "Burst_1.0.0\", True)

        'fix the 

        '-------------------------------------END COPY UPDATED FILES TO INSTALL LOCATION


        '--------------PATCH THE BATCH FILES---------------
        ' Make a reference to a directory.
        Dim di As New DirectoryInfo(SelectedDrive)
        ' Get a reference to each file in that directory.
        Dim fiArr As FileInfo() = di.GetFiles()
        ' Display the names of the files.
        Dim fri As FileInfo
        For Each fri In fiArr

            If fri.Name.EndsWith(".bat") Then
                Dim Reader() As String = System.IO.File.ReadAllLines(fri.FullName)
                Dim Readersize As Integer = Reader.Length
                Dim looper As Integer = 0
                While looper < Readersize
                    'go through each of the files and do a replace 
                    Reader(looper) = Reader(looper).Replace("java -", "C:\Windows\SysWOW64\java -")
                    Reader(looper) = Reader(looper).Replace("Xmx4000m -", "Xmx" & TextBox4.Text & "m -")
                    looper = looper + 1
                End While
                ' MsgBox("File =" & fri.Name)
                System.IO.File.WriteAllLines(fri.FullName, Reader)
            End If

            Console.WriteLine(fri.Name)
        Next fri
        '--------------END PATCH THE BATCH FILES---------------

        If 1 = 3 Then
            '--------------PATCH THE BATCH FILES2---------------
            ' Make a reference to a directory.
            Dim di2 As New DirectoryInfo(SelectedDrive & "Burst_1.0.0\")

            ' Get a reference to each file in that directory.
            Dim fiArr2 As FileInfo() = di.GetFiles()
            ' Display the names of the files.
            Dim fri2 As FileInfo
            For Each fri2 In fiArr2

                If fri2.Name.EndsWith(".bat") Then
                    Dim Reader() As String = System.IO.File.ReadAllLines(fri2.FullName)
                    Dim Readersize As Integer = Reader.Length
                    Dim looper As Integer = 0
                    While looper < Readersize
                        'go through each of the files and do a replace 
                        Reader(looper) = Reader(looper).Replace("java -", "C:\Windows\SysWOW64\java -")
                        Reader(looper) = Reader(looper).Replace("Xmx4000m -", "Xmx" & TextBox4.Text & "m -")
                        looper = looper + 1
                    End While
                    ' MsgBox("File =" & fri.Name)
                    System.IO.File.WriteAllLines(fri2.FullName, Reader)
                End If

                Console.WriteLine(fri2.Name)
            Next fri2
            '--------------END PATCH THE BATCH FILES---------------
        End If


        '------------------------------------IF THERE IS NO ACCOUNT #, MAKE ONE
        Dim Address As String = ""
        If Label3.Text.StartsWith("#") Then
            'then there is already an account, copy the file
            My.Computer.FileSystem.CopyFile("C:\Burst.Today\passphrases.txt", SelectedDrive & "passphrases.txt", True)
            My.Computer.FileSystem.CopyFile("C:\Burst.Today\address.txt", SelectedDrive & "address.txt", True)
        Else
            'There is no account, create one
            '---------CREATE PW
            Dim PassPhrasesTXT(3) As String
            PassPhrasesTXT(0) = TextBox1.Text
            System.IO.File.WriteAllLines(SelectedDrive & "passphrases.txt", PassPhrasesTXT)
            System.IO.File.WriteAllLines("C:\Burst.Today\passphrases.txt", PassPhrasesTXT)
            '---------END CREATE PW

            '----------CREATE NEW ADDRESS
            Try
                Dim procInfo As New ProcessStartInfo()
                'procInfo.UseShellExecute = True
                procInfo.UseShellExecute = False
                Dim JavaExe As String = SelectedDrive & "run_dump_address.bat"
                procInfo.FileName = (JavaExe)
                procInfo.WorkingDirectory = SelectedDrive
                procInfo.Verb = "runas"
                Process.Start(procInfo).WaitForExit()

                Me.BringToFront()

            Catch ex As Exception
                MsgBox(ex.Message)
            End Try

            System.Threading.Thread.Sleep(1000)

            Dim ReadAddress() As String = System.IO.File.ReadAllLines(SelectedDrive & "address.txt")
            ' Dim Address As String = ReadAddress(0)
            Address = ReadAddress(0)
            Address = Address.Remove(0, InStr(Address, "->") + 2)
            Address = Address.Trim

            ' MsgBox("address=" & Address)
            Dim WriteAddress(5) As String
            WriteAddress(0) = Address

            'it might be a different drive, but we run from C:\
            System.IO.File.WriteAllLines("C:\Burst.Today\address.txt", WriteAddress)



            '----------END CREATE NEW ADDRESS

        End If
        '------------------------------------END IF THERE IS NO ACCOUNT #, MAKE ONE

        'zzzzzz
        System.Threading.Thread.Sleep(2500)





        '------------------WRITE RUN_GENERATE.BAT FOR THIS PLOT - GENERATES A PLOT
        Dim RunGenerate(5) As String
        RunGenerate(0) = "C:\Windows\SysWOW64\java -Xmx" & TextBox4.Text & "m -cp pocminer.jar;lib/*;lib/akka/*;lib/jetty/* pocminer.POCMiner generate " & Address & " " & TextBox2.Text & " " & TrackBar1.Value * 4000 & " 1000 " & ComboBox1.Text
        System.IO.File.WriteAllLines(SelectedDrive & "run_generate.bat", RunGenerate)

        System.Threading.Thread.Sleep(2000)

        Try
            Dim procInfo As New ProcessStartInfo()
            'procInfo.UseShellExecute = True
            procInfo.UseShellExecute = False
            Dim JavaExe As String = SelectedDrive & "run_generate.bat"
            procInfo.FileName = (JavaExe)
            procInfo.WorkingDirectory = SelectedDrive
            procInfo.Verb = "runas"
            Process.Start(procInfo)
            Me.BringToFront()

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
        '------------------END WRITE RUN_GENERATE.BAT FOR THIS PLOT - NEW PLOT IS GENERATED

        'zzzzz
        System.Threading.Thread.Sleep(2000)


        '------------------OK SO RUN.BAT WILL OPEN AND CLOSE QUICKLY
        Try
            Dim procInfo As New ProcessStartInfo()
            'procInfo.UseShellExecute = True
            procInfo.UseShellExecute = False
            Dim JavaExe As String = SelectedDrive & "run.bat"
            procInfo.FileName = (JavaExe)
            procInfo.WorkingDirectory = SelectedDrive
            procInfo.Verb = "runas"
            Process.Start(procInfo)
            Me.BringToFront()

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

        System.Threading.Thread.Sleep(2000)

        '------------------OK SO .\BURST1.0.0\RUN.BAT WILL INSTALL
        Try
            Dim procInfo As New ProcessStartInfo()
            'procInfo.UseShellExecute = True
            procInfo.UseShellExecute = False
            Dim JavaExe As String = SelectedDrive & "run.bat"
            procInfo.FileName = (JavaExe)
            procInfo.WorkingDirectory = SelectedDrive
            procInfo.Verb = "runas"
            Process.Start(procInfo)
            Me.BringToFront()

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

        'Wallet is open already

        System.Threading.Thread.Sleep(2000)

        '-----------------------RUN THE MINER
        Try
            Dim procInfo As New ProcessStartInfo()
            'procInfo.UseShellExecute = True
            procInfo.UseShellExecute = False
            Dim JavaExe As String = SelectedDrive & "run_mine.bat"
            procInfo.FileName = (JavaExe)
            procInfo.WorkingDirectory = SelectedDrive
            procInfo.Verb = "runas"
            Process.Start(procInfo)
            Me.BringToFront()

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try


        'The Final Step is to add this plot to the list so it runs on startup
        Dim myplots() As String
        Dim myplotscount As Integer = 0
        For Each ListItem In CheckedListBox1.Items
            ReDim Preserve myplots(myplotscount)
            myplots(myplotscount) = ListItem
            myplotscount = myplotscount
        Next

        ReDim Preserve myplots(myplotscount)
        myplots(myplotscount) = SelectedDrive
        myplotscount = myplotscount

        System.IO.File.WriteAllLines("C:\Burst.Today\Installs.txt", myplots)


    End Sub



    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        'download files from Github


        If My.Computer.FileSystem.DirectoryExists("C:\Burst.Today") Then

        Else
            My.Computer.FileSystem.CreateDirectory("C:\Burst.Today")
        End If

        System.Threading.Thread.Sleep(500)

        'Wallet
        Dim URL As String = "https://github.com/BurstProject/burstcoin/archive/master.zip"
        Dim Path As String = "C:\Burst.Today\Wallet.zip"
        wc = New System.Net.WebClient

        'wc.DownloadFile(New Uri(URL), Path)
        wc.DownloadFileAsync(New Uri(URL), Path)
        ProgressBar2.Value = 0
        ProgressBar2.Maximum = 110
        System.Threading.Thread.Sleep(500)

        'Miner
        Dim URL2 As String = "https://github.com/BurstProject/pocminer/archive/master.zip"
        Dim Path2 As String = "C:\Burst.Today\Miner.zip"
        wc2 = New System.Net.WebClient
        'wc2.DownloadFile(New Uri(URL2), Path2)
        wc2.DownloadFileAsync(New Uri(URL2), Path2)
        ProgressBar3.Value = 0
        ProgressBar3.Maximum = 110
        Application.DoEvents()



    End Sub
    Private Sub DownloadProgress(ByVal sender As Object, ByVal e As System.Net.DownloadProgressChangedEventArgs) Handles wc.DownloadProgressChanged
        ProgressBar2.Value = e.ProgressPercentage
    End Sub
    Private Sub DownloadProgress2(ByVal sender As Object, ByVal e As System.Net.DownloadProgressChangedEventArgs) Handles wc2.DownloadProgressChanged
        ProgressBar3.Value = e.ProgressPercentage
    End Sub



    Private Sub wc_DownloadFileCompleted(sender As Object, e As System.ComponentModel.AsyncCompletedEventArgs) Handles wc.DownloadFileCompleted
        Try
            If e.Cancelled Then

            ElseIf e.Error IsNot Nothing Then
                MessageBox.Show(e.Error.ToString(), "Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Else
                'Extract the wallet
                Using Zippo As Ionic.Zip.ZipFile = Ionic.Zip.ZipFile.Read("C:\Burst.Today\Wallet.zip")
                    System.Threading.Thread.Sleep(100)
                    Zippo.ExtractAll("C:\Burst.Today\Wallet\", Ionic.Zip.ExtractExistingFileAction.OverwriteSilently)
                    ProgressBar2.Value = 110
                    WalletDL = 1
                    System.Threading.Thread.Sleep(500)
                    ProgressBar2.ForeColor = Color.Green
                    ProgressBar2.Refresh()

                    If MinerDL = 1 Then
                        Label8.Text = "Download Complete!"
                        Label8.Refresh()
                        System.Threading.Thread.Sleep(1000)
                        StartWalletClient()

                    End If
                End Using
            End If
        Catch ex As Exception
            ProgressBar2.ForeColor = Color.Red
            ProgressBar2.Refresh()


        End Try
       
    End Sub

    Private Sub wc2_DownloadFileCompleted(sender As Object, e As System.ComponentModel.AsyncCompletedEventArgs) Handles wc2.DownloadFileCompleted
        Try
            If e.Cancelled Then

            ElseIf e.Error IsNot Nothing Then
                MessageBox.Show(e.Error.ToString(), "Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Else
                'Extract the Miner
                Using Zippo As Ionic.Zip.ZipFile = Ionic.Zip.ZipFile.Read("C:\Burst.Today\Miner.zip")
                    System.Threading.Thread.Sleep(100)
                    Zippo.ExtractAll("C:\Burst.Today\Miner\", Ionic.Zip.ExtractExistingFileAction.OverwriteSilently)
                    ProgressBar3.Value = 110


                    MinerDL = 1
                    System.Threading.Thread.Sleep(500)
                    ProgressBar3.ForeColor = Color.DarkGreen
                    ProgressBar3.Refresh()
                    If WalletDL = 1 Then
                        Label8.Text = "Download Complete!"
                        Label8.Refresh()
                        System.Threading.Thread.Sleep(1000)
                        StartWalletClient()

                    End If
                End Using
            End If
        Catch ex As Exception
            ProgressBar3.ForeColor = Color.Red
            ProgressBar3.Refresh()


        End Try
    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked


        Process.Start("Http://www.burst.today")

    End Sub


    Private Sub StartWalletClient()
        System.Threading.Thread.Sleep(500)
        If My.Computer.FileSystem.FileExists("C:\Burst.Today\Wallet\burstcoin-master\burst.jar") Then
            Try
                'If 1 = 2 Then
                '    Dim procInfo As New ProcessStartInfo()
                '    procInfo.UseShellExecute = True

                '    Dim JavaExe As String = ""
                '    If My.Computer.FileSystem.FileExists("C:\Program Files\Java\jre7\bin\java.exe") Then
                '        JavaExe = "C:\Program Files\Java\jre7\bin\java.exe"
                '    End If

                '    If My.Computer.FileSystem.FileExists("C:\Program Files (x86)\Java\jre7\bin\java.exe") Then
                '        JavaExe = "C:\Program Files (x86)\Java\jre7\bin\java.exe"
                '    End If
                '    'procInfo.FileName = ("C:\Program Files\Java\jre7\bin\java.exe")
                '    procInfo.FileName = (JavaExe)
                '    procInfo.WorkingDirectory = "C:\Burst.Today\Wallet\burstcoin-master\"
                '    procInfo.Verb = "runas"
                '    procInfo.Arguments = "-cp burst.jar;lib\*;conf; nxt.Nxt"
                'End If

                Dim procInfo As New ProcessStartInfo()
                procInfo.UseShellExecute = False
                Dim JavaExe As String = "C:\Burst.Today\Wallet\burstcoin-master\run.bat"
                procInfo.FileName = (JavaExe)
                procInfo.Verb = "runas"
                procInfo.WorkingDirectory = "C:\Burst.Today\Wallet\burstcoin-master\"
                Process.Start(procInfo)

                Me.BringToFront()

                PictureBox2.Visible = True
                PictureBox2.Refresh()


                System.Threading.Thread.Sleep(2500)
                ' WebBrowser2.Refresh()
                System.Threading.Thread.Sleep(2500)




                TabControl1.SelectedIndex = 1
                TabControl1.Refresh()
                Button2.Refresh()


                Button1.PerformClick()


                Me.BringToFront()

            Catch ex As Exception
                MsgBox(ex.Message)
            End Try

        End If
    End Sub

    Private Sub TabControl1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles TabControl1.SelectedIndexChanged
        Me.BringToFront()



    End Sub

   
    
    Private Sub ComboBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox2.SelectedIndexChanged
        Dim Tempstring As String = ComboBox2.Text.Remove(0, InStr(ComboBox2.Text, " - ") + 2)
        Tempstring = Tempstring.Trim
        Tempstring = Tempstring.Replace("GB available", "")
        Tempstring = Tempstring.Trim

        Label4.Text = "Space to use (" & TrackBar1.Value & " / " & Tempstring & " GB)"

        TrackBar1.Maximum = Tempstring
        TrackBar1.Value = 0

    End Sub

   
    Private Sub TrackBar1_Scroll(sender As Object, e As EventArgs) Handles TrackBar1.Scroll
        Dim Tempstring As String = ComboBox2.Text.Remove(0, InStr(ComboBox2.Text, " - ") + 2)
        Tempstring = Tempstring.Trim
        Tempstring = Tempstring.Replace("GB available", "")
        Tempstring = Tempstring.Trim

        Label4.Text = "Space to use (" & TrackBar1.Value & " / " & Tempstring & " GB)"
    End Sub

   
    
    
   
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        System.Threading.Thread.Sleep(2000)
        PictureBox1.Visible = True
        PictureBox1.Refresh()

        System.Threading.Thread.Sleep(1000)

       

        'Clipboard.GetText()

        Process.Start("http://localhost:8125/", Clipboard.GetText())

        System.Threading.Thread.Sleep(1000)

        TabControl1.SelectedIndex = 2


        System.Threading.Thread.Sleep(1000)

        Button4.PerformClick()




    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click


        'START MINERS
        Dim xLoop As Integer = 0
        System.Threading.Thread.Sleep(500)
        For Each item In CheckedListBox1.Items

            xLoop = xLoop + 1
            Process.Start(item & "run_mine.bat")


        Next
        TabControl1.SelectedIndex = 3
        TabControl1.Refresh()

        System.Threading.Thread.Sleep(5000)
        'End StartingMiners



    End Sub

   
End Class
