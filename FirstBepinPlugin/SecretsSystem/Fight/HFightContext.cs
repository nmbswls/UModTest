using FirstBepinPlugin.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstBepinPlugin
{
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
        HFaQingThreshold, // 阈值
        Max,
    }

    public class HModeAttribute
    {
        public long BaseVal;
    }

    public class HFightContext
    {
        public HModeFightManager Owner;

        // 动态数值
        public float Tili;
        public float YiZhuang;
        public float[] Xingfen = new float[(int)EPartType.Max];
        public float YuWang;
        public float KuaiGan;

        public bool IsFaQing;

        public HModeEnemyInfo Enemy = new HModeEnemyInfo();

        // 静态属性
        public HModeAttribute[] StaticAttributeBaseVal = new HModeAttribute[(int)HModeAttributeType.Max];

        public HModeState m_hState = HModeState.Invalid;
        public HModeTiWei m_hTiWei = HModeTiWei.None;

        public bool m_isJueDing;

        public Dictionary<int, GUIPackage.Skill> m_stateSkillCache = new Dictionary<int, GUIPackage.Skill>();
        public List<int> m_nonHSkillCache = new List<int>();

        public void InitContenxt()
        {
            Clear();

            InitPlayerAttribute(HModeAttributeType.HAtk, 0);
            InitPlayerAttribute(HModeAttributeType.HDef, 0);
            InitPlayerAttribute(HModeAttributeType.HMaxClothes, 100);
            InitPlayerAttribute(HModeAttributeType.HMaxTili, 10000);
            InitPlayerAttribute(HModeAttributeType.HMaxKuaiGan, 100);
            InitPlayerAttribute(HModeAttributeType.HMeiLi, 5);

            InitPlayerAttribute(HModeAttributeType.HMaxXingFen_1, 0);
            InitPlayerAttribute(HModeAttributeType.HMaxXingFen_2, 0);
            InitPlayerAttribute(HModeAttributeType.HMaxXingFen_3, 0);
            InitPlayerAttribute(HModeAttributeType.HMaxXingFen_4, 0);

            InitPlayerAttribute(HModeAttributeType.HFaQingThreshold, 20);
        }


        public void Clear()
        {
            Tili = 0;
            YiZhuang = 0;
            YuWang = 0;
            KuaiGan = 0;

            IsFaQing = false;
            foreach (var attr in StaticAttributeBaseVal)
            {
                if (attr == null) continue;
                attr.BaseVal = 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public long GetAttributeValue(HModeAttributeType type)
        {
            long baseVal = 0;
            var attr = StaticAttributeBaseVal[(int)type];
            if (attr == null)
            {
                return 0;
            }
            baseVal = attr.BaseVal;

            long addVal = Owner.Player.GetAttributeBuffBonus(type);
            return (baseVal + addVal);
        }


        #region 取属性

        /// <summary>
        /// 衣装变化
        /// </summary>
        /// <param name="modVal"></param>
        public void ModYiZhuang(float modVal)
        {
            YiZhuang += modVal;
            float maxVal = GetAttributeValue(HModeAttributeType.HMaxClothes) * 1.0f / Consts.Float2Int100;
            if (maxVal < 0)
            {
                maxVal = 0;
            }
            if (YiZhuang < 0)
            {
                YiZhuang = 0;
            }
            if (YiZhuang > maxVal)
            {
                YiZhuang = maxVal;
            }

            Owner.OnPlayerModYiZhuang();
        }

        /// <summary>
        /// 衣装变化
        /// </summary>
        /// <param name="modVal"></param>
        public void ModTiLi(float modVal)
        {
            Tili += (modVal);
            float maxVal = GetAttributeValue(HModeAttributeType.HMaxTili) * 1.0f / Consts.Float2Int100;
            if (maxVal < 1)
            {
                maxVal = 1;
            }
            if (Tili > maxVal)
            {
                Tili = maxVal;
            }
            // tili归零
            //if (Tili <= 0)
            //{
            //    SetIsWuLi(true);
            //}
            //else
            //{
            //    SetIsWuLi(false);
            //}
            Owner.UpdateAllStateBuff();
        }

        /// <summary>
        /// 欲望变化
        /// </summary>
        /// <param name="target"></param>
        /// <param name="addVal"></param>
        public void ModYuWang(KBEngine.Avatar avatar, float modVal)
        {
            if (avatar == Owner.Player)
            {
                YuWang += modVal;
                if (YuWang < 0)
                {
                    YuWang = 0;
                }
            }
            else
            {
                Enemy.YuWang += modVal;
                if (Enemy.YuWang < 0)
                {
                    Enemy.YuWang = 0;
                }
            }

            PluginMain.Main.LogError("?ModYuWang");
            Owner.UpdateAllStateBuff();
        }

        public float GetMaxPartXingFen(int part)
        {
            var partInfo = PluginMain.Main.ConfigDataLoader.GetConfigDataHPartFightInfo(part);
            if (partInfo == null) return 0;
            long modXingFenMax = 0;
            switch (part)
            {
                case 1:
                    {
                        modXingFenMax = GetAttributeValue(HModeAttributeType.HMaxXingFen_1);
                    }
                    break;
                case 2:
                    {
                        modXingFenMax = GetAttributeValue(HModeAttributeType.HMaxXingFen_2);
                    }
                    break;
                case 3:
                    {
                        modXingFenMax = GetAttributeValue(HModeAttributeType.HMaxXingFen_3);
                    }
                    break;
                case 4:
                    {
                        modXingFenMax = GetAttributeValue(HModeAttributeType.HMaxXingFen_4);
                    }
                    break;
            }

            int ret = partInfo.MaxXingFen + (int)(modXingFenMax / Consts.Float2Int100);

            return ret;
        }


        /// <summary>
        /// 兴奋度变化 part=0 表示随机
        /// </summary>
        /// <param name="addVal"></param>
        public void ModXingFen(int part, float modVal)
        {
            int realPart = part;
            if (part == 0)
            {
                List<KeyValuePair<int, int>> weights = new List<KeyValuePair<int, int>>()
                {
                    new KeyValuePair<int, int>(1,3),
                    new KeyValuePair<int, int>(2,3),
                    new KeyValuePair<int, int>(3,2),
                    new KeyValuePair<int, int>(4,3),
                };
                int k = HFightUtils.RandomValueByWeight(weights);
                realPart = k;
            }
            if (realPart == 0)
            {
                return;
            }

            Xingfen[realPart] += modVal;
            float maxVal = GetMaxPartXingFen(realPart);

            if (Xingfen[realPart] < 0)
            {
                Xingfen[realPart] = 0;
            }

            if (Xingfen[realPart] > maxVal)
            {
                Xingfen[realPart] = maxVal;
            }

            StringBuilder sb = new StringBuilder("ModXingFen result:");
            foreach (var val in Xingfen)
            {
                sb.Append(val);
                sb.Append("  ");
            }
            PluginMain.Main.LogInfo(sb.ToString());
            Owner.UpdateAllStateBuff();
        }

        /// <summary>
        /// 累加快感
        /// </summary>
        /// <param name="target"></param>
        /// <param name="addVal"></param>
        public void ModKuaiGan(KBEngine.Avatar target, float modVal)
        {
            if (target == Owner.Player)
            {
                KuaiGan += (long)(modVal * Consts.Float2Int100);
                if (KuaiGan < 0) KuaiGan = 0;
            }
            else
            {
                Enemy.KuaiGan += (long)(modVal * Consts.Float2Int100);
                if (Enemy.KuaiGan < 0) Enemy.KuaiGan = 0;
            }

            Owner.OnModKuaiGan();
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
            Owner.SwitchSkillGroup();
            //m_cachedSkillTab.m_isWuLi = isWuLi;
            //m_cachedSkillTab.RefreshUI();
        }

        
        /// <summary>
        /// 切换体位
        /// </summary>
        /// <param name="addVal"></param>
        public void SwitchTiWei(int tiweiId, bool showEffect = false)
        {
            var preTiWei = m_hTiWei;
            if (preTiWei == (HModeTiWei)tiweiId)
            {
                PluginMain.Main.LogInfo("HModeFightManager SwitchTiWei same.");
                return;
            }
            m_hTiWei = (HModeTiWei)tiweiId;
            // 重置标记位buff
            Owner.OnSwitchTiWei(preTiWei, m_hTiWei);
        }


        #endregion


        #region 内部方法

        /// <summary>
        /// 初始化属性值
        /// </summary>
        /// <param name="attr"></param>
        /// <param name="value"></param>
        protected void InitPlayerAttribute(HModeAttributeType attrType, float value)
        {
            var attr = new HModeAttribute();
            attr.BaseVal = (long)(value * Consts.Float2Int100);
            StaticAttributeBaseVal[(int)attrType] = attr;
        }

        

        #endregion
    }

}
