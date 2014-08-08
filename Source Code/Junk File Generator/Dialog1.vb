Imports System.Windows.Forms

Public Class Dialog1
    Dim lastselection As Integer
    Private Sub Error_Handler(ByVal ex As Exception, Optional ByVal identifier_msg As String = "")
        Try
            If ex.Message.IndexOf("Thread was being aborted") < 0 Then
                Dim Display_Message1 As New Display_Message()

                Display_Message1.Message_Textbox.Text = "The Application encountered the following problem: " & vbCrLf & identifier_msg & ":" & ex.ToString

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
            End If
        Catch exc As Exception
            MsgBox("An error occurred in the application's error handling routine. The application will try to recover from this serious error.", MsgBoxStyle.Critical, "Critical Error Encountered")
        End Try
    End Sub

    Private Function FormatSize(ByVal amt As Long, ByVal rounding As Integer) As String
        Try
            If (amt >= Math.Pow(2, 80)) Then Return Math.Round(amt / Math.Pow(2, 80), rounding).ToString() & " YB" 'yettabyte
            If (amt >= Math.Pow(2, 70)) Then Return Math.Round(amt / Math.Pow(2, 70), rounding).ToString() & " ZB" 'zettabyte
            If (amt >= Math.Pow(2, 60)) Then Return Math.Round(amt / Math.Pow(2, 60), rounding).ToString() & " EB" 'exabyte
            If (amt >= Math.Pow(2, 50)) Then Return Math.Round(amt / Math.Pow(2, 50), rounding).ToString() & " PB" 'petabyte
            If (amt >= Math.Pow(2, 40)) Then Return Math.Round(amt / Math.Pow(2, 40), rounding).ToString() & " TB" 'terabyte
            If (amt >= Math.Pow(2, 30)) Then Return Math.Round(amt / Math.Pow(2, 30), rounding).ToString() & " GB" 'gigabyte
            If (amt >= Math.Pow(2, 20)) Then Return Math.Round(amt / Math.Pow(2, 20), rounding).ToString() & " MB" 'megabyte
            If (amt >= Math.Pow(2, 10)) Then Return Math.Round(amt / Math.Pow(2, 10), rounding).ToString() & " KB" 'kilobyte
        Catch ex As Exception
            Throw ex
        End Try
        Return amt.ToString & " Bytes"
    End Function
        

    Private Function FormatSizetoBytes(ByVal amt As Long, ByVal rounding As Integer) As String
        Try
            Select Case ComboBox1.SelectedIndex
                Case 0
                    Return amt.ToString
                Case 1
                    Return Math.Round(amt * Math.Pow(2, 10), rounding).ToString()
                Case 2
                    Return Math.Round(amt * Math.Pow(2, 20), rounding).ToString()
                Case 3
                    Return Math.Round(amt * Math.Pow(2, 30), rounding).ToString()
                Case 4
                    Return Math.Round(amt * Math.Pow(2, 40), rounding).ToString()
            End Select

        Catch ex As Exception
            Throw ex
        End Try
        Return amt.ToString
    End Function
        


    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Try
            If TextBox1.Text.Length < 1 Then
                ComboBox1.SelectedIndex = 0
                TextBox1.Text = "1"
            End If
            Me.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.Close()
        Catch ex As Exception
            Error_Handler(ex, "OK Button Click")
        End Try
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Try
            Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
            Me.Close()
        Catch ex As Exception
            Error_Handler(ex, "Cancel Button Click")
        End Try
    End Sub

    Private Sub Dialog1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Try
            Me.Text = "Choose File Size"
            ComboBox1.SelectedIndex = 2
            TextBox1.Select()
        Catch ex As Exception
            Error_Handler(ex, "Dialog Load")
        End Try
    End Sub

    Private Sub TextBox1_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox1.TextChanged
        Try
            While ((TextBox1.Text.Length > 0) And (IsNumeric(TextBox1.Text) = False))

                If (TextBox1.Text.Length > 0) Then

                    TextBox1.Text = TextBox1.Text.Remove(TextBox1.Text.Length - 1, 1)
                    TextBox1.Select(TextBox1.Text.Length, 0)
                End If
            End While
            If (TextBox1.Text.Length > 0) Then
                TextBox2.Text = FormatSizetoBytes(Long.Parse(TextBox1.Text), 3)
            End If
        Catch ex As Exception
            Error_Handler(ex, "Input Changed")
        End Try
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox1.SelectedIndexChanged
        Try
            lastselection = ComboBox1.SelectedIndex
            If TextBox1.Text.Length > 0 Then
                Textbox2.Text = FormatSizetoBytes(Long.Parse(TextBox1.Text), 3)
            End If
        Catch ex As Exception
            Error_Handler(ex, "Unit Type Changed")
        End Try
    End Sub

    Private Sub ComboBox1_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox1.TextChanged
        Try
            If ComboBox1.SelectedIndex = -1 Then
                ComboBox1.SelectedIndex = 2
            End If
            If ComboBox1.Items.IndexOf(ComboBox1.Text) = -1 Then
                ComboBox1.SelectedIndex = lastselection
                ComboBox1.Refresh()
            Else
                ComboBox1.SelectedIndex = ComboBox1.Items.IndexOf(ComboBox1.Text)
                ComboBox1.Refresh()
            End If
        Catch ex As Exception
            Error_Handler(ex, "ComboBox1 TextChanged")
        End Try
    End Sub

    Private Sub ComboBox1_SelectedValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboBox1.SelectedValueChanged
        Try
            If ComboBox1.SelectedIndex = -1 Then
                ComboBox1.SelectedIndex = 2
            End If
            If ComboBox1.Items.IndexOf(ComboBox1.Text) = -1 Then
                ComboBox1.SelectedIndex = lastselection
                ComboBox1.Refresh()
            Else
                ComboBox1.SelectedIndex = ComboBox1.Items.IndexOf(ComboBox1.Text)
                ComboBox1.Refresh()
            End If
        Catch ex As Exception
            Error_Handler(ex, "ComboBox1 TextChanged")
        End Try
    End Sub
End Class
