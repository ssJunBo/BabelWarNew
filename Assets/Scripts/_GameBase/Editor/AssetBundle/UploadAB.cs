using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace _GameBase.Editor.AssetBundle
{
    public class UploadAB
    {
        [MenuItem("Tools/AB包工具/上传AB包和对比文件")]
        private static void UploadAllABFile()
        {
            DirectoryInfo directoryInfo = Directory.CreateDirectory(Application.dataPath + "/AB");
            // 获取该目录下的所有文件信息
            FileInfo[] fileInfos = directoryInfo.GetFiles();

            foreach (var info in fileInfos)
            {
                // 上传AB包和对比文件
                if (info.Extension == ".meta")
                    continue;

                // 上传文件
                FtpUploadFile(info.FullName, info.Name);
            }
        }

        private static async void FtpUploadFile(string filePath, string fileName)
        {
            await Task.Run(() =>
            {
                try
                {
                    // 1 创建一个FTP连接 用于上传
                    FtpWebRequest req =
                        FtpWebRequest.Create(new Uri("ftp://127.0.0.1/AB/PC/" + fileName)) as FtpWebRequest;

                    // 2 设置一个通信凭证，才能上传
                    NetworkCredential n = new NetworkCredential("songjunbo", "Bo1995927");
                    req.Credentials = n;

                    // 3 其他 设置 【设置代理为null  请求完毕后，是否关闭控制连接  操作命令-上传  指定传输的类型-2进制】
                    req.Proxy = null;
                    req.KeepAlive = false;
                    req.Method = WebRequestMethods.Ftp.UploadFile;
                    req.UseBinary = true;

                    // 4 上传文件 【ftp的流对象  读取文件信息 写入该流对象】
                    Stream upLoadStream = req.GetRequestStream();

                    using (FileStream file = File.OpenRead(filePath))
                    {
                        // 一点点上传内容
                        byte[] bytes = new byte[2048];
                        int contentLength = file.Read(bytes, 0, bytes.Length);

                        // 循环上传文件中数据
                        while (contentLength != 0)
                        {
                            // 写入到上传流中
                            upLoadStream.Write(bytes, 0, contentLength);
                            // 写完在读
                            contentLength = file.Read(bytes, 0, bytes.Length);
                        }

                        // 循环完毕后 证明上传结束
                        file.Close();
                        upLoadStream.Close();
                    }

                    Debug.Log("上传成功 文件名 = " + fileName);
                }
                catch (Exception e)
                {
                    Debug.Log("上传失败 文件名 = " + fileName + " " + e.Message);
                    throw;
                }
            });
        }
    }
}