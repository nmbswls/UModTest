using Bag;
using DebuggingEssentials;
using GUIPackage;
using HarmonyLib;
using JSONClass;
using KBEngine;
using PaiMai;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using YSGame.Fight;
using static ICSharpCode.SharpZipLib.Zip.ExtendedUnixData;

namespace FirstBepinPlugin.Patch
{
    // 扩展性太差 暂时用魔气替代淫灵气
    //#region UIPlayerLingQiController扩展 装slot的

    ///// <summary>
    ///// 技能扩展 刷新灵气数量时考虑淫灵气
    ///// </summary>
    //[HarmonyPatch(typeof(UIPlayerLingQiController), "RefreshLingQiCount")]
    //public class UIPlayerLingQiControllerPatcher_RefreshLingQiCount
    //{
    //    public static void Postfix(UIPlayerLingQiController __instance, bool show)
    //    {
    //        if (show)
    //        {
    //            var slot = __instance.SlotList[Consts.YinLingQiTypeInt];
    //            slot.LingQiCountText.text = slot.LingQiCount.ToString();
    //            slot.SetLingQiCountShow(show: true);
    //        }
    //        else
    //        {
    //            var slot = __instance.SlotList[Consts.YinLingQiTypeInt];
    //            slot.SetLingQiCountShow(show: false);
    //        }

    //        Dictionary<LingQiType, int> nowCacheLingQi = UIFightPanel.Inst.CacheLingQiController.GetNowCacheLingQi();

    //        {
    //            var slot = __instance.SlotList[Consts.YinLingQiTypeInt];

    //            if (UIFightPanel.Inst.UIFightState == UIFightState.释放技能准备灵气阶段)
    //            {
    //                if (nowCacheLingQi.ContainsKey(slot.LingQiType))
    //                {
    //                    slot.HighlightObj.SetActive(value: true);
    //                }
    //                else
    //                {
    //                    slot.HighlightObj.SetActive(value: false);
    //                }
    //            }
    //            else
    //            {
    //                slot.HighlightObj.SetActive(value: false);
    //            }
    //        }
    //    }
    //}
    ///// <summary>
    ///// 技能扩展 重置灵气数量时考虑淫灵气
    ///// </summary>
    //[HarmonyPatch(typeof(UIPlayerLingQiController), "ResetPlayerLingQiCount")]
    //public class UIPlayerLingQiControllerPatcher_ResetPlayerLingQiCount
    //{
    //    public static void Postfix(UIPlayerLingQiController __instance)
    //    {
    //        int num = PlayerEx.Player.cardMag[Consts.YinLingQiTypeInt];
    //        if (__instance.SlotList[Consts.YinLingQiTypeInt].LingQiCount != num)
    //        {
    //            __instance.SlotList[Consts.YinLingQiTypeInt].LingQiCount = num;
    //        }
    //    }
    //}

    ///// <summary>
    ///// 技能扩展 累加灵气数量时考虑淫灵气
    ///// </summary>
    //[HarmonyPatch(typeof(UIPlayerLingQiController), "GetPlayerLingQiSum")]
    //public class UIPlayerLingQiControllerPatcher_GetPlayerLingQiSum
    //{
    //    public static void Postfix(UIPlayerLingQiController __instance, ref int __result)
    //    {
    //        __result += PlayerEx.Player.cardMag[Consts.YinLingQiTypeInt];
    //    }
    //}


    //#endregion


    ///// <summary>
    ///// 技能扩展 重置灵气数量时考虑淫灵气
    ///// </summary>
    //[HarmonyPatch(typeof(UICacheLingQiController), "RefreshShengKe")]
    //public class UICacheLingQiControllerPatcher_RefreshShengKe
    //{
    //    public static bool Prefix(UICacheLingQiController __instance)
    //    {
    //        Dictionary<int, int> nowCacheLingQiIntDict = __instance.GetNowCacheLingQiIntDict();
    //        foreach (KeyValuePair<int, int> item in nowCacheLingQiIntDict)
    //        {
    //            if (item.Key == Consts.YinLingQiTypeInt)
    //            {
    //                return false;
    //            }
    //        }
    //        return true;
    //    }
    //}
    

    ///// <summary>
    ///// 技能扩展 显示淫灵气面板
    ///// </summary>
    //[HarmonyPatch(typeof(UIFightLingQiPlayerSlot), "OnLingQiCountChanged")]
    //public class UIPlayerLingQiControllerPatcher_OnLingQiCountChanged
    //{
    //    public static void Postfix(UIFightLingQiPlayerSlot __instance, int change)
    //    {
    //        PluginMain.Main.LogInfo("UIPlayerLingQiControllerPatcher_OnLingQiCountChanged success.");

