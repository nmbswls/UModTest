using KBEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltimateSurvival.AI.Actions;
using UnityEngine;

namespace FirstBepinPlugin
{
    public static class TransformEx
    {
        public static void LogAll(this Transform transform, int depth = 0)
        {
            var logPrefix = "";
            for(int i=0;i<depth;i++)
            {
                logPrefix += "====";
            }
            PluginMain.Main.LogInfo($"{logPrefix}========{transform.gameObject.name} comps:" );
            var comps = transform.gameObject.GetComponents<Component>();
            foreach(var comp in comps)
            {
                PluginMain.Main.LogInfo($"{logPrefix}========compType:{comp.GetType()} :");
            }
            for(int i=0;i<transform.childCount;i++)
            {
                transform.GetChild(i).LogAll(depth + 1);
            }
        }
    }
    public static class BuffEx
    {
        public static void ListRealizeSeid_ModYuWang(this KBEngine.Buff buff, int seid, KBEngine.Avatar avatar, List<int> buffInfo, List<int> flag)
        {
            float num = buff.getSeidJson(seid)["value1"].f * buffInfo[1];
            SecretsSystem.FightManager.ModYuWang(1, num);
        }

        public static void ListRealizeSeid_ModXingFen(this KBEngine.Buff buff, int seid, KBEngine.Avatar avatar, List<int> buffInfo, List<int> flag)
        {
            int targetPart = buff.getSeidJson(seid)["value1"].I;
            float val = buff.getSeidJson(seid)["value2"].f * buffInfo[1];
            SecretsSystem.FightManager.ModXingFen(1, val);
        }
        //public void ListRealizeSeid9(int seid, Avatar avatar, List<int> buffInfo, List<int> flag)
        //{
        //    int num = getSeidJson(seid)["value2"].I * buffInfo[1];
        //    foreach (JSONObject item in getSeidJson(seid)["value1"].list)
        //    {
        //        if (Tools.instance.getSkillIDByKey(flag[1]) == item.I)
        //        {
        //            flag[0] += num;
        //            break;
        //        }
        //    }
        //}
    }

    public static class SkillEx
    {
        //public static void realizeSeid_SwitchOutHMode(this GUIPackage.Skill skill, int seid, List<int> damage, KBEngine.Avatar attaker, KBEngine.Avatar receiver, int type)
        //{
        //    SecretsSystem.Instance.FightExitHMode();
        //}

        public static void realizeSeid_EnterHMode(this GUIPackage.Skill skill, int seid, List<int> damage, KBEngine.Avatar attaker, KBEngine.Avatar receiver, int type)
        {
            SecretsSystem.FightManager.FightEnterHMode();
        }

        public static void realizeSeid_ModYiZhuang(this GUIPackage.Skill skill, int seid, List<int> damage, KBEngine.Avatar attaker, KBEngine.Avatar receiver, int type)
        {
            float modVal = skill.getSeidJson(seid)["value1"].f;
            int modVal2 = 0;
            if (skill.getSeidJson(seid).HasField("value2"))
            {
                modVal2 = skill.getSeidJson(seid)["value2"].I;
            }
            SecretsSystem.FightManager.ModYiZhuang(modVal);
        }
        public static void realizeSeid_ModYuWang(this GUIPackage.Skill skill, int seid, List<int> damage, KBEngine.Avatar attaker, KBEngine.Avatar receiver, int type)
        {
            int target = skill.getSeidJson(seid)["value1"].I;
            float modVal = skill.getSeidJson(seid)["value2"].f;
            SecretsSystem.FightManager.ModYuWang(target,  modVal);
        }

        public static void realizeSeid_ModXingFen(this GUIPackage.Skill skill, int seid, List<int> damage, KBEngine.Avatar attaker, KBEngine.Avatar receiver, int type)
        {
            int targetPart = skill.getSeidJson(seid)["value1"].I;
            float modVal = skill.getSeidJson(seid)["value2"].f;
            SecretsSystem.FightManager.ModXingFen(targetPart, modVal);
        }

        public static void realizeSeid_SwitchTiWei(this GUIPackage.Skill skill, int seid, List<int> damage, KBEngine.Avatar attaker, KBEngine.Avatar receiver, int type)
        {
            int tiweiId = skill.getSeidJson(seid)["value1"].I;
            SecretsSystem.FightManager.SwitchTiWei(tiweiId);
        }

        public static void realizeSeid_ModKuaiGan(this GUIPackage.Skill skill, int seid, List<int> damage, KBEngine.Avatar attaker, KBEngine.Avatar receiver, int type)
        {
            int target = skill.getSeidJson(seid)["value1"].I;
            float modVal = skill.getSeidJson(seid)["value2"].f;
            SecretsSystem.FightManager.ModKuaiGan(target, modVal);
        }

        public static void realizeSeid_MultiTriggerByUsedTimee(this GUIPackage.Skill skill, int seid, List<int> damage, KBEngine.Avatar attaker, KBEngine.Avatar receiver, int type)
        {
            var usedSkills = skill.attack.fightTemp.NowRoundUsedSkills;
            int usedNum = 0;
            foreach(var usedSkillId in usedSkills)
            {
                if(usedSkillId == skill.skill_ID)
                {
                    usedNum++;
                }
            }
            int initCount = skill.getSeidJson(seid)["value1"].I;
            int modPerUse = skill.getSeidJson(seid)["value2"].I;
            int triggerMax = skill.getSeidJson(seid)["value3"].I;
            int triggerMin = skill.getSeidJson(seid)["value4"].I;

            int triggerCount = initCount + usedNum * modPerUse;
            if(triggerMax > 0 && triggerCount > triggerMax)
            {
                triggerCount = triggerMax;
            }
            if(triggerCount < triggerMin)
            {
                triggerCount = triggerMin;
            }
            damage[4] = triggerCount;
        }

        public static void realizeSeid_YinYi(this GUIPackage.Skill skill, int seid, List<int> damage, KBEngine.Avatar attaker, KBEngine.Avatar receiver, int type)
        {
            SecretsSystem.FightManager.TriggerYinYi();
        }

        public static void realizeSeid_DiscardNonYinQiAddBuff(this GUIPackage.Skill skill, int seid, List<int> damage, KBEngine.Avatar attaker, KBEngine.Avatar receiver, int type)
        {
            PluginMain.Main.LogError(attaker.name);
            int maxDiscardNum = skill.getSeidJson(seid)["value1"].I;
            int discardNum = 0;
            while (discardNum < maxDiscardNum)
            {
                var card = attaker.cardMag.getRandomCard();
                if(card == null)
                {
                    break;
                }
                RoundManager.instance.removeCard(attaker, 1, card.cardType);
                discardNum++;
            }
            int countPerCard = skill.getSeidJson(seid)["value2"].I;
            int buffId = skill.getSeidJson(seid)["value3"].I;
            attaker.spell.addBuff(buffId, countPerCard * discardNum);
        }
        
    }
}
