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
        public static void LogAll(this Transform transform)
        {
            PluginMain.Main.LogInfo($"========{transform.gameObject.name} comps:" );
            var comps = transform.gameObject.GetComponents<MonoBehaviour>();
            foreach(var comp in comps)
            {
                PluginMain.Main.LogInfo($"================{comp.GetType()} :");
            }
            for(int i=0;i<transform.childCount;i++)
            {
                transform.GetChild(i).LogAll();
            }
        }
    }
    public static class BuffEx
    {
        //public static void ListRealizeSeid_SwitchIntoHMode(this KBEngine.Buff buff,int seid, Avatar avatar, List<int> buffInfo, List<int> flag)
        //{
        //    SecretsSystem.Instance.FightEnterHMode();
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
            int modVal = skill.getSeidJson(seid)["value1"].I * 100;
            int modVal2 = skill.getSeidJson(seid)["value2"].I;
            SecretsSystem.FightManager.ModYiZhuang((long)modVal);
        }
        public static void realizeSeid_ModYuWang(this GUIPackage.Skill skill, int seid, List<int> damage, KBEngine.Avatar attaker, KBEngine.Avatar receiver, int type)
        {
            int target = skill.getSeidJson(seid)["value1"].I;
            int modVal = skill.getSeidJson(seid)["value2"].I * 100;
            SecretsSystem.FightManager.ModYuWang(target,(long)modVal);
        }

        public static void realizeSeid_ModXingFen(this GUIPackage.Skill skill, int seid, List<int> damage, KBEngine.Avatar attaker, KBEngine.Avatar receiver, int type)
        {
            int modVal = skill.getSeidJson(seid)["value1"].I * 100;
            SecretsSystem.FightManager.ModXingFen((long)modVal);
        }

        public static void realizeSeid_SwitchTiWei(this GUIPackage.Skill skill, int seid, List<int> damage, KBEngine.Avatar attaker, KBEngine.Avatar receiver, int type)
        {
            int tiweiId = skill.getSeidJson(seid)["value1"].I;
            SecretsSystem.FightManager.SwitchTiWei(tiweiId);
        }

        public static void realizeSeid_ModKuaiGan(this GUIPackage.Skill skill, int seid, List<int> damage, KBEngine.Avatar attaker, KBEngine.Avatar receiver, int type)
        {
            int target = skill.getSeidJson(seid)["value1"].I;
            int modVal = skill.getSeidJson(seid)["value2"].I * 100;
            SecretsSystem.FightManager.ModKuaiGan(target, (long)modVal);
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
    }
}
