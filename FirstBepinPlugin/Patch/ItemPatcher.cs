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

    /// <summary>
    /// 物品扩展
    /// </summary>
    [HarmonyPatch(typeof(ActiveSkill), "SetSkill")]
    public class ActiveSkillPatcher_SetSkill
    {
        public static void Postfix(ActiveSkill __instance, int id, int level)
        {
            // 未找到 则任意获取一个id相同的技能
            if(__instance.Id == 0)
            {
                foreach (_skillJsonData data in _skillJsonData.DataList)
                {
                    if (data.Skill_ID == id)
                    {
                        __instance.Id = data.id;
                        __instance.SkillId = id;
                        __instance.Level = level;
                        __instance.Quality = data.Skill_LV;
                        __instance.Name = data.name.RemoveNumber();
                        __instance.AttackType = new List<int>(data.AttackType);
                        __instance.PinJie = data.typePinJie;
                        break;
                    }
                }
            }
        }
     }
    
    
}
