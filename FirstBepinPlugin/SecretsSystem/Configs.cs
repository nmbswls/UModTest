using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstBepinPlugin.Config
{

    public enum EnumPartType
    {
        Invalid,
        Core,
        Mouse,
        Breast,
        Anal,
        Max,
    }

    #region 静态配置

    public enum HModeState
    {
        Invalid = 0,
        Normal = 1,
        Wuli = 2,
        GaoChao = 3,
        TiWeiStart = 10,
        Shou = 11,
        Ru = 12,
        Kou = 13,
        Gang = 14,
        Xue = 15,
        Max = 16,
    }

    public class ConfigTiWeiSwitchInfo
    {
        public int Id; // id
        public List<int> NeedYiZhuang;
        public List<int> NeedYuWang;
        public int TargetTiWei;
    }
    

    public static class StaticConfigContainer
    {
        public static int[] s_Jingjie2HAtk = new int[] { 5, 8, 14, 28, 50, 50, 50};

        public static int GetHAtkByJingjie(int jingjie)
        {
            if (jingjie < 0) return 0;
            if (jingjie >= s_Jingjie2HAtk.Length)
            {
                return s_Jingjie2HAtk[s_Jingjie2HAtk.Length - 1];
            }
            return s_Jingjie2HAtk[jingjie];
        }

        public static int[] s_Jingjie2HActionTimes = new int[] { 3,4,5,6,6,6 };
        public static int GetHActionTimesByJingjie(int jingjie)
        {
            if (jingjie < 0) return 0;
            if(jingjie >= s_Jingjie2HActionTimes.Length)
            {
                return s_Jingjie2HActionTimes[s_Jingjie2HActionTimes.Length - 1];
            }
            return s_Jingjie2HActionTimes[jingjie];
        }

        public static int[] s_Jingjie2JingLiang = new int[] { 10, 20, 60, 150, 300, 300, 300 };

        public static int[] s_DefaultPrefer = new int[] { 2,2,2,4 };

        public static List<KeyValuePair<int, int>> m_YuWang2TurnXingFen = new List<KeyValuePair<int, int>>()
        {
            new KeyValuePair<int, int>(0,1),
            new KeyValuePair<int, int>(20,2),
            new KeyValuePair<int, int>(40,4),
            new KeyValuePair<int, int>(80,8),
            new KeyValuePair<int, int>(100,12),
        };
        public static int GetTurnXingFenByYuWang(int yuWang)
        {
            if (m_YuWang2TurnXingFen.Count == 0) return 0;
            int ret = m_YuWang2TurnXingFen[0].Value;
            for(int i= 0;i < m_YuWang2TurnXingFen.Count; i++)
            {
                if(yuWang < m_YuWang2TurnXingFen[i].Key)
                {
                    break;
                }
                ret = m_YuWang2TurnXingFen[i].Value;
            }
            return ret;
        }

        public static List<ConfigTiWeiSwitchInfo> s_ConfigTiWeiSwitchDict = new List<ConfigTiWeiSwitchInfo>()
        {
            new ConfigTiWeiSwitchInfo()
            {
                Id = 1,
                NeedYiZhuang = new List<int>(),
                NeedYuWang = new List<int>(),
                TargetTiWei = (int)HModeState.Shou,
            },
            new ConfigTiWeiSwitchInfo()
            {
                Id = 2,
                NeedYiZhuang = new List<int>(),
                NeedYuWang = new List<int>(),
                TargetTiWei = (int)HModeState.Ru,
            },
        };
    }
    

    #endregion


    /// <summary>
    /// 配置总入口
    /// </summary>
    public class SecretsConfig
    {
        public int EntryTalentId;

        public List<string> PartJingjieNameList = new List<string>();

        public Dictionary<int, ConfigDataSecretsPartInfo> PartInfo;
    }

    public partial class ConfigDataSecretsPartInfo
    {
        public EnumPartType PartType;

        public List<ConfigDataSecretsPartLevelInfo> LevelInfos = new List<ConfigDataSecretsPartLevelInfo>();

        public ConfigDataSecretsPartLevelInfo LevelInfoGet(int level)
        {
            if(level > LevelInfos.Count)
            {
                return LevelInfos[LevelInfos.Count - 1];
            }
            return LevelInfos[level -1];
        }
    }

    /// <summary>
    /// 部位通用等级信息
    /// </summary>
    public class ConfigDataSecretsPartLevelInfo
    {
        /// <summary>
        /// 存储上限
        /// </summary>
        public int JingStorage;

        /// <summary>
        /// 每日转化速度
        /// </summary>
        public int JingAbsorbSpeed;

        /// <summary>
        /// 转化效率 单位百分比
        /// </summary>
        public int JingAbsorbRate;
    }

    public class ConfigDataHAttackShowInfo
    {

        public Int32 ID; // 主键

        public String Name; // 名称

        public Int32 TargetPart; // 部位 1口 2乳 3穴 4鬼

        public Int32 AttackType; // 攻击类型 1 轻 2 重

        public Int32 RaceType; // 种族（1人，2妖 3魔 4鬼）

        public Int32 SexType; // 性别要求

        public Int32 DefaultWeight; // 基础权重

        public String HintContent; // 提示语

        public String AnimName; // 动画名

    }
}
