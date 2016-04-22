Imports System.Text.RegularExpressions
Imports Client.GLOBALS

Module Message_Processor
    Dim currentmessage As String 'this whill be complete message when we recieve new line from server
    Function ProcessNetMesage(msg As String)
        currentmessage &= msg 'we append msg to current message 
        If currentmessage.Contains(Chr(10)) Then 'if it contains new line, or is new line because we recieve one byte at a time, decrypt the message
            currentmessage = cp.Decrypt(currentmessage) 'decryption is done by cp which stands for CryptoProvider ..
            If currentmessage.Split(":").Length > 1 Then 'if the split by 
                Dim temp() As String = currentmessage.Split(":") ' split it by : 
                If temp(0).ToLower = "changestate" Then 'if first param which is function is changestate , then call changestate on each device
                    For Each dev In devicelist
                        dev.changeState(temp(1), temp(2))
                    Next
                End If

            End If
            currentmessage = "" 'we recieved our message, empty the string
        End If
        Return 0
    End Function
    Function ProcesSPMessage(msg As String) ' this processes the data from serial port
        If devicescreated Then ' if all the devices are created 
            For Each dev As Device In devicelist
                If dev.dtype = Device.DevType.BOOL Then
                    Dim match As Match = Regex.Match(msg, "<ds\b[^>]*>(.*?)</ds>") 'use regex to find matches
                    If match.Groups.Count = 2 Then
                        dev.processSM(match.Groups(1).Value) 'process inner value of <ds>....</ds>
                    End If
                End If
                'the same is for all device types, we get the inner of typeof state they're bound to.
                If dev.dtype = Device.DevType.SENSOR Then
                    Dim match As Match = Regex.Match(msg, "<an\b[^>]*>(.*?)</an>")
                    If match.Groups.Count = 2 Then
                        dev.processSM(match.Groups(1).Value)
                    End If
                End If
                If dev.dtype = Device.DevType.PWM Or dev.dtype = Device.DevType.RGB Then
                    Dim match As Match = Regex.Match(msg, "<pwm\b[^>]*>(.*?)</pwm>")
                    If match.Groups.Count = 2 Then
                        dev.processSM(match.Groups(1).Value)
                    End If
                End If
                If dev.dtype = Device.DevType.SERVO Or dev.dtype = Device.DevType.DOOR Then
                    Dim match As Match = Regex.Match(msg, "<servo\b[^>]*>(.*?)</servo>")
                    If match.Groups.Count = 2 Then
                        dev.processSM(match.Groups(1).Value)
                    End If
                End If
            Next
        End If
        If msg.Contains("OK") Then '' device returned ok ? this doesn't work everytime .. loosing data is bug, so while's with this should have timeour
            okawait = False
        End If
        If msg.Contains("clb") Then
            spdriver.clearbuffer() ''if clb in any way , clear buffer of spdriver
        End If

        Return 0
    End Function
End Module
