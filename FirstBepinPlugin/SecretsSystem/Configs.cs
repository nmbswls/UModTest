using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WXB.TextParser;

namespace FirstBepinPlugin.Config
{

    public enum EnumPartType
    {
        Invalid,
        Mouse,
        Breast,
        Pussy,
        Anal,
        Max,
    }

    #region 静态配置

    public enum HModeState
    {
        Invalid = 0,
        Normal = 1,
        JueDing = 2,
    }

    public enum HModeTiWei
    {
        None,
        Shou,
        Ru,
        Kou,
        Gang,
        Xue,
        Max,
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
        public static int GetJingLiangByJingjie(int jingjie)
        {
            if (jingjie < 0) return 0;
            if (jingjie >= s_Jingjie2JingLiang.Length)
            {
                return s_Jingjie2JingLiang[s_Jingjie2JingLiang.Length - 1];
            }
            return s_Jingjie2JingLiang[jingjie];
        }

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

        public static List<KeyValuePair<int, int>> m_XingFen2KuaiGanRate = new List<KeyValuePair<int, int>>()
        {
            new KeyValuePair<int, int>(0,100),
            new KeyValuePair<int, int>(20,120),
            new KeyValuePair<int, int>(40,140),
            new KeyValuePair<int, int>(80,180),
            new KeyValuePair<int, int>(100,250),
        };

