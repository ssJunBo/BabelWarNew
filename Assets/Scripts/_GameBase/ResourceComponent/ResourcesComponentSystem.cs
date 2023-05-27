using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ET;
using Managers;
using UnityEngine;

namespace _GameBase
{
    public static class ResourcesComponentSystem
    {
        private static string[] GetDependencies(this ResourcesComponent self, string assetBundleName)
        {
            string[] dependencies = Array.Empty<string>();
            if (self.DependenciesCache.TryGetValue(assetBundleName, out dependencies))
            {
                return dependencies;
            }

            if (!Define.IsAsync)
            {
                if (Define.IsEditor)
                {
                    dependencies = Define.GetAssetBundleDependencies(assetBundleName, true);
                }
            }
            else
            {
                dependencies = self.AssetBundleManifestObject.GetAllDependencies(assetBundleName);
            }

            self.DependenciesCache.Add(assetBundleName, dependencies);
            return dependencies;
        }

        private static string[] GetSortedDependencies(this ResourcesComponent self, string assetBundleName)
        {
            var info = new Dictionary<string, int>();
            var parents = new List<string>();
            self.CollectDependencies(parents, assetBundleName, info);
            string[] ss = info.OrderBy(x => x.Value).Select(x => x.Key).ToArray();
            return ss;
        }

        private static void CollectDependencies(this ResourcesComponent self, List<string> parents,
            string assetBundleName, Dictionary<string, int> info)
        {
            parents.Add(assetBundleName);
            string[] deps = self.GetDependencies(assetBundleName);
            foreach (string parent in parents)
            {
                if (!info.ContainsKey(parent))
                {
                    info[parent] = 0;
                }

                info[parent] += deps.Length;
            }

            foreach (string dep in deps)
            {
                if (parents.Contains(dep))
                {
                    throw new Exception($"包有循环依赖，请重新标记: {assetBundleName} {dep}");
                }

                self.CollectDependencies(parents, dep, info);
            }

            parents.RemoveAt(parents.Count - 1);
        }



        public static bool Contains(this ResourcesComponent self, string bundleName)
        {
            return self.bundles.ContainsKey(bundleName);
        }

        public static UnityEngine.Object GetAsset(this ResourcesComponent self, string bundleName, string prefab)
        {
            Dictionary<string, UnityEngine.Object> dict;
            if (!self.resourceCache.TryGetValue(bundleName.BundleNameToLower(), out dict))
            {
                throw new Exception($"not found asset: {bundleName} {prefab}");
            }

            UnityEngine.Object resource = null;
            if (!dict.TryGetValue(prefab, out resource))
            {
                throw new Exception($"not found asset: {bundleName} {prefab}");
            }

            return resource;
        }

        // 通过 asset 路径 加载prefab
        public static UnityEngine.Object GetAssetWithPath(this ResourcesComponent self, string bundleName, string prefabPath)
        {
            
            
            return null;
        }

