using FirstBepinPlugin.Config;
using KBEngine;
using SkySwordKill.Next.DialogSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WXB;
using YSGame;
using YSGame.Fight;
using static FirstBepinPlugin.HModeFightManager;
using static KBEngine.Buff;
using static SeaAvatarObjBase;

namespace FirstBepinPlugin
{

    public class HModeEnemyInfo
    {
        public int Id;
        public float YuWang;
        public float KuaiGan;
        public float MaxKuaiGan;

        public int Xingge;
        public int SexType;
        public int JingJie;

        public bool IsFaQing;
        public int TiWeiAccRate;
    }


    public class HModeFightManager
    {
        public enum HSkillGroupType
        {
            Invalid,
            Common,
            Wuli,
            JueDing,
            TiWeiNone,
            TiWeiShou,
            TiWeiKou,
            TiWeiRu,
            TiWeiXue,
            TiWeiGang,
            Function,
        }

        public KBEngine.Avatar Player;
        public HFightContext Ctx = new HFightContext();
        public bool IsInBattle;

        public int TotalJingAmount = 0;
        /// <summary>
        /// 是否正在等待结算h攻击
        /// </summary>
        public bool IsWaitHAttack;

        #region 生命周期

        /// <summary>
        /// 初始化
        /// </summary>
        public void FightHInit()
        {
            Ctx.Owner = this;
            Ctx.InitContenxt();

            IsInBattle = true;
            Player = Tools.instance.getPlayer();

            Ctx.YiZhuang = 100.0f;

            Ctx.Enemy.MaxKuaiGan = 100;

            JSONObject jSONObject = jsonData.instance.AvatarJsonData[string.Concat(Tools.instance.MonstarID)];
            if (jSONObject.HasField("XingGe"))
            {
                Ctx.Enemy.Xingge = jSONObject.GetField("XingGe").I;
            }
            if (jSONObject.HasField("SexType"))
            {
                Ctx.Enemy.SexType = jSONObject.GetField("SexType").I;
            }
            if (jSONObject.HasField("Level"))
            {
                Ctx.Enemy.JingJie = jSONObject.GetField("Level").I / 3;
            }

            // 玩家每回合增加欲望
            Player.SetBuffLayer(Consts.BuffId_TurnModYuWang, 10);
            // 玩家使用技能时损耗衣装
            Player.SetHasBuff(Consts.BuffId_SelfCostYiZhuang);
            // 玩家弃置灵气时 恢复衣装
            Player.SetHasBuff(Consts.BuffId_SelfRecoverYiZhuang);
            // 玩家衣装较高时 降低欲望
            Player.SetHasBuff(Consts.BuffId_SelfCalmDown);
            // 玩家战败保护
            Player.SetHasBuff(Consts.BuffId_HDefeatProtect);
            

            m_skillIdCache.Clear();
            // 缓存初始技能
            foreach (var originSkill in Player.skill)
            {
                m_skillIdCache.Add(originSkill.skill_ID);
            }

            // 初始化ui
            //更新 魅力buff
            int meiLiLevel = StaticConfigContainer.GetMeiLiLevel(Ctx.GetAttributeValue(HModeAttributeType.HMeiLi) * 1.0f / Consts.Float2Int100);
            Player.ClearAndSetMeiliBuff(meiLiLevel);

            // 移除双方魔气debuff
            Player.buffmag.RemoveBuff(10000);
            Player.OtherAvatar.buffmag.RemoveBuff(10000);

            InitHModeUI();
        }

        /// <summary>
        /// tick
        /// </summary>
        public void FightTick(float dt)
        {
            // 一帧一个
            if (m_runningProcessList.Count != 0)
            {
                var firstProcess = m_runningProcessList.Peek();
                if (!firstProcess.m_isStart)
                {
                    firstProcess.OnStart();
                    firstProcess.m_isStart = true;
                }
                // tick firstProcess
                firstProcess.Tick(dt);
                // 持续时间非0 表示通过倒计时结束
                if (firstProcess.m_isEnd)
                {
                    firstProcess.OnEnd();
                    m_runningProcessList.Dequeue();
                }
            }
        }

        public void FightOnRoundStartPre(KBEngine.Avatar avatar)
        {
            if (avatar == Player)
            {
                CheckDefeated();

                // 计算兴奋度
                ApplyTurnXingFen();

                if(Ctx.m_hTiWei != HModeTiWei.None)
                {
                    Ctx.ModTiLi(5);
                }
                // check 
                if (Ctx.YuWang > Player.shengShi)
                {
                    int yuheLevel = Mathf.FloorToInt((Ctx.YuWang - Player.shengShi) / Consts.YuWang2ExtraYinLingqi);
                    //增加淫灵气
                    Player.spell.addBuff(Consts.BuffId_AddYinLingQi, yuheLevel);
                }
            }
            else
            {
                // check  
                if (Ctx.Enemy.YuWang > avatar.shengShi)
                {
                    int yuheLevel = Mathf.FloorToInt((Ctx.Enemy.YuWang - avatar.shengShi) / Consts.YuWang2ExtraYinLingqi);
                    //增加淫灵气
                    avatar.spell.addBuff(Consts.BuffId_AddYinLingQi, yuheLevel);
                }
            }
        }

        public void FightOnRoundStartPost(KBEngine.Avatar avatar)
        {
            // 玩家回合开始
            if (avatar == Player)
            {
                TryFinishJueDing();
                CheckKuaiGanReachMax(1);
            }
            else
            {
                CheckKuaiGanReachMax(2);
            }

            CheckFaQing(avatar);
        }

