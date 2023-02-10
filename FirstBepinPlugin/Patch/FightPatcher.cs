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
using System.Threading.Tasks;
using UnityEngine;
using YSGame.Fight;
using static ICSharpCode.SharpZipLib.Zip.ExtendedUnixData;

namespace FirstBepinPlugin.Patch
{
    /// <summary>
    /// 技能扩展 支持淫灵气
    /// </summary>
    [HarmonyPatch(typeof(UIPlayerLingQiController), "RefreshLingQiCount")]
    public class UIPlayerLingQiControllerPatcher_RefreshLingQiCount
    {
        public static void Postfix(UIPlayerLingQiController __instance, bool show)
        {
            PluginMain.Main.LogInfo("UIPlayerLingQiControllerPatcher_RefreshLingQiCount");

            if (show)
            {
                var slot = __instance.SlotList[Consts.YinLingQiTypeInt];
                slot.LingQiCountText.text = slot.LingQiCount.ToString();
                slot.SetLingQiCountShow(show: true);
            }
            else
            {
                var slot = __instance.SlotList[Consts.YinLingQiTypeInt];
                slot.SetLingQiCountShow(show: false);
            }
            Dictionary<LingQiType, int> nowCacheLingQi = UIFightPanel.Inst.CacheLingQiController.GetNowCacheLingQi();

            {
                var slot = __instance.SlotList[Consts.YinLingQiTypeInt];

                if (UIFightPanel.Inst.UIFightState == UIFightState.释放技能准备灵气阶段)
                {
                    if (nowCacheLingQi.ContainsKey(slot.LingQiType))
                    {
                        slot.HighlightObj.SetActive(value: true);
                    }
                    else
                    {
                        slot.HighlightObj.SetActive(value: false);
                    }
                }
                else
                {
                    slot.HighlightObj.SetActive(value: false);
                }
            }
        }
    }

    /// <summary>
    /// 技能扩展 支持淫灵气
    /// </summary>
    [HarmonyPatch(typeof(UIFightPanel), "Awake")]
    public class UIFightPanelPatcher_Awake
    {
        public static void Postfix(UIFightPanel __instance)
        {
            var playerLingQiController = __instance.PlayerLingQiController;

            var lastData = __instance.LingQiImageDatas[__instance.LingQiImageDatas.Count - 1];
            while(__instance.LingQiImageDatas.Count <= Consts.YinLingQiTypeInt)
            {
                __instance.LingQiImageDatas.Add(null);
            }
            var newImageData = new UILingQiImageData();

            newImageData.Normal = PluginMain.Main.LoadAsset<Sprite>("Icons/Lingqi_yin/MCS_ZD_linqi_yin.png");
            newImageData.Lock = PluginMain.Main.LoadAsset<Sprite>("Icons/Lingqi_yin/MCS_ZD_linqi_yin_an.png");
            newImageData.Press = PluginMain.Main.LoadAsset<Sprite>("Icons/Lingqi_yin/MCS_ZD_linqi_yin_bk.png");
            newImageData.Highlight = PluginMain.Main.LoadAsset<Sprite>("Icons/Lingqi_yin/MCS_ZD_linqi_yin_hlg.png");

            newImageData.Normal2 = lastData.Normal2;
            newImageData.Lock2 = lastData.Lock2;
            newImageData.Press2 = lastData.Press2;
            newImageData.Highlight2 = lastData.Highlight2;

            newImageData.Normal3 = lastData.Normal3;
            newImageData.Lock3 = lastData.Lock3;
            newImageData.Press3 = lastData.Press3;
            newImageData.Highlight3 = lastData.Highlight3;


            __instance.LingQiImageDatas[Consts.YinLingQiTypeInt] = lastData;

            var moSlot = playerLingQiController.SlotList[5];

            while (playerLingQiController.SlotList.Count <= Consts.YinLingQiTypeInt)
            {
                playerLingQiController.SlotList.Add(null);
            }

            var newSlotGo = UnityEngine.GameObject.Instantiate(moSlot.gameObject, moSlot.transform.parent);
            newSlotGo.transform.localPosition += Vector3.right * 100;
            var newSlot = newSlotGo.GetComponent<UIFightLingQiPlayerSlot>();

            playerLingQiController.SlotList[Consts.YinLingQiTypeInt] = newSlot;

            newSlot.LingQiType = Consts.YinLingQiType;

            PluginMain.Main.LogInfo("UIFightPanelPatcher_Awake end ");
        }
    }
    

    /// <summary>
    /// 技能扩展 支持淫灵气
    /// </summary>
    [HarmonyPatch(typeof(RoundManager), "RandomDrawCard")]
    public class RoundManagerPatcher_RandomDrawCard
    {
        public static bool Prefix(RoundManager __instance, KBEngine.Avatar avatar, int count = 1)
        {
            PluginMain.Main.LogInfo("RoundManagerPatcher_RandomDrawCard start ");

            int yinCount = 0;
            for(int i=0;i<count;i++)
            {
                if(UnityEngine.Random.Range(0,1.0f) < 0.6f)
                {
                    yinCount++;
                }
            }
            int leftCount = count - yinCount;
            int[] randomLingQiTypes = __instance.GetRandomLingQiTypes(avatar, leftCount);


            for (int i = 0; i < 6; i++)
            {
                if (randomLingQiTypes[i] > 0)
                {
                    __instance.DrawCardCreatSpritAndAddCrystal(avatar, i, randomLingQiTypes[i]);
                }
            }

            __instance.DrawCardCreatSpritAndAddCrystal(avatar, Consts.YinLingQiTypeInt, yinCount);

            PluginMain.Main.LogInfo("RoundManagerPatcher_RandomDrawCard end ");

            return false;
        }
    }