        // 一帧卸载一个包，避免卡死
        public static async ETTask UnloadBundleAsync(this ResourcesComponent self, string assetBundleName,
            bool unload = true)
        {
            assetBundleName = assetBundleName.BundleNameToLower();

            string[] dependencies = self.GetSortedDependencies(assetBundleName);

            //Log.Debug($"-----------dep unload start {assetBundleName} dep: {dependencies.ToList().ListToString()}");
            foreach (string dependency in dependencies)
            {
                using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.Resources, assetBundleName.GetHashCode()))
                {
                    self.UnloadOneBundle(dependency, unload);
                    await TimerComponent.Instance.WaitFrameAsync();
                }
            }
            //Log.Debug($"-----------dep unload finish {assetBundleName} dep: {dependencies.ToList().ListToString()}");
        }

        // 只允许场景设置unload为false
        public static void UnloadBundle(this ResourcesComponent self, string assetBundleName, bool unload = true)
        {
            assetBundleName = assetBundleName.BundleNameToLower();

            string[] dependencies = self.GetSortedDependencies(assetBundleName);

            //Log.Debug($"-----------dep unload start {assetBundleName} dep: {dependencies.ToList().ListToString()}");
            foreach (string dependency in dependencies)
            {
                self.UnloadOneBundle(dependency, unload);
            }

            //Log.Debug($"-----------dep unload finish {assetBundleName} dep: {dependencies.ToList().ListToString()}");
        }

        private static void UnloadOneBundle(this ResourcesComponent self, string assetBundleName, bool unload = true)
        {
            assetBundleName = assetBundleName.BundleNameToLower();

            ABInfo abInfo;
            if (!self.bundles.TryGetValue(assetBundleName, out abInfo))
            {
                return;
            }

            //Log.Debug($"---------------unload one bundle {assetBundleName} refcount: {abInfo.RefCount - 1}");

            --abInfo.RefCount;

            if (abInfo.RefCount > 0)
            {
                return;
            }

            //Log.Debug($"---------------truly unload one bundle {assetBundleName} refcount: {abInfo.RefCount}");
            self.bundles.Remove(assetBundleName);
            self.resourceCache.Remove(assetBundleName);
            abInfo.Release(unload);
            // Log.Debug($"cache count: {self.cacheDictionary.Count}");
        }

        /// <summary>
        /// 同步加载assetbundle
        /// </summary>
        /// <param name="assetBundleName"></param>
        /// <returns></returns>
        public static void LoadBundle(this ResourcesComponent self, string assetBundleName)
        {
            assetBundleName = assetBundleName.ToLower();

            string[] dependencies = self.GetSortedDependencies(assetBundleName);
            //Log.Debug($"-----------dep load start {assetBundleName} dep: {dependencies.ToList().ListToString()}");
            foreach (string dependency in dependencies)
            {
                if (string.IsNullOrEmpty(dependency))
                {
                    continue;
                }

                self.LoadOneBundle(dependency);
            }

            //Log.Debug($"-----------dep load finish {assetBundleName} dep: {dependencies.ToList().ListToString()}");
        }

        private static void AddResource(this ResourcesComponent self, string bundleName, string assetName,
            UnityEngine.Object resource)
        {
            Dictionary<string, UnityEngine.Object> dict;
            if (!self.resourceCache.TryGetValue(bundleName.BundleNameToLower(), out dict))
            {
                dict = new Dictionary<string, UnityEngine.Object>();
                self.resourceCache[bundleName] = dict;
            }

            dict[assetName] = resource;
        }

        public static void LoadOneBundle(this ResourcesComponent self, string assetBundleName)
        {
            assetBundleName = assetBundleName.BundleNameToLower();
            ABInfo abInfo;
            if (self.bundles.TryGetValue(assetBundleName, out abInfo))
            {
                ++abInfo.RefCount;
                //Log.Debug($"---------------load one bundle {assetBundleName} refcount: {abInfo.RefCount}");
                return;
            }

            if (!Define.IsAsync)
            {
                if (Define.IsEditor)
                {
                    string[] realPath = null;
                    realPath = Define.GetAssetPathsFromAssetBundle(assetBundleName);
                    foreach (string s in realPath)
                    {
                        string assetName = Path.GetFileNameWithoutExtension(s);
                        UnityEngine.Object resource = Define.LoadAssetAtPath(s);
                        self.AddResource(assetBundleName, assetName, resource);
                    }

                    if (realPath.Length > 0)
                    {
                        // abInfo = self.AddChild<ABInfo, string, AssetBundle>(assetBundleName, null);
                        self.bundles[assetBundleName] = abInfo;
                        //Log.Debug($"---------------load one bundle {assetBundleName} refcount: {abInfo.RefCount}");
                    }
                    else
                    {
                        Debug.LogError($"assets bundle not found: {assetBundleName}");
                    }
                }

                return;
            }

            string p = Path.Combine(PathHelper.AppHotfixResPath, assetBundleName);
            AssetBundle assetBundle = null;
            if (File.Exists(p))
            {
                assetBundle = AssetBundle.LoadFromFile(p);
            }
            else
            {
                p = Path.Combine(PathHelper.AppResPath, assetBundleName);
                assetBundle = AssetBundle.LoadFromFile(p);
            }

            if (assetBundle == null)
            {
                // 获取资源的时候会抛异常，这个地方不直接抛异常，因为有些地方需要Load之后判断是否Load成功
                Debug.LogWarning($"assets bundle not found: {assetBundleName}");
                return;
            }

            if (!assetBundle.isStreamedSceneAssetBundle)
            {
                // 异步load资源到内存cache住
                var assets = assetBundle.LoadAllAssets();
                foreach (UnityEngine.Object asset in assets)
                {
                    self.AddResource(assetBundleName, asset.name, asset);
                }
            }

            // abInfo = self.AddChild<ABInfo, string, AssetBundle>(assetBundleName, assetBundle);
            self.bundles[assetBundleName] = abInfo;

            //Log.Debug($"---------------load one bundle {assetBundleName} refcount: {abInfo.RefCount}");
        }

        /// <summary>
        /// 异步加载assetbundle, 加载ab包分两部分，第一部分是从硬盘加载，第二部分加载all assets。两者不能同时并发
        /// </summary>
        public static async ETTask LoadBundleAsync(this ResourcesComponent self, string assetBundleName)
        {
            assetBundleName = assetBundleName.BundleNameToLower();

            string[] dependencies = self.GetSortedDependencies(assetBundleName);
            //Log.Debug($"-----------dep load async start {assetBundleName} dep: {dependencies.ToList().ListToString()}");

            using (ListComponent<ABInfo> abInfos = ListComponent<ABInfo>.Create())
            {
                async ETTask LoadDependency(string dependency, List<ABInfo> abInfosList)
                {
                    using CoroutineLock coroutineLock = await CoroutineLockComponent.Instance.Wait(CoroutineLockType.Resources,
                            dependency.GetHashCode());

                    ABInfo abInfo = await self.LoadOneBundleAsync(dependency);
                    if (abInfo == null || abInfo.RefCount > 1)
                    {
                        return;
                    }

                    abInfosList.Add(abInfo);
                }

                // LoadFromFileAsync部分可以并发加载
                using (ListComponent<ETTask> tasks = ListComponent<ETTask>.Create())
                {
                    foreach (string dependency in dependencies)
                    {
                        tasks.Add(LoadDependency(dependency, abInfos));
                    }

                    await ETTaskHelper.WaitAll(tasks);

                    // ab包从硬盘加载完成，可以再并发加载all assets
                    tasks.Clear();
                    foreach (ABInfo abInfo in abInfos)
                    {
                        tasks.Add(self.LoadOneBundleAllAssets(abInfo));
                    }

                    await ETTaskHelper.WaitAll(tasks);
                }
            }
        }

        private static async ETTask<ABInfo> LoadOneBundleAsync(this ResourcesComponent self, string assetBundleName)
        {
            assetBundleName = assetBundleName.BundleNameToLower();
            ABInfo abInfo;
            if (self.bundles.TryGetValue(assetBundleName, out abInfo))
            {
                ++abInfo.RefCount;
                //Log.Debug($"---------------load one bundle {assetBundleName} refcount: {abInfo.RefCount}");
                return null;
            }

            string p = "";
            AssetBundle assetBundle = null;

            if (!Define.IsAsync)
            {
                if (Define.IsEditor)
                {
                    string[] realPath = Define.GetAssetPathsFromAssetBundle(assetBundleName);
                    foreach (string s in realPath)
                    {
                        string assetName = Path.GetFileNameWithoutExtension(s);
                        UnityEngine.Object resource = Define.LoadAssetAtPath(s);
                        self.AddResource(assetBundleName, assetName, resource);
                    }

                    if (realPath.Length > 0)
                    {
                        // abInfo = self.AddChild<ABInfo, string, AssetBundle>(assetBundleName, null);
                        abInfo = new ABInfo(assetBundleName, null);
                        
                        self.bundles[assetBundleName] = abInfo;
                        //Log.Debug($"---------------load one bundle {assetBundleName} refcount: {abInfo.RefCount}");
                    }
                    else
                    {
                        Debug.LogError("Bundle not exist! BundleName: " + assetBundleName);
                    }

                    // 编辑器模式也不能同步加载  
                    await TimerComponent.Instance.WaitAsync(100);

                    return abInfo;
                }
            }

            p = Path.Combine(PathHelper.AppHotfixResPath, assetBundleName);
            if (!File.Exists(p))
            {
                p = Path.Combine(PathHelper.AppResPath, assetBundleName);
            }

            Debug.Log("Async load bundle BundleName : " + p);

            // if (!File.Exists(p))
            // {
            //     Debug.LogError("Async load bundle not exist! BundleName : " + p);
            //     return null;
            // }
            AssetBundleCreateRequest assetBundleCreateRequest = AssetBundle.LoadFromFileAsync(p);
            // await assetBundleCreateRequest;
            assetBundle = assetBundleCreateRequest.assetBundle;
            if (assetBundle == null)
            {
                // 获取资源的时候会抛异常，这个地方不直接抛异常，因为有些地方需要Load之后判断是否Load成功
                Debug.LogWarning($"assets bundle not found: {assetBundleName}");
                return null;
            }

            abInfo = new ABInfo(assetBundleName, assetBundle);
            
            self.bundles[assetBundleName] = abInfo;
            return abInfo;
            //Log.Debug($"---------------load one bundle {assetBundleName} refcount: {abInfo.RefCount}");
        }

        // 加载ab包中的all assets
        private static async ETTask LoadOneBundleAllAssets(this ResourcesComponent self, ABInfo abInfo)
        {
            using CoroutineLock coroutineLock = await CoroutineLockComponent.Instance.Wait(CoroutineLockType.Resources, abInfo.Name.GetHashCode());

            if (abInfo.AlreadyLoadAssets)
            {
                return;
            }

            if (abInfo.AssetBundle != null && !abInfo.AssetBundle.isStreamedSceneAssetBundle)
            {
                // 异步load资源到内存cache住
                AssetBundleRequest request = abInfo.AssetBundle.LoadAllAssetsAsync();
                // await request;
                UnityEngine.Object[] assets = request.allAssets;

                foreach (UnityEngine.Object asset in assets)
                {
                    self.AddResource(abInfo.Name, asset.name, asset);
                }
            }

            abInfo.AlreadyLoadAssets = true;
        }

        public static string DebugString(this ResourcesComponent self)
        {
            StringBuilder sb = new StringBuilder();
            foreach (ABInfo abInfo in self.bundles.Values)
            {
                sb.Append($"{abInfo.Name}:{abInfo.RefCount}\n");
            }

            return sb.ToString();
        }
    }
}