    //        if (__instance.LingQiType == Consts.YinLingQiType)
    //        {
    //            if (__instance.LingQiCount > 0)
    //            {
    //                __instance.gameObject.SetActive(value: true);
    //                //UIFightPanel.Inst.PlayerLingQiController.MoBG.SetActive(value: true);
    //            }
    //            else
    //            {
    //                __instance.gameObject.SetActive(value: false);
    //                //UIFightPanel.Inst.PlayerLingQiController.MoBG.SetActive(value: false);
    //            }
    //        }
    //    }
    //}

    ///// <summary>
    ///// 技能扩展 使用技能时 不能用淫灵气
    ///// </summary>
    //[HarmonyPatch(typeof(UIFightLingQiPlayerSlot), "UseSkillMoveLingQi")]
    //public class UIPlayerLingQiControllerPatcher_UseSkillMoveLingQi
    //{
    //    public static bool Prefix(UIFightLingQiPlayerSlot __instance)
    //    {
    //        if (__instance.LingQiType != Consts.YinLingQiType)
    //        {
    //            return true;
    //        }
    //        if (UIFightPanel.Inst.UIFightState != UIFightState.释放技能准备灵气阶段)
    //        {
    //            return false;
    //        }
    //        UIPopTip.Inst.Pop("无法使用淫气");
    //        return false;
    //    }
    //}

    ///// <summary>
    ///// 技能扩展 消散时 不能消散淫灵气
    ///// </summary>
    //[HarmonyPatch(typeof(UIFightLingQiPlayerSlot), "XiaoSanLingQiMoveOne")]
    //public class UIPlayerLingQiControllerPatcher_XiaoSanLingQiMoveOne
    //{
    //    public static bool Prefix(UIFightLingQiPlayerSlot __instance)
    //    {
    //        if (__instance.LingQiType != Consts.YinLingQiType)
    //        {
    //            return true;
    //        }
    //        if (UIFightPanel.Inst.UIFightState != UIFightState.回合结束弃置灵气阶段 || __instance.LingQiCount <= 0)
    //        {
    //            return false;
    //        }
    //        UIPopTip.Inst.Pop("无法消散淫气");
    //        return false;
    //    }
    //}

    ///// <summary>
    ///// 技能扩展 消散时 不能消散淫灵气
    ///// </summary>
    //[HarmonyPatch(typeof(UIFightLingQiPlayerSlot), "XiaoSanLingQiMoveAll")]
    //public class UIPlayerLingQiControllerPatcher_XiaoSanLingQiMoveAll
    //{
    //    public static bool Prefix(UIFightLingQiPlayerSlot __instance)
    //    {
    //        if (__instance.LingQiType != Consts.YinLingQiType)
    //        {
    //            return true;
    //        }
    //        if (UIFightPanel.Inst.UIFightState != UIFightState.回合结束弃置灵气阶段 || __instance.LingQiCount <= 0)
    //        {
    //            return false;
    //        }
    //        UIPopTip.Inst.Pop("无法消散淫气");
    //        return false;
    //    }
    //}
    


    ///// <summary>
    ///// 技能扩展 支持淫灵气
    ///// </summary>
    //[HarmonyPatch(typeof(UIFightPanel), "Awake")]
    //public class UIFightPanelPatcher_Awake
    //{
    //    public static void Postfix(UIFightPanel __instance)
    //    {
    //        var playerLingQiController = __instance.PlayerLingQiController;

    //        var lastData = __instance.LingQiImageDatas[__instance.LingQiImageDatas.Count - 1];
    //        while(__instance.LingQiImageDatas.Count <= Consts.YinLingQiTypeInt)
    //        {
    //            __instance.LingQiImageDatas.Add(null);
    //        }
    //        var newImageData = new UILingQiImageData();

    //        newImageData.Normal = PluginMain.Main.LoadAsset<Sprite>("Icons/Lingqi_yin/MCS_ZD_linqi_yin.png");
    //        newImageData.Lock = PluginMain.Main.LoadAsset<Sprite>("Icons/Lingqi_yin/MCS_ZD_linqi_yin_an.png");
    //        newImageData.Press = PluginMain.Main.LoadAsset<Sprite>("Icons/Lingqi_yin/MCS_ZD_linqi_yin_bk.png");
    //        newImageData.Highlight = PluginMain.Main.LoadAsset<Sprite>("Icons/Lingqi_yin/MCS_ZD_linqi_yin_hlg.png");