        public void FightOnRoundEndPre(KBEngine.Avatar avatar)
        {

        }

        public void FightOnRoundEndPost(KBEngine.Avatar avatar)
        {
            m_fightHud?.m_recordController?.AddRecord("回合结束了.");
        }

        #endregion


        /// <summary>
        /// 进入H形态
        /// </summary>
        public void FightEnterHMode()
        {
            PluginMain.Main.LogError("Error FightEnterHMode Obsolete");
        }

        /// <summary>
        /// 检查更新动态buff
        /// </summary>
        protected void ApplyTurnXingFen()
        {
            int turnXingFen = StaticConfigContainer.GetTurnXingFenByYuWang((int)Ctx.YuWang / 100);
            Ctx.ModXingFen(0, turnXingFen);
        }

        /// <summary>
        /// 决定状态平复
        /// </summary>
        /// <param name="target"></param>
        protected void TryFinishJueDing()
        {
            if (!Ctx.m_isJueDing)
                return;

            // 基础平复效率
            Ctx.ModKuaiGan(Player, -40);
        }

        /// <summary>
        /// 检查战败
        /// </summary>
        protected void CheckDefeated()
        {
            if(!Player.buffmag.HasBuff(Consts.BuffId_FlagDefeated))
            {
                return;
            }

            var dialogProcess = new FightProcessWaitDialog(this, Consts.HDialogId_Defeated);
            dialogProcess.SetArg("branch", 1);
            dialogProcess.EventOnEnd += delegate ()
            {
                // 结束战败后 直接把对面扬了
                RoundManager.instance.NpcTempHp = 0;
                Player.OtherAvatar.setHP(-1);
            };

            m_runningProcessList.Enqueue(dialogProcess);
        }

        /// <summary>
        /// 检查快感满
        /// </summary>
        /// <param name="target"></param>
        protected void CheckKuaiGanReachMax(int targetId)
        {
            float kuaiGan;
            float maxKuaiGan;
            if (targetId == 1)
            {
                kuaiGan = Ctx.KuaiGan;
                maxKuaiGan = Ctx.GetAttributeValue(HModeAttributeType.HMaxKuaiGan);
            }
            else
            {
                kuaiGan = Ctx.Enemy.KuaiGan;
                maxKuaiGan = Ctx.Enemy.MaxKuaiGan;
            }

            // 没达到满 无事发生
            if (kuaiGan < maxKuaiGan)
            {
                return;
            }

            if (targetId == 1)
            {
                OnPlayerJueding();
            }
            else
            {
                OnEnemyJueding(Player.OtherAvatar);
            }
        }

        /// <summary>
        /// 流程向逻辑 检验是否faqing
        /// </summary>
        /// <param name="avatar"></param>
        public void CheckFaQing(KBEngine.Avatar avatar)
        {
            // 已经发情 不检查
            if (avatar.buffmag.HasBuff(Consts.BuffId_FlagFaQing))
            {
                return;
            }

            int faqingThreshold = 0;
            if (avatar == Player)
            {
                faqingThreshold = (int)(Ctx.GetAttributeValue(HModeAttributeType.HFaQingThreshold) * 1.0 / Consts.Float2Int100);
            }
            else
            {
                long extraThreshold = avatar.GetAttributeBuffBonus(HModeAttributeType.HFaQingThreshold);
                faqingThreshold = Consts.FaQingThreshold - (int)(extraThreshold * 1.0f / Consts.Float2Int100);
                faqingThreshold = faqingThreshold > 0 ? faqingThreshold : 0;
            }

            // 引起不足
            if (avatar.cardMag.getCardTypeNum(5) < faqingThreshold)
            {
                return;
            }

            if (avatar == Player)
            {
                Ctx.IsFaQing = true;
                OnPlayerFaQing();
            }
            else
            {
                Ctx.Enemy.IsFaQing = true;
                OnEnemyFaQing(avatar);
            }

            avatar.SetHasBuff(Consts.BuffId_FlagFaQing);
            UpdateAllStateBuff();
        }

        /// <summary>
        /// 计算发情状态下敌人的h技能
        /// </summary>
        public void ApplyEnemyHAction()
        {
            if(!Ctx.Enemy.IsFaQing)
            {
                return;
            }

            int actTimes = StaticConfigContainer.GetHActionTimesByJingjie(Ctx.Enemy.JingJie); // 计算性行动数 仅和境界相关？
            // check Tiwei
            if (Ctx.m_hTiWei > HModeTiWei.None)
            {
                var jiaoheBuff = Player.OtherAvatar.buffmag.GetBuffById(Consts.BuffId_JiaoHe);
                actTimes -= jiaoheBuff[1];
                if (actTimes < 0) actTimes = 0;
            }

            // 基础h伤害
            int hAtk = StaticConfigContainer.GetHAtkByJingjie(Ctx.Enemy.JingJie);
            // 计算增幅

            // 执行h行动
            for (int i=0;i< actTimes; i++)
            {
                int hAtkId = RandomChooseHAttack();
            }
        }


