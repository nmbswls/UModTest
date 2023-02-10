using HarmonyLib;
using KBEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstBepinPlugin.Patch
{
    /// <summary>
    /// 创建角色扩展
    /// </summary>
    [HarmonyPatch(typeof(MainUISelectTianFu), "NextPage")]
    public class MainUISelectTianFuPatcher_NextPage
    {
        public static bool Prefix(MainUISelectTianFu __instance)
        {
            // 当选择入口天赋时 检查前置条件
            if(!__instance.hasSelectList.ContainsKey(PluginMain.Main.config.EntryTalentId))
            {
                return true;
            }

            PluginMain.Main.LogInfo("选择入口天赋");

            // 最后一页 检查情况
            if (__instance.curPage == 8)
            {
                if(MainUIPlayerInfo.inst.sex != 2)
                {
                    PluginMain.Main.LogInfo("性别不对");
                    UIPopTip.Inst.Pop("圣女开局必须指定女性");
                    return false;
                }
            }

            return true;
        }
    }

    /// <summary>
    /// 创建角色扩展
    /// </summary>
    [HarmonyPatch(typeof(CreateNewPlayerFactory), "createPlayer")]
    public class CreateNewPlayerFactoryPatcher_createPlayer
    {
        public static void Postfix(CreateNewPlayerFactory __instance, int id, int index, string firstName, string lastName, KBEngine.Avatar avatar)
        {
            if(SecretsSystem.Instance.IsSecretOpen())
            {
                SecretsSystem.Instance.m_SecretsInfo = SecretsPlayerInfo.CreateNew();
            }
        }
    }
}