        public static int GetKuaiGanRateByXingFen(int xingFen)
        {
            if (m_XingFen2KuaiGanRate.Count == 0) return 0;
            int ret = m_XingFen2KuaiGanRate[0].Value;
            for (int i = 0; i < m_XingFen2KuaiGanRate.Count; i++)
            {
                if (xingFen < m_XingFen2KuaiGanRate[i].Key)
                {
                    break;
                }
                ret = m_XingFen2KuaiGanRate[i].Value;
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
                TargetTiWei = (int)HModeTiWei.Shou,
            },
            new ConfigTiWeiSwitchInfo()
            {
                Id = 2,
                NeedYiZhuang = new List<int>(),
                NeedYuWang = new List<int>(),
                TargetTiWei = (int)HModeTiWei.Ru,
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

    public partial class ConfigDataHAttackPoolInfo
    {

        public List<ConfigDataHAttackInfo> AttackConfList = new List<ConfigDataHAttackInfo>(); // 攻击Id列表

    }


    public partial class ConfigDataHAttackInfo
    {

        public List<ConfigDataHAttackConditionInfo> ConditionConfList = new List<ConfigDataHAttackConditionInfo>(); // 攻击Id列表

    }


    public partial class ConfigDataHAttackPoolInfo
    {

        public Int32 ID; // 池ID

        public List<int> AttackIdList = new List<int>(); // 攻击Id列表

    }


    public partial class ConfigDataHAttackInfo
    {

        public Int32 ID; // 主键

        public String Name; // 名称

        public List<int> TargetPart = new List<int>(); // 部位 1口 2乳 3穴 4尻

        public Int32 AttackType; // 攻击类型 1 轻 2 重 3 淫

        public List<int> Conditions = new List<int>(); // 使用条件列表

        public Int32 DamageRate; // 伤害率

        public Int32 KuaiGanRate; // 快感率

        public List<int> Tags = new List<int>(); // Tags

        public Int32 DefaultWeight; // 基础权重

        public String HintContent; // 提示语

        public String AnimName; // 动画名

    }


    public partial class ConfigDataHAttackConditionInfo
    {

        public Int32 ID; // 主键

        public Int32 Type; // 条件类型 1 衣装 2 自身欲望 3 主角欲望

        public List<int> Params = new List<int>(); // 参数列表

    }


    public partial class ConfigDataHPartFightInfo
    {

        public Int32 ID; // 主键

        public String Name; // 名称

        public Int32 Armar; // 护甲

        public Int32 HitKuaiGan; // 受击快感

        public Int32 CounterKuaiGan; // 反馈快感

        public Int32 MaxXingFen; // 兴奋上限

        public Int32 MinXingFen; // 兴奋下限

        public Int32 SuoJing; // 基础索精

    }

    public class ConfigDataLoader
    {

        public void LoadConfig(string pathPrefix)
        {
            try
            {
                {
                    var hInfoConfigName = pathPrefix + "HAttackPoolInfo.json";
                    if (!File.Exists(hInfoConfigName))
                    {
                        PluginMain.Main.LogError($"load HAttackPoolInfo Error");
                        return;
                    }
                    var json = File.ReadAllText(hInfoConfigName);
                    m_ConfigDataHAttackPoolInfoDict = JsonConvert.DeserializeObject<Dictionary<int, ConfigDataHAttackPoolInfo>>(json);
                }
                {
                    var hInfoConfigName = pathPrefix + "HAttackInfo.json";
                    if (!File.Exists(hInfoConfigName))
                    {
                        PluginMain.Main.LogError($"load HAttackInfo Error");
                        return;
                    }
                    var json = File.ReadAllText(hInfoConfigName);
                    m_ConfigDataHAttackInfoDict = JsonConvert.DeserializeObject<Dictionary<int, ConfigDataHAttackInfo>>(json);
                }
                {
                    var hInfoConfigName = pathPrefix + "HAttackConditionInfo.json";
                    if (!File.Exists(hInfoConfigName))
                    {
                        PluginMain.Main.LogError($"load HAttackConditionInfo Error");
                        return;
                    }
                    var json = File.ReadAllText(hInfoConfigName);
                    m_ConfigDataHAttackConditionInfoDict = JsonConvert.DeserializeObject<Dictionary<int, ConfigDataHAttackConditionInfo>>(json);
                }
                {
                    var hInfoConfigName = pathPrefix + "HPartFightInfo.json";
                    if (!File.Exists(hInfoConfigName))
                    {
                        PluginMain.Main.LogError($"load ConfigDataHPartFightInfo Error");
                        return;
                    }
                    var json = File.ReadAllText(hInfoConfigName);
                    m_ConfigDataHPartFightInfoDict = JsonConvert.DeserializeObject<Dictionary<int, ConfigDataHPartFightInfo>>(json);
                }
            }
            catch (Exception e)
            {
                PluginMain.Main.LogError($"load config Error");
            }
        }


        public Dictionary<int, ConfigDataHAttackPoolInfo> m_ConfigDataHAttackPoolInfoDict = new Dictionary<int, ConfigDataHAttackPoolInfo>();

        public Dictionary<int, ConfigDataHAttackInfo> m_ConfigDataHAttackInfoDict = new Dictionary<int, ConfigDataHAttackInfo>();

        public Dictionary<int, ConfigDataHAttackConditionInfo> m_ConfigDataHAttackConditionInfoDict = new Dictionary<int, ConfigDataHAttackConditionInfo>();

        public Dictionary<int, ConfigDataHPartFightInfo> m_ConfigDataHPartFightInfoDict = new Dictionary<int, ConfigDataHPartFightInfo>();

        public ConfigDataHAttackPoolInfo GetConfigDataHAttackPoolInfo(int key)
        {
            if(m_ConfigDataHAttackPoolInfoDict.TryGetValue(key, out var ret))
            {
                return ret;
            }
            return null;
        }

        public ConfigDataHAttackInfo GetConfigDataHAttackInfo(int key)
        {
            if (m_ConfigDataHAttackInfoDict.TryGetValue(key, out var ret))
            {
                return ret;
            }
            return null;
        }

        public ConfigDataHAttackConditionInfo GetConfigDataHAttackConditionInfo(int key)
        {
            if (m_ConfigDataHAttackConditionInfoDict.TryGetValue(key, out var ret))
            {
                return ret;
            }
            return null;
        }

        public ConfigDataHPartFightInfo GetConfigDataHPartFightInfo(int key)
        {
            if (m_ConfigDataHPartFightInfoDict.TryGetValue(key, out var ret))
            {
                return ret;
            }
            return null;
        }

    }
}
