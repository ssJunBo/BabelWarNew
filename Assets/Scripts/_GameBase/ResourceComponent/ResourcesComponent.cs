using System.Collections.Generic;
using UnityEngine;

namespace _GameBase
{
    public class ABInfo
    {
        public string Name { get; set; }
        public int RefCount { get; set; }

        public AssetBundle AssetBundle;

        public bool AlreadyLoadAssets;

        public ABInfo(string abName, AssetBundle assetBundle)
        {
            AssetBundle = assetBundle;
            Name = abName;
            RefCount = 1;
            AlreadyLoadAssets = false;
        }
    }

    public static class AbInfoHelp
    {
        public static void Release(this ABInfo self, bool unload = true)
        {
            self.RefCount = 0;
            self.Name = "";
            self.AlreadyLoadAssets = false;

            if (self.AssetBundle != null)
            {
                self.AssetBundle.Unload(unload);
                self.AssetBundle = null;
            }
        }
    }

    // 用于字符串转换，减少GC
    public static class AssetBundleHelper
    {
        public static string IntToString(this int value)
        {
            string result;
            if (ResourcesComponent.Instance.IntToStringDict.TryGetValue(value, out result))
            {
                return result;
            }

            result = value.ToString();
            ResourcesComponent.Instance.IntToStringDict[value] = result;
            return result;
        }

        public static string StringToAB(this string value)
        {
            string result;
            if (ResourcesComponent.Instance.stringToAbDict.TryGetValue(value, out result))
            {
                return result;
            }

            result = value + ".unity3d";
            ResourcesComponent.Instance.stringToAbDict[value] = result;
            return result;
        }

        public static string IntToAB(this int value)
        {
            return value.IntToString().StringToAB();
        }

        public static string BundleNameToLower(this string value)
        {
            string result;
            if (ResourcesComponent.Instance.BundleNameToLowerDict.TryGetValue(value, out result))
            {
                return result;
            }

            result = value.ToLower();
            ResourcesComponent.Instance.BundleNameToLowerDict[value] = result;
            return result;
        }
    }

    public class ResourcesComponent : Singleton<ResourcesComponent>, ISingletonAwake, ISingletonDestroy
    {
        public AssetBundleManifest AssetBundleManifestObject { get; set; }

        public readonly Dictionary<int, string> IntToStringDict = new();

        public readonly Dictionary<string, string> stringToAbDict = new();

        public readonly Dictionary<string, string> BundleNameToLowerDict = new() { { "StreamingAssets", "StreamingAssets" } };

        public readonly Dictionary<string, Dictionary<string, Object>> resourceCache = new();

        public readonly Dictionary<string, ABInfo> bundles = new();

        // 缓存包依赖，不用每次计算
        public readonly Dictionary<string, string[]> DependenciesCache = new();

        public void Awake()
        {
            if (Define.IsAsync)
            {
                this.LoadOneBundle("StreamingAssets");
                AssetBundleManifestObject = (AssetBundleManifest)this.GetAsset("StreamingAssets", "AssetBundleManifest");
                this.UnloadBundle("StreamingAssets", false);
            }
        }

        public void Destroy()
        {
            foreach (var abInfo in bundles)
            {
                abInfo.Value.Release();
            }

            bundles.Clear();
            resourceCache.Clear();
            IntToStringDict.Clear();
            stringToAbDict.Clear();
            BundleNameToLowerDict.Clear();
            if (AssetBundleManifestObject != null)
            {
                Object.Destroy(AssetBundleManifestObject);
                AssetBundleManifestObject = null;
            }
        }
    }
}