using Bag;
using BehaviorDesigner.Runtime;
using DebuggingEssentials;
using GUIPackage;
using HarmonyLib;
using JSONClass;
using KBEngine;
using PaiMai;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YSGame.Fight;
using static ICSharpCode.SharpZipLib.Zip.ExtendedUnixData;
using static UIIconShow;

namespace FirstBepinPlugin.Patch
{

    /// <summary>
    /// 技能扩展 接入战斗初始化
    /// </summary>
    [HarmonyPatch(typeof(RoundManager), "gameStart")]
    public class RoundManagerPatcher_gameStart
    {
        public static void Postfix(RoundManager __instance)
        {
            PluginMain.Main.LogInfo("RoundManagerPatcher_gameStart Postfix ");

            if (UIFightPanel.Inst == null)
            {
                return;
            }
            // 屏蔽一些战斗类型
            switch (Tools.instance.monstarMag.FightType)
            {
                case Fungus.StartFight.FightEnumType.JieDan:
                case Fungus.StartFight.FightEnumType.JieYing:
                case Fungus.StartFight.FightEnumType.ZhuJi:
                case Fungus.StartFight.FightEnumType.HuaShen:
                case Fungus.StartFight.FightEnumType.FeiSheng:
                case Fungus.StartFight.FightEnumType.天劫秘术领悟:
                case Fungus.StartFight.FightEnumType.煅体:
                    return;
            }

            SecretsSystem.FightManager.FightHInit();
        }
    }

    /// <summary>
    /// RoundManager扩展 结束后清理现场
    /// </summary>
    [HarmonyPatch(typeof(RoundManager), "OnDestroy")]
    public class RoundManagerPatcher_OnDestroy
    {
        public static void Postfix(RoundManager __instance)
        {
            PluginMain.Main.LogInfo("RoundManagerPatcher_OnDestroy Postfix ");

            SecretsSystem.FightManager.Ctx.Clear();
            SecretsSystem.FightManager.IsInBattle = false;
        }
    }
    

    /// <summary>
    /// 回合开始扩展事件
    /// </summary>
    [HarmonyPatch(typeof(RoundManager), "startRound")]
    public class RoundManagerPatcher_startRound
    {
        public static void Prefix(RoundManager __instance, Entity _avater)
        {
            PluginMain.Main.LogInfo("RoundManagerPatcher_startRound Prefix ");
            SecretsSystem.FightManager.FightOnRoundStartPre((KBEngine.Avatar)_avater);
        }

        public static void Postfix(RoundManager __instance, Entity _avater)
        {
            PluginMain.Main.LogInfo("RoundManagerPatcher_startRound Postfix ");
            SecretsSystem.FightManager.FightOnRoundStartPost((KBEngine.Avatar)_avater);
        }
    }

    /// <summary>
    /// 回合介绍扩展事件
    /// </summary>
    [HarmonyPatch(typeof(RoundManager), "endRound")]
    public class RoundManagerPatcher_endRound
    {
        public static void Prefix(RoundManager __instance, Entity _avater)
        {
            PluginMain.Main.LogInfo("RoundManagerPatcher_endRound Prefix ");
            SecretsSystem.FightManager.FightOnRoundEndPre((KBEngine.Avatar)_avater);
        }

        public static void Postfix(RoundManager __instance, Entity _avater)
        {
            PluginMain.Main.LogInfo("RoundManagerPatcher_endRound Postfix ");
            SecretsSystem.FightManager.FightOnRoundEndPost((KBEngine.Avatar)_avater);
        }
    }

    /// <summary>
    /// 技能扩展 H 状态下 改变提示
    /// </summary>
    [HarmonyPatch(typeof(UIFightLingQiPlayerSlot), "UseSkillMoveLingQi")]
    public class UIPlayerLingQiControllerPatcher_UseSkillMoveLingQi
    {
        public static bool Prefix(UIFightLingQiPlayerSlot __instance)
        {
            if (UIFightPanel.Inst.UIFightState != UIFightState.释放技能准备灵气阶段)
            {
                return false;
            }

            if (__instance.LingQiType == LingQiType.魔)
            {
                UIPopTip.Inst.Pop("无法使用淫气");
                return false;
            }

            return true;
        }
    }

