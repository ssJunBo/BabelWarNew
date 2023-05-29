using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using _GameBase;
using UnityEngine.Events;
using UnityEngine.Networking;
using Application = UnityEngine.Device.Application;
using Task = System.Threading.Tasks.Task;

namespace Managers
{
    public class ABUpdateManager : MonoSingleton<ABUpdateManager>
    {
        // 远端的 ab info 字典 之后和本地进行对比 即可完成 更新下载相关逻辑
        private readonly Dictionary<string, ABInfo> remoteABInfoDict = new();

        // 本地ab包 信息存储 主要用于和远端进行对比
        private readonly Dictionary<string, ABInfo> localABInfoDict = new();

        // 待下载的ab包列表文件 存储ab包的名字
        private readonly List<string> downLoadList = new();

        public void CheckUpdate(UnityAction<bool> overCallback,UnityAction<string> updateInfoCallback)
        {
            // 1 加载远端资源对比文件
            DownLoadABCompareFile(isOver =>
            {
                updateInfoCallback?.Invoke("开始更新资源！");
                if (isOver)
                {
                    updateInfoCallback?.Invoke("对比文件下载结束！");
                    string remoteInfo = File.ReadAllText(Application.persistentDataPath+"/ABCompareInfo_TMP.txt");
                    updateInfoCallback?.Invoke("解析远端对比文件！");
                    GetRemoteABCompareFileInfo(remoteInfo, remoteABInfoDict);
                    updateInfoCallback?.Invoke("解析远端对比文件完成！");
                    
                    // 2 加载本地资源对比文件
                    GetLocalABCompareFileInfo(isOver =>
                    {
                        if (isOver)
                        {
                            updateInfoCallback?.Invoke("解析本地对比文件完成！");
                            // 3 对比它们然后进行ab包的下载

                        }
                        else
                        {
                            overCallback?.Invoke(false);
                        }
                    });
                }
                else
                {
                    overCallback?.Invoke(false);
                }
            });
        }

        private async void DownLoadABCompareFile(UnityAction<bool> overCallBack)
        {
            // 1 从资源服下载资源对比文件
            // www UnityWebRequest ftp 相关api

            bool isOver = false;
            int reDownLoadMax = 5;
            while (!isOver && reDownLoadMax > 0)
            {
                await Task.Run(() =>
                {
                    isOver = DownLoadFile("ABCompareInfo.txt",
                        Application.persistentDataPath + "/ABCompareInfo_TMP.txt");
                });
                --reDownLoadMax;
            }

            overCallBack?.Invoke(isOver);
        }

        // 获取下载下来的ab包中信息
        private void GetRemoteABCompareFileInfo(string info,Dictionary<string, ABInfo> ABInfo)
        {
            Log.Info("Application.persistentDataPath = " + Application.persistentDataPath);
            // 2 获取资源对比文件中的 字符串信息 进行拆分
            // string info = File.ReadAllText(Application.persistentDataPath + "/ABCompareInfo_TMP.txt");
            string[] allInfos = info.Split('|');

            foreach (var abInfo in allInfos)
            {
                string[] infos = abInfo.Split(' ');

                // 记录每一个远端AB包的信息 之后 好用来对比
                ABInfo.Add(infos[0], new ABInfo(infos[0], infos[1], infos[2]));
            }

            Log.Info("AB包对比文件 内容获取结束！");
        }

        /// <summary>
        /// 本地AB包对比文件加载 解析信息
        /// </summary>
        private void GetLocalABCompareFileInfo(UnityAction<bool> overCallback)
        {
            // 如果可读可写文件夹中 存在对比文件 说明之前我们已经下载更新过了
            if (File.Exists(Application.persistentDataPath + "/ABCompareInfo.txt"))
            {
                StartCoroutine(GetLocalABCompareFileInfo(Application.persistentDataPath + "/ABCompareInfo.txt",overCallback));
            }
            // 只有 当可读可写中 没有对比文件时 才回来加载默认资源（第一次进游戏时才会发生）
            else  if (File.Exists(Application.streamingAssetsPath + "/ABCompareInfo.txt"))
            {
                StartCoroutine(GetLocalABCompareFileInfo(Application.streamingAssetsPath + "/ABCompareInfo.txt",overCallback));
            }
            else
            {
                // 如果两个都不进入 证明第一没有默认资源
                overCallback?.Invoke(true);
            }
        }

