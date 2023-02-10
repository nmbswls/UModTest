using Bag;
using GUIPackage;
using HarmonyLib;
using KBEngine;
using Newtonsoft.Json.Linq;
using script.NewLianDan.LianDan;
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
    /// 炼丹扩展 针对特殊炉特殊处理
    /// </summary>
    [HarmonyPatch(typeof(LianDanPanel), "PutDanLu")]
    public class LianDanPanelPatcher_PutDanLu
    {
        public static void Postfix(LianDanPanel __instance, DanLuSlot dragSlot)
        {
            if(dragSlot.Item.Id == Consts.ItemId_MineLu)
            {
                var naijiuText = typeof(LianDanPanel).GetField("EquipFightShowTypeDict", BindingFlags.NonPublic | BindingFlags.Static)?.GetValue(__instance) as Text;
                naijiuText.SetText($"");

                __instance.CaoYaoList[2].SetIsLock(value: false);
                __instance.MaxNum = 10;
            }
            
        }
    }

}