    /// <summary>
    /// 技能扩展 改变为淫灵气
    /// </summary>
    [HarmonyPatch(typeof(UIFightPanel), "Awake")]
    public class UIFightPanelPatcher_Awake
    {
        public static void Postfix(UIFightPanel __instance)
        {
            var playerLingQiController = __instance.PlayerLingQiController;


            PluginMain.Main.LogInfo("UIFightPanelPatcher_Awake end ");
        }
    }

    /// <summary>
    /// buff扩展 支持更多的seid 实现
    /// </summary>
    [HarmonyPatch(typeof(KBEngine.Buff), "loopRealizeSeid")]
    public class BuffPatcher_loopRealizeSeid
    {
        public static bool Prefix(KBEngine.Buff __instance, int seid, Entity _avatar, List<int> buffInfo, List<int> flag)
        {
            if (seid != Consts.BuffSeId_ModYuWang
                && seid != Consts.BuffSeId_ModYiZhuang
                && seid != Consts.BuffSeId_ModXingFen
                && seid != Consts.BuffSeId_ModKuaiGan
                && seid != Consts.BuffSeId_ModTiLi)
            {
                return true;
            }

            switch (seid)
            {
                case Consts.BuffSeId_ModYuWang:
                    {
                        PluginMain.Main.LogInfo("Buff loopRealizeSeid Handle ListRealizeSeid_ModYuWang ");
                        __instance.ListRealizeSeid_ModYuWang(seid, (KBEngine.Avatar)_avatar, buffInfo, flag);
                    }
                    break;
                case Consts.BuffSeId_ModYiZhuang:
                    {
                        PluginMain.Main.LogInfo("Buff loopRealizeSeid Handle ListRealizeSeid_ModYiZhuang ");
                        __instance.ListRealizeSeid_ModYiZhuang(seid, (KBEngine.Avatar)_avatar, buffInfo, flag);
                    }
                    break;
                case Consts.BuffSeId_ModXingFen:
                    {
                        PluginMain.Main.LogInfo("Buff loopRealizeSeid Handle ListRealizeSeid_ModXingFen ");
                        __instance.ListRealizeSeid_ModXingFen(seid, (KBEngine.Avatar)_avatar, buffInfo, flag);
                    }
                    break;

                case Consts.BuffSeId_ModTiLi:
                    {
                        PluginMain.Main.LogInfo("Buff loopRealizeSeid Handle ListRealizeSeid_ModTiLi ");
                        __instance.ListRealizeSeid_ModTiLi(seid, (KBEngine.Avatar)_avatar, buffInfo, flag);
                    }
                    break;
            }

            return false;
        }
    }



    /// <summary>
    /// buff扩展 支持更多的seid attach
    /// </summary>
    [HarmonyPatch(typeof(KBEngine.Buff), "onAttachRealizeSeid")]
    public class BuffPatcher_onAttachRealizeSeid
    {
        public static bool Prefix(KBEngine.Buff __instance, int seid, Entity _avatar, List<int> buffInfo)
        {
            if (seid != Consts.BuffSeId_ModMeiLi
                )
            {
                return true;
            }
            PluginMain.Main.LogInfo($"onAttachRealizeSeid activate seid:{seid}");
            SecretsSystem.FightManager.OnBonusBuffUpdate(seid);

            return false;
        }
    }

    /// <summary>
    /// buff扩展 支持更多的seid detach
    /// </summary>
    [HarmonyPatch(typeof(KBEngine.Buff), "onDetachRealizeSeid")]
    public class BuffPatcher_onDetachRealizeSeid
    {
        public static bool Prefix(KBEngine.Buff __instance, int seid, Entity _avatar, List<int> buffInfo)
        {
            if (seid != Consts.BuffSeId_ModMeiLi
                )
            {
                return true;
            }
            SecretsSystem.FightManager.OnBonusBuffUpdate(seid);

            return false;
        }
    }

