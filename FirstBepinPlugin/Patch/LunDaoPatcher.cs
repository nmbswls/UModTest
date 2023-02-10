using HarmonyLib;
using KBEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace FirstBepinPlugin.Patch
{

    [HarmonyPatch(typeof(UINPCJiaoHuPop), "OnLunDaoBtnClick")]
    public class UINPCJiaoHuPopPatcher_OnLunDaoBtnClick
    {
        public static bool Prefix(UINPCJiaoHuPop __instance)
        {
            //if(SecretsSystem.Instance.Level == 0)
            //{
            //    return true;
            //}

            // 正常论道 直接继续
            UINPCData npc = typeof(UINPCJiaoHuPop).GetField("npc", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(__instance) as UINPCData;
            PluginMain.Main.LogInfo($"交谈对象ID : [{npc.ID}] 绑定ID : [{npc.ZhongYaoNPCID}]");

            UINPCJiaoHu.Inst.IsLunDaoClicked = true;
            UINPCJiaoHu.Inst.HideJiaoHuPop();

            SecretsSystem.Instance.isSecretsLunDao = true;

            Tools.instance.FinalScene = SceneManager.GetActiveScene().name;
            Tools.instance.LunDaoNpcId = npc.ID;
            
            Tools.instance.loadOtherScenes("LunDao");

            return false;
        }
    }


    /// <summary>
    /// LunDaoSuccess panel扩展
    /// </summary>
    [HarmonyPatch(typeof(LunDaoSuccess), "Init")]
    public class LunDaoSuccessPatcher_Init
    {
        [HarmonyPrefix]
        public static bool Init(LunDaoSuccess __instance)
        {
            // 正常论道 直接继续
            if (!SecretsSystem.Instance.isSecretsLunDao)
            {
                return true;
            }

			if (__instance.gameObject.activeSelf)
			{
				return false;
			}

			__instance.gameObject.SetActive(value: true);

			KBEngine.Avatar player = Tools.instance.getPlayer();

			var addWuDaoZhiText = typeof(LunDaoSuccess).GetField("addWuDaoZhiText", AccessTools.all).GetValue(__instance) as UnityEngine.UI.Text;


			addWuDaoZhiText.text = "勤思全用";
            return false;
		}
    }

    /// <summary>
    /// SelectLunTi扩展
    /// </summary>
    [HarmonyPatch(typeof(SelectLunTi), "Init")]
    public class SelectLunTiPatcher_Init
    {
        [HarmonyPrefix]
        public static bool Init(SelectLunTi __instance)
        {
            // 正常论道 直接继续
            if(!SecretsSystem.Instance.isSecretsLunDao)
            {
                return true;
            }

            // 
            __instance.gameObject.SetActive(value: true);
            var lunTiCell = typeof(SelectLunTi).GetField("lunTiCell", AccessTools.all).GetValue(__instance) as UnityEngine.GameObject;
            Transform parent = lunTiCell.transform.parent;

            var YinKeys = new Dictionary<int, string>();
            YinKeys.Add(1,"口1");
            YinKeys.Add(2, "口2");
            YinKeys.Add(3, "口3");
            YinKeys.Add(4, "口4");

            foreach (int key in YinKeys.Keys)
            {
                LunTiCell component = UnityEngine.Object.Instantiate(lunTiCell, parent).gameObject.GetComponent<LunTiCell>();
                component.InitLunTiCell(SecretsSystem.Instance.SecretsLunDaoSpriteGet(key), SecretsSystem.Instance.SecretsLunDaoSpriteGet(0), key, YinKeys[key], __instance.AddLunTiToList, __instance.RemoveLunTiByList);
                component.gameObject.SetActive(value: true);
            }

            return false;
        }
    }
    
}
