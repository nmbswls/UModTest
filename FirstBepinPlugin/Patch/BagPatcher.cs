using Bag;
using HarmonyLib;
using JSONClass;
using KBEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tab;

namespace FirstBepinPlugin.Patch
{
    /// <summary>
    /// 背包扩展 隐藏道具
    /// </summary>
    [HarmonyPatch(typeof(UIInventory), "LoadPlayerItems")]
    public class BaseBag2Patcher_LoadPlayerItems
    {
        public static void Postfix(UIInventory __instance)
        {
            PluginMain.Main.LogError("BaseBag2Patcher_LoadPlayerItems");

            int numOfHide = 0;
            for(int i=0;i< __instance.InventoryLayout.GridDataList.Count;i++)
            {
                var grid = __instance.InventoryLayout.GridDataList[i];

                var itemJsonData = _ItemJsonData.DataDict[grid.Item.itemID];
                grid.Index -= numOfHide;
                // 在背包中隐藏
                if (itemJsonData.ItemFlag.Contains(-99))
                {
                    numOfHide++;
                    __instance.InventoryLayout.GridDataList[i] = null;
                }
            }
            for (int i = __instance.InventoryLayout.GridDataList.Count - 1; i >=0; i--)
            {
                if (__instance.InventoryLayout.GridDataList[i] == null)
                {
                    __instance.InventoryLayout.GridDataList.RemoveAt(i);
                }
            }
        }
    }

    /// <summary>
    /// 背包扩展 隐藏道具
    /// </summary>
    [HarmonyPatch(typeof(BaseBag2), "FiddlerItem")]
    public class BaseBag2Patcher_FiddlerItem
    {
        public static void Postfix(BaseBag2 __instance, BaseItem baseItem, ref bool __result)
        {

            if (__result == false)
            {
                return;
            }

            var itemJsonData = _ItemJsonData.DataDict[baseItem.Id];
            if (itemJsonData != null && itemJsonData.ItemFlag.Contains(-99))
            {
                PluginMain.Main.LogError("TabBag FiddlerItem false.");
                __result = false;
            }
            if(__instance.EquipType == EquipType.丹炉)
            {
                __result = true;
            }
        }
    }



    /// <summary>
    /// 背包扩展 隐藏道具
    /// </summary>
    [HarmonyPatch(typeof(TabBag), "FiddlerItem")]
    public class TabBagPatcher_FiddlerItem
    {
        public static void Postfix(TabBag __instance, BaseItem baseItem, ref bool __result)
        {

            if(__result == false)
            {
                return;
            }

            var itemJsonData = _ItemJsonData.DataDict[baseItem.Id];
            if(itemJsonData != null && itemJsonData.ItemFlag.Contains(-99))
            {
                PluginMain.Main.LogError("TabBag FiddlerItem false.");
                __result = false;
            }
        }
    }
    /// <summary>
    /// 背包扩展 隐藏道具
    /// </summary>
    [HarmonyPatch(typeof(TabBag), "OpenBag")]
    public class TabBagPatcher_OpenBag
    {
        public static void Postfix(TabBag __instance, BagType bagType)
        {
            __instance.UpdateItem();
        }
    }
    
}

