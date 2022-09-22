using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace Main.Game.ResourceFrame
{
    [System.Serializable]
    public class AssetBundleConfig
    {
        [XmlElement("ABList")]
        public List<ABBase> ABList { get; set; }
    }

    [System.Serializable]
    public class ABBase
    {
        [XmlAttribute("Path")]
        public string Path { get; set; }
        [XmlAttribute("Crc")]
        public uint Crc { get; set; }// 文件唯一标识
        [XmlAttribute("ABName")]
        public string ABName { get; set; }
        [XmlAttribute("AssetName")]
        public string AssetName { get; set; }
        [XmlElement("ABDependce")]
        public List<string> ABDependce { get; set; }
    }
}