        public void ApplyHAttack(int hAtkId)
        {
            // 虚脱状态直接回
            if (Player.OtherAvatar.buffmag.HasBuff(Consts.BuffId_FlagJingJin))
            {
                return;
            }

            // 基础h伤害
            int hAtk = StaticConfigContainer.GetHAtkByJingjie(Ctx.Enemy.JingJie);

            var hAtckInfo = PluginMain.Main.ConfigDataLoader.GetConfigDataHAttackInfo(hAtkId);
            if (hAtckInfo == null)
            {
                PluginMain.Main.LogError("wrong config not Found.");
                return;
            }

            // 对每次部位伤害进行结算
            foreach (var p in hAtckInfo.TargetPart)
            {
                if(p == 0)
                {
                    ApplyNonTouchAttack(hAtckInfo,  hAtk);
                }
                else
                {
                    ApplyPartAttack(hAtckInfo, p, hAtk);
                }
            }

            CheckKuaiGanReachMax(2);

            //洗牌
            EnemyShuffleSkill(Player.OtherAvatar);
        }

        protected void EnemyShuffleSkill(KBEngine.Avatar avatar)
        {
            var skillList = avatar.skill;
            var weights = HFightUtils.GetHAttackWeightList(ref skillList);
            HFightUtils.ShuffleWithWeight(skillList, weights);
            foreach(var skill in skillList)
            {
                PluginMain.Main.LogInfo("skill after shuffle " + skill.skill_ID);
            }
        }

        public void OnBonusBuffUpdate(int seid)
        {
            switch(seid)
            {
                case (int)Consts.BuffSeId_ModMeiLi:
                    {
                        //更新 魅力buff
                        int meiLiLevel = StaticConfigContainer.GetMeiLiLevel(Ctx.GetAttributeValue(HModeAttributeType.HMeiLi) * 1.0f / Consts.Float2Int100);
                        Player.ClearAndSetMeiliBuff(meiLiLevel);
                    }
                    break;
            }
        }

        #region 表现相关



        /// <summary>
        /// 使用ctx数据更新显示用buff列表
        /// </summary>
        public void UpdateAllStateBuff()
        {
            if(Ctx.m_hTiWei == HModeTiWei.None)
            {
                Player.buffmag.RemoveBuff(Consts.BuffId_YinTili);
                Player.buffmag.RemoveBuff(Consts.BuffId_FlagWuLi);
            }
            else
            {
                if(Ctx.Tili > 0)
                {
                    Player.SetBuffLayer(Consts.BuffId_YinTili, (int)(Ctx.Tili));
                }
                else
                {
                    Player.SetBuffLayer(Consts.BuffId_YinTili, 0);
                    Player.SetHasBuff(Consts.BuffId_FlagWuLi);
                }
            }
            
            Player.SetBuffLayer(Consts.BuffId_YinYiZhuang, (int)(Ctx.YiZhuang));

            Player.SetBuffLayer(Consts.BuffId_YinKuaiGan, (int)(Ctx.KuaiGan));
            Player.SetBuffLayer(Consts.BuffId_YinYuWang, (int)(Ctx.YuWang));

            Player.OtherAvatar.SetBuffLayer(Consts.BuffId_YinKuaiGan, (int)(Ctx.Enemy.KuaiGan));
            Player.OtherAvatar.SetBuffLayer(Consts.BuffId_YinYuWang, (int)(Ctx.Enemy.YuWang));

            
        }