    //        newImageData.Normal2 = PluginMain.Main.LoadAsset<Sprite>("Icons/Lingqi_yin/MCS_ZD_linqi_yinduo.png");
    //        newImageData.Lock2 = PluginMain.Main.LoadAsset<Sprite>("Icons/Lingqi_yin/MCS_ZD_linqi_yinduo_an.png");
    //        newImageData.Press2 = PluginMain.Main.LoadAsset<Sprite>("Icons/Lingqi_yin/MCS_ZD_linqi_yinduo_bk.png");
    //        newImageData.Highlight2 = PluginMain.Main.LoadAsset<Sprite>("Icons/Lingqi_yin/MCS_ZD_linqi_yinduo_hlg.png");

    //        newImageData.Normal3 = PluginMain.Main.LoadAsset<Sprite>("Icons/Lingqi_yin/MCS_ZD_linqi_yinduo2.png");
    //        newImageData.Lock3 = PluginMain.Main.LoadAsset<Sprite>("Icons/Lingqi_yin/MCS_ZD_linqi_yinduo2_an.png");
    //        newImageData.Press3 = PluginMain.Main.LoadAsset<Sprite>("Icons/Lingqi_yin/MCS_ZD_linqi_yinduo2_bk.png");
    //        newImageData.Highlight3 = PluginMain.Main.LoadAsset<Sprite>("Icons/Lingqi_yin/MCS_ZD_linqi_yinduo2_hlg.png");

    //        __instance.LingQiImageDatas[Consts.YinLingQiTypeInt] = newImageData;

    //        var moSlot = playerLingQiController.SlotList[5];

    //        while (playerLingQiController.SlotList.Count <= Consts.YinLingQiTypeInt)
    //        {
    //            playerLingQiController.SlotList.Add(null);
    //        }

    //        var newSlotGo = UnityEngine.GameObject.Instantiate(moSlot.gameObject, moSlot.transform.parent);
    //        newSlotGo.transform.localPosition += Vector3.up * 100;
    //        var newSlot = newSlotGo.GetComponent<UIFightLingQiPlayerSlot>();
    //        playerLingQiController.SlotList[Consts.YinLingQiTypeInt] = newSlot;

    //        newSlot.LingQiType = Consts.YinLingQiType;

    //        // 填充UIFightMoveLingQi static 信息
    //        var pColors = typeof(UIFightMoveLingQi).GetField("pColors", BindingFlags.NonPublic | BindingFlags.Static)?.GetValue(null) as List<Color>;
    //        while(pColors.Count <= Consts.YinLingQiTypeInt)
    //        {
    //            pColors.Add(Color.white);
    //        }
    //        pColors[Consts.YinLingQiTypeInt] = new Color(0.9f, 0.4f, 0.9f);

    //        while(__instance.FightSkillTip.CostSprites.Count <= Consts.YinLingQiTypeInt)
    //        {
    //            __instance.FightSkillTip.CostSprites.Add(null);
    //        }
    //        __instance.FightSkillTip.CostSprites[Consts.YinLingQiTypeInt] = PluginMain.Main.LoadAsset<Sprite>("Icons/Cast/cast_type_9.png");


    //        PluginMain.Main.LogInfo("UIFightPanelPatcher_Awake end ");
    //    }
    //}
    

    ///// <summary>
    ///// 技能扩展 支持淫灵气
    ///// </summary>
    //[HarmonyPatch(typeof(RoundManager), "RandomDrawCard")]
    //public class RoundManagerPatcher_RandomDrawCard
    //{
    //    public static bool Prefix(RoundManager __instance, KBEngine.Avatar avatar, int count = 1)
    //    {

    //        int yinCount = 0;
    //        for(int i=0;i<count;i++)
    //        {
    //            if(UnityEngine.Random.Range(0,1.0f) < 0.6f)
    //            {
    //                yinCount++;
    //            }
    //        }
    //        int leftCount = count - yinCount;
    //        int[] randomLingQiTypes = __instance.GetRandomLingQiTypes(avatar, leftCount);


    //        for (int i = 0; i < 6; i++)
    //        {
    //            if (randomLingQiTypes[i] > 0)
    //            {
    //                __instance.DrawCardCreatSpritAndAddCrystal(avatar, i, randomLingQiTypes[i]);
    //            }
    //        }

    //        __instance.DrawCardCreatSpritAndAddCrystal(avatar, Consts.YinLingQiTypeInt, yinCount);

    //        return false;
    //    }
    //}

