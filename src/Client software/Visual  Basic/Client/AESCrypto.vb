Imports System.IO
Imports System.Security.Cryptography

Public Class AESCrypto
    Dim AESKey As Byte() = {&HFF, &HAF, &HC3, &H67, &H67, &HBB, &HAB, &HBF, &H88, &H54, &HF1, &H22, &H96, &HF4, &HAA, &HAF}
    Dim AESIV As Byte() = {&HCC, &HFB, &HC3, &H44, &H67, &HBB, &HBA, &HCF, &H88, &H54, &HF1, &H22, &H96, &HF4, &HAA, &HAF}

    Function Decrypt(S As String)
        If S = "" Then
            Return S
        End If
        ' Turn the cipherText into a ByteArray from Base64
        Dim cipherText As Byte()
        Try
            ' Replace any + that will lead to the error
            cipherText = Convert.FromBase64String(S.Replace("BIN00101011BIN", "+"))
        Catch ex As Exception
            ' There is a problem with the string, perhaps it has bad base64 padding
            Return S
        End Try
        'Creates the default implementation, which is RijndaelManaged.
        Dim rijn As SymmetricAlgorithm = SymmetricAlgorithm.Create()
        Try
            ' Create the streams used for decryption.
            Using msDecrypt As New MemoryStream(cipherText)
                Using csDecrypt As New CryptoStream(msDecrypt, rijn.CreateDecryptor(AESKey, AESIV), CryptoStreamMode.Read)
                    Using srDecrypt As New StreamReader(csDecrypt)
                        ' Read the decrypted bytes from the decrypting stream and place them in a string.
                        S = srDecrypt.ReadToEnd()
                    End Using
                End Using
            End Using
        Catch E As CryptographicException
            Return S
        End Try
        Return S
    End Function


    Function Encrypt(S As String)
        'Creates the default implementation, which is RijndaelManaged.
        Dim rijn As SymmetricAlgorithm = SymmetricAlgorithm.Create()
        Dim encrypted() As Byte
        Using msEncrypt As New MemoryStream()
            Dim csEncrypt As New CryptoStream(msEncrypt, rijn.CreateEncryptor(AESKey, AESIV), CryptoStreamMode.Write)
            Using swEncrypt As New StreamWriter(csEncrypt)
                'Write all data to the stream.
                swEncrypt.Write(S)
            End Using
            encrypted = msEncrypt.ToArray()
        End Using

        ' You cannot convert the byte to a string or you will get strange characters so base64 encode the string
        ' Replace any + that will lead to the error
        Return Convert.ToBase64String(encrypted).Replace("+", "BIN00101011BIN")
    End Function
End Class
