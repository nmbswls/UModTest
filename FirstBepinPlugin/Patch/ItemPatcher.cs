using Bag;
using HarmonyLib;
using JSONClass;
using KBEngine;
using SkySwordKill.Next.DialogEvent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstBepinPlugin.Patch
{
    /// <summary>
    /// 物品扩展
    /// </summary>
    [HarmonyPatch(typeof(BaseItem), "Create")]
    public class BaseItemPatcher_Create
    {
        public static bool Prefix(int id, int count, string uuid, JSONObject seid, ref BaseItem __result)
        {
            int type = 0;
            try
            {
                type = _ItemJsonData.DataDict[id].type;
            }
            catch (Exception message)
            {
                return true;
            }
            switch (type)
            {
                case Consts.SecretEquipType:
                {
                    var baseItem = new EquipItem();
                    baseItem.SetItem(id, count, seid);
                    baseItem.Uid = uuid;
                    __result = baseItem;
                    return false;
                }
            }
            return true;
        }
    }
}