    /// <summary>
    /// 技能扩展 支持更多的trigger判断条件
    /// </summary>
    [HarmonyPatch(typeof(KBEngine.Buff), "CanRealizeSeid")]
    public class BuffPatcher_CanRealizeSeid
    {
        public static bool Prefix(KBEngine.Buff __instance, ref bool __result, KBEngine.Avatar _avatar, List<int> flag, int nowSeid, BuffLoopData buffLoopData = null, List<int> buffInfo = null)
        {
            if (nowSeid != Consts.BuffSeId_CheckYiZhuang
                )
            {
                return true;
            }

            __result = false;

            switch (nowSeid)
            {
                case Consts.BuffSeId_CheckYiZhuang:
                    {
                        var targetVal = __instance.getSeidJson(nowSeid)["value1"].f;
                        var cmpType = __instance.getSeidJson(nowSeid)["value2"].I;
                        var val = SecretsSystem.FightManager.Ctx.YiZhuang;
                        __result = HFightUtils.CustomCompare(val, cmpType, targetVal);
                    }
                    break;
            }

            return false;
        }
    }

    ///// <summary>
    ///// 技能扩展 支持更多的trigger判断条件
    ///// </summary>
    //[HarmonyPatch(typeof(GUIPackage.Skill), "CanRealizeSeid")]
    //public class SkillPatcher_CanRealizeSeid
    //{
    //    public static void Postfix(GUIPackage.Skill __instance, ref bool __result, KBEngine.Avatar attaker, KBEngine.Avatar receiver)
    //    {
            

    //    }
    //}

