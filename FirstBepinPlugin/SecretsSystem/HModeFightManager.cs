using BehaviorDesigner.Runtime;
using DebuggingEssentials;
using FirstBepinPlugin.Config;
using FirstBepinPlugin.Patch;
using GUIPackage;
using HarmonyLib;
using JSONClass;
using KBEngine;
using Newtonsoft.Json;
using SkySwordKill.Next;
using SkySwordKill.Next.DialogSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using UltimateSurvival;
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
    public class FightUIPointerListener : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public event Action EventOnPointerEnter;
        public event Action EventOnPointerExit;
        public void OnPointerEnter(PointerEventData eventData)
        {
            EventOnPointerEnter?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            EventOnPointerExit?.Invoke();
        }
    }
    public class FightUISkillTabController : MonoBehaviour
    {
        public HModeFightManager Owner;
        public List<Toggle> m_toggleList = new List<Toggle>();
        public List<Text> m_toggleText = new List<Text>();

        public int m_currSelectIdx = -1;

        public bool m_isWuLi = false;
        public void Init(HModeFightManager owner)
        {
            this.Owner = owner;

            m_toggleList.Clear();
            var tabRoot = gameObject.transform.GetChild(0);
            for(int i=0;i<tabRoot.childCount;i++)
            {
                var tab = tabRoot.GetChild(i).GetComponent<UnityEngine.UI.Toggle>();
                tab.gameObject.SetActive(false);
                m_toggleList.Add(tab);
                int tabIdx = i;
                var listener = tab.gameObject.AddComponent<FightUIPointerListener>();
                listener.EventOnPointerEnter += delegate ()
                {
                    OnTogglePointerEnter(tabIdx);
                };

                listener.EventOnPointerExit += delegate ()
                {
                    OnTogglePointerExit(tabIdx);
                };

                tab.onValueChanged.AddListener(delegate(bool isOn)
                {
                    OnToggleValueChange(tabIdx);
                });

                var textComp = tab.GetComponentInChildren<Text>();
                m_toggleText.Add(textComp);
                textComp.font = PluginMain.Main.font_YaHei;
            }

            RefreshUI();
        }

        public void RefreshUI()
        {
            m_toggleList[0].gameObject.SetActive(true);
            m_toggleList[1].gameObject.SetActive(true);
            m_toggleList[2].gameObject.SetActive(true);

            switch(Owner.m_ctx.m_hTiWei)
            {
                case HModeTiWei.None:
                    {
                        m_toggleText[1].text = "体位:无";
                    }
                    break;
                case HModeTiWei.Shou:
                    {
                        m_toggleText[1].text = "体位:手";
                    }
                    break;
                case HModeTiWei.Ru:
                    {
                        m_toggleText[1].text = "体位:乳";
                    }
                    break;
                case HModeTiWei.Kou:
                    {
                        m_toggleText[1].text = "体位:口";
                    }
                    break;
                case HModeTiWei.Gang:
                    {
                        m_toggleText[1].text = "体位:尻";
                    }
                    break;
                case HModeTiWei.Xue:
                    {
                        m_toggleText[1].text = "体位:穴";
                    }
                    break;
            }

            if (m_currSelectIdx == -1)
            {
                m_currSelectIdx = 0;
            }

            m_toggleList[m_currSelectIdx].isOn = true;
        }

        public void OnToggleValueChange(int idx)
        {
            if (!m_toggleList[idx].isOn)
            {
                return;
            }
            m_currSelectIdx = idx;
            SwitchSkillGroup();
        }

        public void SwitchSkillGroup()
        {
            PluginMain.Main.LogError($"SwitchSkillGroup m_currSelectIdx {m_currSelectIdx}");
            int skillGroupId = -1;

            if (m_currSelectIdx == 0)
            {
                skillGroupId = Owner.GetCurrStateSkillGroupId();
            }
            else if (m_currSelectIdx == 1)
            {
                skillGroupId = Owner.GetCurrTiWeiSkillGroupId();
            }
            else if (m_currSelectIdx == 2)
            {
                skillGroupId = (int)HModeFightManager.HSkillGroupType.Function;
            }

            if (skillGroupId == -1)
            {
                return;
            }
            var newSkill = SecretsSystem.FightManager.HSkillListGetByGroup(skillGroupId);

            SecretsSystem.FightManager.SwitchSkill(newSkill);
        }

        public void OnTogglePointerEnter(int idx)
        {
            var hintStr = "";
            hintStr = "切换技能组：通用\r\n通用技能";
            UToolTip.Show("收起天赋", 150f);
        }

        public void OnTogglePointerExit(int idx)
        {
            UToolTip.Close();
        }

        public void OnDestroy()
        {
            // 取消事件监听 注册
            Owner.m_cachedSkillTab = null;
        }
    }

    public class HModeEnemyInfo
    {
        public int Id;
        public long YuWang;
        public long KuaiGan;
        public long MaxKuaiGan;

        public int Xingge;
        public int SexType;
        public int JingJie;

        public bool IsFaQing;
        public int TiWeiAccRate;
    }

    // HMode属性类型
    public enum HModeAttributeType
    {
        Invalid,
        HAtk,
        HDef,
        HMaxClothes,
        HMaxTili,
        HMaxKuaiGan, // 受耐力影响 达到后gc
        HMeiLi,
        HMaxXingFen_1,
        HMaxXingFen_2,
        HMaxXingFen_3,
        HMaxXingFen_4,
        Max,
    }

    public class HModeAttribute
    {
        public long BaseVal;
    }

    public class HFightCtx
    {

        public bool m_isInHMode;

        // 动态数值
        public long Tili;
        public long YiZhuang;
        public long[] Xingfen = new long[(int)EnumPartType.Max];
        public long YuWang;
        public long KuaiGan;

        public HModeEnemyInfo Enemy = new HModeEnemyInfo();

        // 静态属性
        public HModeAttribute[] StaticAttributeBaseVal = new HModeAttribute[(int)HModeAttributeType.Max];

        public HModeState m_hState = HModeState.Invalid;
        public HModeTiWei m_hTiWei = HModeTiWei.None;

        public Dictionary<int, GUIPackage.Skill> m_stateSkillCache = new Dictionary<int, GUIPackage.Skill>();
        public List<int> m_nonHSkillCache = new List<int>();

        public void Clear()
        {
            m_isInHMode = false;
            foreach(var attr in StaticAttributeBaseVal)
            {
                if (attr == null) continue;
                attr.BaseVal = 0;
            }
        }
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



        public KBEngine.Avatar m_player;
        public HFightCtx m_ctx = new HFightCtx();
        public bool IsInBattle;

        /// <summary>
        /// 是否正在等待结算h攻击
        /// </summary>
        public bool IsWaitHAttack;

        /// <summary>
        /// 初始化
        /// </summary>
        public void FightHInit()
        {
            m_ctx.Clear();
            IsInBattle = true;
            m_player = Tools.instance.getPlayer();

            InitAttribute(HModeAttributeType.HAtk, 0);
            InitAttribute(HModeAttributeType.HDef, 0);
            InitAttribute(HModeAttributeType.HMaxClothes, 100);
            InitAttribute(HModeAttributeType.HMaxTili, 100);
            InitAttribute(HModeAttributeType.HMaxKuaiGan, 100);
            InitAttribute(HModeAttributeType.HMeiLi, 5);

            InitAttribute(HModeAttributeType.HMaxXingFen_1, 0);
            InitAttribute(HModeAttributeType.HMaxXingFen_2, 0);
            InitAttribute(HModeAttributeType.HMaxXingFen_3, 0);
            InitAttribute(HModeAttributeType.HMaxXingFen_4, 0);


            JSONObject jSONObject = jsonData.instance.AvatarJsonData[string.Concat(Tools.instance.MonstarID)];
            if(jSONObject.HasField("XingGe"))
            {
                m_ctx.Enemy.Xingge = jSONObject.GetField("XingGe").I;
            }
            if (jSONObject.HasField("SexType"))
            {
                m_ctx.Enemy.SexType = jSONObject.GetField("SexType").I;
            }
            if (jSONObject.HasField("Level"))
            {
                m_ctx.Enemy.JingJie = jSONObject.GetField("Level").I / 3;
            }
        }

        /// <summary>
        /// tick
        /// </summary>
        public void FightTick(float dt)
        {
            // 一帧一个
            if(m_runningProcessList.Count != 0)
            {
                var firstProcess = m_runningProcessList.Peek();
                if(!firstProcess.m_isStart)
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

        public void FightOnRoungStartPre(KBEngine.Avatar avatar)
        {
            if (avatar == m_player)
            {
                // 计算兴奋度
                ApplyTurnXingFen();
            }
                
        }

        public void FightOnRoungStartPost(KBEngine.Avatar avatar)
        {
            // 玩家回合开始
            if(avatar == m_player)
            {
                CheckKuaiGanReachMax(1);
            }
            else
            {
                if (CheckEnemyFaQing())
                {
                    ApplyEnemyFaQing();
                }
                CheckKuaiGanReachMax(2);

                // startRound
            }
        }

        public void FightOnRoungEndPre(KBEngine.Avatar avatar)
        {

        }

        public void FightOnRoungEndPost(KBEngine.Avatar avatar)
        {

        }

        /// <summary>
        /// 进入H形态
        /// </summary>
        public void FightEnterHMode()
        {
            if(m_ctx.m_isInHMode)
            {
                return;
            }

            // 主动使用技能进入H形态
            // 需要进行状态初始化 改变UI等操作
            var player = Tools.instance.getPlayer();

            SwitchHModeUI();

            // 初始化属性
            m_ctx.m_hState = HModeState.Normal;
            m_ctx.Tili = 100 * Consts.Float2Int100;
            m_ctx.YiZhuang = 100 * Consts.Float2Int100;

            m_ctx.KuaiGan = 0;
            m_ctx.YuWang = 0;

            m_ctx.Enemy.KuaiGan = 0;
            m_ctx.Enemy.YuWang = 0;

            UpdateAllStateBuff();

            //    player.OtherAvatar.spell.addBuff(9971009, 1);

            // 赋予H技能
            // 清空技能
            var skillList = HSkillListGetByGroup((int)HSkillGroupType.Common);
            SwitchSkill(skillList);

            m_ctx.m_isInHMode = true;

            RoundManager.instance.removeCard(m_player, 6);

            for (int i=0;i<10;i++)
            {
                RoundManager.instance.DrawCard(m_player, (int)LingQiType.魔);
            }

            // 提高双方 淫比重
            SetHasBuff(player, Consts.BuffId_BasicYinLingen);
            SetHasBuff(player.OtherAvatar, Consts.BuffId_BasicYinLingen);

            // 移除魔气buff
            player.buffmag.RemoveBuff(10000);
            player.OtherAvatar.buffmag.RemoveBuff(10000);

            // 每回合增加欲望
            SetBuffLayer(player, Consts.BuffId_TurnModYuWang, 10);
        }


        /// <summary>
        /// 检查更新动态buff
        /// </summary>
        protected void ApplyTurnXingFen()
        {
            int turnXingFen = StaticConfigContainer.GetTurnXingFenByYuWang((int)m_ctx.YuWang / 100);
            ModXingFen(0, turnXingFen);
        }

        /// <summary>
        /// 检查快感满
        /// </summary>
        /// <param name="target"></param>
        protected void CheckKuaiGanReachMax(int targetId)
        {
            long kuaiGan;
            long maxKuaiGan;
            if (targetId == 1)
            {
                kuaiGan = m_ctx.KuaiGan;
                maxKuaiGan = GetAttributeValue(HModeAttributeType.HMaxKuaiGan);
            }
            else
            {
                kuaiGan = m_ctx.Enemy.KuaiGan;
                maxKuaiGan = m_ctx.Enemy.MaxKuaiGan;
            }

            // 没达到满 无事发生
            if (kuaiGan < maxKuaiGan)
            {
                return;
            }

            if (targetId == 1)
            {
                m_ctx.KuaiGan = 0;
                var damage = CalculateSelfGaoChaoDamage();
                m_player.recvDamage(m_player, m_player, 10000, (int)damage);
                SwitchTiWei((int)HModeState.JueDing);

                OnJueDing();
            }
            else
            {
                m_ctx.Enemy.KuaiGan = 0;
                var damage = CalculateEnemyGaoChaoDamage(0);
                m_player.OtherAvatar.recvDamage(m_player, m_player, 10000, (int)damage);
                SetHasBuff(m_player.OtherAvatar, Consts.BuffId_FlagJingJin);
            }
        }

        /// <summary>
        /// 计算发情状态下敌人的h技能
        /// </summary>
        public void ApplyEnemyHAction()
        {
            if(!m_ctx.m_isInHMode)
            {
                return;
            }
            // 只有发情时才结算
            if (!m_ctx.Enemy.IsFaQing)
            {
                return;
            }

            // 检查敌人进入体位
            if(m_ctx.m_hState == HModeState.Normal)
            {
                CheckEnemySwitchTiWei();
            }

            int actTimes = StaticConfigContainer.GetHActionTimesByJingjie(m_ctx.Enemy.JingJie); // 计算性行动数 仅和境界相关？
            // check Tiwei
            if (m_ctx.m_hTiWei > HModeTiWei.None)
            {
                var jiaoheBuff = m_player.OtherAvatar.buffmag.GetBuffById(Consts.BuffId_JiaoHe);
                actTimes -= jiaoheBuff[1];
                if (actTimes < 0) actTimes = 0;
            }

            // 基础h伤害
            int hAtk = StaticConfigContainer.GetHAtkByJingjie(m_ctx.Enemy.JingJie);
            // 计算增幅

            // 执行h行动
            for (int i=0;i< actTimes; i++)
            {
                int hAtkId = RandomChooseHAttack();
                var hAtckInfo = PluginMain.Main.ConfigDataLoader.GetConfigDataHAttackInfo(hAtkId);
                if(hAtckInfo == null)
                {
                    PluginMain.Main.LogError("wrong config not Found.");
                    continue;
                }
                // 对每次部位伤害进行结算
                foreach (var p in hAtckInfo.TargetPart)
                {
                    ApplyPartAttack(hAtckInfo, p, hAtk);
                }

                CheckKuaiGanReachMax(2);
                //虚脱则跳出
                if (m_player.OtherAvatar.buffmag.HasBuff(Consts.BuffId_FlagJingJin) /*&& nojianting*/)
                {
                    break;
                }
            }
        }


        /// <summary>
        /// 使用ctx数据更新显示用buff列表
        /// </summary>
        public void UpdateAllStateBuff()
        {
            SetBuffLayer(m_player, Consts.BuffId_YinTili, (int)(m_ctx.Tili/Consts.Float2Int100));
            SetBuffLayer(m_player, Consts.BuffId_YinYiZhuang, (int)(m_ctx.YiZhuang/ Consts.Float2Int100));

            SetBuffLayer(m_player, Consts.BuffId_YinKuaiGan, (int)(m_ctx.KuaiGan / Consts.Float2Int100));
            SetBuffLayer(m_player, Consts.BuffId_YinYuWang, (int)(m_ctx.YuWang / Consts.Float2Int100));

            SetBuffLayer(m_player.OtherAvatar, Consts.BuffId_YinKuaiGan, (int)(m_ctx.Enemy.KuaiGan / Consts.Float2Int100));
            SetBuffLayer(m_player.OtherAvatar, Consts.BuffId_YinYuWang, (int)(m_ctx.Enemy.YuWang / Consts.Float2Int100));
        }


        /// <summary>
        /// 执行切换技能
        /// </summary>
        public void SwitchSkill(List<int> newSkillList)
        {
            if(newSkillList == null)
            {
                return;
            }

            m_player.FightClearSkill(0, 10);

            // 提示并返回错误
            if (newSkillList.Count > 10)
            {
                UIPopTip.Inst.Pop("技能超过10个 仅保留10个");
            }
            for(int i=0;i<newSkillList.Count && i<10;i++)
            {
                int skillId = newSkillList[i];
                var skillItem = m_player.skill.Find(delegate (GUIPackage.Skill s) { return s.skill_ID == skillId; });
                if (skillItem == null)
                {
                    skillItem = new GUIPackage.Skill(skillId, 0, 10);
                }
                m_player.skill.Add(skillItem);
                int num = 0;
                foreach (UIFightSkillItem fightSkill in UIFightPanel.Inst.FightSkills)
                {
                    if (num >= 0 && num < 10 && !fightSkill.HasSkill)
                    {
                        fightSkill.SetSkill(skillItem);
                        break;
                    }
                    num++;
                }
            }
        }

        private List<int> m_skillIdCache = new List<int>();

        public FightUISkillTabController m_cachedSkillTab;
        public FightHShowController m_cachedHAnimController;

        private UILingQiImageData m_cachedNormalUI;
        private Sprite m_cachedNormalUI2;
        /// <summary>
        /// 进入HMode ui
        /// </summary>
        public void SwitchHModeUI()
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

            var skillTab = UnityEngine.GameObject.Instantiate(PluginMain.Main.LoadGameObjectFromAB("FightSkillGroupTab"), UIFightPanel.Inst.transform);
            m_cachedSkillTab = skillTab.AddComponent<FightUISkillTabController>();
            m_cachedSkillTab.Init(this);

            m_cachedSkillTab.transform.SetAsFirstSibling();

            var HShowPanel = UnityEngine.GameObject.Instantiate(PluginMain.Main.LoadGameObjectFromAB("HShowPanel"), UIFightPanel.Inst.transform);
            m_cachedHAnimController = HShowPanel.AddComponent<FightHShowController>();
            m_cachedHAnimController.Init();

            ExtendAvatarShowDamageUI(m_player);
            ExtendAvatarShowDamageUI(m_player.OtherAvatar);
        }

        public void ExtendAvatarShowDamageUI(KBEngine.Avatar avatar)
        {
            var compShow = ((UnityEngine.GameObject)avatar.renderObj).GetComponentInChildren<AvatarShowHpDamage>();
            if (compShow == null || compShow.DamageTemp == null)
            {
                return;
            }

            var damagePrefab = compShow.DamageTemp;
            damagePrefab.name = "nmbbbbbbbbbbbbb";
            Transform child0 = damagePrefab.transform.GetChild(0);
            var newChild = UnityEngine.GameObject.Instantiate(child0.gameObject, child0.parent);
            newChild.transform.SetAsLastSibling();



            var go = new UnityEngine.GameObject();
            var image = go.AddComponent<Image>();
            image.color = new Color(1, 0, 0);
            go.transform.SetParent(newChild.transform);

            damagePrefab.transform.LogAll();
            PluginMain.Main.LogError("newChild parent:" + newChild.transform.parent.name);
            
        }


        #region 取属性

        /// <summary>
        /// 衣装变化
        /// </summary>
        /// <param name="modVal"></param>
        public void ModYiZhuang(float modVal)
        {
            m_ctx.YiZhuang += (long)(modVal * Consts.Float2Int100);
            long maxVal = GetAttributeValue(HModeAttributeType.HMaxClothes);
            if(maxVal < 0)
            {
                maxVal = 0;
            }
            if (m_ctx.YiZhuang < 0)
            {
                m_ctx.YiZhuang = 0;
            }
            if(m_ctx.YiZhuang > maxVal)
            {
                m_ctx.YiZhuang = maxVal;
            }
            UpdateAllStateBuff();
        }

        /// <summary>
        /// 衣装变化
        /// </summary>
        /// <param name="modVal"></param>
        public void ModTiLi(float modVal)
        {
            m_ctx.Tili += (long)(modVal * Consts.Float2Int100);
            long maxVal = GetAttributeValue(HModeAttributeType.HMaxTili);
            if (maxVal < 1)
            {
                maxVal = 1;
            }
            if (m_ctx.Tili > maxVal)
            {
                m_ctx.Tili = maxVal;
            }
            // tili归零
            if (m_ctx.Tili <= 0)
            {
                SetIsWuLi(true);
            }
            else
            {
                SetIsWuLi(false);
            }
            UpdateAllStateBuff();
        }

        /// <summary>
        /// 欲望变化
        /// </summary>
        /// <param name="target"></param>
        /// <param name="addVal"></param>
        public void ModYuWang(int target, float modVal)
        {
            if(target == 1)
            {
                m_ctx.YuWang += (long)(modVal * Consts.Float2Int100);
                if(m_ctx.YuWang < 0)
                {
                    m_ctx.YuWang = 0;
                }
            }
            else
            {
                m_ctx.Enemy.YuWang += (long)(modVal * Consts.Float2Int100);
                if (m_ctx.Enemy.YuWang < 0)
                {
                    m_ctx.Enemy.YuWang = 0;
                }
            }
            

            PluginMain.Main.LogError("?ModYuWang");
            UpdateAllStateBuff();

            if(target == 2)
            {
                var hpShow = ((UnityEngine.GameObject)m_player.OtherAvatar.renderObj).GetComponentInChildren<AvatarShowHpDamage>();
                var go = hpShow.DamageTemp;
                go.transform.LogAll();
                //((UnityEngine.GameObject)m_player.OtherAvatar.renderObj).GetComponentInChildren<AvatarShowHpDamage>().SetText($"欲望+{addVal / 100}",4);
            }
        }

        /// <summary>
        /// 兴奋度变化 part=0 表示随机
        /// </summary>
        /// <param name="addVal"></param>
        public void ModXingFen(int part, float modVal)
        {
            int realPart = part;
            if(part == 0)
            {
                List<KeyValuePair<int, int>> weights = new List<KeyValuePair<int, int>>() 
                { 
                    new KeyValuePair<int, int>(1,3),
                    new KeyValuePair<int, int>(2,3),
                    new KeyValuePair<int, int>(3,2),
                    new KeyValuePair<int, int>(4,3),
                };
                int k = RandomValueByWeight(weights);
                realPart = k;
            }
            if(realPart == 0)
            {
                return;
            }
            

            m_ctx.Xingfen[realPart] += (long)(modVal * Consts.Float2Int100);
            long maxVal = (long)(GetMaxPartXingFen(realPart) * Consts.Float2Int100);

            if (m_ctx.Xingfen[realPart] < 0)
            {
                m_ctx.Xingfen[realPart] = 0;
            }

            if (m_ctx.Xingfen[realPart] > maxVal)
            {
                m_ctx.Xingfen[realPart] = maxVal;
            }

            UpdateAllStateBuff();
        }

        /// <summary>
        /// 累加快感
        /// </summary>
        /// <param name="target"></param>
        /// <param name="addVal"></param>
        public void ModKuaiGan(int target, float modVal)
        {
            if (target == 1)
            {
                m_ctx.KuaiGan += (long)(modVal * Consts.Float2Int100);
                if (m_ctx.KuaiGan < 0) m_ctx.KuaiGan = 0;
            }
            else
            {
                m_ctx.Enemy.KuaiGan += (long)(modVal * Consts.Float2Int100);
                if (m_ctx.Enemy.KuaiGan < 0) m_ctx.Enemy.KuaiGan = 0;
            }

            UpdateAllStateBuff();
        }

        public void TriggerYinYi()
        {
            // 初始时仅增加1点Kuaigan
            ModKuaiGan(1, 1.0f);

            UIFightPanel.Inst.FightJiLu.AddText($"{m_player.name} 快感增加了");
        }

        public void CheckHFightEnd()
        {
            
        }

        /// <summary>
        /// 切换无力状态
        /// </summary>
        /// <param name="isWuLi"></param>
        public void SetIsWuLi(bool isWuLi)
        {
            m_cachedSkillTab.m_isWuLi = isWuLi;
            m_cachedSkillTab.RefreshUI();
        }

        public void OnJueDing()
        {
            //trigger
        }

        /// <summary>
        /// 切换体位
        /// </summary>
        /// <param name="addVal"></param>
        public void SwitchTiWei(int tiweiId, bool showEffect = false)
        {
            var preTiWei = m_ctx.m_hTiWei;
            if(preTiWei == (HModeTiWei)tiweiId)
            {
                PluginMain.Main.LogInfo("HModeFightManager SwitchTiWei same.");
                return;
            }
            m_ctx.m_hTiWei = (HModeTiWei)tiweiId;
            // 重置标记位buff
            ResetTiWeiBuff(preTiWei, m_ctx.m_hTiWei);

            m_cachedSkillTab.RefreshUI();
            PluginMain.Main.LogInfo($"SwitchTiWei new Tiwei {tiweiId}.");
        }

        /// <summary>
        /// 切换buff
        /// </summary>
        protected void ResetTiWeiBuff(HModeTiWei preTiWei, HModeTiWei nowTiWei)
        {
            int oldBuffId = FlagBuffIdGetByTiWei(preTiWei);
            int newBuffId = FlagBuffIdGetByTiWei(nowTiWei);

            if(oldBuffId == newBuffId)
            {
                return;
            }
            if(oldBuffId != -1)
            {
                m_player.buffmag.RemoveBuff(oldBuffId);
            }
            if(newBuffId != -1)
            {
                SetHasBuff(m_player, newBuffId);
            }
        }

        /// <summary>
        /// 获得姿态专属标记buff
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static int FlagBuffIdGetByTiWei(HModeTiWei tiWei)
        {
            switch(tiWei)
            {
                case HModeTiWei.None:
                    return Consts.BuffId_FlagNormal;
                case HModeTiWei.Shou:
                    return Consts.BuffId_FlagShou;
            }
            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public long GetAttributeValue(HModeAttributeType type)
        {
            long baseVal = 0;
            long addVal = 0;

            var attr = m_ctx.StaticAttributeBaseVal[(int)type]; 
            if(attr == null)
            {
                return 0;
            }
            baseVal = attr.BaseVal;
            int modSeid = ModSeidGetByAttribute(type);

            if(modSeid != 0)
            {
                // sum mode
                foreach (var buffInfo in m_player.buffmag.getBuffBySeid(modSeid))
                {
                    var buffId = buffInfo[2];
                    int buffLayer = buffInfo[1];

                    JSONObject jSONObject = jsonData.instance.BuffSeidJsonData[modSeid];

                    if (!jSONObject.HasField(buffId.ToString()))
                    {
                        continue;
                    }

                    var configInfo = jSONObject[buffId.ToString()];

                    addVal += (long)configInfo["value1"].n * 100 * buffLayer;
                }
            }
            PluginMain.Main.LogInfo($"try GetAttributeValue {type} base {baseVal} add {addVal} ");
            return (baseVal + addVal);
        }

        /// <summary>
        /// 通过属性获取seid
        /// </summary>
        /// <returns></returns>
        protected int ModSeidGetByAttribute(HModeAttributeType type)
        {
            switch(type)
            {
                case HModeAttributeType.HMaxClothes:
                    return Consts.BuffSeId_ModMaxYizhuang;
                case HModeAttributeType.HMeiLi:
                    return Consts.BuffSeId_ModMeiLi;
                case HModeAttributeType.HMaxTili:
                    return Consts.BuffSeId_ModMaxTili;
                    
            }
            return 0;
        }

        /// <summary>
        /// 初始化属性值
        /// </summary>
        /// <param name="attr"></param>
        /// <param name="value"></param>
        protected void InitAttribute(HModeAttributeType attrType, float value)
        {
            var attr = new HModeAttribute();
            attr.BaseVal = (long)(value * Consts.Float2Int100);
            m_ctx.StaticAttributeBaseVal[(int)attrType] = attr;
        }

        #endregion

        /// <summary>
        /// 检查是否进行发情
        /// </summary>
        /// <returns></returns>
        public bool CheckEnemyFaQing()
        {
            PluginMain.Main.LogInfo("CheckEnemyFaQing");
            var enemy = m_player.OtherAvatar;
            int shenshi = enemy.shengShi;
            if (m_ctx.Enemy.YuWang <= shenshi * 100)
            {
                PluginMain.Main.LogInfo("欲望低于神识 不进入发情");
                return false;
            }

            // 之后每点欲望提高1%发情几率
            int rate10000 = (int)(m_ctx.Enemy.YuWang - (long)(shenshi * 100));
            if (rate10000 >= 10000)
            {
                return true;
            }
            PluginMain.Main.LogInfo($"发情几率：{rate10000} / 10000");
            int randVal = UnityEngine.Random.Range(0, 10000);
            if (randVal < rate10000)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 实现发情
        /// </summary>
        public void ApplyEnemyFaQing()
        {
            PluginMain.Main.LogInfo("ApplyEnemyFaQing");
            m_ctx.Enemy.IsFaQing = true;
            SetHasBuff(m_player.OtherAvatar, Consts.BuffId_FlagFaQing);

            long preYuWang = m_ctx.Enemy.YuWang;
            long convertKuaiGan = (long)(preYuWang * 0.5f);

            m_ctx.Enemy.YuWang = 0;
            m_ctx.Enemy.KuaiGan += convertKuaiGan;
            UpdateAllStateBuff();
        }

        /// <summary>
        /// 指定avatar 设置buff层数 仅支持叠加的
        /// </summary>
        /// <param name="target"></param>
        /// <param name="buffId"></param>
        /// <param name="newLayer"></param>
        public void SetBuffLayer(KBEngine.Avatar target, int buffId, int newLayer)
        {
            var buffInfo = _BuffJsonData.DataDict[buffId];
            if(buffInfo == null)
            {
                return;
            }
            if(buffInfo.BuffType != 0)
            {
                PluginMain.Main.LogError($"Only Stackable Buff Can Use SetBuffLayer. {buffId}");
                return;
            }

            List<List<int>> buffByID = target.buffmag.getBuffByID(buffId);
            int oldLayer = 0;
            if (buffByID.Count > 0)
            {
                oldLayer = buffByID.Count;
                buffByID[0][1] = newLayer;
            }
            else
            {
                target.spell.addDBuff(buffId, newLayer);
            }

            // 特殊处理
            if(buffInfo.seid.Contains(64))
            {
                var seidJson = Buff.getSeidJson(64, buffId);
                var v1 = seidJson["value1"].I;
                var v2 = seidJson["value2"].I;
                int oldVal = 0;
                if(!target.SkillSeidFlag.ContainsKey(13))
                {
                    target.SkillSeidFlag[13] = new Dictionary<int, int>();
                }

                PluginMain.Main.LogError($"seid64 check v1 {v1} v2 {v2} newLayer {newLayer} oldLayer {oldLayer}");

                if (target.SkillSeidFlag[13].ContainsKey(v1))
                {
                    oldVal = target.SkillSeidFlag[13][v1];
                }
                int changedVal = (newLayer - oldLayer) * v2;
                target.SkillSeidFlag[13][v1] = oldVal + changedVal;

                // 打印额外灵根情况
                foreach(var kv in target.SkillSeidFlag[13])
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
        public void SetHasBuff(KBEngine.Avatar target, int buffId)
        {
            var buffInfo = _BuffJsonData.DataDict[buffId];
            if (buffInfo == null)
            {
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

        public void OnFightTalkFinish(int param)
        {
            // use skill
            PluginMain.Main.LogError($"OnFightTalkFinish {param}");
        }



        #region 内部方法

        private List<int> m_candicateHSkills = new List<int>();
        protected int RandomChooseHAttack()
        {
            m_candicateHSkills.Clear();
            var poolInfo = PluginMain.Main.ConfigDataLoader.GetConfigDataHAttackPoolInfo(20000);
            foreach (var attackId in poolInfo.AttackIdList)
            {
                var atkInfo = PluginMain.Main.ConfigDataLoader.GetConfigDataHAttackInfo(attackId);
                bool checkFail = false;
                foreach (var condId in atkInfo.Conditions)
                {
                    var condInfo = PluginMain.Main.ConfigDataLoader.GetConfigDataHAttackConditionInfo(condId);
                }
                if (checkFail)
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
            int hAtakId = RandomValueByWeight(weights);
            return hAtakId;
        }


        /// <summary>
        /// 检查敌方主动进入体位
        /// </summary>
        protected void CheckEnemySwitchTiWei()
        {
            int valRate = UnityEngine.Random.Range(0, 100);
            int totalRate = m_ctx.Enemy.TiWeiAccRate;
            // 计算魅力
            {
                long meiLiVal = GetAttributeValue(HModeAttributeType.HMeiLi);
                int meiLiLevel = (int)(meiLiVal / 2000);
                if (meiLiLevel > 5) meiLiLevel = 5;
                totalRate += 5 * meiLiLevel;
            }
            {
                int yuwangLevel = (int)(m_ctx.Enemy.YuWang / 2000);
                totalRate += 5 * yuwangLevel;
            }
            if (valRate < totalRate)
            {
                // 进体位 todo 计算权重
                SwitchTiWei((int)HModeTiWei.Shou);
            }
            else
            {
                m_ctx.Enemy.TiWeiAccRate += 10;
            }
        }

        #endregion


        public void FightExitHMode()
        {
            PluginMain.Main.LogInfo("try exit h mode.");

            if (m_ctx == null || !m_ctx.m_isInHMode)
            {
                return;
            }

            var player = Tools.instance.getPlayer();
            m_ctx.m_isInHMode = false;

            // 清空技能
            player.FightClearSkill(0, 10);

            foreach (var oldSkillId in m_ctx.m_nonHSkillCache)
            {
                var skillItem = player.skill.Find(delegate (GUIPackage.Skill s) { return s.skill_ID == oldSkillId; });

                // 一定在列表中
                if (skillItem == null)
                {
                    continue;
                }
                int num = 0;
                foreach (UIFightSkillItem fightSkill in UIFightPanel.Inst.FightSkills)
                {
                    if (num >= 0 && num < 10 && !fightSkill.HasSkill)
                    {
                        fightSkill.SetSkill(skillItem);
                        break;
                    }
                    num++;
                }
            }
        }

        #region 工具

        /// <summary>
        /// 获取对应组的技能
        /// </summary>
        /// <returns></returns>
        public List<int> HSkillListGetByGroup(int skillGroup)
        {
            m_skillIdCache.Clear();
            // skil list
            switch (skillGroup)
            {
                case (int)HSkillGroupType.Common: // 通用技能
                    {
                        m_skillIdCache.Add(99720);
                        m_skillIdCache.Add(99721);
                        m_skillIdCache.Add(99722);
                    }
                    break;
                case (int)HSkillGroupType.TiWeiNone: // 无体位 非接触技能
                    {
                        m_skillIdCache.Add(99723);
                    }
                    break;
                case (int)HSkillGroupType.Function: // 功能技能
                    {
                        m_skillIdCache.Add(99799);
                    }
                    break;
                case (int)HSkillGroupType.TiWeiShou: // 手技能
                    {
                        m_skillIdCache.Add(99730);
                    }
                    break;
                case (int)HSkillGroupType.Wuli: // 体力归零技能组 复活
                    {
                        m_skillIdCache.Add(99730);
                    }
                    break;
                case (int)HSkillGroupType.JueDing: // 绝顶后 技能组 复活
                    {
                        m_skillIdCache.Add(99730);
                    }
                    break;
            }
            return m_skillIdCache;
        }

        /// <summary>
        /// 获取当前state 技能组id
        /// </summary>
        public int GetCurrStateSkillGroupId()
        {
            switch (m_ctx.m_hState)
            {
                case HModeState.Normal:
                    return (int)HSkillGroupType.Common;
                case HModeState.JueDing:
                    return (int)HSkillGroupType.JueDing;
            }
            return -1;
        }


        /// <summary>
        /// 获取当前pose 技能组id
        /// </summary>
        public int GetCurrTiWeiSkillGroupId()
        {
            switch (m_ctx.m_hTiWei)
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
        /// 通过weight获取id
        /// </summary>
        /// <param name="weights"></param>
        /// <returns></returns>
        protected int RandomValueByWeight(List<KeyValuePair<int, int>> weights)
        {
            int maxWeighht = 0;
            for(int i=0;i<weights.Count;i++)
            {
                maxWeighht += weights[i].Value;
            }
            if(maxWeighht == 0)
            {
                return 0;
            }
            int randVal = UnityEngine.Random.Range(0, maxWeighht);
            int currWeight = 0;
            for (int i = 0; i < weights.Count; i++)
            {
                currWeight += weights[i].Value;
                if(randVal < currWeight)
                {
                    return weights[i].Key;
                }
            }
            return 0;
        }

        protected float GetMaxPartXingFen(int part)
        {
            var partInfo = PluginMain.Main.ConfigDataLoader.GetConfigDataHPartFightInfo(part);
            if (partInfo == null) return 0;
            long modXingFen = 0;
            switch(part)
            {
                case 1:
                {
                    modXingFen = GetAttributeValue(HModeAttributeType.HMaxXingFen_1);
                }
                break;
                case 2:
                {
                    modXingFen = GetAttributeValue(HModeAttributeType.HMaxXingFen_2);
                }
                break;
                case 3:
                {
                    modXingFen = GetAttributeValue(HModeAttributeType.HMaxXingFen_3);
                }
                break;
                case 4:
                {
                    modXingFen = GetAttributeValue(HModeAttributeType.HMaxXingFen_4);
                }
                break;
            }
            
            int ret = partInfo.MaxXingFen + (int)(modXingFen / 100);

            return ret;
        }

        /// <summary>
        /// 计算自身高潮伤害
        /// </summary>
        /// <returns></returns>
        protected long CalculateSelfGaoChaoDamage()
        {
            return (long)(m_player.HP_Max * 0.3f);
        }

        /// <summary>
        /// 计算自身高潮伤害
        /// </summary>
        /// <returns></returns>
        protected float CalculateEnemyGaoChaoDamage(int enemyId)
        {
            int suoJing = 0;
            switch(m_ctx.m_hTiWei)
            {
                case HModeTiWei.Shou:
                    {
                        suoJing = 1;
                    }
                    break;
                case HModeTiWei.Kou:
                    {
                        suoJing = PluginMain.Main.ConfigDataLoader.GetConfigDataHPartFightInfo((int)EnumPartType.Mouse).SuoJing;
                    }
                    break;
                case HModeTiWei.Ru:
                    {
                        suoJing = PluginMain.Main.ConfigDataLoader.GetConfigDataHPartFightInfo((int)EnumPartType.Breast).SuoJing;
                    }
                    break;
                case HModeTiWei.Xue:
                    {
                        suoJing = PluginMain.Main.ConfigDataLoader.GetConfigDataHPartFightInfo((int)EnumPartType.Pussy).SuoJing;
                    }
                    break;
                case HModeTiWei.Gang:
                    {
                        suoJing = PluginMain.Main.ConfigDataLoader.GetConfigDataHPartFightInfo((int)EnumPartType.Anal).SuoJing;
                    }
                    break;
            }
            int damage = (suoJing + StaticConfigContainer.GetJingLiangByJingjie(m_ctx.Enemy.JingJie)) * 5;

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

            if (partInfo == null) return;

            float conterKuaigan = partInfo.CounterKuaiGan * 0.01f;
            float hitKuaigan = partInfo.HitKuaiGan * 0.01f * hAtkInfo.KuaiGanRate * 0.01f;


            m_runningProcessList.Enqueue(new FightProcessWaitAnimation(this, hAtkInfo.ID + ""));

            float damage = hAtk * hAtkInfo.DamageRate * 0.01f;
            float reduceRae = (partInfo.Armar * 0.06f) / (1 + partInfo.Armar * 0.06f);
            float xingfenRate = StaticConfigContainer.GetKuaiGanRateByXingFen((int)(m_ctx.Xingfen[part] / 100)) * 0.01f;

            // 进行结算
            m_runningProcessList.Enqueue(new FightProcessImmediate(this, delegate ()
            {
                ModKuaiGan(1, hitKuaigan);
                ModKuaiGan(2, conterKuaigan);
                ModTiLi(damage * reduceRae * xingfenRate);
                ModXingFen(part, 1.0f);
            }));

            // 显示跳字
            m_runningProcessList.Enqueue(new FightProcessWaitHHint(this, 0.5f, hAtkInfo.HintContent));
            // 结算
            m_runningProcessList.Enqueue(new FightProcessBalance(this));
        }


        #endregion
        


        public void ShowProcessApplyDamage()
        {

        }

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

        #endregion
    }

    #region process

    public abstract class FightProcessBase
    {
        public HModeFightManager Owner;
        public ulong m_id;
        public bool m_isEnd;
        public bool m_isStart;

        public FightProcessBase(HModeFightManager owner)
        {
            Owner = owner;
            m_isEnd = false;
            m_isStart = false;
        }

        public virtual void OnStart()
        {
        }
        public abstract void Tick(float dt);

        public event Action EventOnEnd;
        public virtual void OnEnd()
        {
            EventOnEnd?.Invoke();
        }
    }

    public class FightProcessWait : FightProcessBase
    {
        public float m_lastTime;
        public float m_tickTimer;

        public FightProcessWait(HModeFightManager owner, float lastTime) : base(owner)
        {
            m_lastTime = lastTime;
            m_tickTimer = 0;
        }

        public override void Tick(float dt)
        {
            m_tickTimer += dt;
            if(m_tickTimer > m_lastTime)
            {
                m_isEnd = true;
            }
        }
    }

    public class FightProcessWaitHHint : FightProcessWait
    {
        private string m_hintContent;
        public FightProcessWaitHHint(HModeFightManager owner, float lastTime, string hintContent) : base(owner, lastTime)
        {
            m_hintContent = hintContent;
        }

        public override void OnStart()
        {
            base.OnStart();
            UIPopTip.Inst.Pop(m_hintContent, (PopTipIconType)12);
        }
    }

    public class FightProcessWaitAnimation : FightProcessBase
    {
        public string m_animationName;
        public bool m_isLoop;
        public bool m_loopTime;

        private float m_tickTimer;

        public FightProcessWaitAnimation(HModeFightManager owner, string animationName) : base(owner)
        {
            m_animationName = animationName;
            m_tickTimer = 0;
        }

        public override void OnStart()
        {
            base.OnStart();
            Owner.m_cachedHAnimController.PlayHAnim(m_animationName, OnAnimationEnd);
        }

        public override void Tick(float dt)
        {
            if(!m_isLoop)
            {
                return;
            }
        }

        /// <summary>
        /// 是否播放完成
        /// </summary>
        /// <param name="isFinish"></param>
        public void OnAnimationEnd(bool isFinish)
        {
            m_isEnd = true;
        }
    }

    public class FightProcessImmediate: FightProcessBase
    {
        public FightProcessImmediate(HModeFightManager owner, Action onEnd = null) : base(owner)
        {
            this.EventOnEnd += onEnd;
        }

        public override void OnStart()
        {
            base.OnStart();
        }

        public override void Tick(float dt)
        {
            m_isEnd = true;
        }
    }

    public class FightProcessBalance : FightProcessBase
    {
        public FightProcessBalance(HModeFightManager owner) : base(owner)
        {
        }

        public override void OnStart()
        {
            base.OnStart();
        }

        public override void Tick(float dt)
        {
            m_isEnd = true;
        }
    }


    #endregion

    public class FightHShowController : MonoBehaviour
    {
        public UnityEngine.GameObject m_showImage;
        public Animator m_animator;

        public event Action<bool> EventOnAnimEnd;

        private float m_playTime;
        private float m_timer;


        private bool m_isPlaying;

        public void Init()
        {
            m_showImage = transform.Find("ShowImage").gameObject;
            m_animator = m_showImage.GetComponentInChildren<Animator>();

            m_showImage.SetActive(false);
        }

        public void PlayHAnim(string stateName, Action<bool> onEnd = null)
        {
            if(m_isPlaying)
            {
                // 缓存或暂不处理？
                return;
            }
            EventOnAnimEnd = onEnd;
            m_showImage.SetActive(true);

            PluginMain.Main.LogInfo($"PlayHAnim {stateName}" );
            m_animator.Play(stateName, 0, 0f);
            m_isPlaying = true;
            m_timer = 0;
            m_playTime = 1f;
        }

        public void Update()
        {
            if (!m_isPlaying)
            {
                return;
            }
            m_timer += Time.deltaTime;

            var currStateInfo = m_animator.GetCurrentAnimatorStateInfo(0);
            if (currStateInfo.normalizedTime < 1.0)
            {
                return;
            }
            //if (m_timer > m_playTime)
            {
                EventOnAnimEnd?.Invoke(true);
                EventOnAnimEnd = null;
                m_isPlaying = false;
                m_showImage.SetActive(false);
            }
        }

        public void OnDestroy()
        {
            SecretsSystem.FightManager.m_cachedHAnimController = null;
        }
    }
}