    ///// <summary>
    ///// 技能扩展 结束阶段 暂时移除淫灵气 不参与消散计算
    ///// </summary>
    //[HarmonyPatch(typeof(RoundManager), "PlayerEndRound")]
    //public class RoundManagerPatcher_PlayerEndRound
    //{

    //    public static void Prefix(RoundManager __instance, bool canCancel, out List<card> __state)
    //    {
    //        // 进入逻辑前将淫灵气缓存并排除
    //        __state = new List<card>();
    //        KBEngine.Avatar player = PlayerEx.Player;
    //        var cards = player.cardMag._cardlist;
    //        for(int i= cards.Count-1;i>=0;i--)
    //        {
    //            if (cards[i].cardType == Consts.YinLingQiTypeInt)
    //            {
    //                __state.Add(cards[i]); ;
    //                cards.RemoveAt(i);
    //            }
    //        }

    //    }

    //    public static void Postfix(RoundManager __instance, bool canCancel, List<card> __state)
    //    {
    //        if(__state != null)
    //        {
    //            KBEngine.Avatar player = PlayerEx.Player;
    //            foreach (var card in __state)
    //            {
    //                player.cardMag._cardlist.Add(card);
    //            }
    //        }
    //    }
    //}


    ///// <summary>
    ///// 技能扩展 结束阶段 暂时移除淫灵气 不参与消散计算
    ///// </summary>
    //[HarmonyPatch(typeof(RoundManager), "SetChoiceSkill")]
    //public class RoundManagerPatcher_SetChoiceSkill
    //{
    //    public static bool Prefix(RoundManager __instance, ref GUIPackage.Skill skill)
    //    {
    //        KBEngine.Avatar avatar = (KBEngine.Avatar)KBEngineApp.app.player();
    //        CardMag cardMag = avatar.cardMag;
    //        if (skill.CanUse(avatar, KBEngineApp.app.entities[11]) != SkillCanUseType.可以使用)
    //        {
    //            __instance.ChoiceSkill = null;
    //            UIFightPanel.Inst.CacheLingQiController.MoveAllLingQiToPlayer();
    //            UIFightPanel.Inst.CacheLingQiController.ChangeCacheSlotNumber(0);
    //            UIFightPanel.Inst.CancelSkillHighlight();
    //            avatar.onCrystalChanged(cardMag);
    //            return false;
    //        }
    //        UIFightPanel.Inst.CacheLingQiController.MoveAllLingQiToPlayer();
    //        UIFightPanel.Inst.UIFightState = UIFightState.释放技能准备灵气阶段;
    //        UIFightPanel.Inst.FightCenterButtonController.ButtonType = UIFightCenterButtonType.OkCancel;
    //        _ = skill.skillSameCast;
    //        UIFightPanel.Inst.FightCenterButtonController.SetOkCancelEvent(delegate
    //        {
    //            if (__instance.UseSkill())
    //            {
    //                UIFightPanel.Inst.CacheLingQiController.ChangeCacheSlotNumber(0);
    //                UIFightPanel.Inst.CancelSkillHighlight();
    //            }
    //            else
    //            {
    //                UIFightPanel.Inst.UIFightState = UIFightState.释放技能准备灵气阶段;
    //            }
    //        }, delegate
    //        {
    //            __instance.ChoiceSkill = null;
    //            UIFightPanel.Inst.CacheLingQiController.MoveAllLingQiToPlayer();
    //            UIFightPanel.Inst.CacheLingQiController.ChangeCacheSlotNumber(0);
    //            UIFightPanel.Inst.CancelSkillHighlight();
    //            UIFightPanel.Inst.FightCenterButtonController.ButtonType = UIFightCenterButtonType.EndRound;
    //            UIFightPanel.Inst.UIFightState = UIFightState.自己回合普通状态;
    //        });
    //        bool flag = true;
    //        if (skill == __instance.ChoiceSkill)
    //        {
    //            flag = false;
    //        }
    //        else
    //        {
    //            __instance.CalcTongLingQiKeNeng(avatar, skill);
    //        }
    //        __instance.ChoiceSkill = skill;
    //        Dictionary<int, int> skillCast = skill.getSkillCast(avatar);
    //        UIFightPanel.Inst.CacheLingQiController.ChangeCacheSlotNumber(skillCast.Count + skill.skillSameCast.Count);
    //        UIFightPanel.Inst.CacheLingQiController.SetLingQiLimit(skillCast, skill.skillSameCast);
    //        bool flag2 = false;
    //        if (flag && avatar.FightCostRecord.HasField(skill.skill_ID.ToString()))
    //        {
    //            JSONObject jSONObject = avatar.FightCostRecord[skill.skill_ID.ToString()];
    //            Dictionary<int, int> dictionary = new Dictionary<int, int>();
    //            for (int i = 0; i < 6; i++)
    //            {
    //                if (jSONObject[i.ToString()].I > 0)
    //                {
    //                    dictionary.Add(i, jSONObject[i.ToString()].I);
    //                }
    //            }
    //            bool flag3 = true;
    //            foreach (KeyValuePair<int, int> item in dictionary)
    //            {
    //                if (skillCast.ContainsKey(item.Key))
    //                {
    //                    if (avatar.cardMag.HasNoEnoughNum(item.Key, item.Value + skillCast[item.Key]))
    //                    {
    //                        flag3 = false;
    //                        break;
    //                    }
    //                }
    //                else if (avatar.cardMag.HasNoEnoughNum(item.Key, item.Value))
    //                {
    //                    flag3 = false;
    //                    break;
    //                }
    //            }
    //            if (flag3)
    //            {
    //                flag2 = true;
    //                __instance.MoveLingQiToCacheFromPlayer(skillCast, LingQiCacheType.DontMove);
    //                __instance.MoveLingQiToCacheFromPlayer(dictionary, LingQiCacheType.None);
    //            }
    //        }