        /// <summary>
        /// 进入HMode ui
        /// </summary>
        public void InitHModeUI()
        {
            m_cachedNormalUI = UIFightPanel.Inst.LingQiImageDatas[UIFightPanel.Inst.LingQiImageDatas.Count - 1];
            var newData = new UILingQiImageData();
            newData.Normal = PluginMain.Main.LoadAsset<Sprite>("Icons/Lingqi_yin/MCS_ZD_linqi_yin.png");
            newData.Lock = PluginMain.Main.LoadAsset<Sprite>("Icons/Lingqi_yin/MCS_ZD_linqi_yin_an.png");
            newData.Press = PluginMain.Main.LoadAsset<Sprite>("Icons/Lingqi_yin/MCS_ZD_linqi_yin_bk.png");
            newData.Highlight = PluginMain.Main.LoadAsset<Sprite>("Icons/Lingqi_yin/MCS_ZD_linqi_yin_hlg.png");

            newData.Normal2 = PluginMain.Main.LoadAsset<Sprite>("Icons/Lingqi_yin/MCS_ZD_linqi_yinduo.png");
            newData.Lock2 = PluginMain.Main.LoadAsset<Sprite>("Icons/Lingqi_yin/MCS_ZD_linqi_yinduo_an.png");
            newData.Press2 = PluginMain.Main.LoadAsset<Sprite>("Icons/Lingqi_yin/MCS_ZD_linqi_yinduo_bk.png");
            newData.Highlight2 = PluginMain.Main.LoadAsset<Sprite>("Icons/Lingqi_yin/MCS_ZD_linqi_yinduo_hlg.png");

            newData.Normal3 = PluginMain.Main.LoadAsset<Sprite>("Icons/Lingqi_yin/MCS_ZD_linqi_yinduo2.png");
            newData.Lock3 = PluginMain.Main.LoadAsset<Sprite>("Icons/Lingqi_yin/MCS_ZD_linqi_yinduo2_an.png");
            newData.Press3 = PluginMain.Main.LoadAsset<Sprite>("Icons/Lingqi_yin/MCS_ZD_linqi_yinduo2_bk.png");
            newData.Highlight3 = PluginMain.Main.LoadAsset<Sprite>("Icons/Lingqi_yin/MCS_ZD_linqi_yinduo2_hlg.png");

            UIFightPanel.Inst.LingQiImageDatas[UIFightPanel.Inst.LingQiImageDatas.Count - 1] = newData;

            m_cachedNormalUI2 = UIFightPanel.Inst.FightSkillTip.CostSprites[5];
            UIFightPanel.Inst.FightSkillTip.CostSprites[5] = PluginMain.Main.LoadAsset<Sprite>("Icons/Cast/cast_type_9.png");

            // 填充UIFightMoveLingQi static 信息
            var pColors = typeof(UIFightMoveLingQi).GetField("pColors", BindingFlags.NonPublic | BindingFlags.Static)?.GetValue(null) as List<Color>;
            pColors[5] = new Color(0.9f, 0.4f, 0.9f);
            //var _costIconDict = typeof(ToolTipsMag).GetField("_costIconDict", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(ToolTipsMag.Inst) as Dictionary<string, Sprite>;
            //_costIconDict["5"] = PluginMain.Main.LoadAsset<Sprite>("Icons/Cast/cast_type_9.png");

            // 刷新ui
            var moSlot = UIFightPanel.Inst.PlayerLingQiController.SlotList[5];
            int tempCount = moSlot.LingQiCount;
            moSlot.LingQiCount = 0;
            moSlot.LingQiCount = tempCount;


            var fightHud = UnityEngine.GameObject.Instantiate(PluginMain.Main.LoadGameObjectFromAB("FightHud"), UIFightPanel.Inst.transform);
            m_fightHud = fightHud.AddComponent<FightHudRootController>();
            m_fightHud.InitUI(this);


            //var skillTab = UnityEngine.GameObject.Instantiate(PluginMain.Main.LoadGameObjectFromAB("FightSkillGroupTab"), UIFightPanel.Inst.transform);
            //m_cachedSkillTab = skillTab.AddComponent<FightUISkillTabController>();
            //m_cachedSkillTab.Init(this);
            //m_cachedSkillTab.transform.SetAsFirstSibling();

            //var HShowPanel = UnityEngine.GameObject.Instantiate(PluginMain.Main.LoadGameObjectFromAB("HShowPanel"), UIFightPanel.Inst.transform);
            //m_cachedHAnimController = HShowPanel.AddComponent<FightHShowController>();
            //m_cachedHAnimController.Init();

            //var HRecordPanel = UnityEngine.GameObject.Instantiate(PluginMain.Main.LoadGameObjectFromAB("FightHRecordPanel"), UIFightPanel.Inst.transform);
            //m_cachedHRecordController = HRecordPanel.AddComponent<FightUIHRecordController>();
            //m_cachedHRecordController.Init();
        }

        public void ExtendAvatarShowDamageUI(KBEngine.Avatar avatar)
        {
            var compShow = ((UnityEngine.GameObject)avatar.renderObj).GetComponentInChildren<AvatarShowHpDamage>();
            if (compShow == null || compShow.DamageTemp == null)
            {
                return;
            }

            //var damagePrefab = compShow.DamageTemp;
            //damagePrefab.name = "nmbbbbbbbbbbbbb";
            //Transform child0 = damagePrefab.transform.GetChild(0);
            //var newChild = UnityEngine.GameObject.Instantiate(child0.gameObject, child0.parent);
            //newChild.transform.SetAsLastSibling();



            //var go = new UnityEngine.GameObject();
            //var image = go.AddComponent<Image>();
            //image.color = new Color(1, 0, 0);
            //go.transform.SetParent(newChild.transform);

            //damagePrefab.transform.LogAll();
            //PluginMain.Main.LogError("newChild parent:" + newChild.transform.parent.name);
        }


        #endregion

        /// <summary>
        /// 切换技能组
        /// </summary>
        /// <param name="skillGroupId"></param>
        public void SwitchSkillGroup()
        {
            m_fightHud.m_skillTabController.OnSkillGroupSwitch();
        }

        


        private List<int> m_skillIdCache = new List<int>();
        private List<GUIPackage.Skill> m_enemySkillCache = new List<GUIPackage.Skill>();

        public FightHudRootController m_fightHud;

        private UILingQiImageData m_cachedNormalUI;
        private Sprite m_cachedNormalUI2;


        #region ctx改变回调


        public void OnSwitchTiWei(HModeTiWei preTiwei, HModeTiWei currTiwei)
        {
            // 重置标记位buff
            ResetTiWeiBuff(preTiwei, currTiwei);
            SwitchSkillGroup();

            // 离开时重置体力
            if(Ctx.m_hTiWei == HModeTiWei.None)
            {
                Ctx.Tili = 0;
            }
            else
            {
                Ctx.Tili = Player.MP_Max * 0.2f;
            }
            UpdateAllStateBuff();
        }

        public void OnModKuaiGan()
        {
            // 决定态下 改变时需要检查状态
            if (Ctx.m_isJueDing)
            {
                if (Ctx.KuaiGan <= 0)
                {
                    Ctx.m_isJueDing = false;
                    SwitchSkillGroup();
                }
            }
            UpdateAllStateBuff();
        }

        /// <summary>
        /// 当玩家更新衣装
        /// </summary>
        public void OnPlayerModYiZhuang()
        {
            int yizhuangLevel = StaticConfigContainer.GetYiZhuangLevel(Ctx.YiZhuang);
            Player.ClearAndSetYiZhuangBuff(yizhuangLevel);
            UpdateAllStateBuff();
        }

        #endregion


        #region 技能特殊效果