    /// <summary>
    /// 技能扩展 支持更多的技能特性支持
    /// </summary>
    [HarmonyPatch(typeof(GUIPackage.Skill), "realizeSeid")]
    public class SkillPatcher_realizeSeid
    {
        public static bool Prefix(GUIPackage.Skill __instance, int seid, List<int> damage, Entity _attaker, Entity _receiver, int type)
        {
            if (seid != Consts.SkillSeId_EnterHMode
                && seid != Consts.SkillSeId_ModYiZhuang
                && seid != Consts.SkillSeId_ModYuWang
                && seid != Consts.SkillSeId_ModXingFen
                && seid != Consts.SkillSeId_SwitchTiWei
                && seid != Consts.SkillSeId_ModKuaiGan
                && seid != Consts.SkillSeId_MultiTriggerByUsedTimee
                && seid != Consts.SkillSeId_YinYi
                && seid != Consts.SkillSeId_DiscardNonYinQiAddBuff
                && seid != Consts.SkillSeId_ApplyHAttack
                && seid != Consts.SkillSeId_CheckFirstUse
                && seid != Consts.SkillSeId_CheckTargetNotFaQing
                && seid != Consts.SkillSeId_CheckTargetFaQing)
            {
                return true;
            }

            switch (seid)
            {
                case Consts.SkillSeId_EnterHMode:
                    {
                        PluginMain.Main.LogInfo("Skill realizeSeid Handle SkillSeId_EnterHMode ");
                        __instance.realizeSeid_EnterHMode(seid, damage, (KBEngine.Avatar)_attaker, (KBEngine.Avatar)_receiver, type);
                    }
                    break;
                case Consts.SkillSeId_ModYiZhuang:
                    {
                        PluginMain.Main.LogInfo("Skill realizeSeid Handle realizeSeid_ModYiZhuang ");
                        __instance.realizeSeid_ModYiZhuang(seid, damage, (KBEngine.Avatar)_attaker, (KBEngine.Avatar)_receiver, type);
                    }
                    break;
                case Consts.SkillSeId_ModYuWang:
                    {
                        PluginMain.Main.LogInfo("Skill realizeSeid Handle realizeSeid_ModYiZhuang ");
                        __instance.realizeSeid_ModYuWang(seid, damage, (KBEngine.Avatar)_attaker, (KBEngine.Avatar)_receiver, type);
                        
                    }
                    break;
                case Consts.SkillSeId_ModXingFen:
                    {
                        PluginMain.Main.LogInfo("Skill realizeSeid Handle realizeSeid_ModYiZhuang ");
                        __instance.realizeSeid_ModXingFen(seid, damage, (KBEngine.Avatar)_attaker, (KBEngine.Avatar)_receiver, type);

                    }
                    break;
                case Consts.SkillSeId_SwitchTiWei:
                    {
                        PluginMain.Main.LogInfo("Skill realizeSeid Handle realizeSeid_SwitchTiWei ");
                        __instance.realizeSeid_SwitchTiWei(seid, damage, (KBEngine.Avatar)_attaker, (KBEngine.Avatar)_receiver, type);

                    }
                    break;
                case Consts.SkillSeId_ModKuaiGan:
                    {
                        PluginMain.Main.LogInfo("Skill realizeSeid Handle realizeSeid_ModKuaiGan ");
                        __instance.realizeSeid_ModKuaiGan(seid, damage, (KBEngine.Avatar)_attaker, (KBEngine.Avatar)_receiver, type);

                    }
                    break;
                case Consts.SkillSeId_MultiTriggerByUsedTimee:
                    {
                        PluginMain.Main.LogInfo("Skill realizeSeid Handle realizeSeid_MultiTriggerByUsedTimee ");
                        __instance.realizeSeid_MultiTriggerByUsedTimee(seid, damage, (KBEngine.Avatar)_attaker, (KBEngine.Avatar)_receiver, type);

                    }
                    break;
                case Consts.SkillSeId_YinYi:
                    {
                        PluginMain.Main.LogInfo("Skill realizeSeid Handle realizeSeid_YinYi ");
                        __instance.realizeSeid_YinYi(seid, damage, (KBEngine.Avatar)_attaker, (KBEngine.Avatar)_receiver, type);

                    }
                    break;
                case Consts.SkillSeId_DiscardNonYinQiAddBuff:
                    {
                        PluginMain.Main.LogInfo("Skill realizeSeid Handle realizeSeid_YinYi ");
                        __instance.realizeSeid_DiscardNonYinQiAddBuff(seid, damage, (KBEngine.Avatar)_attaker, (KBEngine.Avatar)_receiver, type);

                    }
                    break;
                case Consts.SkillSeId_ApplyHAttack:
                    {
                        PluginMain.Main.LogInfo("Skill realizeSeid Handle realizeSeid_ApplyHAttack ");
                        __instance.realizeSeid_ApplyHAttack(seid, damage, (KBEngine.Avatar)_attaker, (KBEngine.Avatar)_receiver, type);

                    }
                    break;
                #region check类

                case Consts.SkillSeId_CheckFirstUse:
                    {
                        PluginMain.Main.LogInfo("Skill realizeSeid Handle realizeSeid_ApplyHAttack ");
                        __instance.realizeSeid_CheckFirstUse(seid, damage, (KBEngine.Avatar)_attaker, (KBEngine.Avatar)_receiver, type);

                    }
                    break;
                case Consts.SkillSeId_CheckTargetNotFaQing:
                    {
                        PluginMain.Main.LogInfo("Skill realizeSeid Handle realizeSeid_CheckTargetNotFaQing ");
                        __instance.realizeSeid_CheckTargetNotFaQing(seid, damage, (KBEngine.Avatar)_attaker, (KBEngine.Avatar)_receiver, type);

                    }
                    break;
                case Consts.SkillSeId_CheckTargetFaQing:
                    {
                        PluginMain.Main.LogInfo("Skill realizeSeid Handle realizeSeid_CheckTargetFaQing ");
                        __instance.realizeSeid_CheckTargetFaQing(seid, damage, (KBEngine.Avatar)_attaker, (KBEngine.Avatar)_receiver, type);

                    }
                    break;

                #endregion
            }

            return false;
        }
    }