    //        var clickSkillChangeLingQiIndex = (int)typeof(RoundManager).GetField("clickSkillChangeLingQiIndex", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
    //        var lingQiKeNengXingPaiLie = typeof(RoundManager).GetField("lingQiKeNengXingPaiLie", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null) as Dictionary<int, List<int[]>>;
    //        var lingQiKeNengXingZuHe = typeof(RoundManager).GetField("lingQiKeNengXingZuHe", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null) as Dictionary<int, List<int[]>>;
    //        var choiceSkillCanUseLingQiIndexList = typeof(RoundManager).GetField("choiceSkillCanUseLingQiIndexList", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance) as List<int>;
    //        PluginMain.Main.LogError("RoundManagerPatcher_SetChoiceSkill before flag2");
            
    //        if (!flag2)
    //        {
    //            Dictionary<int, int> dictionary2 = new Dictionary<int, int>();
    //            for (int j = 0; j <= Consts.YinLingQiTypeInt; j++)
    //            {
    //                dictionary2.Add(j, avatar.cardMag[j]);
    //            }
    //            foreach (KeyValuePair<int, int> item2 in skillCast)
    //            {
    //                dictionary2[item2.Key] -= item2.Value;
    //            }
    //            __instance.MoveLingQiToCacheFromPlayer(skillCast, LingQiCacheType.DontMove);
    //            if (skill.skillSameCast.Count > 0)
    //            {
    //                bool flag4 = true;
    //                for (int k = 1; k < skill.skillSameCast.Count; k++)
    //                {
    //                    if (skill.skillSameCast[0] != skill.skillSameCast[k])
    //                    {
    //                        flag4 = false;
    //                        break;
    //                    }
    //                }
    //                Dictionary<int, int> dictionary3 = new Dictionary<int, int>();
    //                int num = 0;
    //                bool flag5;
    //                do
    //                {
    //                    int[] array = ((!flag4) ? lingQiKeNengXingPaiLie[skill.skillSameCast.Count][choiceSkillCanUseLingQiIndexList[clickSkillChangeLingQiIndex]] : lingQiKeNengXingZuHe[skill.skillSameCast.Count][choiceSkillCanUseLingQiIndexList[clickSkillChangeLingQiIndex]]);
    //                    clickSkillChangeLingQiIndex++;
    //                    if (clickSkillChangeLingQiIndex >= choiceSkillCanUseLingQiIndexList.Count)
    //                    {
    //                        clickSkillChangeLingQiIndex = 0;
    //                    }
    //                    dictionary3.Clear();
    //                    string text = "";
    //                    for (int l = 0; l < array.Length; l++)
    //                    {
    //                        dictionary3.Add(array[l], skill.skillSameCast[l]);
    //                        text += $"{array[l]}x{skill.skillSameCast[l]} ";
    //                    }
    //                    flag5 = true;
    //                    foreach (KeyValuePair<int, int> item3 in dictionary3)
    //                    {
    //                        if (UIFightPanel.Inst.PlayerLingQiController.SlotList[item3.Key].LingQiCount < item3.Value)
    //                        {
    //                            flag5 = false;
    //                        }
    //                    }
    //                    num++;
    //                }
    //                while (!flag5 && num < 100);
    //                if (num >= 100)
    //                {
    //                    Debug.LogError("在计算灵气可能性时出现异常，保底循环超过100次");
    //                }
    //                __instance.MoveLingQiToCacheFromPlayer(dictionary3, LingQiCacheType.None);
    //            }
    //        }
    //        avatar.onCrystalChanged(cardMag);