        public void TriggerYinYi()
        {
            // 初始时仅增加1点Kuaigan
            Ctx.ModKuaiGan(Player, 1.0f);

            UIFightPanel.Inst.FightJiLu.AddText($"{Player.name} 快感增加了");
        }

        #endregion


        public void OnFightTalkFinish(int param)
        {
            // use skill
            PluginMain.Main.LogError($"OnFightTalkFinish {param}");
        }


        #region 内部方法
        
        protected void OnEnemyJueding(KBEngine.Avatar avatar)
        {
            Ctx.Enemy.KuaiGan = 0;
            var damage = CalculateEnemyGaoChaoDamage(0);
            Player.OtherAvatar.recvDamage(Player, Player.OtherAvatar, 10000, (int)damage);
            Player.OtherAvatar.SetHasBuff(Consts.BuffId_FlagJingJin);

            // 重置体位
            if(Ctx.m_hTiWei != HModeTiWei.None)
            {
                Ctx.SwitchTiWei((int)HModeTiWei.None);
                m_runningProcessList.Enqueue(new FightProcessWaitDialog(this, Consts.HDialogId_EnterTiwei));
            }

            TotalJingAmount += (int)(damage / 5);
        }

        protected void OnPlayerJueding()
        {
            Ctx.KuaiGan = 0;
            var damage = CalculateSelfGaoChaoDamage();
            Player.recvDamage(Player, Player, 10000, (int)damage);

            Ctx.m_isJueDing = true;
        }

        protected void OnEnemyFaQing(KBEngine.Avatar avatar)
        {
            float preYuWang = Ctx.Enemy.YuWang;
            float convertKuaiGan = (preYuWang * 0.5f);

            Ctx.Enemy.YuWang = 0;
            Ctx.Enemy.KuaiGan += convertKuaiGan;

            m_enemySkillCache.Clear();
            m_enemySkillCache.AddRange(avatar.skill);

            avatar.skill.Clear();
            {
                var skillItem = new GUIPackage.Skill(9979000, 0, 10);
                avatar.skill.Add(skillItem);
            }
            {
                var skillItem = new GUIPackage.Skill(9979010, 0, 10);
                avatar.skill.Add(skillItem);
            }
            //洗牌
            EnemyShuffleSkill(Player.OtherAvatar);

            // 禁用所有被动技能天赋 缓存
            var buffList = Player.OtherAvatar.buffmag.getAllBuffByType(6);
            foreach(var buff in buffList)
            {
                Player.OtherAvatar.buffmag.RemoveBuff(buff[2]);
            }

            // 移除护盾
            Player.OtherAvatar.buffmag.RemoveBuff(5);
            // 移除被动装备
            foreach (ITEM_INFO value in Player.OtherAvatar.equipItemList.values)
            {
                foreach (JSONObject item2 in jsonData.instance.ItemJsonData[string.Concat(value.itemId)]["seid"].list)
                {
                    if (item2.I == 1)
                    {
                        int buffid = (int)jsonData.instance.EquipSeidJsonData[1][string.Concat(value.itemId)]["value1"].n;
                        Player.OtherAvatar.buffmag.RemoveBuff(buffid);
                    }
                }
            }
            // 移除悟道
            foreach (SkillItem allWuDaoSkill in Player.OtherAvatar.wuDaoMag.GetAllWuDaoSkills())
            {
                foreach (JSONObject item in jsonData.instance.StaticSkillJsonData[string.Concat(allWuDaoSkill.itemId)]["seid"].list)
                {
                    if((int)item.n != 1)
                    {
                        continue;
                    }
                    var seidConfig = jsonData.instance.StaticSkillSeidJsonData[1][allWuDaoSkill.itemId];
                    for (int i = 0; i < seidConfig["value1"].Count; i++)
                    {
                        int buffid = (int)seidConfig["value1"][i].n;
                        Player.OtherAvatar.buffmag.RemoveBuff(buffid);
                    }
                }
            }
        }


        protected void OnPlayerFaQing()
        {
            float preYiZhuang = Ctx.YiZhuang;
            Ctx.ModYiZhuang(-preYiZhuang);

            // 初始化H状态 体位信息
            Ctx.m_hState = HModeState.Normal;
            Ctx.m_hTiWei = HModeTiWei.None;

            // 提高双方 淫比重
            Player.SetHasBuff(Consts.BuffId_BasicYinLingGen);
            Player.OtherAvatar.SetHasBuff(Consts.BuffId_BasicYinLingGen);

            RoundManager.instance.removeCard(Player, 6);

            for (int i = 0; i < 10; i++)
            {
                RoundManager.instance.DrawCard(Player, (int)LingQiType.魔);
            }

            // 赋予H技能
            // 清空技能
            SwitchSkillGroup();


            UpdateAllStateBuff();
        }


        /// <summary>
        /// 切换buff
        /// </summary>
        protected void ResetTiWeiBuff(HModeTiWei preTiWei, HModeTiWei nowTiWei)
        {
            Player.ClearTiWeiBuffs();

            int newBuffId = HFightUtils.FlagBuffIdGetByTiWei(nowTiWei);

            if (newBuffId != -1)
            {
                Player.SetHasBuff(
                    newBuffId);
            }
        }

        private List<int> m_candicateHSkills = new List<int>();

