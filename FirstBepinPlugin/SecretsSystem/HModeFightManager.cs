using KBEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using YSGame.Fight;

namespace FirstBepinPlugin
{
    public class HModeFightManager
    {
        public enum HModeState
        {
            Invalid,
            Normal,
            Max,
        }

        // HMode属性类型
        public enum HModeAttributeType
        {
            Invalid,
            HAtk,
            HDef,
            HMaxClothes,
            HMaxTili,
        }

        public class HFightCtx
        {
            public bool m_isInHMode;

            // 动态数值
            public float Tili;
            public float NaiLi;
            public float YiZhuang;
            public float YuWang;
            public float KuaiGan;
            public float HXingfen;

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

        public KBEngine.Avatar m_player;
        public HFightCtx m_ctx = new HFightCtx();

        /// <summary>
        /// 初始化
        /// </summary>
        public void FightHInit()
        {
            m_ctx.Clear();
            m_player = Tools.instance.getPlayer();

            m_ctx.StaticAttributeBaseVal.Add(HModeAttributeType.HAtk, 0);
            m_ctx.StaticAttributeBaseVal.Add(HModeAttributeType.HDef, 0);
            m_ctx.StaticAttributeBaseVal.Add(HModeAttributeType.HMaxClothes, 100);
            m_ctx.StaticAttributeBaseVal.Add(HModeAttributeType.HMaxTili, 100);
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
            m_ctx.Tili = 100;
            m_ctx.NaiLi = 3;
            m_ctx.KuaiGan = 0;
            m_ctx.YuWang = 0;
            m_ctx.YiZhuang = 100;

            UpdateAllStateBuff();

            //    player.OtherAvatar.spell.addBuff(9971009, 1);

            // 赋予H技能
            // 清空技能
            player.FightClearSkill(0, 10);
            var skillList = HSkillListGetByState();
            
            // 赋予技能
            foreach (var skillId in skillList)
            {
                var skillItem = player.skill.Find(delegate (GUIPackage.Skill s) { return s.skill_ID == skillId; });
                if (skillItem == null)
                {
                    skillItem = new GUIPackage.Skill(skillId, 0, 10);
                }
                player.skill.Add(skillItem);
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

            m_ctx.m_isInHMode = true;

            for(int i=0;i<6;i++)
            {
                RoundManager.instance.DrawCard(m_player, (int)LingQiType.魔);
            }
            // 基础淫比重 不高 也就50%
            SetBuffLayer(player, Consts.BuffId_BasicYinLingen, 50); 
        }

        /// <summary>
        /// 使用ctx数据更新显示用buff列表
        /// </summary>
        public void UpdateAllStateBuff()
        {
            var player = Tools.instance.getPlayer();

            SetBuffLayer(player, Consts.BuffId_YinTili, UnityEngine.Mathf.CeilToInt(m_ctx.Tili));
            SetBuffLayer(player, Consts.BuffId_YinNaili, UnityEngine.Mathf.CeilToInt(m_ctx.NaiLi));
            SetBuffLayer(player, Consts.BuffId_YinKuaiGan, UnityEngine.Mathf.CeilToInt(m_ctx.KuaiGan));
            SetBuffLayer(player, Consts.BuffId_YinYuWang, UnityEngine.Mathf.CeilToInt(m_ctx.YuWang));
            SetBuffLayer(player, Consts.BuffId_YinYiZhuang, UnityEngine.Mathf.CeilToInt(m_ctx.YiZhuang));
        }

        /// <summary>
        /// 获取各形态技能列表
        /// </summary>
        /// <returns></returns>
        public List<int> HSkillListGetByState()
        {
            m_skillIdCache.Clear();
            // skil list
            switch (m_ctx.m_hState)
            {
                case HModeState.Normal: // 原始形态
                    {
                        m_skillIdCache.Add(997101);
                        m_skillIdCache.Add(997201);
                        m_skillIdCache.Add(997211);
                        m_skillIdCache.Add(997221);
                    }
                    break;
            }
            return m_skillIdCache;
        }
        private List<int> m_skillIdCache = new List<int>();

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
        }



        #region 取属性

        /// <summary>
        /// 衣装变化
        /// </summary>
        /// <param name="addVal"></param>
        public void ModYiZhuang(long addVal)
        {
            m_ctx.YiZhuang += addVal;
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

                    addVal += (long)configInfo["value1"].n * buffLayer;
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

        #endregion

        /// <summary>
        /// 检查是否能进入HMode
        /// </summary>
        /// <returns></returns>
        public bool CheckFightEnterHMode(KBEngine.Avatar avatar)
        {
            PluginMain.Main.LogInfo("CheckFightEnterHMode");
            var player = Tools.instance.getPlayer();
            int buffNum = avatar.buffmag.GetBuffSum(Consts.BuffId_YinYuWang);

            if (buffNum <= 20)
            {
                PluginMain.Main.LogInfo("层数不足 不进入Hzhan");
                return false;
            }

            int rate10000 = (buffNum - 20) * 80;
            if (rate10000 >= 10000)
            {
                return true;
            }
            int randVal = UnityEngine.Random.Range(0, 10000);
            if (randVal < rate10000)
            {
                return true;
            }
            else
            {
                return true;
            }
        }

        //public void FightHInit(KBEngine.Avatar player)
        //{
        //    m_ctx.Clear();


        //}

        /// <summary>
        /// 指定avatar 设置buff层数
        /// </summary>
        /// <param name="target"></param>
        /// <param name="buffId"></param>
        /// <param name="newLayer"></param>
        public void SetBuffLayer(KBEngine.Avatar target, int buffId, int newLayer)
        {
            List<List<int>> buffByID = target.buffmag.getBuffByID(buffId);
            if (buffByID.Count > 0)
            {
                buffByID[0][1] = newLayer;
            }
            else
            {
                target.spell.addDBuff(buffId, newLayer);
            }
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

    }
}