    //        return false;
    //    }

    //}

    ///// <summary>
    ///// 技能扩展 结束阶段 暂时移除淫灵气 不参与消散计算
    ///// </summary>
    //[HarmonyPatch(typeof(RoundManager), "UseSkill")]
    //public class RoundManagerPatcher_UseSkill
    //{
    //    public static bool Prefix(RoundManager __instance, string uuid, bool showTip, ref bool __result)
    //    {
    //        __result = false;

    //        Buff._NeiShangLoopCount = 0;
    //        UIFightPanel.Inst.UIFightState = UIFightState.自己回合普通状态;
    //        if (__instance.ChoiceSkill != null)
    //        {
    //            Dictionary<LingQiType, int> nowCacheLingQi = UIFightPanel.Inst.CacheLingQiController.GetNowCacheLingQi();
    //            Dictionary<int, int> skillCast = __instance.ChoiceSkill.getSkillCast(Tools.instance.getPlayer());
    //            int lingQiSum = __instance.GetLingQiSum(skillCast);
    //            int lingQiSum2 = __instance.GetLingQiSum(__instance.ChoiceSkill.skillSameCast);
    //            int lingQiSum3 = __instance.GetLingQiSum(nowCacheLingQi);
    //            if (lingQiSum + lingQiSum2 != lingQiSum3)
    //            {
    //                if (showTip)
    //                {
    //                    UIPopTip.Inst.Pop("选择的灵气与技能消耗不一致");
    //                    Debug.Log("选择的灵气与技能消耗不一致1");
    //                }
    //                return false;
    //            }
    //            foreach (KeyValuePair<int, int> item in __instance.ChoiceSkill.getSkillCast(Tools.instance.getPlayer()))
    //            {
    //                LingQiType key = (LingQiType)item.Key;
    //                if (nowCacheLingQi.ContainsKey(key))
    //                {
    //                    if (nowCacheLingQi[key] >= item.Value)
    //                    {
    //                        nowCacheLingQi[key] -= item.Value;
    //                        continue;
    //                    }
    //                    if (showTip)
    //                    {
    //                        UIPopTip.Inst.Pop("选择的灵气与技能消耗不一致");
    //                        Debug.Log("选择的灵气与技能消耗不一致2");
    //                    }
    //                    return false;
    //                }
    //                if (showTip)
    //                {
    //                    UIPopTip.Inst.Pop("选择的灵气与技能消耗不一致");
    //                    Debug.Log("选择的灵气与技能消耗不一致3");
    //                }
    //                return false;
    //            }
    //            Dictionary<LingQiType, int> dictionary = new Dictionary<LingQiType, int>();
    //            foreach (KeyValuePair<int, int> item2 in __instance.ChoiceSkill.skillSameCast)
    //            {
    //                bool flag = false;
    //                foreach (KeyValuePair<LingQiType, int> item3 in nowCacheLingQi)
    //                {
    //                    if (item3.Value == item2.Value && !dictionary.ContainsKey(item3.Key))
    //                    {
    //                        dictionary.Add(item3.Key, item2.Value);
    //                        flag = true;
    //                        nowCacheLingQi[item3.Key] -= item2.Value;
    //                        break;
    //                    }
    //                }
    //                if (!flag)
    //                {
    //                    if (showTip)
    //                    {
    //                        UIPopTip.Inst.Pop("选择的灵气与技能消耗不一致");
    //                        Debug.Log("选择的灵气与技能消耗不一致4");
    //                    }
    //                    return false;
    //                }
    //            }
    //            foreach (KeyValuePair<LingQiType, int> item4 in nowCacheLingQi)
    //            {
    //                if (item4.Value != 0)
    //                {
    //                    if (showTip)
    //                    {
    //                        UIPopTip.Inst.Pop("选择的灵气与技能消耗不一致");
    //                        Debug.Log("选择的灵气与技能消耗不一致5");
    //                    }
    //                    return false;
    //                }
    //            }
    //            KBEngine.Avatar avatar = (KBEngine.Avatar)KBEngineApp.app.player();
    //            if (avatar.isPlayer())
    //            {
    //                int[] array = new int[Consts.YinLingQiTypeInt + 1];
    //                foreach (KeyValuePair<LingQiType, int> item5 in UIFightPanel.Inst.CacheLingQiController.GetNowCacheLingQi())
    //                {
    //                    array[(int)item5.Key] = item5.Value;
    //                }
    //                foreach (KeyValuePair<int, int> item6 in skillCast)
    //                {
    //                    array[item6.Key] -= item6.Value;
    //                }
    //                JSONObject jSONObject = new JSONObject(JSONObject.Type.OBJECT);
    //                for (int i = 0; i < 6; i++)
    //                {
    //                    jSONObject.SetField(i.ToString(), array[i]);
    //                }
    //                avatar.FightCostRecord.SetField(__instance.ChoiceSkill.skill_ID.ToString(), jSONObject);
    //            }
    //            __instance.NowSkillUsedLingQiSum = lingQiSum3;
    //            avatar.spell.spellSkill(__instance.ChoiceSkill.skill_ID, uuid);
    //        }
    //        UIFightPanel.Inst.FightCenterButtonController.ButtonType = UIFightCenterButtonType.EndRound;
    //        __instance.setSkillChoicOk();

