using KBEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstBepinPlugin
{
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
            int modVal = skill.getSeidJson(seid)["value1"].I;
            int modVal2 = skill.getSeidJson(seid)["value2"].I;
            SecretsSystem.FightManager.ModYiZhuang((long)modVal);
        }
        
    }
}
