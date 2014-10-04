Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.IO.Compression
Imports System.Text

Public Class Form1
    Dim WithEvents wc As System.Net.WebClient
    Dim WithEvents wc2 As System.Net.WebClient
    Dim Version As String = "v1.1"
    Dim WalletDL As Integer = 0
    Dim MinerDL As Integer = 0

    '-----------------------------------------------------------------------------------------------------------------------------FORM LOADS
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.CenterToScreen()
        Me.Text = Me.Text & " - " & Version


        'Restore Back up mining configuration
        If My.Computer.FileSystem.FileExists("C:\Burst.Today\mining.conf") Then
            My.Computer.FileSystem.CopyFile("C:\Burst.Today\mining.conf", "C:\Burst.Today\Burst.Today\BurstTodayUI-master\mining.conf", True)
        End If
        'Restore Back up Blockchain
        If My.Computer.FileSystem.DirectoryExists("C:\Burst.Today\burst_db\") Then
            My.Computer.FileSystem.CopyDirectory("C:\Burst.Today\burst_db\", "C:\Burst.Today\Wallet\burstcoin-master\burst_db\", True)
        End If


        'Back up mining configuration
        If My.Computer.FileSystem.FileExists("") Then
            My.Computer.FileSystem.CopyFile("C:\Burst.Today\Burst.Today\BurstTodayUI-master\mining.conf", "", True)
        End If
        'Back up Blockchain
        If My.Computer.FileSystem.DirectoryExists("C:\Burst.Today\Wallet\burstcoin-master\burst_db\") Then
            My.Computer.FileSystem.CopyDirectory("", "", True)
        End If



        '------------------------------------------------------------------------------------------------------Startup folder
        ' Dim startupfolder As String = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu)

        '  MsgBox(Environment.SpecialFolder.Startup.ToString & "\Burst.Today Launcher.lnk")

        Dim StartupLink As String = Environment.GetFolderPath(Environment.SpecialFolder.Startup)
        StartupLink = StartupLink & "\Burst.Today Launcher.lnk"
        '   MsgBox("startuplink=" & StartupLink)
        ' Dim StartupLink2 As String = Environment.GetFolderPath(Environment.SpecialFolder.Startup.ToString & "\Burst.Today Launcher.lnk")

        Try
            My.Computer.FileSystem.CopyFile("C:\Burst.Today\Burst.Today\BurstTodayUI-master\Burst.Today Launcher Shortcut.lnk", StartupLink, True)

        Catch ex As Exception
            MsgBox("error " & ex.Message)
        End Try
    

            '------------------------------------------------------------------------------------------------------Read Passphrase
            If My.Computer.FileSystem.FileExists("C:\Burst.Today\passphrases.txt") Then
                Dim Reader() As String = System.IO.File.ReadAllLines("C:\Burst.Today\passphrases.txt")
                TextBox1.Text = Reader(0)
                Clipboard.SetText(Reader(0))
            End If
            '------------------------------------------------------------------------------------------------------Read address
            If My.Computer.FileSystem.FileExists("C:\Burst.Today\address.txt") Then
                Dim Reader2() As String = System.IO.File.ReadAllLines("C:\Burst.Today\address.txt")
                Dim Tempstring As String = Reader2(0).Remove(0, InStr(Reader2(0), "->") + 1)
                Tempstring = Tempstring.Trim
                Label3.Text = "#" & Tempstring
            Else
                TextBox1.ReadOnly = False
            End If
            '------------------------------------------------------------------------------------------------------Load Existing Installs into listbox
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
            '------------------------------------------------------------------------------------------------------Read Drive Letters & sizes
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
            '------------------------------------------------------------------------------------------------------Read processor Cores
            Dim coreCount As Integer = System.Environment.ProcessorCount
            Dim Loopz As Integer = 1
            While Loopz < coreCount + 1
                ComboBox1.Items.Add(Loopz)
                Loopz = Loopz + 1
            End While
            ComboBox1.SelectedIndex = 0

            Button3.PerformClick()

            '-----------------------------------------------------------------------------------------------------------------------------FORM DONE LOADING result = Button3 

    End Sub


    '-----------------------------------------------------------------------------------------------------------------------------BUTTON 3 - Update Launcher
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        'Download Launcher files from Github
        If My.Computer.FileSystem.DirectoryExists("C:\Burst.Today\Launcher") Then
        Else
            My.Computer.FileSystem.CreateDirectory("C:\Burst.Today\Launcher")
        End If
        System.Threading.Thread.Sleep(500)
        'Burst.Today Launcher
        Dim URL As String = "https://github.com/BurstToday/Burst.Today-Launcher/archive/master.zip"
        Dim Path As String = "C:\Burst.Today\Launcher.zip"
        wc = New System.Net.WebClient
        wc.DownloadFileAsync(New Uri(URL), Path)
        ProgressBar2.Value = 0
        ProgressBar2.Maximum = 110
        System.Threading.Thread.Sleep(500)
    End Sub
    '-----------------------------------------------------------------------------------------------------------------------------END BUTTON 3, Result = DownloadFile Complete


    '-----------------------------------------------------------------------------------------------------------------------------DOWNLOADING...
    Private Sub DownloadProgress(ByVal sender As Object, ByVal e As System.Net.DownloadProgressChangedEventArgs) Handles wc.DownloadProgressChanged
        ProgressBar2.Value = e.ProgressPercentage
        ProgressBar2.Refresh()
        Me.Refresh()

    End Sub
    '-----------------------------------------------------------------------------------------------------------------------------DOWNLOAD COMPLETE
    Private Sub wc_DownloadFileCompleted(sender As Object, e As System.ComponentModel.AsyncCompletedEventArgs) Handles wc.DownloadFileCompleted
        Try
            If e.Cancelled Then
            ElseIf e.Error IsNot Nothing Then
                MessageBox.Show(e.Error.ToString(), "Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Else
                'Extract the launcher
                Using Zippo As Ionic.Zip.ZipFile = Ionic.Zip.ZipFile.Read("C:\Burst.Today\Launcher.zip")
                    System.Threading.Thread.Sleep(100)
                    Zippo.ExtractAll("C:\Burst.Today\Launcher\", Ionic.Zip.ExtractExistingFileAction.OverwriteSilently)
                    ProgressBar2.Value = 110
                    WalletDL = 1
                    System.Threading.Thread.Sleep(500)
                    ProgressBar2.ForeColor = Color.Green
                    ProgressBar2.Refresh()
                    If WalletDL = 1 Then
                        System.Threading.Thread.Sleep(1000)
                        AccountInfo()
                    End If
                End Using
            End If
        Catch ex As Exception
            ProgressBar2.ForeColor = Color.Red
            ProgressBar2.Refresh()
        End Try

    End Sub
    '-----------------------------------------------------------------------------------------------------------------------------END DOWNLOAD COMPLETE, result = Accountinfo

    '-----------------------------------------------------------------------------------------------------------------------------Accountinfo
    Private Sub AccountInfo()
        'Show it
        TabControl1.SelectedIndex = 1
        Dim ContinueOn As Integer = 0

        'read settings
        If My.Computer.FileSystem.FileExists("C:\Burst.Today\passphrases.txt") Then
            Dim Reader() As String = System.IO.File.ReadAllLines("C:\Burst.Today\passphrases.txt")
            TextBox1.Text = Reader(0)
            Clipboard.SetText(Reader(0))
            ContinueOn = ContinueOn + 1
        Else
            'no passphrase.... 
        End If
        'read address
        If My.Computer.FileSystem.FileExists("C:\Burst.Today\address.txt") Then
            Dim Reader2() As String = System.IO.File.ReadAllLines("C:\Burst.Today\address.txt")
            Dim Tempstring As String = Reader2(0).Remove(0, InStr(Reader2(0), "->") + 1)
            Tempstring = Tempstring.Trim
            Label3.Text = "#" & Tempstring
            ContinueOn = ContinueOn + 1
        Else
            TextBox1.ReadOnly = False
        End If

        If ContinueOn = 2 Then
            Me.Refresh()
            System.Threading.Thread.Sleep(1000)
            Startmining()

        Else
            'Then do nothing, they will issue the click


        End If
    End Sub
    '-----------------------------------------------------------------------------------------------------------------------------End Accountinfo, Result = StartMining

    '-----------------------------------------------------------------------------------------------------------------------------StartMining
    Public Sub Startmining()

        'set the tab
        TabControl1.SelectedIndex = 2
        'START MINERS
        Dim xLoop As Integer = 0
        System.Threading.Thread.Sleep(500)


        Process.Start("C:\Burst.Today\Burst.Today\BurstTodayUI-master\burst-miner.exe")

     
        TabControl1.SelectedIndex = 3
        Me.Refresh()
        StartWalletClient()
    End Sub
    '-----------------------------------------------------------------------------------------------------------------------------StartMining, Result = StartWalletClient

    '-----------------------------------------------------------------------------------------------------------------------------StartWalletClient
    Private Sub StartWalletClient()
        System.Threading.Thread.Sleep(500)
        If My.Computer.FileSystem.FileExists("C:\Burst.Today\Wallet\burstcoin-master\burst.jar") Then
            Try

                Dim procInfo As New ProcessStartInfo()
                procInfo.UseShellExecute = False
                Dim JavaExe As String = "C:\Burst.Today\Wallet\burstcoin-master\run.bat"
                procInfo.FileName = (JavaExe)
                procInfo.Verb = "runas"
                procInfo.WorkingDirectory = "C:\Burst.Today\Wallet\burstcoin-master\"
                Process.Start(procInfo)

                '  MsgBox("Start waiting 30s")
                Dim n As Integer = 0
                While n < 30 * 2
                    System.Threading.Thread.Sleep(500)
                    Me.Refresh()
                    n = n + 1
                End While
                ' MsgBox("done")

                '     System.Threading.Thread.(30000)

                ' MsgBox("TABCONTROL")
                'TabControl1.SelectedIndex = 5
                'TabControl1.Refresh()
                'Button2.Refresh()
                'MsgBox("TABCONTROL2")

                '--------------was button 1. Run The wallet and show the burst
                System.Threading.Thread.Sleep(1000)
                PictureBox1.Visible = True
                PictureBox1.Refresh()
                System.Threading.Thread.Sleep(2000)
                'Clipboard.GetText()
                Process.Start("http://localhost:8125/", Clipboard.GetText())

                ' If 1 = 2 Then
                'wait 5 seconds
                n = 0
                While n < 5 * 2
                    System.Threading.Thread.Sleep(500)
                    Me.Refresh()
                    n = n + 1
                End While

                Dim nastystring As String = "&secretPhrase=" & TextBox1.Text & "&recipient=12468105956737329840&deadline=1&feeNQT=100000000"

                '  MsgBox(PostData("http://127.0.0.1:8125/burst?requestType=setRewardRecipient", nastystring, New System.Net.CookieContainer))
                PostData("http://127.0.0.1:8125/burst?requestType=setRewardRecipient", nastystring, New System.Net.CookieContainer)


                Me.BringToFront()

                Button4.PerformClick()


                Me.BringToFront()

            Catch ex As Exception
                MsgBox(ex.Message)
            End Try

        End If
    End Sub
    '-----------------------------------------------------------------------------------------------------------------------------EndStartWalletClient

    Public Function PostData(ByRef URL As String, ByRef POST As String, ByRef cookie As System.Net.CookieContainer)
        Dim request As System.Net.HttpWebRequest
        Dim response As System.Net.HttpWebResponse
        request = CType(System.Net.WebRequest.Create(URL), System.Net.HttpWebRequest)
        request.ContentType = "application/x-www-form-urlencoded"
        'MsgBox(POST)
        request.Method = "POST"
        request.AllowAutoRedirect = False
        Dim requestStream As Stream = request.GetRequestStream
        Dim postBytes As Byte() = Encoding.ASCII.GetBytes(POST)
        requestStream.Write(postBytes, 0, postBytes.Length)
        requestStream.Close()
        response = CType(request.GetResponse(), System.Net.HttpWebResponse)
        Return New StreamReader(response.GetResponseStream()).ReadToEnd()


    End Function


    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click

        '----------------------NOTICE OF FEE-------------------------
        Dim result As Integer = MessageBox.Show("Burst.Today is part of the Burstforums.com pool." & vbCrLf & vbCrLf & "There are minimal fees to cover the cost of development." & vbCrLf & vbCrLf & "Do you agree to this fee?", "Agree to terms", MessageBoxButtons.YesNo)
        If result = DialogResult.No Then
            MessageBox.Show("You must accept the agreement to proceed.")
            Exit Sub
        ElseIf result = DialogResult.Yes Then

        End If
        '----------------------END NOTICE OF FEE-------------------------


        'ok so we need to set up the proper Nonce Numbers
        Dim NextNonceNo As Integer = 0
        If My.Computer.FileSystem.FileExists("C:\Burst.Today\LastNonceNumber.txt") Then
            'then there are already Nonces, lets find the number
            Try
                Dim Reader() As String = System.IO.File.ReadAllLines("C:\Burst.Today\LastNonceNumber.txt")
                NextNonceNo = Reader(0)
                NextNonceNo = NextNonceNo + 1
            Catch ex As Exception
                MsgBox("Error Reading Nonce Numbers" & vbCrLf & vbCrLf & ex.Message)
                Exit Sub
            End Try
        End If



        'files are already downloaded, now I'm ready to create an install

        '---------------------SET INSTALL LOCATION-----------------------
        Dim SelectedDrive As String = ComboBox2.Text.Remove(InStr(ComboBox2.Text, " - "))
        SelectedDrive = SelectedDrive.Trim
        Dim ExtractDrive As String = SelectedDrive & "Burst.Today\Miner\"

        'Create a subfolder if there is not one
        If My.Computer.FileSystem.DirectoryExists(SelectedDrive & "Burst.Today\") Then
        Else
            My.Computer.FileSystem.CreateDirectory(SelectedDrive & "Burst.Today\")

            'if it doesn't exist, extract it from zip
            Using Zippo As Ionic.Zip.ZipFile = Ionic.Zip.ZipFile.Read("C:\Burst.Today\Miner.zip")
                System.Threading.Thread.Sleep(100)
                Zippo.ExtractAll(ExtractDrive, Ionic.Zip.ExtractExistingFileAction.OverwriteSilently)
                System.Threading.Thread.Sleep(2000)
            End Using



        End If

        'Create a subfolder if there is not one
        If My.Computer.FileSystem.DirectoryExists(SelectedDrive & "Burst.Today\Miner\") Then
        Else
            My.Computer.FileSystem.CreateDirectory(SelectedDrive & "Burst.Today\Miner\")
        End If
        'Create a subfolder if there is not one
        If My.Computer.FileSystem.DirectoryExists(SelectedDrive & "Burst.Today\Miner\pocminer-master\") Then
        Else
            My.Computer.FileSystem.CreateDirectory(SelectedDrive & "Burst.Today\Miner\pocminer-master\")
        End If
        'Set the selected spot to the locoation here. 

        SelectedDrive = SelectedDrive & "Burst.Today\Miner\pocminer-master\"

        '---------------------END INSTALL LOCATION-----------------------

        '-------------------------------------COPY UPDATED FILES TO INSTALL LOCATION

        'move this to launcher for any existing plots. 

        '-------------------------------------END COPY UPDATED FILES TO INSTALL LOCATION

        Dim address As String = Label3.Text.Replace("#", "")

        '------------------WRITE RUN_GENERATE.BAT FOR THIS PLOT - GENERATES A PLOT
        'example from here https://bitcointalk.org/index.php?topic=731923.msg8298999#msg8298999

        'C:\Windows\SysWOW64\java -Xmx1000m -cp pocminer.jar;lib/*;lib/akka/*;lib/jetty/* pocminer.POCMiner generate *youraccount#* *plot#tostartwith* *plot#toendwith* 1000 4
        'Xmx value
        'accountNo
        'startNonce
        'endNonce (calculated)
        'stagger
        'cpucores

        Dim RunGenerate(5) As String
        ' RunGenerate(0) = "C:\Windows\SysWOW64\java -Xmx" & TextBox4.Text & "m -cp pocminer.jar;lib/*;lib/akka/*;lib/jetty/* pocminer.POCMiner generate " & address & " " & TextBox2.Text & " " & TrackBar1.Value * 4000 & " " & TextBox3.Text & " " & ComboBox1.Text
        RunGenerate(0) = "C:\Windows\SysWOW64\java -Xmx" & TextBox4.Text & "m -cp pocminer.jar;lib/*;lib/akka/*;lib/jetty/* pocminer.POCMiner generate " & address & " " & NextNonceNo & " " & TrackBar1.Value * 4000 & " " & TextBox3.Text & " " & ComboBox1.Text


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


        If 1 = 2 Then
            'The Final Step is to add this plot to the list so it runs on startup
            Dim myplots() As String
            Dim myplotscount As Integer = 0
            For Each ListItem In CheckedListBox1.Items
                ReDim Preserve myplots(myplotscount)
                myplots(myplotscount) = ListItem
                myplotscount = myplotscount + 1
            Next

            ReDim Preserve myplots(myplotscount)
            myplots(myplotscount) = SelectedDrive
            myplotscount = myplotscount

            System.IO.File.WriteAllLines("C:\Burst.Today\Installs.txt", myplots)

        Else
            'implement Urays Miner. Run 1 exe for all files. 
            Dim ReadMiningConf() As String = System.IO.File.ReadAllLines("C:\Burst.Today\Burst.Today\BurstTodayUI-master\mining.conf")
            Dim MiningConfLength As Integer = ReadMiningConf.Length
            Dim DoesDirectoryExist As Integer = 0
            Dim ocho As Integer = 0

            While ocho < MiningConfLength
                If ReadMiningConf(ocho) = "        " & Chr(34) & SelectedDrive.Replace("\", "\\") & "plots" & Chr(34) & "," Then
                    'then directory exists
                    DoesDirectoryExist = 1
                End If
                ocho = ocho + 1
            End While

            Dim newMiningConf() As String = ReadMiningConf
            ReDim Preserve newMiningConf(newMiningConf.Length + 1)




            If DoesDirectoryExist = 0 Then
                'then add the directory to the list
                ocho = 0
                While ocho < MiningConfLength
                    If ReadMiningConf(ocho).Trim = "]" Then
                        newMiningConf(ocho) = "        " & Chr(34) & SelectedDrive.Replace("\", "\\") & "plots" & Chr(34)
                        If ReadMiningConf(ocho - 1).Trim = "[" Then
                            'then there are no plots
                        Else
                            'then there are plots
                            newMiningConf(ocho - 1) = newMiningConf(ocho - 1) & ","
                        End If
                        newMiningConf(ocho + 1) = "    ]"
                        newMiningConf(ocho + 2) = "}"
                        System.IO.File.WriteAllLines("C:\Burst.Today\Burst.Today\BurstTodayUI-master\mining.conf", newMiningConf)
                        ocho = MiningConfLength
                    End If


                    ocho = ocho + 1
                End While

            End If
        End If




        Dim WriteNonceNo(2) As String
        WriteNonceNo(0) = NextNonceNo + (TrackBar1.Value * 4000)
        System.IO.File.WriteAllLines("C:\Burst.Today\LastNonceNumber.txt", WriteNonceNo)


    End Sub


    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        Process.Start("Http://www.burst.today")
    End Sub


    Private Sub TabControl1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles TabControl1.SelectedIndexChanged
        Me.BringToFront()
    End Sub

    Private Sub ComboBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox2.SelectedIndexChanged
        Dim Tempstring As String = ComboBox2.Text.Remove(0, InStr(ComboBox2.Text, " - ") + 2)
        Tempstring = Tempstring.Trim
        Tempstring = Tempstring.Replace("GB available", "")
        Tempstring = Tempstring.Trim
        TrackBar1.Maximum = Tempstring
        TrackBar1.Value = 0
        Label4.Text = "Space to use (" & TrackBar1.Value & " / " & Tempstring & " GB)"
        
    End Sub


    Private Sub TrackBar1_Scroll(sender As Object, e As EventArgs) Handles TrackBar1.Scroll
        Dim Tempstring As String = ComboBox2.Text.Remove(0, InStr(ComboBox2.Text, " - ") + 2)
        Tempstring = Tempstring.Trim
        Tempstring = Tempstring.Replace("GB available", "")
        Tempstring = Tempstring.Trim
        Label4.Text = "Space to use (" & TrackBar1.Value & " / " & Tempstring & " GB)"
    End Sub




    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click






        System.Threading.Thread.Sleep(1000)

        '------------------------------------IF THERE IS NO ACCOUNT #, MAKE ONE
        Dim Address As String = ""

        If Label3.Text.StartsWith("#") Then
            'then there is already an account, copy the file
            'My.Computer.FileSystem.CopyFile("C:\Burst.Today\passphrases.txt", SelectedDrive & "passphrases.txt", True)
            'My.Computer.FileSystem.CopyFile("C:\Burst.Today\address.txt", SelectedDrive & "address.txt", True)
        Else

            If TextBox1.Text.Length < 35 Then
                MsgBox("Please make your password longer than 35 characters")
                Exit Sub

            End If
            'There is no account, create one
            '---------CREATE PW
            Dim PassPhrasesTXT(3) As String
            PassPhrasesTXT(0) = TextBox1.Text
            System.IO.File.WriteAllLines("C:\Burst.Today\Miner\pocminer-master\passphrases.txt", PassPhrasesTXT)
            System.IO.File.WriteAllLines("C:\Burst.Today\passphrases.txt", PassPhrasesTXT)
            '---------END CREATE PW

            '----------CREATE NEW ADDRESS
            Try
                Dim procInfo As New ProcessStartInfo()
                'procInfo.UseShellExecute = True
                procInfo.UseShellExecute = False
                Dim JavaExe As String = "C:\Burst.Today\Miner\pocminer-master\run_dump_address.bat"
                procInfo.FileName = (JavaExe)
                procInfo.WorkingDirectory = "C:\Burst.Today\Miner\pocminer-master"
                procInfo.Verb = "runas"
                Process.Start(procInfo).WaitForExit()
                My.Computer.FileSystem.CopyFile("C:\Burst.Today\Miner\pocminer-master\address.txt", "C:\Burst.Today\address.txt", True)
                Me.BringToFront()
            Catch ex As Exception
                MsgBox(ex.Message)
            End Try

            System.Threading.Thread.Sleep(1000)

            Dim ReadAddress() As String = System.IO.File.ReadAllLines("C:\Burst.Today\address.txt")
            ' Dim Address As String = ReadAddress(0)
            Address = ReadAddress(0)
            Address = Address.Remove(0, InStr(Address, "->") + 2)
            Address = Address.Trim


            Label3.Text = "#" & Address



            ' MsgBox("address=" & Address)
            Dim WriteAddress(5) As String
            WriteAddress(0) = Address

            'it might be a different drive, but we run from C:\
            System.IO.File.WriteAllLines("C:\Burst.Today\address.txt", WriteAddress)



            '----------END CREATE NEW ADDRESS

            Startmining()


        End If
        '------------------------------------END IF THERE IS NO ACCOUNT #, MAKE ONE


    End Sub


End Class