    //        __result = true;

    //        return false;
    //    }
    //}



    ///// <summary>
    ///// 技能扩展 支持淫灵气
    ///// </summary>
    //[HarmonyPatch(typeof(RoundManager), "gameStart")]
    //public class RoundManagerPatcher_gameStart
    //{
    //    public static void Postfix(RoundManager __instance)
    //    {
    //        PluginMain.Main.LogInfo("RoundManagerPatcher_gameStart Postfix ");

    //        if (UIFightPanel.Inst == null)
    //        {
    //            return;
    //        }
    //        // 屏蔽一些战斗类型
    //        switch(Tools.instance.monstarMag.FightType)
    //        {
    //            case Fungus.StartFight.FightEnumType.JieDan:
    //            case Fungus.StartFight.FightEnumType.JieYing:
    //            case Fungus.StartFight.FightEnumType.ZhuJi:
    //            case Fungus.StartFight.FightEnumType.HuaShen:
    //            case Fungus.StartFight.FightEnumType.FeiSheng:
    //            case Fungus.StartFight.FightEnumType.天劫秘术领悟:
    //            case Fungus.StartFight.FightEnumType.煅体:
    //                return;
    //        }

    //        var player = (KBEngine.Avatar)KBEngineApp.app.player();

    //        SecretsSystem.Instance.FightHInit(player);
    //    }
    //}

    ///// <summary>
    ///// 技能扩展 TODO
    ///// </summary>
    //[HarmonyPatch(typeof(RoundManager), "startRound")]
    //public class RoundManagerPatcher_startRound
    //{
    //    public static void Postfix(RoundManager __instance, Entity _avater)
    //    {
    //        PluginMain.Main.LogInfo("RoundManagerPatcher_startRound Postfix ");
    //    }
    //}

    ///// <summary>
    ///// 技能扩展 支持更多的seid 实现
    ///// </summary>
    //[HarmonyPatch(typeof(KBEngine.Buff), "loopRealizeSeid")]
    //public class BuffPatcher_loopRealizeSeid
    //{ 
    //    public static bool Prefix(KBEngine.Buff __instance, int seid, Entity _avatar, List<int> buffInfo, List<int> flag)
    //    {
    //        if (seid != Consts.BuffSeId_SwitchIntoHMode
    //            && seid != Consts.BuffSeId_SwitchOutHMode)
    //        {
    //            return true;
    //        }

    //        switch(seid)
    //        {
    //            case Consts.BuffSeId_SwitchIntoHMode:
    //                {
    //                    PluginMain.Main.LogInfo("Buff loopRealizeSeid Handle ListRealizeSeid_CheckIntoHMode ");
    //                    __instance.ListRealizeSeid_SwitchIntoHMode(seid, (Avatar)_avatar, buffInfo, flag);
    //                }
    //                break;
    //        }
            
    //        return false;
    //    }
    //}

    ///// <summary>
    ///// 技能扩展 支持更多的trigger判断条件
    ///// </summary>
    //[HarmonyPatch(typeof(KBEngine.Buff), "CanRealizeSeid")]
    //public class BuffPatcher_CanRealizeSeid
    //{
    //    public static bool Prefix(KBEngine.Buff __instance, ref bool __result, Avatar _avatar, List<int> flag, int nowSeid, BuffLoopData buffLoopData = null, List<int> buffInfo = null)
    //    {
    //        if (nowSeid != Consts.BuffSeId_CheckIntoHMode
    //            && nowSeid != Consts.BuffSeId_CheckOutHMode)
    //        {
    //            return true;
    //        }

