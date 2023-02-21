using BehaviorDesigner.Runtime;
using DebuggingEssentials;
using FirstBepinPlugin.Patch;
using GUIPackage;
using JSONClass;
using KBEngine;
using Newtonsoft.Json;
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

        public Text m_textPose;
        public int m_currSelectIdx = -1;
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
                listener.EventOnPointerEnter += delegate ()
                {
                    OnTogglePointerExit(tabIdx);
                };
                tab.onValueChanged.AddListener(delegate(bool isOn) {
                    OnToggleValueChange(tabIdx);
                });

                var textComp = tab.GetComponentInChildren<Text>();
                textComp.font = PluginMain.Main.font_YaHei;
            }
            m_textPose = m_toggleList[1].GetComponentInChildren<Text>();

            RefreshUI();
        }

        public void RefreshUI()
        {
            m_toggleList[0].gameObject.SetActive(true);
            m_toggleList[1].gameObject.SetActive(true);
            m_toggleList[2].gameObject.SetActive(true);

            if(Owner.m_ctx.m_hState == HModeFightManager.HModeState.Normal)
            {
                m_textPose.text = "正常";
            }
            else if (Owner.m_ctx.m_hState == HModeFightManager.HModeState.Wuli)
            {
                m_textPose.text = "虚脱";
            }

            if (m_currSelectIdx == -1)
            {
                m_toggleList[0].isOn = true;
            }
            else
            {
                m_toggleList[m_currSelectIdx].isOn = true;
            }
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
                skillGroupId = (int)HModeFightManager.HSkillGroupType.Common;
            }
            else if (m_currSelectIdx == 1)
            {
                skillGroupId = SecretsSystem.FightManager.GetCurrPoseSkillGroupId();
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

    public class HModeActor
    {
        public int Id;
        public long YuWang;
        public long KuaiGan;
        public long NaiLi;

        public KBEngine.Avatar BindAvatar;
    }

    public class HFightCtx
    {

        public bool m_isInHMode;

        // 动态数值
        public long Tili;
        public long YiZhuang;
        public long MeiLi;
        public long Xingfen;


        public HModeActor Self = new HModeActor();
        public HModeActor Enemy = new HModeActor();

        public bool IsEnemyFaQing = false;
        public int EnemyXingge = 0;
        public int EnemySexType = 1;

        // 静态属性
        public Dictionary<HModeAttributeType, long> StaticAttributeBaseVal = new Dictionary<HModeAttributeType, long>();

        public HModeState m_hState = HModeState.Invalid;

        public Dictionary<int, GUIPackage.Skill> m_stateSkillCache = new Dictionary<int, GUIPackage.Skill>();
        public List<int> m_nonHSkillCache = new List<int>();

        public void Clear()
        {
            m_isInHMode = false;
            StaticAttributeBaseVal.Clear();
        }
    }

    public class HModeFightManager
    {
        public enum HModeState
        {
            Invalid,
            Normal,
            Wuli,
            Shou,
            Max,
        }

        public enum HSkillGroupType
        {
            Invalid,
            Common,
            Normal,
            Function,
            Wuli,
            JueDing,
            Shou,
        }

        // HMode属性类型
        public enum HModeAttributeType
        {
            Invalid,
            HAtk,
            HDef,
            HMaxClothes,
            HMaxTili,
            HMaxXingFen,
            HMaxKuaiGan, // 受耐力影响 达到后gc
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
            InitAttribute(HModeAttributeType.HMaxXingFen, 100);
            InitAttribute(HModeAttributeType.HMaxKuaiGan, 100);

            JSONObject jSONObject = jsonData.instance.AvatarJsonData[string.Concat(Tools.instance.MonstarID)];
            if(jSONObject.HasField("XingGe"))
            {
                m_ctx.EnemyXingge = jSONObject.GetField("XingGe").I;
            }
            if (jSONObject.HasField("SexType"))
            {
                m_ctx.EnemySexType = jSONObject.GetField("SexType").I;
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
                PluginMain.Main.LogInfo("firstProcess " + firstProcess.GetType());
                // 持续时间非0 表示通过倒计时结束
                if (firstProcess.m_isEnd)
                {
                    firstProcess.OnEnd();
                    m_runningProcessList.Dequeue();
                }
            }
        }

        public void FightOnRoungStart(KBEngine.Avatar avatar)
        {
            // 玩家回合开始
            if(avatar == m_player)
            {
                CheckKuaiGanReachMax(m_ctx.Self);
            }
            else
            {
                if (CheckEnemyFaQing())
                {
                    ApplyEnemyFaQing();
                }
            }
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

            m_ctx.Self.NaiLi = 3;
            m_ctx.Self.KuaiGan = 0;
            m_ctx.Self.YuWang = 0;

            m_ctx.Enemy.NaiLi = 1;
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

            // 初始魅力
            {
                m_ctx.MeiLi += 2000;
            }

            SetBuffLayer(player, Consts.BuffId_TurnModYuWang, 10);
        }

        /// <summary>
        /// 检查快感满
        /// </summary>
        /// <param name="target"></param>
        protected void CheckKuaiGanReachMax(HModeActor target)
        {
            long maxKuaiGan;
            if (target == m_ctx.Self)
            {
                maxKuaiGan = GetAttributeValue(HModeAttributeType.HMaxKuaiGan);
            }
            else
            {
                maxKuaiGan = 10000;
            }

            // 没达到满 无事发生
            if (target.KuaiGan < maxKuaiGan)
            {
                return;
            }

            // 播放特效 扣减耐力
            target.KuaiGan = 0;
            target.NaiLi -= 1;

            if (target.NaiLi <= 0)
            {
                CheckHFightEnd();
            }
        }

        /// <summary>
        /// 计算发情状态下敌人的h技能
        /// </summary>
        public void ApplyEnemyHSkill()
        {
            if(!m_ctx.m_isInHMode)
            {
                return;
            }
            // 只有发情时才结算
            if (!m_ctx.IsEnemyFaQing)
            {
                return;
            }

            int actTimes = 3; // 计算性行动数 仅和境界相关？
            int hAck = 5;
            // 获取敌人配置
            for (int i=0;i< actTimes; i++)
            {
                int randTarget = UnityEngine.Random.Range(1, 5);
                
                // 男性 更倾向于
                bool isEvil = false;
                if (NpcXingGeDate.DataDict.ContainsKey(m_ctx.EnemyXingge))
                {
                    isEvil = NpcXingGeDate.DataDict[m_ctx.EnemyXingge].zhengxie == 2;
                }
                int attackType = 1;
                if(isEvil)
                {
                    attackType = 2;
                }

                List<KeyValuePair<int, int>> weights = new List<KeyValuePair<int, int>>();
                foreach (var pair in PluginMain.Main.ConfigDataHAttackShowInfo)
                {
                    if (pair.Value.AttackType != attackType)
                    {
                        continue;
                    }
                    if(pair.Value.SexType != 0 && pair.Value.SexType != m_ctx.EnemySexType)
                    {
                        continue;
                    }
                    if(pair.Value.TargetPart != randTarget)
                    {
                        continue;
                    }
                    weights.Add(new KeyValuePair<int,int>(pair.Key, pair.Value.DefaultWeight));
                }
                if(weights.Count == 0)
                {
                    continue;
                }
                int showId = weights.First().Key;
                ShowHAnim($"Animation/h/{showId}.anim");
                var hContent = PluginMain.Main.ConfigDataHAttackShowInfo[showId].HintContent;
                ShowHHint(hContent);
            }
        }


        /// <summary>
        /// 使用ctx数据更新显示用buff列表
        /// </summary>
        public void UpdateAllStateBuff()
        {
            SetBuffLayer(m_player, Consts.BuffId_YinTili, (int)(m_ctx.Tili/Consts.Float2Int100));
            SetBuffLayer(m_player, Consts.BuffId_YinYiZhuang, (int)(m_ctx.YiZhuang/ Consts.Float2Int100));

            SetBuffLayer(m_player, Consts.BuffId_YinNaili, (int)(m_ctx.Self.NaiLi));
            SetBuffLayer(m_player, Consts.BuffId_YinKuaiGan, (int)(m_ctx.Self.KuaiGan / Consts.Float2Int100));
            SetBuffLayer(m_player, Consts.BuffId_YinYuWang, (int)(m_ctx.Self.YuWang / Consts.Float2Int100));

            SetBuffLayer(m_player.OtherAvatar, Consts.BuffId_YinNaili, (int)(m_ctx.Enemy.NaiLi));
            SetBuffLayer(m_player.OtherAvatar, Consts.BuffId_YinKuaiGan, (int)(m_ctx.Enemy.KuaiGan / Consts.Float2Int100));
            SetBuffLayer(m_player.OtherAvatar, Consts.BuffId_YinYuWang, (int)(m_ctx.Enemy.YuWang / Consts.Float2Int100));
        }

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
                case (int)HSkillGroupType.Normal: // 普通技能
                    {
                        m_skillIdCache.Add(99723);
                    }
                    break;
                case (int)HSkillGroupType.Function: // 功能技能
                    {
                        m_skillIdCache.Add(99799);
                    }
                    break;
                case (int)HSkillGroupType.Shou: // 普通技能
                    {
                        m_skillIdCache.Add(99730);
                    }
                    break;
            }
            return m_skillIdCache;
        }

        /// <summary>
        /// 获取当前pose 技能组id
        /// </summary>
        public int GetCurrPoseSkillGroupId()
        {
            switch(m_ctx.m_hState)
            {
                case HModeState.Normal:
                    return (int)HSkillGroupType.Normal;
                case HModeState.Shou:
                    return (int)HSkillGroupType.Shou;
                case HModeState.Wuli:
                    return (int)HSkillGroupType.Wuli;
            }
            return -1;
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
        /// 欲望变化
        /// </summary>
        /// <param name="target"></param>
        /// <param name="addVal"></param>
        public void ModYuWang(int target, float modVal)
        {
            HModeActor actor;
            if(target == 1)
            {
                actor = m_ctx.Self;
            }
            else
            {
                actor = m_ctx.Enemy;
            }
            actor.YuWang += (long)(modVal * Consts.Float2Int100);
            
            if (actor.YuWang < 0)
            {
                actor.YuWang = 0;
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
        /// 兴奋度变化
        /// </summary>
        /// <param name="addVal"></param>
        public void ModXingFen(float modVal)
        {
            m_ctx.Xingfen += (long)(modVal * Consts.Float2Int100);
            long maxVal = GetAttributeValue(HModeAttributeType.HMaxXingFen);

            if (m_ctx.Xingfen < 0)
            {
                m_ctx.Xingfen = 0;
            }

            if (m_ctx.Xingfen > maxVal)
            {
                m_ctx.Xingfen = maxVal;
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

            HModeActor actor;
            if (target == 1)
            {
                actor = m_ctx.Self;
            }
            else
            {
                actor = m_ctx.Enemy;
            }

            actor.KuaiGan += (long)(modVal * Consts.Float2Int100);

            if (actor.KuaiGan < 0)
            {
                actor.KuaiGan = 0;
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
            // 优先自己死
            if(m_ctx.Self.NaiLi <= 0)
            {
                m_player.setHP(-99999);
            }
            
            if(m_ctx.Enemy.NaiLi <= 0)
            {
                m_player.OtherAvatar.setHP(-99999);
            }
        }


        /// <summary>
        /// 切换体位
        /// </summary>
        /// <param name="addVal"></param>
        public void SwitchTiWei(int tiweiId)
        {
            var preState = m_ctx.m_hState;
            if(preState == (HModeState)tiweiId)
            {
                PluginMain.Main.LogInfo("HModeFightManager SwitchTiWei same.");
                return;
            }
            m_ctx.m_hState = (HModeState)tiweiId;
            // 重置标记位buff
            ResetTiWeiBuff(preState, m_ctx.m_hState);

            m_cachedSkillTab.RefreshUI();
        }

        /// <summary>
        /// 切换buff
        /// </summary>
        protected void ResetTiWeiBuff(HModeState preState, HModeState nowState)
        {
            int oldBuffId = FlagBuffIdGetByState(preState);
            int newBuffId = FlagBuffIdGetByState(nowState);

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
        public static int FlagBuffIdGetByState(HModeState state)
        {
            switch(state)
            {
                case HModeState.Normal:
                    return Consts.BuffId_FlagNormal;
                case HModeState.Wuli:
                    return Consts.BuffId_FlagWuLi;
                case HModeState.Shou:
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

            m_ctx.StaticAttributeBaseVal.TryGetValue(type, out baseVal);
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
            return baseVal + addVal;
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
            }
            return 0;
        }

        /// <summary>
        /// 初始化属性值
        /// </summary>
        /// <param name="attr"></param>
        /// <param name="value"></param>
        protected void InitAttribute(HModeAttributeType attr, float value)
        {
            if(m_ctx.StaticAttributeBaseVal.ContainsKey(attr))
            {
                m_ctx.StaticAttributeBaseVal[attr] = (long)(value * Consts.Float2Int100);
            }
            else
            {
                m_ctx.StaticAttributeBaseVal.Add(attr, (long)(value * Consts.Float2Int100));
            }
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
            m_ctx.IsEnemyFaQing = true;
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



        //public void FightEnterHMode()
        //{
        //    PluginMain.Main.LogInfo("try enter h mode.");

        //    if (m_ctx == null || m_ctx.m_isInHMode)
        //    {
        //        return;
        //    }
        //    var player = Tools.instance.getPlayer();
        //    m_ctx.m_isInHMode = true;

        //    m_ctx.m_hState = "";

        //    var skillList = new List<int>();
        //    // skil list
        //    switch (m_ctx.m_hState)
        //    {
        //        case "": // 原始形态
        //            {
        //                skillList.Add(997201);
        //                skillList.Add(997211);
        //                skillList.Add(997221);
        //            }
        //            break;
        //        case "WuLi":
        //            {
        //                skillList.Add(997231);
        //                skillList.Add(997241);
        //                skillList.Add(997251);
        //            }
        //            break;
        //        case "GaoChao":
        //            {
        //                skillList.Add(997261);
        //                skillList.Add(997271);
        //                skillList.Add(997281);
        //            }
        //            break;
        //    }

        //    // 清空技能
        //    player.FightClearSkill(0, 10);

        //    // 赋予技能
        //    foreach (var skillId in skillList)
        //    {
        //        var skillItem = player.skill.Find(delegate (GUIPackage.Skill s) { return s.skill_ID == skillId; });
        //        if (skillItem == null)
        //        {
        //            skillItem = new GUIPackage.Skill(skillId, 0, 10);
        //        }
        //        player.skill.Add(skillItem);
        //        int num = 0;
        //        foreach (UIFightSkillItem fightSkill in UIFightPanel.Inst.FightSkills)
        //        {
        //            if (num >= 0 && num < 10 && !fightSkill.HasSkill)
        //            {
        //                fightSkill.SetSkill(skillItem);
        //                break;
        //            }
        //            num++;
        //        }
        //    }
        //}


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


        /// <summary>
        /// 回调
        /// </summary>
        public void ShowHAnim(string animState)
        {
            var newProcess = new FightProcessWaitAnimation(this, animState);
            m_runningProcessList.Enqueue(newProcess);
        }

        /// <summary>
        /// 显示h信息
        /// </summary>
        public void ShowHHint(string hHintContent)
        {
            m_runningProcessList.Enqueue(new FightProcessApplyEffect(this, 500, 200));
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

    #region 表现

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

    public class FightProcessApplyEffect : FightProcessBase
    {
        public int m_damageSelf;
        public int m_damageEnemy;

        public FightProcessApplyEffect(HModeFightManager owner, int damageSelf, int damageEnemy) : base(owner)
        {
            m_damageSelf = damageSelf;
            m_damageEnemy = damageEnemy;
        }

        public override void OnStart()
        {
            base.OnStart();
            Owner.ModKuaiGan(1, m_damageSelf);
            Owner.ModKuaiGan(2, m_damageEnemy);
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
