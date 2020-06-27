using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace QuickHealSpell
{
    public static class QuickHealSpellUtils
    {
        public static GameObject healingOrb;
        public static GameObject healingAoe;

        public static List<T> LoadResources<T>(string[] names, string assetName) where T : class
        {
            FileInfo[] files = new DirectoryInfo(BetterStreamingAssets.Root + "/QuickHealSpell/Bundles").GetFiles(assetName + ".assets", SearchOption.AllDirectories);
            AssetBundle assetBundle;
            if (AssetBundle.GetAllLoadedAssetBundles().Count() > 0)
            {
                if (AssetBundle.GetAllLoadedAssetBundles().Where(x => files[0].Name.Contains(x.name)).Count() == 0)
                {
                    assetBundle = AssetBundle.LoadFromFile(files[0].FullName);
                }
                else
                {
                    assetBundle = AssetBundle.GetAllLoadedAssetBundles().Where(x => files[0].Name.Contains(x.name)).First();
                }
            }
            else
            {
                assetBundle = AssetBundle.LoadFromFile(files[0].FullName);
            }
            List<T> objects = new List<T>();
            foreach (string k in assetBundle.GetAllAssetNames())
            {
                foreach (string j in names)
                    if (k.Contains(j))
                    {
                        objects.Add(assetBundle.LoadAsset(k) as T);
                        Debug.Log(Assembly.GetExecutingAssembly().GetName() + " loaded asset: " + k);
                    }
            }
            if (objects.Count == 0)
            {
                Debug.LogError(Assembly.GetExecutingAssembly().GetName() + " found no objects in array. The functions may not work as intended.");
                return null;
            }
            return objects;
        }
        public static IEnumerator<float> DoActionAfter(float seconds, System.Action action)
        {
            float time = 0;
            while (time < 1f)
            {
                time += Time.fixedDeltaTime / seconds;
                yield return Time.fixedDeltaTime;
            }

            action();
        }
    }
}
