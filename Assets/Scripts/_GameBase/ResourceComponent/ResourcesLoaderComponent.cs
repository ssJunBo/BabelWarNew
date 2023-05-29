// using System.Collections.Generic;
// using ET;
//
// namespace _GameBase
// {
//     public class ResourcesLoaderComponent
//     {
//         public HashSet<string> LoadedResource = new HashSet<string>();
//
//         public async ETTask LoadAsync(string ab)
//         {
//             using CoroutineLock coroutineLock = await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ResourcesLoader, ab.GetHashCode(), 0);
//
//             if (LoadedResource.Contains(ab))
//             {
//                 return;
//             }
//
//             if (!Define.IsEditor)
//             {
//                 LoadedResource.Add(ab);
//             }
//             
//             await ResourcesComponent.Instance.LoadBundleAsync(ab);
//         }
//
//         public void Destroy()
//         {
//             if (Define.IsEditor)
//             {
//                 return;
//             }
//             
//             async ETTask UnLoadAsync()
//             {
//                 using (ListComponent<string> list = ListComponent<string>.Create())
//                 {
//                     list.AddRange(LoadedResource);
//                     LoadedResource = null;
//
//                     if (TimerComponent.Instance == null)
//                     {
//                         return;
//                     }
//
//                     // 延迟5秒卸载包，因为包卸载是引用计数，5秒之内假如重新有逻辑加载了这个包，那么可以避免一次卸载跟加载
//                     await TimerComponent.Instance.WaitAsync(5000);
//
//                     foreach (string abName in list)
//                     {
//                         using CoroutineLock coroutineLock = 
//                             await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ResourcesLoader, abName.GetHashCode(), 0);
//                         {
//                             if (ResourcesComponent.Instance == null)
//                             {
//                                 return;
//                             }
//
//                             await ResourcesComponent.Instance.UnloadBundleAsync(abName);
//                         }
//                     }
//                 }
//             }
//
//             UnLoadAsync().Coroutine();
//         }
//     }
// }