using System;
using System.Diagnostics;
using System.Reflection;
using Modding;
using JetBrains.Annotations;
using MonoMod.RuntimeDetour;
using UnityEngine.SceneManagement;
using UnityEngine;
using USceneManager = UnityEngine.SceneManagement.SceneManager;
using UObject = UnityEngine.Object;
using System.Collections.Generic;
using System.IO;

namespace SceneTutorial
{
    [UsedImplicitly]
    public class SceneTutorial : Mod, ITogglableMod
    {
        public override string GetVersion() => "0.0.0.0";
        public static Dictionary<string, AssetBundle> Bundles;
        
        public override void Initialize()
        {
            Log("Initalizing.");
            Bundles = new Dictionary<string, AssetBundle>();
            Unload();
            ModHooks.Instance.AfterSavegameLoadHook += AfterSaveGameLoad;
            ModHooks.Instance.NewGameHook += AddComponent;

            Assembly asm = Assembly.GetExecutingAssembly();
            foreach (string res in asm.GetManifestResourceNames())
            {
                using (Stream s = asm.GetManifestResourceStream(res))
                {
                    if (s == null) continue;
                    byte[] buffer = new byte[s.Length];
                    s.Read(buffer, 0, buffer.Length);
                    s.Dispose();
                    string bundleName = Path.GetExtension(res).Substring(1);
                    if (bundleName != "newbund") continue;
                    Bundles[bundleName] = AssetBundle.LoadFromMemory(buffer);  
                }
            }
        }

        private void AfterSaveGameLoad(SaveGameData data) => AddComponent();

        private void AddComponent()
        {
            GameManager.instance.gameObject.AddComponent<LoadScene>();
        }

        public void Unload()
        {
            ModHooks.Instance.AfterSavegameLoadHook -= AfterSaveGameLoad;
            ModHooks.Instance.NewGameHook -= AddComponent;

            // ReSharper disable once Unity.NoNullPropogation
            var x = GameManager.instance?.gameObject.GetComponent<LoadScene>();
            if (x == null) return;
            UObject.Destroy(x);
        }
    }
}