    //        __result = false;

    //        switch (nowSeid)
    //        {
    //            case Consts.BuffSeId_CheckIntoHMode:
    //                {
    //                    __result = SecretsSystem.Instance.CheckFightEnterHMode(_avatar);
    //                }
    //                break;
    //        }

    //        return false;
    //    }
    //}

    ///// <summary>
    ///// 技能扩展 支持更多的技能特性支持
    ///// </summary>
    //[HarmonyPatch(typeof(GUIPackage.Skill), "realizeSeid")]
    //public class SkillPatcher_realizeSeid
    //{
    //    public static bool Prefix(GUIPackage.Skill __instance, int seid, List<int> damage, Entity _attaker, Entity _receiver, int type)
    //    {
    //        if (seid != Consts.SkillSeId_CheckOutHMode
    //            && seid != Consts.SkillSeId_SwitchOutHMode)
    //        {
    //            return true;
    //        }

    //        switch (seid)
    //        {
    //            case Consts.SkillSeId_SwitchOutHMode:
    //                {
    //                    PluginMain.Main.LogInfo("Skill realizeSeid Handle SkillSeId_SwitchOutHMode ");
    //                    __instance.realizeSeid_SwitchOutHMode(seid, damage, (KBEngine.Avatar)_attaker, (KBEngine.Avatar)_receiver, type);
    //                }
    //                break;
    //        }

    //        return false;
    //    }
    //}
    


    ///// <summary>
    ///// 技能扩展 支持淫灵气
    ///// </summary>
    //[HarmonyPatch(typeof(CardMag), "ToListInt32")]
    //public class CardMagPatcher_ToListInt32
    //{
    //    public static bool Prefix(CardMag __instance, ref List<int> __result)
    //    {
    //        List<int> crystal = new List<int>();
    //        for (int i = 0; i <= Consts.YinLingQiTypeInt; i++)
    //        {
    //            crystal.Add(0);
    //        }
    //        foreach(var card in __instance._cardlist)
    //        {
    //            crystal[card.cardType]++;
    //        }
            
    //        __result = crystal;

    //        return false;
    //    }
    //}

    ///// <summary>
    ///// 技能扩展 支持淫灵气
    ///// </summary>
    //[HarmonyPatch(typeof(CardMag), "getCardNum")]
    //public class CardMagPatcher_getCardNum
    //{
    //    public static bool Prefix(CardMag __instance, ref int __result)
    //    {
    //        int num = 0;
    //        foreach(var card in __instance._cardlist)
    //        {
    //            if(card.cardType < (int)LingQiType.Count)
    //            {
    //                num++;
    //            }
    //        }

    //        __result = num;

    //        return false;
    //    }
    //}


    ///// <summary>
    ///// 技能扩展 支持淫灵气
    ///// </summary>
    //[HarmonyPatch(typeof(Spell), "onBuffTickByType", new Type[] { typeof(int), typeof(List<int>)})]
    //public class SpellPatcher_onBuffTickByType
    //{
    //    public static bool Prefix(Spell __instance, int type, List<int> flag)
    //    {
    //        // 消散灵气时 不触发因零七
    //        if(type == 27)
    //        {
    //            if(flag.Count >= 2 && flag[1] == Consts.YinLingQiTypeInt)
    //            {
    //                return false;
    //            }
    //        }

    //        return true;
    //    }
    //}

    ///// <summary>
    ///// 技能扩展 支持淫灵气
    ///// </summary>
    //[HarmonyPatch(typeof(GUIPackage.Skill), "reduceBuff")]
    //public class SkillPatcher_reduceBuff
    //{
    //    public static void Postfix(GUIPackage.Skill __instance, KBEngine.Avatar Targ, int X, int Y, int __state)
    //    {
    //        //_BuffJsonData.DataDict[avatar.bufflist[buff][2]] //层数 持续 id
    //    }
    //}


    ///// <summary>
    ///// 技能详情 面板扩展 支持淫灵气
    ///// </summary>
    //[HarmonyPatch(typeof(ToolTipsMag), "Awake")]
    //public class ToolTipsMagPatcher_Awake
    //{
    //    public static void Postfix(ToolTipsMag __instance)
    //    {
    //        var _costIconDict = typeof(ToolTipsMag).GetField("_costIconDict", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(__instance) as Dictionary<string, Sprite>;
    //        _costIconDict.Add(Consts.YinLingQiTypeInt.ToString(), PluginMain.Main.LoadAsset<Sprite>("Icons/Cast/cast_type_9.png"));
    //    }
    //}

}
