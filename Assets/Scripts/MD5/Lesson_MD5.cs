using System.IO;
using System.Security.Cryptography;
using System.Text;

public static class Lesson_MD5
{
    public static string GetMD5(string filePath)
    {
        // 将文件已流的形式打开
        using (FileStream file = new FileStream(filePath, FileMode.Open))
        {
            // 声明一个md5 对象 用于生成md5
            MD5 md5 = new MD5CryptoServiceProvider();
            // 利用API 得到数据的MD5码 16个字节 数组
            byte[] md5Info = md5.ComputeHash(file);

            file.Close();

            // 把16字节 转化为 16 进制 拼接成了字符串 为了减少md5码的长度
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < md5Info.Length; i++)
            {
                // "x2" 含义是转16进制
                sb.Append(md5Info[i].ToString("x2"));
            }

            return sb.ToString();
        }
    }
}