    /// <summary>
    /// 技能扩展 支持淫灵气
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
            switch(Tools.instance.monstarMag.FightType)
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

            var player = (KBEngine.Avatar)KBEngineApp.app.player();

            SecretsSystem.Instance.FightHInit(player);
        }
    }

    /// <summary>
    /// 技能扩展 TODO
    /// </summary>
    [HarmonyPatch(typeof(RoundManager), "startRound")]
    public class RoundManagerPatcher_startRound
    {
        public static void Postfix(RoundManager __instance, Entity _avater)
        {
            PluginMain.Main.LogInfo("RoundManagerPatcher_startRound Postfix ");
        }
    }

    /// <summary>
    /// 技能扩展 支持更多的seid 实现
    /// </summary>
    [HarmonyPatch(typeof(KBEngine.Buff), "loopRealizeSeid")]
    public class BuffPatcher_loopRealizeSeid
    { 
        public static bool Prefix(KBEngine.Buff __instance, int seid, Entity _avatar, List<int> buffInfo, List<int> flag)
        {
            if (seid != Consts.BuffSeId_SwitchIntoHMode
                && seid != Consts.BuffSeId_SwitchOutHMode)
            {
                return true;
            }

            switch(seid)
            {
                case Consts.BuffSeId_SwitchIntoHMode:
                    {
                        PluginMain.Main.LogInfo("Buff loopRealizeSeid Handle ListRealizeSeid_CheckIntoHMode ");
                        __instance.ListRealizeSeid_SwitchIntoHMode(seid, (Avatar)_avatar, buffInfo, flag);
                    }
                    break;
            }
            
            return false;
        }
    }

    /// <summary>
    /// 技能扩展 支持更多的trigger判断条件
    /// </summary>
    [HarmonyPatch(typeof(KBEngine.Buff), "CanRealizeSeid")]
    public class BuffPatcher_CanRealizeSeid
    {
        public static bool Prefix(KBEngine.Buff __instance, ref bool __result, Avatar _avatar, List<int> flag, int nowSeid, BuffLoopData buffLoopData = null, List<int> buffInfo = null)
        {
            if (nowSeid != Consts.BuffSeId_CheckIntoHMode
                && nowSeid != Consts.BuffSeId_CheckOutHMode)
            {
                return true;
            }

            __result = false;

            switch (nowSeid)
            {
                case Consts.BuffSeId_CheckIntoHMode:
                    {
                        __result = SecretsSystem.Instance.CheckFightEnterHMode(_avatar);
                    }
                    break;
            }

            return false;
        }
    }

    /// <summary>
    /// 技能扩展 支持更多的技能特性支持
    /// </summary>
    [HarmonyPatch(typeof(GUIPackage.Skill), "realizeSeid")]
    public class SkillPatcher_realizeSeid
    {
        public static bool Prefix(GUIPackage.Skill __instance, int seid, List<int> damage, Entity _attaker, Entity _receiver, int type)
        {
            if (seid != Consts.SkillSeId_CheckOutHMode
                && seid != Consts.SkillSeId_SwitchOutHMode)
            {
                return true;
            }

            switch (seid)
            {
                case Consts.SkillSeId_SwitchOutHMode:
                    {
                        PluginMain.Main.LogInfo("Skill realizeSeid Handle SkillSeId_SwitchOutHMode ");
                        __instance.realizeSeid_SwitchOutHMode(seid, damage, (KBEngine.Avatar)_attaker, (KBEngine.Avatar)_receiver, type);
                    }
                    break;
            }

            return false;
        }
    }
    


    /// <summary>
    /// 技能扩展 支持淫灵气
    /// </summary>
    [HarmonyPatch(typeof(CardMag), "ToListInt32")]
    public class CardMagPatcher_ToListInt32
    {
        public static bool Prefix(CardMag __instance, ref List<int> __result)
        {
            List<int> crystal = new List<int>();
            for (int i = 0; i < 6; i++)
            {
                crystal.Add(0);
            }
            foreach(var card in __instance._cardlist)
            {
                if (card.cardType < 6)
                {
                    crystal[card.cardType]++;
                }
            }
            
            __result = crystal;

            return false;
        }
    }

    /// <summary>
    /// 技能扩展 支持淫灵气
    /// </summary>
    [HarmonyPatch(typeof(CardMag), "getCardNum")]
    public class CardMagPatcher_getCardNum
    {
        public static bool Prefix(CardMag __instance, ref int __result)
        {
            int num = 0;
            foreach(var card in __instance._cardlist)
            {
                if(card.cardType < (int)LingQiType.Count)
                {
                    num++;
                }
            }

            __result = num;

            return false;
        }
    }


    /// <summary>
    /// 技能扩展 支持淫灵气
    /// </summary>
    [HarmonyPatch(typeof(Spell), "onBuffTickByType", new Type[] { typeof(int), typeof(List<int>)})]
    public class SpellPatcher_onBuffTickByType
    {
        public static bool Prefix(Spell __instance, int type, List<int> flag)
        {
            // 消散灵气时 不触发因零七
            if(type == 27)
            {
                if(flag.Count >= 2 && flag[1] == Consts.YinLingQiTypeInt)
                {
                    return false;
                }
            }

            return true;
        }
    }

    /// <summary>
    /// 技能扩展 支持淫灵气
    /// </summary>
    [HarmonyPatch(typeof(GUIPackage.Skill), "reduceBuff")]
    public class SkillPatcher_reduceBuff
    {
        public static void Postfix(GUIPackage.Skill __instance, KBEngine.Avatar Targ, int X, int Y, int __state)
        {
            //_BuffJsonData.DataDict[avatar.bufflist[buff][2]] //层数 持续 id
        }
    }
    



}