        protected int RandomChooseHAttack()
        {
            m_candicateHSkills.Clear();
            var poolInfo = PluginMain.Main.ConfigDataLoader.GetConfigDataHAttackPoolInfo(20000);
            foreach (var attackId in poolInfo.AttackIdList)
            {
                var atkInfo = PluginMain.Main.ConfigDataLoader.GetConfigDataHAttackInfo(attackId);
                
                // 校验失败 不可使用
                if (!CheckHConditions(atkInfo.Conditions))
                {
                    continue;
                }
                m_candicateHSkills.Add(attackId);
            }

            List<KeyValuePair<int, int>> weights = new List<KeyValuePair<int, int>>();


            foreach (var skillId in m_candicateHSkills)
            {
                var conf = PluginMain.Main.ConfigDataLoader.GetConfigDataHAttackInfo(skillId);
                weights.Add(new KeyValuePair<int, int>(conf.ID, conf.DefaultWeight));
            }

            if (weights.Count == 0)
            {
                return -1;
            }
            int hAtakId = HFightUtils.RandomValueByWeight(weights);
            return hAtakId;
        }


        /// <summary>
        /// 检查敌方主动进入体位
        /// </summary>
        public void CheckEnemySwitchTiWei()
        {
            // 延迟执行
            m_runningProcessList.Enqueue(new FightProcessImmediate(this, delegate ()
            {
                // 先结算want体位
                CheckApplyWantTiWei();

                // 施加体位
                CheckAddWantTiWei();
            }));
        }

        /// <summary>
        /// 结算want进入tiwei
        /// </summary>
        public void CheckApplyWantTiWei()
        {
            if (Ctx.m_hState != HModeState.Normal)
            {
                return;
            }

            int[] wantLayers = new int[(int)HModeTiWei.Max];
            wantLayers[1] = Player.OtherAvatar.buffmag.GetBuffSum(Consts.BuffId_GuideShou);
            wantLayers[2] = Player.OtherAvatar.buffmag.GetBuffSum(Consts.BuffId_GuideKou);
            wantLayers[2] = Player.OtherAvatar.buffmag.GetBuffSum(Consts.BuffId_GuideRu);
            wantLayers[3] = Player.OtherAvatar.buffmag.GetBuffSum(Consts.BuffId_GuideXue);
            wantLayers[4] = Player.OtherAvatar.buffmag.GetBuffSum(Consts.BuffId_GuideGang);


            var wantingTiWei = HModeTiWei.None;

            for (int i=0;i< wantLayers.Length;i++)
            {
                if(wantLayers[1] > 6)
                {
                    wantingTiWei = (HModeTiWei)i;
                }
            }

            if(wantingTiWei == HModeTiWei.None)
            {
                return;
            }

            Ctx.SwitchTiWei((int)wantingTiWei);

            Player.OtherAvatar.SetBuffLayer((int)(Consts.BuffId_GuideShou + (int)wantingTiWei - 1), 0);

            m_runningProcessList.Enqueue(new FightProcessWaitDialog(this, Consts.HDialogId_EnterTiwei));
        }

        public void CheckAddWantTiWei()
        {
            if (Ctx.m_hState != HModeState.Normal)
            {
                return;
            }

            List<KeyValuePair<int, int>> weights = new List<KeyValuePair<int, int>>();
            foreach (var switchInfo in StaticConfigContainer.s_ConfigTiWeiSwitchDict)
            {
                if (!CheckHConditions(switchInfo.Conditions))
                {
                    continue;
                }
                weights.Add(new KeyValuePair<int, int>(switchInfo.Id, 10));
            }

            // todo 增加个体倾向性

            int switchId = HFightUtils.RandomValueByWeight(weights);
            if (switchId == -1)
            {
                return;
            }

            Player.OtherAvatar.AddTiWeiGuideBuff((HModeTiWei)switchId);

            //var processDialog = new FightProcessWaitDialog(this, Consts.HDialogId_WantEnterTiwei);
            //processDialog.SetArg("branch", switchId);
            //processDialog.EventOnEnd += delegate ()
            //{
            //    int choice = processDialog.Ret1;
            //    switch (choice)
            //    {
            //        case 0: // 未抵抗 直接进入
            //            {
            //                int buffId = Player.GetTiWeiWantBuffId((HModeTiWei)switchId);
            //                Player.SetHasBuff(buffId);
            //                break;
            //            }
            //        case 1: // 使用灵气抵抗
            //            {
            //                Player.cardMag.removeCard(5, 4);
            //                break;
            //            }
            //    }
            //};
            //m_runningProcessList.Enqueue(processDialog);
        }
        #endregion


        #region 工具


        ///// <summary>
        ///// 获取技能组列表
        ///// </summary>
        ///// <returns></returns>
        //public List<int> GetCurrSkillGroupList()
        //{
        //    var retList = new List<int>();
        //    var allSkillGroups = PluginMain.Main.ConfigDataLoader.GetAllConfigDataHSkillGroupInfo();
        //    foreach(var pair in allSkillGroups)
        //    {
        //        if(!CheckHConditions(pair.Value.ShowCondition))
        //        {
        //            PluginMain.Main.LogInfo("Cond Check Fail " + pair.Value.ID);
        //            continue;
        //        }
        //        retList.Add(pair.Key);
        //    }
        //    retList.Sort((a,b)=>a.CompareTo(b));
        //    return retList;
        //}


