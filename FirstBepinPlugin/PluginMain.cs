using BepInEx;
using FirstBepinPlugin.Config;
using HarmonyLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace FirstBepinPlugin
{
    [BepInPlugin("MiChangSheng_Secrets_Extension", "MiChangSheng Secrets Extension", "0.1")]
    public class PluginMain : BaseUnityPlugin
    {
        public static PluginMain Main;

		public Dictionary<string, GameObject> m_GoDict;
		public string Path => System.IO.Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));

		//Unity的Start生命周期
		void Start()
        {
            Main = this;
            //输出日志
            Logger.LogInfo("HelloWorld!");

            new Harmony("MiChangSheng_Secrets_Extension").PatchAll();

			m_GoDict = new Dictionary<string, GameObject>();

			LoadConfig();

			StartCoroutine(LoadAssetAsync());
		}

		public void Update()
		{
			if(SecretsSystem.FightManager.IsInBattle)
			{
				SecretsSystem.FightManager.FightTick(Time.deltaTime);
            }
		}

		#region 日志
		public void LogInfo(string msg)
        {
            Logger.LogInfo(msg);
        }

		public void LogError(string msg)
		{
			Logger.LogError(msg);
		}
        #endregion

        public SecretsConfig config;

		public Dictionary<int, ConfigDataHAttackShowInfo> ConfigDataHAttackShowInfo = new Dictionary<int, ConfigDataHAttackShowInfo>();
		public void LoadConfig()
		{

			try
			{
				{
                    var fileName = Path + "/Config/SecretsConfig.json";
                    if (!File.Exists(fileName))
                    {
                        Logger.LogError($"load config Error");
                        return;
                    }
                    var json = File.ReadAllText(fileName);
                    config = JsonConvert.DeserializeObject<SecretsConfig>(json);
                    if (config == null)
                        throw new JsonException("json data is invalid.");
                }

				{
                    var hInfoConfigName = Path + "/Config/HAttackShowInfo.json";
                    if (!File.Exists(hInfoConfigName))
                    {
                        Logger.LogError($"load hInfoConfigName Error");
                        return;
                    }
                    var json = File.ReadAllText(hInfoConfigName);
                    ConfigDataHAttackShowInfo = JsonConvert.DeserializeObject<Dictionary<int, ConfigDataHAttackShowInfo>>(json);
                }
            }
			catch (Exception e)
			{
				Logger.LogError($"load config Error");
			}
		}


		public static string ABName = "allneed";
		public static string NpcSecretsDetailButtonName = "NpcSecretsDetailButton";
		public static string NpcSecretDetailPanelName = "NpcSecretDetailPanel";

		public AssetBundle allNeed;
		public Font font_YaHei;

		public IEnumerator LoadAssetAsync()
		{
			Debug.LogError("path is " + Path + ABName);
			AssetBundleCreateRequest assetBundleCreateRequest = AssetBundle.LoadFromFileAsync(Path + "/" + ABName);
			yield return assetBundleCreateRequest;
			allNeed = assetBundleCreateRequest.assetBundle;

			if (allNeed == null)
			{
				Debug.LogError("Failed to load AssetBundle:ABName!");
				yield break;
			}
			GameObject[] assetLoadRequestGo = allNeed.LoadAllAssets<GameObject>();
			yield return assetLoadRequestGo;
			GameObject[] array = assetLoadRequestGo;
			foreach (GameObject gameObject in array)
			{
				Logger.LogInfo("------------------------------- name" + gameObject.name);
				m_GoDict.TryAdd(gameObject.name, gameObject);
			}

            font_YaHei = Font.CreateDynamicFontFromOSFont("Microsoft YaHei", 6);
        }

		public UnityEngine.GameObject LoadGameObjectFromAB(string assetName)
        {
			if(!m_GoDict.TryGetValue(assetName, out var go))
            {
				return null;
            }
			return go;
        }

		private static string AbAssetPrefix = "Assets/Res/allneed/";
		public T LoadAsset<T>(string assetName) where T:UnityEngine.Object
        {
			if(allNeed == null)
            {
				return null;
            }
			return allNeed.LoadAsset<T>(AbAssetPrefix + assetName);
		}
	}
}
