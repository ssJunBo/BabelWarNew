using System.IO;
using UnityEditor;
using UnityEngine;
using Application = UnityEngine.Device.Application;

namespace _GameBase.Editor.AssetBundle
{
    // 移动到 streaming assets
    public class MoveABToSA
    {
        [MenuItem("Tools/AB包工具/移动选中资源到StreamingAssets中")]
        private static void MoveABToStreamingAsstes()
        {
            // 通过编辑器Selection的方法 获取在Project窗口中选中的资源
            Object[] selectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);

            if (selectedAsset.Length == 0)
                return;

            string abCompareInfo = "";

            foreach (Object asset in selectedAsset)
            {
                var assetPath = AssetDatabase.GetAssetPath(asset);
                // 截取路径当中的文件名 用于作为StreamingAssets中的文件名
                var fileName = assetPath.Substring(assetPath.LastIndexOf('/'));

                // 是否有 . 符号 有则不处理 、也可以copy前获取全路径 然后通过 file info去获取后缀判断也ok
                if (fileName.IndexOf('.') != -1) 
                    continue;

                // 利用AssetDataBase中的API 将选中文件 复制到目标路径
                AssetDatabase.CopyAsset(assetPath, "Assets/StreamingAssets" + fileName);

                // 获取拷贝到StreamingAssets文件夹中的文件的全部信息
                var fileInfo = new FileInfo(Application.streamingAssetsPath + fileName);

                //拼接ab包信息到字符串中
                abCompareInfo += fileInfo.Name + " " + fileInfo.Length + " " +
                                 Lesson_MD5.GetMD5(fileInfo.FullName);
                //用指定字符隔开多个ab包
                abCompareInfo += "|";
            }

            // 去掉最后多余的 分隔符 ”|“
            abCompareInfo = abCompareInfo.Substring(0, abCompareInfo.Length - 1);

            File.WriteAllText(Application.streamingAssetsPath + "/ABCompareInfo.txt", abCompareInfo);
            AssetDatabase.Refresh();
        }
    }
}