        /// <summary>
        /// 获取当前state 技能组id
        /// </summary>
        public int GetCurrSkillGroupId()
        {
            if(!Ctx.IsFaQing)
            {
                return 0;
            }

            if(Ctx.m_hState == HModeState.Normal)
            {
                if(Ctx.m_hTiWei == HModeTiWei.None)
                {
                    return 1;
                }
                switch(Ctx.m_hTiWei)
                {
                    case HModeTiWei.Shou:
                        {
                            return (int)100;
                        }
                    case HModeTiWei.Kou:
                        {
                            return (int)101;
                        }
                    case HModeTiWei.Ru:
                        {
                            return (int)102;
                        }
                    case HModeTiWei.Xue:
                        {
                            return (int)103;
                        }
                    case HModeTiWei.Gang:
                        {
                            return (int)104;
                        }
                }
            }

            if(Ctx.m_hState == HModeState.JueDing)
            {
                return 3;
            }
            
            return 0;
        }


        public List<int> GetSkillListByGroupId(int skillGroupId)
        {
            // 0 读取缓存id
            if (skillGroupId == 0)
            {
                return m_skillIdCache;
            }
            var newSkills = HFightUtils.HSkillListGetByGroup(skillGroupId);
            return newSkills;
        }

        /// <summary>
        /// 获取当前pose 技能组id
        /// </summary>
        public int GetCurrTiWeiSkillGroupId()
        {
            switch (Ctx.m_hTiWei)
            {
                case HModeTiWei.None:
                    return (int)HSkillGroupType.TiWeiNone;
                case HModeTiWei.Shou:
                    return (int)HSkillGroupType.TiWeiShou;
                case HModeTiWei.Kou:
                    return (int)HSkillGroupType.TiWeiKou;
                case HModeTiWei.Ru:
                    return (int)HSkillGroupType.TiWeiRu;
                case HModeTiWei.Xue:
                    return (int)HSkillGroupType.TiWeiXue;
                case HModeTiWei.Gang:
                    return (int)HSkillGroupType.TiWeiGang;
            }
            return -1;
        }



