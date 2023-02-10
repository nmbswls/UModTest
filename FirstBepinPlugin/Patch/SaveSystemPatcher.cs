using HarmonyLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstBepinPlugin.Patch
{
    /// <summary>
    /// 对话事件扩展
    /// </summary>
    [HarmonyPatch(typeof(YSNewSaveSystem), "SaveAvatar")]
    public class SaveSystemPatcher_SaveAvatar
    {
        [HarmonyPostfix]
        public static void SaveAvatar(object avatar)
        {
            PluginMain.Main.LogInfo("SaveAvatar extension");
            if(SecretsSystem.Instance.m_SecretsInfo != null)
            {
                var jsonStr = JsonConvert.SerializeObject(SecretsSystem.Instance.m_SecretsInfo);

                JSONObject jsondata = new JSONObject(jsonStr);

                YSNewSaveSystem.Save("MosSecrets.json", jsondata);
            }
            
        }

    }

    /// <summary>
    /// 对话事件扩展
    /// </summary>
    [HarmonyPatch(typeof(YSNewSaveSystem), "LoadAvatar")]
    public class SaveSystemPatcher_LoadAvatar
    {
        [HarmonyPostfix]
        public static void LoadAvatar(KBEngine.Avatar avatar)
        {
            PluginMain.Main.LogInfo("LoadAvatar extension");

            JSONObject jSONObject = YSNewSaveSystem.LoadJSONObject("MosSecrets.json");
            if(jSONObject != null)
            {
                //PluginMain.Main.LogInfo("saved data is " + jSONObject.GetInt("SecretsLevel"));
                var playerInfo = JsonConvert.DeserializeObject<SecretsPlayerInfo>(jSONObject.ToString());
                SecretsSystem.Instance.Reset(playerInfo);
            }
        }
    }
   
}
