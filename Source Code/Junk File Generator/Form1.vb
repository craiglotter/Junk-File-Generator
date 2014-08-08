Imports System.IO

Public Class Form1
    Dim input1 As Dialog1
    Dim byteswritten As Long

    Private Sub Error_Handler(ByVal ex As Exception, Optional ByVal identifier_msg As String = "")
        Try
            If ex.Message.IndexOf("Thread was being aborted") < 0 Then
                Dim Display_Message1 As New Display_Message()
                If FullErrors_Checkbox.Checked = True Then
                    Display_Message1.Message_Textbox.Text = "The Application encountered the following problem: " & vbCrLf & identifier_msg & ":" & ex.ToString
                Else
                    Display_Message1.Message_Textbox.Text = "The Application encountered the following problem: " & vbCrLf & identifier_msg & ":" & ex.Message.ToString
                End If
                Display_Message1.Timer1.Interval = 1000
                Display_Message1.ShowDialog()
                Dim dir As System.IO.DirectoryInfo = New System.IO.DirectoryInfo((Application.StartupPath & "\").Replace("\\", "\") & "Error Logs")
                If dir.Exists = False Then
                    dir.Create()
                End If
                dir = Nothing
                Dim filewriter As System.IO.StreamWriter = New System.IO.StreamWriter((Application.StartupPath & "\").Replace("\\", "\") & "Error Logs\" & Format(Now(), "yyyyMMdd") & "_Error_Log.txt", True)
                filewriter.WriteLine("#" & Format(Now(), "dd/MM/yyyy hh:mm:ss tt") & " - " & identifier_msg & ":" & ex.ToString)
                filewriter.Flush()
                filewriter.Close()
                filewriter = Nothing
                Label2.Text = "Error encountered in last action"
            End If
        Catch exc As Exception
            MsgBox("An error occurred in the application's error handling routine. The application will try to recover from this serious error.", MsgBoxStyle.Critical, "Critical Error Encountered")
        End Try
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Try
            
            Label2.Text = ""
            If input1.ShowDialog = Windows.Forms.DialogResult.OK Then
                Label2.Text = "Desired file size selected"
                If SaveFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
                    Label2.Text = "File save location selected"
                    Label1.Text = ""
                    ProgressBar1.Value = 0
                    Button1.Enabled = False
                    LinkLabel1.Visible = True
                    LinkLabel1.Enabled = True
                    BackgroundWorker1.RunWorkerAsync()
                End If
            End If
        Catch ex As Exception
            Error_Handler(ex, "Button Click")
        End Try
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            Control.CheckForIllegalCrossThreadCalls = False
            Me.Text = My.Application.Info.ProductName & " (" & Format(My.Application.Info.Version.Major, "0000") & Format(My.Application.Info.Version.Minor, "00") & Format(My.Application.Info.Version.Build, "00") & "." & Format(My.Application.Info.Version.Revision, "00") & ")"
            input1 = New Dialog1
            Label2.Text = "Application loaded"
        Catch ex As Exception
            Error_Handler(ex, "Form Load")
        End Try
    End Sub

    Private Sub BackgroundWorker1_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        Try
            Dim fileName As String = SaveFileDialog1.FileName

            Dim fstJunk As FileStream = New FileStream(fileName, FileMode.Create)
            Dim binwrite As BinaryWriter = New BinaryWriter(fstJunk)
            Dim bit As Byte
            Dim percentcomplete As Integer
            bit = 1
            byteswritten = 0

            For i As Long = 0 To input1.Textbox2.Text - 1
                If BackgroundWorker1.CancellationPending Then
                    e.Cancel = True
                    Exit For
                End If
                binwrite.Write(bit)
                byteswritten = byteswritten + 1
                If i Mod 10000 = 0 Then
                    If input1.Textbox2.Text > 0 Then
                        percentcomplete = CSng(i) / CSng(input1.Textbox2.Text) * 100
                    Else
                        percentcomplete = 100
                    End If
                    BackgroundWorker1.ReportProgress(percentcomplete)
                End If
                If BackgroundWorker1.CancellationPending Then
                    e.Cancel = True
                    Exit For
                End If
            Next
            binwrite.Close()
            fstJunk.Close()
        Catch ex As Exception
            Error_Handler(ex, "Generate File")
        End Try
    End Sub

    Private Sub BackgroundWorker1_ProgressChanged(ByVal sender As System.Object, ByVal e As System.ComponentModel.ProgressChangedEventArgs) Handles BackgroundWorker1.ProgressChanged
        Try
            ProgressBar1.Value = e.ProgressPercentage
            Label1.Text = byteswritten.ToString & " bytes written (of " & input1.Textbox2.Text & ")"
            Label2.Text = "Generating junk file"
        Catch ex As Exception
            Error_Handler(ex, "Worker Progress Changed")
        End Try
    End Sub

    Private Sub BackgroundWorker1_RunWorkerCompleted(ByVal sender As System.Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
        Try
            ProgressBar1.Value = 100
            Button1.Enabled = True
            LinkLabel1.Visible = False
            LinkLabel1.Enabled = False
            If e.Cancelled = True Then
                MsgBox("The operation was cancelled", MsgBoxStyle.Exclamation, "Cancelled")
                Label2.Text = "File generation cancelled"
            Else
                MsgBox("File generation complete", MsgBoxStyle.Information, "Complete")
                Label2.Text = "File generation complete"
            End If
        Catch ex As Exception
            Error_Handler(ex, "Run Worker Completed")
        End Try
    End Sub

    Private Sub AboutToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AboutToolStripMenuItem.Click
        Try
            Label2.Text = "About displayed"
            AboutBox1.ShowDialog()
        Catch ex As Exception
            Error_Handler(ex, "Display About Screen")
        End Try
    End Sub

    Private Sub HelpToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles HelpToolStripMenuItem.Click
        Try
            Label2.Text = "Help displayed"
            HelpBox1.ShowDialog()
        Catch ex As Exception
            Error_Handler(ex, "Display Help Screen")
        End Try
    End Sub

    Private Sub LinkLabel1_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        Try
            ' Cancel the asynchronous operation.
            Me.BackgroundWorker1.CancelAsync()
            Label2.Text = "Cancelling operation"
            Label2.Refresh()

            ' Disable the Cancel button.
            LinkLabel1.Enabled = False
        Catch ex As Exception
            Error_Handler(ex, "Cancel Operation")
        End Try
    End Sub
End Class