        /// <summary>
        /// 协同程序 加载本地信息 并解析存入字典
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private IEnumerator GetLocalABCompareFileInfo(string filePath,UnityAction<bool> overCallback)
        {
            // 通过 UnityWebRequest 去 加载本地文件
            UnityWebRequest req=UnityWebRequest.Get(filePath);
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                // 获取文件成功 继续往下执行
                GetRemoteABCompareFileInfo(req.downloadHandler.text, localABInfoDict);
            }
            
            overCallback?.Invoke(req.result == UnityWebRequest.Result.Success);
        }

        public async void DownLoadABFile(UnityAction<bool> overCallBack, UnityAction<int, int> updatePro)
        {
            // 1 遍历字典的键 根据文件名 去下载AB包到本地
            foreach (var name in remoteABInfoDict.Keys)
            {
                // TODO 先直接放入 待下载列表
                downLoadList.Add(name);
            }

            // 本地存储路径 
            string localPath = Application.persistentDataPath + "/";

            // 是否下载成功
            bool isOver;
            //重新下载的最大次数
            int reDownLoadMaxNum = 5;
            // 下载了多少资源
            int downLoadOverNum = 0;
            // 最大数量
            int downLoadMaxNum = downLoadList.Count;

            while (downLoadList.Count > 0 && reDownLoadMaxNum > 0)
            {
                for (int i = downLoadList.Count - 1; i >= 0; i--)
                {
                    isOver = false;
                    await Task.Run(() => { isOver = DownLoadFile(downLoadList[i], localPath + downLoadList[i]); });

                    if (!isOver) continue;

                    // 2 需要知道现在下载了多少 结束与否
                    updatePro?.Invoke(++downLoadOverNum, downLoadMaxNum);

                    // 把下载成功的文件名 从列表中移除
                    downLoadList.RemoveAt(i);
                }

                --reDownLoadMaxNum;
            }

            // 所有内容下载完了
            overCallBack?.Invoke(downLoadList.Count == 0);
        }

        private bool DownLoadFile(string fileName, string localPath)
        {
            try
            {
                // 1 创建一个FTP连接 用于下载
                FtpWebRequest req =
                    FtpWebRequest.Create(new Uri("ftp://127.0.0.1/AB/PC/" + fileName)) as FtpWebRequest;

                // 2 设置一个通信凭证，才能下载 （如果有匿名账号 可以不设置凭证，但是实际开发 建议 还是不要设置匿名账号）
                NetworkCredential n = new NetworkCredential("songjunbo", "Bo1995927");
                req.Credentials = n;

                // 3 其他 设置 【设置代理为null  请求完毕后，是否关闭控制连接  操作命令-下载  指定传输的类型-2进制】
                req.Proxy = null;
                req.KeepAlive = false;
                req.Method = WebRequestMethods.Ftp.DownloadFile;
                req.UseBinary = true;

                // 4 上传文件 【ftp的流对象  读取文件信息 写入该流对象】
                FtpWebResponse res = req.GetResponse() as FtpWebResponse;
                Stream downLoadStream = res.GetResponseStream();

                using (FileStream file = File.Create(localPath))
                {
                    // 一点点下载内容
                    byte[] bytes = new byte[2048];
                    int contentLength = downLoadStream.Read(bytes, 0, bytes.Length);

                    // 循环下载文件中数据
                    while (contentLength != 0)
                    {
                        // 写入到本地文件流中
                        file.Write(bytes, 0, contentLength);
                        // 写完在读
                        contentLength = downLoadStream.Read(bytes, 0, bytes.Length);
                    }

                    // 循环完毕后 证明上传结束
                    file.Close();
                    downLoadStream.Close();

                    Log.Info("下载成功 " + fileName);

                    return true;
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return false;
            }
        }

        public class ABInfo
        {
            public string name;
            public long size;
            public string md5;

            public ABInfo(string name, string size, string md5)
            {
                this.name = name;
                this.size = long.Parse(size);
                this.md5 = md5;
            }

            public static bool operator ==(ABInfo b, ABInfo c)
            {
                return b.md5 == c.md5;
            }

            public static bool operator !=(ABInfo b, ABInfo c)
            {
                return b.md5 != c.md5;
            }
        }
    }
}