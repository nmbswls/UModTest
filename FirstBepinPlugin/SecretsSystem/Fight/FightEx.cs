using FirstBepinPlugin.Config;
using JSONClass;
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
            SecretsSystem.FightManager.Ctx.ModYuWang(avatar, num);
        }

        public static void ListRealizeSeid_ModYiZhuang(this KBEngine.Buff buff, int seid, KBEngine.Avatar avatar, List<int> buffInfo, List<int> flag)
        {
            float num = buff.getSeidJson(seid)["value1"].f * buffInfo[1];
            SecretsSystem.FightManager.Ctx.ModYiZhuang(num);
        }
        public static void ListRealizeSeid_ModXingFen(this KBEngine.Buff buff, int seid, KBEngine.Avatar avatar, List<int> buffInfo, List<int> flag)
        {
            var config = buff.getSeidJson(seid);
            int targetPart = config["part"].I;
            float val = 0;
            if (config.HasField("value1"))
            {
                val = config["value1"].f * buffInfo[1];
            }
            if(config.HasField("value2"))
            {
                val = config["value2"].f ;
            }
            SecretsSystem.FightManager.Ctx.ModXingFen(targetPart, val);
        }

        public static void ListRealizeSeid_ModTiLi(this KBEngine.Buff buff, int seid, KBEngine.Avatar avatar, List<int> buffInfo, List<int> flag)
        {
            var config = buff.getSeidJson(seid);
            float val = 0;
            if (config.HasField("value1"))
            {
                val = config["value1"].f * buffInfo[1];
            }
            if (config.HasField("value2"))
            {
                val = config["value2"].f;
            }
            SecretsSystem.FightManager.Ctx.ModTiLi(val);
        }

        public static void ListRealizeSeid_ModKuaiGan(this KBEngine.Buff buff, int seid, KBEngine.Avatar avatar, List<int> buffInfo, List<int> flag)
        {
            var config = buff.getSeidJson(seid);
            float val = 0;
            if (config.HasField("value1"))
            {
                val = config["value1"].f * buffInfo[1];
            }
            if (config.HasField("value2"))
            {
                val = config["value2"].f;
            }
            SecretsSystem.FightManager.Ctx.ModKuaiGan(avatar, val);
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
            SecretsSystem.FightManager.Ctx.ModYiZhuang(modVal);
        }
        public static void realizeSeid_ModYuWang(this GUIPackage.Skill skill, int seid, List<int> damage, KBEngine.Avatar attaker, KBEngine.Avatar receiver, int type)
        {
            int target = skill.getSeidJson(seid)["target"].I;
            float modVal = skill.getSeidJson(seid)["value1"].f;
            var targetAvatar = skill.getTargetAvatar(seid, attaker);
            if (target == 1)
            {
                SecretsSystem.FightManager.Ctx.ModYuWang(targetAvatar, modVal);
            }
            else
            {
                SecretsSystem.FightManager.Ctx.ModYuWang(targetAvatar, modVal);
            }
            
        }

        public static void realizeSeid_ModXingFen(this GUIPackage.Skill skill, int seid, List<int> damage, KBEngine.Avatar attaker, KBEngine.Avatar receiver, int type)
        {
            int targetPart = skill.getSeidJson(seid)["value1"].I;
            float modVal = skill.getSeidJson(seid)["value2"].f;
            SecretsSystem.FightManager.Ctx.ModXingFen(targetPart, modVal);
        }

        public static void realizeSeid_SwitchTiWei(this GUIPackage.Skill skill, int seid, List<int> damage, KBEngine.Avatar attaker, KBEngine.Avatar receiver, int type)
        {
            int tiweiId = skill.getSeidJson(seid)["value1"].I;
            SecretsSystem.FightManager.Ctx.SwitchTiWei(tiweiId);
        }

        public static void realizeSeid_ModKuaiGan(this GUIPackage.Skill skill, int seid, List<int> damage, KBEngine.Avatar attaker, KBEngine.Avatar receiver, int type)
        {
            float modVal = skill.getSeidJson(seid)["value1"].f;
            var targetAvatar = skill.getTargetAvatar(seid, attaker);

            SecretsSystem.FightManager.Ctx.ModKuaiGan(targetAvatar, modVal);
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
        
        public static void realizeSeid_ApplyHAttack(this GUIPackage.Skill skill, int seid, List<int> damage, KBEngine.Avatar attaker, KBEngine.Avatar receiver, int type)
        {
            SecretsSystem.FightManager.ApplyHAttack(skill.skill_ID);
        }

        public static void realizeSeid_CheckFirstUse(this GUIPackage.Skill skill, int seid, List<int> damage, KBEngine.Avatar attaker, KBEngine.Avatar receiver, int type)
        {
            // 使用过就截断
            if(attaker.UsedSkills.Contains(skill.skill_ID))
            {
                damage[2] = 1;
            }
        }
        public static void realizeSeid_CheckTargetNotFaQing(this GUIPackage.Skill skill, int seid, List<int> damage, KBEngine.Avatar attaker, KBEngine.Avatar receiver, int type)
        {
            // 发情时截断
            if(SecretsSystem.FightManager.Ctx.IsTargetFaQing(receiver))
            {
                damage[2] = 1;
            }
        }

        public static void realizeSeid_CheckTargetFaQing(this GUIPackage.Skill skill, int seid, List<int> damage, KBEngine.Avatar attaker, KBEngine.Avatar receiver, int type)
        {
            // 未发情时截断
            if (!SecretsSystem.FightManager.Ctx.IsTargetFaQing(receiver))
            {
                damage[2] = 1;
            }
        }
    }

    public static class AvatarEx
    {
        /// <summary>
        /// 指定avatar 设置buff层数 仅支持叠加的
        /// </summary>
        /// <param name="target"></param>
        /// <param name="buffId"></param>
        /// <param name="newLayer"></param>
        public static void SetBuffLayer(this KBEngine.Avatar target, int buffId, int newLayer)
        {
            var buffInfo = _BuffJsonData.DataDict[buffId];
            if (buffInfo == null)
            {
                return;
            }
            if (buffInfo.BuffType != 0)
            {
                PluginMain.Main.LogError($"Only Stackable Buff Can Use SetBuffLayer. {buffId}");
                return;
            }

            var buffByID = target.buffmag.GetBuffById(buffId);
            int oldLayer = 0;
            if (buffByID != null)
            {
                oldLayer = buffByID[1];
                buffByID[1] = newLayer;
            }
            else
            {
                target.spell.addDBuff(buffId, newLayer);
            }

            // 特殊处理
            if (buffInfo.seid.Contains(64))
            {
                var seidJson = Buff.getSeidJson(64, buffId);
                var v1 = seidJson["value1"].I;
                var v2 = seidJson["value2"].I;
                int oldVal = 0;
                if (!target.SkillSeidFlag.ContainsKey(13))
                {
                    target.SkillSeidFlag[13] = new Dictionary<int, int>();
                }

                PluginMain.Main.LogError($"seid64 check type {v1} weight {v2} newLayer {newLayer} oldLayer {oldLayer}");

                if (target.SkillSeidFlag[13].ContainsKey(v1))
                {
                    oldVal = target.SkillSeidFlag[13][v1];
                }
                int changedVal = (newLayer - oldLayer) * v2;
                target.SkillSeidFlag[13][v1] = oldVal + changedVal;

                // 打印额外灵根情况
                foreach (var kv in target.SkillSeidFlag[13])
                {
                    PluginMain.Main.LogError($"灵根情况 {kv.Key} {kv.Value}");
                }
            }
        }

        /// <summary>
        /// 使对象拥有指定buff
        /// </summary>
        /// <param name="target"></param>
        /// <param name="buffId"></param>
        public static void SetHasBuff(this KBEngine.Avatar target, int buffId)
        {
            if (!_BuffJsonData.DataDict.TryGetValue(buffId, out var buffInfo))
            {
                PluginMain.Main.LogError($"Buff Not Found {buffId}");
                return;
            }
            if (buffInfo.BuffType != 1)
            {
                PluginMain.Main.LogError($"Only Override Buff Can Use SetHasBuff. {buffId}");
                return;
            }
            List<List<int>> buffByID = target.buffmag.getBuffByID(buffId);
            if (buffByID.Count > 0)
            {
                return;
            }
            target.spell.addDBuff(buffId, 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="avatar"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static long GetAttributeBuffBonus(this KBEngine.Avatar avatar, HModeAttributeType type)
        {
            long addVal = 0;
            int modSeid = HFightUtils.ModSeidGetByAttribute(type);

            if (modSeid != 0)
            {
                // sum mode
                foreach (var buffInfo in avatar.buffmag.getBuffBySeid(modSeid))
                {
                    var buffId = buffInfo[2];
                    int buffLayer = buffInfo[1];

                    JSONObject jSONObject = jsonData.instance.BuffSeidJsonData[modSeid];

                    if (!jSONObject.HasField(buffId.ToString()))
                    {
                        continue;
                    }

                    var configInfo = jSONObject[buffId.ToString()];
                    // 与层数相关的加成
                    if(configInfo.HasField("value1"))
                    {
                        addVal += (long)configInfo["value1"].f * Consts.Float2Int100 * buffLayer;
                    }
                    // 层数代表倒计时的buff 使用value2 固定加成
                    if (configInfo.HasField("value2"))
                    {
                        addVal += (long)configInfo["value2"].f * Consts.Float2Int100;
                    }
                }
            }
            return addVal;
        }

        /// <summary>
        /// 使对象拥有指定buff
        /// </summary>
        /// <param name="target"></param>
        /// <param name="buffId"></param>
        public static void ClearWantBuffs(this KBEngine.Avatar target)
        {
            target.buffmag.RemoveBuff(Consts.BuffId_WantShou);
            target.buffmag.RemoveBuff(Consts.BuffId_WantKou);
            target.buffmag.RemoveBuff(Consts.BuffId_WantRu);
            target.buffmag.RemoveBuff(Consts.BuffId_WantXue);
            target.buffmag.RemoveBuff(Consts.BuffId_WantGang);
        }

        /// <summary>
        /// 设置衣装
        /// </summary>
        /// <param name="target"></param>
        /// <param name="buffId"></param>
        public static void ClearAndSetYiZhuangBuff(this KBEngine.Avatar target, int yizhuangLevel)
        {
            target.buffmag.RemoveBuff(Consts.BuffId_FlagYiZhuang0);
            target.buffmag.RemoveBuff(Consts.BuffId_FlagYiZhuang1);
            target.buffmag.RemoveBuff(Consts.BuffId_FlagYiZhuang2);
            target.buffmag.RemoveBuff(Consts.BuffId_FlagYiZhuang3);

            if(yizhuangLevel == 0)
            {
                target.SetHasBuff(Consts.BuffId_FlagYiZhuang0);
            }
            else if(yizhuangLevel == 1)
            {
                target.SetHasBuff(Consts.BuffId_FlagYiZhuang1);
            }
            else if (yizhuangLevel == 2)
            {
                target.SetHasBuff(Consts.BuffId_FlagYiZhuang2);
            }
            else if (yizhuangLevel == 3)
            {
                target.SetHasBuff(Consts.BuffId_FlagYiZhuang3);
            }
        }

        /// <summary>
        /// 设置衣装
        /// </summary>
        /// <param name="target"></param>
        /// <param name="buffId"></param>
        public static void ClearAndSetMeiliBuff(this KBEngine.Avatar target, int meiliLevel)
        {
            target.buffmag.RemoveBuff(Consts.BuffId_FlagMeiLi0);
            target.buffmag.RemoveBuff(Consts.BuffId_FlagMeiLi1);
            target.buffmag.RemoveBuff(Consts.BuffId_FlagMeiLi2);
            target.buffmag.RemoveBuff(Consts.BuffId_FlagMeiLi3);
            target.buffmag.RemoveBuff(Consts.BuffId_FlagMeiLi4);
            
            if (meiliLevel == 0)
            {
                target.SetHasBuff(Consts.BuffId_FlagMeiLi0);
            }
            else if (meiliLevel == 1)
            {
                target.SetHasBuff(Consts.BuffId_FlagMeiLi1);
            }
            else if (meiliLevel == 2)
            {
                target.SetHasBuff(Consts.BuffId_FlagMeiLi2);
            }
            else if (meiliLevel == 3)
            {
                target.SetHasBuff(Consts.BuffId_FlagMeiLi3);
            }
            else if (meiliLevel == 4)
            {
                target.SetHasBuff(Consts.BuffId_FlagMeiLi4);
            }
        }

        /// <summary>
        /// 使对象拥有指定buff
        /// </summary>
        /// <param name="target"></param>
        /// <param name="buffId"></param>
        public static void ClearTiWeiBuffs(this KBEngine.Avatar target)
        {
            target.buffmag.RemoveBuff(Consts.BuffId_FlagTiWeiShou);
            target.buffmag.RemoveBuff(Consts.BuffId_FlagTiWeiKou);
            target.buffmag.RemoveBuff(Consts.BuffId_FlagTiWeiRu);
            target.buffmag.RemoveBuff(Consts.BuffId_FlagTiWeiXue);
            target.buffmag.RemoveBuff(Consts.BuffId_FlagTiWeiGang);
        }

        /// <summary>
        /// 使对象拥有指定buff
        /// </summary>
        /// <param name="target"></param>
        /// <param name="buffId"></param>
        public static HModeTiWei GetCurrWantingTiwei(this KBEngine.Avatar target)
        {
            // 如果身上带着buff 移除并进入
            if (!target.buffmag.HasBuff(Consts.BuffId_WantShou)
                && !target.buffmag.HasBuff(Consts.BuffId_WantKou)
                && !target.buffmag.HasBuff(Consts.BuffId_WantRu)
                && !target.buffmag.HasBuff(Consts.BuffId_WantXue)
                && !target.buffmag.HasBuff(Consts.BuffId_WantGang))
            {
                return HModeTiWei.None;
            }

            if (target.buffmag.HasBuff(Consts.BuffId_WantShou))
            {
                return HModeTiWei.Shou;
            }
            else if (target.buffmag.HasBuff(Consts.BuffId_WantKou))
            {
                return HModeTiWei.Kou;
            }
            else if (target.buffmag.HasBuff(Consts.BuffId_WantRu))
            {
                return HModeTiWei.Ru;
            }
            else if (target.buffmag.HasBuff(Consts.BuffId_WantXue))
            {
                return HModeTiWei.Xue;
            }
            else if (target.buffmag.HasBuff(Consts.BuffId_WantGang))
            {
                return HModeTiWei.Gang;
            }
            return HModeTiWei.None;
        }

        public static int AddTiWeiGuideBuff(this KBEngine.Avatar target, HModeTiWei tiwei)
        {
            switch (tiwei)
            {
                case HModeTiWei.Shou:
                {
                    target.spell.addBuff(Consts.BuffId_GuideShou, 1);
                    break;
                }
                case HModeTiWei.Kou:
                {
                    target.spell.addBuff(Consts.BuffId_GuideKou, 1);
                    break;
                }
                case HModeTiWei.Ru:
                {
                    target.spell.addBuff(Consts.BuffId_GuideRu, 1);
                    break;
                }
                case HModeTiWei.Xue:
                {
                    target.spell.addBuff(Consts.BuffId_GuideXue, 1);
                    break;
                }
                case HModeTiWei.Gang:
                {
                    target.spell.addBuff(Consts.BuffId_GuideGang, 1);
                    break;
                }
            }
            return 0;
        }
    }
}
