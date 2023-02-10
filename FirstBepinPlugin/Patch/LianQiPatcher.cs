using GUIPackage;
using HarmonyLib;
using KBEngine;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using YSGame.Fight;

namespace FirstBepinPlugin.Patch
{

    /// <summary>
    /// 炼器界面扩展
    /// </summary>
    [HarmonyPatch(typeof(SelectTypePageManager), "init")]
    public class SelectTypePageManagerPatcher_init
    {
        public static void Postfix(SelectTypePageManager __instance)
        {
            var toggleList = typeof(SelectTypePageManager).GetField("equipToggles", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(__instance) as List<Toggle>;

            if(toggleList == null || toggleList.Count == 0)
            {
                PluginMain.Main.LogError("equipToggles not found");
                return;
            }

            // 查找是否已初始化
            if(toggleList.Find((toggle=>toggle.gameObject.name == Consts.sLianqi_SecretTabName)))
            {
                return;
            }

            var newToggleGo = UnityEngine.GameObject.Instantiate(toggleList[0].gameObject, toggleList[0].transform.parent) as UnityEngine.GameObject;
            var newToggle = newToggleGo.GetComponent<Toggle>();

            newToggleGo.name = Consts.sLianqi_SecretTabName;
            toggleList.Add(newToggle);

            newToggle.transform.localPosition += Vector3.up * 100;
            // 关闭所有编辑器拖进去的事件
            for (int listenerIdx = 0; listenerIdx < newToggle.onValueChanged.GetPersistentEventCount(); listenerIdx++)
            {
                newToggle.onValueChanged.SetPersistentListenerState(listenerIdx, UnityEngine.Events.UnityEventCallState.Off);
            }

            // 添加监听
            newToggle.onValueChanged.AddListener(delegate (bool isOn)
            {
                PluginMain.Main.LogInfo("newToggle new listener clicked ");
                __instance.selectEquipType(Consts.SecretEquipType);
            });

            //for (int j = 0; j < newToggleGo.transform.childCount; j++)
            //{
            //    var child2 = newToggleGo.transform.GetChild(j);
            //    PluginMain.Main.LogInfo("jjjjjjjjjjjjjjjjjj" + child2.gameObject.name);
            //    var components = child2.gameObject.GetComponents<MonoBehaviour>();
            //    foreach(var comp in components)
            //    {
            //        PluginMain.Main.LogInfo("components " + comp.GetType());
            //    }
            //}
            //newToggleGo.



            //for (int i=0;i< __instance.transform.childCount;i++)
            //{
            //    var child = __instance.transform.GetChild(i);
            //    PluginMain.Main.LogInfo("iiiiiiiiiiiiiiiiiiiiiiiiii" + child.gameObject.name);

            //    for (int j = 0; j < child.childCount; j++)
            //    {
            //        var child2 = child.GetChild(j);
            //        PluginMain.Main.LogInfo("jjjjjjjjjjjjjjjjjj" + child2.gameObject.name);
            //    }
            //}
        }
    }

    /// <summary>
    /// 炼器界面扩展
    /// </summary>
    [HarmonyPatch(typeof(SelectTypePageManager), "selectEquipType")]
    public class SelectTypePageManagerPatcher_selectEquipType
    {
        public static bool Prefix(SelectTypePageManager __instance, int type = 1)
        {
            if(type != Consts.SecretEquipType)
            {
                return true;
            }

            var toggleList = typeof(SelectTypePageManager).GetField("equipToggles", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(__instance) as List<Toggle>;

            foreach(var toggle in toggleList)
            {
                if(toggle.gameObject.name == Consts.sLianqi_SecretTabName)
                {
                    if (toggle.isOn)
                    {
                        toggle.transform.GetChild(1).GetComponent<Image>().sprite = null;
                    }
                    else
                    {
                        toggle.transform.GetChild(1).GetComponent<Image>().sprite = null;
                    }
                }
            }


            LianQiTotalManager.inst.setCurSelectEquipType(type);
            Traverse.Create(__instance).Method("InitEquipType", new Type[] { typeof(int)}).GetValue(type);

            return false;
        }
    }

    /// <summary>
    /// 炼器界面扩展
    /// </summary>
    [HarmonyPatch(typeof(SelectTypePageManager), "checkCanSelect")]
    public class SelectTypePageManagerPatcher_checkCanSelect
    {
        public static bool Prefix(SelectTypePageManager __instance, int id, ref bool __result)
        {
            if (id == 9970 || id == 9971 || id == 9972 || id == 9973)
            {
                __result = true;
                return false;
            }

            return true;
        }
    }