        /// <summary>
        /// 检查条件列表
        /// </summary>
        /// <param name="conditionList"></param>
        /// <returns></returns>
        protected bool CheckHConditions(List<Tuple4> conditionList)
        {
            foreach(var cond in conditionList)
            {
                if(!CheckHCondition(cond))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 检查条件
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        protected bool CheckHCondition(Tuple4 condition)
        {
            switch(condition.P1)
            {
                case (int)EConditionType.YiZhuang:
                    {
                        PluginMain.Main.LogError($"check cond curr {Ctx.YiZhuang} conf {condition.P3}");
                        var val = Ctx.YiZhuang;
                        return HFightUtils.CustomCompare(val, condition.P2, condition.P3);
                    }
                    break;
                case (int)EConditionType.SelfYuWang:
                    {
                        var val = Ctx.YuWang;
                        return HFightUtils.CustomCompare(val, condition.P2, condition.P3);
                    }
                    break;
                case (int)EConditionType.EnemyYuWang:
                    {
                        var val = Ctx.Enemy.YuWang;
                        return HFightUtils.CustomCompare(val, condition.P2, condition.P3);
                    }
                    break;
                case (int)EConditionType.TiWei:
                    {
                        var val = (int)Ctx.m_hTiWei;
                        return HFightUtils.CustomCompare(val, condition.P2, condition.P3);
                    }
                    break;
                case (int)EConditionType.IsJueDing:
                    {
                        if (condition.P2 == (int)EConditionCompareType.Equal)
                        {
                            return Ctx.m_isJueDing;
                        }
                        else if (condition.P2 == (int)EConditionCompareType.NotEqual)
                        {
                            return !Ctx.m_isJueDing;
                        }
                    }
                    break;
                case (int)EConditionType.XingFen:
                    {
                        var val = Ctx.Xingfen[condition.P2];
                        return HFightUtils.CustomCompare(val, condition.P3, condition.P4);
                    }
                    break;
                default:
                    return false;
            }

            return false;
        }



        /// <summary>
        /// 计算自身高潮伤害
        /// </summary>
        /// <returns></returns>
        protected long CalculateSelfGaoChaoDamage()
        {
            return (long)(Player.HP_Max * 0.3f);
        }

        /// <summary>
        /// 计算自身高潮伤害
        /// </summary>
        /// <returns></returns>
        protected float CalculateEnemyGaoChaoDamage(int enemyId)
        {
            int suoJing = 0;
            switch(Ctx.m_hTiWei)
            {
                case HModeTiWei.Shou:
                    {
                        suoJing = 1;
                    }
                    break;
                case HModeTiWei.Kou:
                    {
                        suoJing = PluginMain.Main.ConfigDataLoader.GetConfigDataHPartFightInfo((int)EPartType.Mouse).SuoJing;
                    }
                    break;
                case HModeTiWei.Ru:
                    {
                        suoJing = PluginMain.Main.ConfigDataLoader.GetConfigDataHPartFightInfo((int)EPartType.Breast).SuoJing;
                    }
                    break;
                case HModeTiWei.Xue:
                    {
                        suoJing = PluginMain.Main.ConfigDataLoader.GetConfigDataHPartFightInfo((int)EPartType.Pussy).SuoJing;
                    }
                    break;
                case HModeTiWei.Gang:
                    {
                        suoJing = PluginMain.Main.ConfigDataLoader.GetConfigDataHPartFightInfo((int)EPartType.Anal).SuoJing;
                    }
                    break;
            }
            int damage = (suoJing + StaticConfigContainer.GetJingLiangByJingjie(Ctx.Enemy.JingJie)) * 5;

            return damage;
        }


        /// <summary>
        /// 对单个部位实施h攻击
        /// </summary>
        /// <param name="hAtkId"></param>
        /// <param name="part"></param>
        /// <param name="hAtk"></param>
        /// <returns></returns>
        protected void ApplyPartAttack(ConfigDataHAttackInfo hAtkInfo, int part, int hAtk)
        {
            var partInfo = PluginMain.Main.ConfigDataLoader.GetConfigDataHPartFightInfo(part);
            if(partInfo == null)
            {
                return;
            }
            int armar = partInfo.Armar;
            float selfKuaiGan = hAtkInfo.SelfKuaiGan * 0.01f;
            float counterKuaiGan = partInfo.CounterKuaiGan * 0.01f;
            float applyKuaiGan = hAtkInfo.ApplyKuaiGan[0] + partInfo.HitKuaiGan * 0.01f;
            
            m_runningProcessList.Enqueue(new FightProcessWaitAnimation(this, hAtkInfo.ID + ""));

            float damage = hAtkInfo.Damage[0] + hAtk * hAtkInfo.Damage[1] * 0.01f;

            float reduceRae = (armar * 0.06f) / (1 + armar * 0.06f);
            float xingfenRate = StaticConfigContainer.GetKuaiGanRateByXingFen((int)Ctx.Xingfen[part]) * 0.01f;

            float finalDamage = damage * reduceRae * xingfenRate;

            float buffHResist = 0;
            var buffs = Player.buffmag.getBuffBySeid(Consts.BuffSeId_HResist);
            foreach(var buff in buffs)
            {
                JSONObject seidConfig = jsonData.instance.BuffSeidJsonData[Consts.BuffSeId_HResist][buff[2]];
                if(!seidConfig.HasField("value1"))
                {
                    continue;
                }
                var val = seidConfig["value1"].f;
                buffHResist += val;
            }

            // 减伤上限
            if(buffHResist > 95.0f)
            {
                buffHResist = 95.0f;
            }

            finalDamage = finalDamage * ( 1 - buffHResist * 0.01f);

            // 进行结算
            m_runningProcessList.Enqueue(new FightProcessImmediate(this, delegate ()
            {
                if(Ctx.IsFaQing)
                {
                    Ctx.ModKuaiGan(Player, applyKuaiGan);
                }
                else
                {
                    Ctx.ModYuWang(Player, applyKuaiGan * 0.5f);
                }
                Ctx.ModKuaiGan(Player.OtherAvatar, counterKuaiGan + selfKuaiGan);
                if(Ctx.Tili > 0)
                {
                    Ctx.ModTiLi(finalDamage);
                }
                else
                {
                    Player.recvDamage(Player.OtherAvatar, Player, 10000, (int)damage);
                }
                Ctx.ModXingFen(part, 1.0f);
                Ctx.ModYiZhuang(-5.0f);
            }));

            // 显示跳字
            m_runningProcessList.Enqueue(new FightProcessWaitHHint(this, 0.5f, hAtkInfo.HintContent));
        }

        protected void ApplyNonTouchAttack(ConfigDataHAttackInfo hAtkInfo, int hAtk)
        {
            m_runningProcessList.Enqueue(new FightProcessWaitAnimation(this, hAtkInfo.ID + ""));
            float damage = hAtkInfo.Damage[0] + hAtk * hAtkInfo.Damage[1] * 0.01f;
            float applyKuaiGan = hAtkInfo.ApplyKuaiGan[0] + hAtk * hAtkInfo.ApplyKuaiGan[1] * 0.01f;
            float selfKuaiGan = hAtkInfo.SelfKuaiGan;

            // 进行结算
            m_runningProcessList.Enqueue(new FightProcessImmediate(this, delegate ()
            {
                Ctx.ModKuaiGan(Player, applyKuaiGan);
                Ctx.ModKuaiGan(Player.OtherAvatar, selfKuaiGan);
                Ctx.ModTiLi(damage);
            }));

            // 显示跳字
            m_runningProcessList.Enqueue(new FightProcessWaitHHint(this, 0.5f, hAtkInfo.HintContent));
        }

        #endregion
        

        /// <summary>
        /// 显示h信息
        /// </summary>
        public void ShowHHint(string hHintContent)
        {
            var newProcess = new FightProcessWaitHHint(this, 0.5f, hHintContent);
            m_runningProcessList.Enqueue(newProcess);
        }

        public Queue<FightProcessBase> m_runningProcessList = new Queue<FightProcessBase>();

        #region temp

        public static List<int> TempHPreferByXingGe(int Xingge)
        {
            var ret = new List<int>();
            switch(Xingge)
            {
                case 1:
                { 
                }
                break;
            }
            return ret;
        }

        public static string GetHAnimNameByInfo(int targetPart, int attackType, int sex)
        {
            return "";
        }

        public int GetResistYinLingQiCost()
        {
            return 4;
        }

        #endregion
    }


    public class DialogEnvironmentEx : DialogEnvironment
    {
        public int GetCost()
        {
            return 10;
        }

        public int GetSelfYuWang()
        {
            return (int)(SecretsSystem.FightManager.Ctx.YuWang / Consts.Float2Int100);
        }

        public int GetYinLingQiCount()
        {
            return SecretsSystem.FightManager.Player.cardMag.getCardTypeNum(5);
        }
        public int GetResistYinLingQiCost()
        {
            return SecretsSystem.FightManager.GetResistYinLingQiCost();
        }
    }
}
