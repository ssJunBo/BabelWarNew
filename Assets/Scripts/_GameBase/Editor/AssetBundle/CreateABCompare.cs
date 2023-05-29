using System.IO;
using UnityEditor;
using UnityEngine;

namespace _GameBase.Editor.AssetBundle
{
    public class CreateABCompare
    {
        [MenuItem("Tools/AB包工具/创建信息对比文件")]
        public static void CreateABCompareFile()
        {
            DirectoryInfo directoryInfo = Directory.CreateDirectory(Application.dataPath + "/AB");
            // 获取该目录下的所有文件信息
            FileInfo[] fileInfos = directoryInfo.GetFiles();

            string abCompareInfo = "";
        
            foreach (var info in fileInfos)
            {
                if (info.Extension == ".meta")
                    continue;

                // 拼接一个ab的信息
                abCompareInfo += info.Name + " " + info.Length + " " + Lesson_MD5.GetMD5(info.FullName);
                abCompareInfo += '|';
            }

            // 因为循环完毕后 最后多一个 | 符号
            abCompareInfo=abCompareInfo.Substring(0, abCompareInfo.Length - 1);

            // 存储拼接好的 AB包资源信息
            File.WriteAllText(Application.dataPath + "/AB/ABCompareInfo.txt", abCompareInfo);
        
            Debug.Log(abCompareInfo);
            Debug.Log("AB包对比文件生成成功");
        
            AssetDatabase.Refresh();
        
        }
    }
}