    /// <summary>
    /// 炼器界面扩展
    /// </summary>
    [HarmonyPatch(typeof(SelectTypePageManager), "InitEquipType")]
    public class SelectTypePageManagerPatcher_InitEquipType
    {
        public static bool Prefix(SelectTypePageManager __instance, int type = 1)
        {
            if (type != Consts.SecretEquipType)
            {
                return true;
            }

            var lianQiEquipCell = typeof(SelectTypePageManager).GetField("lianQiEquipCell", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(__instance) as UnityEngine.GameObject;
            var equipTypeIcon = typeof(SelectTypePageManager).GetField("equipTypeIcon", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(__instance) as List<Sprite>;

            if(lianQiEquipCell == null)
            {
                PluginMain.Main.LogInfo("InitEquipType lianQiEquipCell null ");
            }

            if (equipTypeIcon == null)
            {
                PluginMain.Main.LogInfo("InitEquipType equipTypeIcon null ");
            }

            Tools.ClearObj(lianQiEquipCell.transform);
            PluginMain.Main.LogInfo("InitEquipType after toget ");

            foreach (KeyValuePair<string, JToken> item in jsonData.instance.LianQiEquipType)
            {
                if ((int)item.Value["zhonglei"] == type)
                {

                    LianQiEquipCell component = Tools.InstantiateGameObject(lianQiEquipCell, lianQiEquipCell.transform.parent).GetComponent<LianQiEquipCell>();
                    PluginMain.Main.LogInfo("InitEquipType component ");
                    int fakeId = 0;
                    switch((int)item.Value["id"])
                    {
                        case 9970:
                            fakeId = 8;
                            break;
                        case 9971:
                            fakeId = 3;
                            break;
                        case 9972:
                            fakeId = 5;
                            break;
                        case 9973:
                            fakeId = 4;
                            break;
                    }
                    PluginMain.Main.LogInfo("InitEquipType component ");
                    component.setEquipIcon(equipTypeIcon[fakeId - 1]);
                    component.setEquipName(item.Value["desc"]?.ToString());
                    component.setEquipID((int)item.Value["ItemID"]);
                    component.setZhongLei((int)item.Value["id"]);
                }
            }

            return false ;
        }
    }

    /// <summary>
    /// 炼器界面扩展
    /// </summary>
    [HarmonyPatch(typeof(LianQiPageManager), "updateEuipImage")]
    public class LianQiPageManagerPatcher_updateEuipImage
    {
        public static bool Prefix(LianQiPageManager __instance)
        {
            int selectZhongLei = LianQiTotalManager.inst.selectTypePageManager.getSelectZhongLei();

            if (selectZhongLei == 9970 || selectZhongLei == 9971 || selectZhongLei == 9972 || selectZhongLei == 9973)
            {

                int fakeId = 0;
                switch (selectZhongLei)
                {
                    case 9970:
                        fakeId = 8;
                        break;
                    case 9971:
                        fakeId = 3;
                        break;
                    case 9972:
                        fakeId = 5;
                        break;
                    case 9973:
                        fakeId = 4;
                        break;
                }
                __instance.showEquipCell.setEquipImage((Sprite)LianQiTotalManager.inst.equipSprites[fakeId]);
                return false;
            }
            return true;
        }
    }

    /// <summary>
    /// 炼器界面扩展
    /// </summary>
    [HarmonyPatch(typeof(LianQiResultManager), "GetImagePath")]
    public class LianQiResultManagerPatcher_GetImagePath
    {
        public static bool Prefix(LianQiResultManager __instance)
        {
            var EquipFightShowTypeDict = typeof(FightFaBaoShow).GetField("EquipFightShowTypeDict", BindingFlags.NonPublic | BindingFlags.Static)?.GetValue(null) as Dictionary<int, string>;
            EquipFightShowTypeDict[9970] = "zhu";
            EquipFightShowTypeDict[9971]= "huan";
            EquipFightShowTypeDict[9972]= "xia";
            EquipFightShowTypeDict[9973]= "zhen";

            PluginMain.Main.LogInfo("LianQiResultManagerPatcher_GetImagePath ");

            return true;
        }
    }

}