    /// <summary>
    /// 技能扩展 支持自定义canuse
    /// </summary>
    [HarmonyPatch(typeof(GUIPackage.Skill), "CanUse")]
    public class SkillPatcher_CanUse
    {
        public static void Postfix(GUIPackage.Skill __instance, ref SkillCanUseType __result, Entity _attaker, Entity _receiver, bool showError = true, string uuid = "")
        {
            if(__result != SkillCanUseType.可以使用)
            {
                return;
            }

            foreach (int item in _skillJsonData.DataDict[__instance.skill_ID].seid)
            {
                if (item == Consts.SkillSeId_CheckNotWuLi)
                {
                    if(SecretsSystem.FightManager.Ctx.Tili <= 0)
                    {
                        if (_attaker.isPlayer() && showError)
                        {
                            UIPopTip.Inst.Pop("无力状态不可使用");
                        }
                        __result = (SkillCanUseType)199;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 技能详情 面板扩展 支持淫灵气
    /// </summary>
    [HarmonyPatch(typeof(ToolTipsMag), "CreateSkillCostImg", new Type[] { typeof(ActiveSkill) })]
    public class ToolTipsMagPatcher_CreateSkillCostImg
    {
        public static void Prefix(ToolTipsMag __instance, ActiveSkill activeSkill)
        {
            var _costIconDict = typeof(ToolTipsMag).GetField("_costIconDict", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(__instance) as Dictionary<string, Sprite>;
            if(!_costIconDict.ContainsKey("5_bac"))
            {
                _costIconDict["5_bac"] = _costIconDict["5"];
            }

            if (!_costIconDict.ContainsKey("5_mod"))
            {
                _costIconDict["5_mod"] = PluginMain.Main.LoadAsset<Sprite>("Icons/Cast/cast_type_9.png");
            }

            if (SecretsSystem.FightManager.Ctx.IsFaQing)
            {
                _costIconDict["5"] = _costIconDict["5_mod"];
            }
            else
            {
                _costIconDict["5"] = _costIconDict["5_bac"];
            }
        }
    }


    /// <summary>
    /// 重写ai模块 canendround逻辑 必须等待h逻辑完毕
    /// </summary>
    [HarmonyPatch(typeof(BehaviorDesigner.Runtime.Tasks.Basic.UnityAnimation.EndRound), "CanEndRound")]
    public class TaskEndRoundPatcher_CanEndRound
    {
        public static void Postfix(BehaviorDesigner.Runtime.Tasks.Basic.UnityAnimation.EndRound __instance, ref bool __result)
        {
            if(__result == false)
            {
                return;
            }

            if(!SecretsSystem.FightManager.IsInBattle)
            {
                return;
            }

            // 检查是否有正在进行的process
            if(SecretsSystem.FightManager.m_runningProcessList.Count == 0)
            {
                return;
            }

            __result = false;
        }
    }

    /// <summary>
    /// 重写ai模块 EndRound节点 进入节点时触发h结算
    /// </summary>
    [HarmonyPatch(typeof(BehaviorDesigner.Runtime.Tasks.Basic.UnityAnimation.EndRound), "OnStart")]
    public class TaskEndRoundPatcher_OnStart
    {
        public static void Postfix(BehaviorDesigner.Runtime.Tasks.Basic.UnityAnimation.EndRound __instance)
        {
            // 检查敌人进入体位
            SecretsSystem.FightManager.CheckEnemySwitchTiWei();
        }
    }

    /// <summary>
    /// 支持poptip新格式
    /// </summary>
    [HarmonyPatch(typeof(UIPopTip), "CreateTipObject")]
    public class UIPopTipPatcher_CreateTipObject
    {
        public static void Postfix(UIPopTip __instance, PopTipData data, ref UIPopTipItem __result)
        {
            if(data.IconType == (PopTipIconType)12)
            {
                // 更改图标及颜色
                __result.GetComponent<Image>().color = new Color(0.9f, 0.2f, 0.9f);
            }
        }
    }
    /// <summary>
    /// 增加新图标支持
    /// </summary>
    [HarmonyPatch(typeof(UIPopTip), "Awake")]
    public class UIPopTipPatcher_Awake
    {
        public static void Postfix(UIPopTip __instance)
        {
            var icons = typeof(UIPopTip).GetField("Icon", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(__instance) as List<Sprite>;
            while(icons.Count <= 12)
            {
                icons.Add(null);
            }
            icons[12] = PluginMain.Main.LoadAsset<Sprite>("Icons/icon_lianqi_tab_sex.png");
        }
    }


    
    
    

    #region 提示相关

    /// <summary>
    /// 增加自定义的描述字
    /// </summary>
    [HarmonyPatch(typeof(ActiveSkill), "GetDesc1")]
    public class ActiveSkillPatcher_GetDesc1
    {
        public static void Postfix(ActiveSkill __instance, ref string __result)
        {
            if(__result.Contains("(JiLv)"))
            {
            }
        }
    }

    /// <summary>
    /// 增加自定义的描述字
    /// </summary>
    [HarmonyPatch(typeof(KeyCell), "showLianJiHightLight")]
    public class KeyCellPatcher_showLianJiHightLight
    {
        public static void Postfix(KeyCell __instance)
        {
            __instance.Icon.transform.LogAll();
        }
    }
    


    #endregion
}
