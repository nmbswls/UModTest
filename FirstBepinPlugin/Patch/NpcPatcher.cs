using FirstBepinPlugin.MonoScripts;
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
    [HarmonyPatch(typeof(UINPCJiaoHu), "Awake")]
    public class UINPCJiaoHu_Awake
    {
        public static void Postfix(UINPCJiaoHu __instance)
        {
            var parent = __instance.InfoPanel.transform.parent;

            var secretsDetailPanel = parent.Find(PluginMain.NpcSecretDetailPanelName);

            if (secretsDetailPanel != null)
            {
                return;
            }

            var newPanelGo = UnityEngine.Object.Instantiate(PluginMain.Main.LoadGameObjectFromAB(PluginMain.NpcSecretDetailPanelName), parent);
            var panel = newPanelGo.AddComponent<SecretsDetailPanel>();

            panel.Owner = __instance;

            PluginMain.Main.LogInfo("newPanelGonewPanelGonewPanelGonewPanelGonewPanelGonewPanelGonewPanelGo" + panel.RT.sizeDelta);
            newPanelGo.transform.localPosition = Vector3.zero;
            newPanelGo.transform.localScale = Vector3.one;
            newPanelGo.name = PluginMain.NpcSecretDetailPanelName;
            newPanelGo.SetActive(false);

            SecretsSystem.Instance.panel = panel;
        }
    }




    [HarmonyPatch(typeof(UINPCJiaoHuPop), "RefreshUI")]
    public class UINPCJiaoHuPopPatcher_RefreshUI
    {
        public static void Postfix(UINPCJiaoHuPop __instance)
        {
            var secretsDetailBtn = __instance.transform.Find(PluginMain.NpcSecretsDetailButtonName);

            if(secretsDetailBtn != null)
            {
                return;
            }
            var parent = __instance.transform;

            var newButtonGo = UnityEngine.Object.Instantiate(PluginMain.Main.LoadGameObjectFromAB(PluginMain.NpcSecretsDetailButtonName), parent);

            newButtonGo.name = PluginMain.NpcSecretsDetailButtonName;
            newButtonGo.SetActive(true);
            newButtonGo.transform.localPosition = new Vector3(0,-300,0);
            PluginMain.Main.LogInfo("icon scale is" + newButtonGo.transform.lossyScale);
            newButtonGo.GetComponent<Button>().onClick.AddListener(delegate () {
                SecretsSystem.Instance.ShowNpcSecretsUI();
            });
            //secretsDetailBtn = UnityEngine.Object.Instantiate(PluginMain.Main.LoadGameObjectFromAB("NpcSecretsDetailButton"), nameComp.transform.parent);
        }
    }
}
