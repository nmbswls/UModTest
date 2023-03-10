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

    public enum EPartType
    {
        Invalid,
        Mouse,
        Breast,
        Pussy,
        Anal,
        Body,
        Max,
    }

    public enum EConditionType
    {
        Invalid,
        YiZhuang,
        SelfYuWang,
        EnemyYuWang,
        TiWei,
        IsJueDing,
        Max,
    }

    public enum EConditionCompareType
    {
        Equal,
        Gte,
        Gt,
        Lte,
        Le,
        NotEqual,
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
        Xue,
        Gang,
        Max,
    }

    public class ConfigTiWeiSwitchInfo
    {
        public int Id; // id
        public List<Tuple4> Conditions = new List<Tuple4>();
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

        public static int[] s_YizhuangLevelThreshold = new int[] { 1, 30, 60, 100 };

        public static int GetYiZhuangLevel(float yizhuang)
        {
            if (s_YizhuangLevelThreshold.Length == 0) return 0;
            for(int i=0 ; i < s_YizhuangLevelThreshold.Length;i++)
            {
                if(yizhuang < s_YizhuangLevelThreshold[i])
                {
                    return i;
                }
            }
            return s_YizhuangLevelThreshold.Length - 1;
        }

        public static int[] s_MeiLiLevelThreshold = new int[] { 25, 50, 75, 100, 9999};

        public static int GetMeiLiLevel(float meili)
        {
            if (s_MeiLiLevelThreshold.Length == 0) return 0;
            for (int i = 0; i < s_MeiLiLevelThreshold.Length; i++)
            {
                if (meili < s_MeiLiLevelThreshold[i])
                {
                    return i;
                }
            }
            return s_MeiLiLevelThreshold.Length - 1;
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
                Id = 0,
                Conditions = new List<Tuple4>(){ new Tuple4() { P1 = 1, P2 = 3, P3 = 3}},
                TargetTiWei = (int)HModeTiWei.Shou,
            },
            new ConfigTiWeiSwitchInfo()
            {
                Id = 1,
                Conditions = new List<Tuple4>(),
                TargetTiWei = (int)HModeTiWei.Ru,
            },
            new ConfigTiWeiSwitchInfo()
            {
                Id = 2,
                Conditions = new List<Tuple4>(),
                TargetTiWei = (int)HModeTiWei.Kou,
            },
            new ConfigTiWeiSwitchInfo()
            {
                Id = 3,
                Conditions = new List<Tuple4>(),
                TargetTiWei = (int)HModeTiWei.Gang,
            },
            new ConfigTiWeiSwitchInfo()
            {
                Id = 4,
                Conditions = new List<Tuple4>(),
                TargetTiWei = (int)HModeTiWei.Xue,
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
        public EPartType PartType;

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
                    m_ConfigDataHAttackPoolInfoData = JsonConvert.DeserializeObject<Dictionary<int, ConfigDataHAttackPoolInfo>>(json);
                }
                {
                    var hInfoConfigName = pathPrefix + "HAttackInfo.json";
                    if (!File.Exists(hInfoConfigName))
                    {
                        PluginMain.Main.LogError($"load HAttackInfo Error");
                        return;
                    }
                    var json = File.ReadAllText(hInfoConfigName);
                    m_ConfigDataHAttackInfoData = JsonConvert.DeserializeObject<Dictionary<int, ConfigDataHAttackInfo>>(json);
                }
                {
                    var hInfoConfigName = pathPrefix + "HAttackConditionInfo.json";
                    if (!File.Exists(hInfoConfigName))
                    {
                        PluginMain.Main.LogError($"load HAttackConditionInfo Error");
                        return;
                    }
                    var json = File.ReadAllText(hInfoConfigName);
                    m_ConfigDataHAttackConditionInfoData = JsonConvert.DeserializeObject<Dictionary<int, ConfigDataHAttackConditionInfo>>(json);
                }
                {
                    var hInfoConfigName = pathPrefix + "HPartFightInfo.json";
                    if (!File.Exists(hInfoConfigName))
                    {
                        PluginMain.Main.LogError($"load ConfigDataHPartFightInfo Error");
                        return;
                    }
                    var json = File.ReadAllText(hInfoConfigName);
                    m_ConfigDataHPartFightInfoData = JsonConvert.DeserializeObject<Dictionary<int, ConfigDataHPartFightInfo>>(json);
                }
                {
                    var hInfoConfigName = pathPrefix + "HSkillGroupInfo.json";
                    if (!File.Exists(hInfoConfigName))
                    {
                        PluginMain.Main.LogError($"load ConfigDataHSkillGroupInfo Error");
                        return;
                    }
                    var json = File.ReadAllText(hInfoConfigName);
                    m_ConfigDataHSkillGroupInfoData = JsonConvert.DeserializeObject<Dictionary<int, ConfigDataHSkillGroupInfo>>(json);
                }
            }
            catch (Exception e)
            {
                PluginMain.Main.LogError($"load config Error");
            }
        }


        #region 获取方法


        public ConfigDataHAttackPoolInfo GetConfigDataHAttackPoolInfo(int key)
        {
            ConfigDataHAttackPoolInfo data;
            if (m_ConfigDataHAttackPoolInfoData.TryGetValue(key, out data))
            {
                return data;
            }
            return null;
        }


        public Dictionary<int, ConfigDataHAttackPoolInfo> GetAllConfigDataHAttackPoolInfo()
        {
            return m_ConfigDataHAttackPoolInfoData;
        }


        public void ClearConfigDataHAttackPoolInfo()
        {
            m_ConfigDataHAttackPoolInfoData.Clear();
        }


        public ConfigDataHAttackInfo GetConfigDataHAttackInfo(int key)
        {
            ConfigDataHAttackInfo data;
            if (m_ConfigDataHAttackInfoData.TryGetValue(key, out data))
            {
                return data;
            }
            return null;
        }


        public Dictionary<int, ConfigDataHAttackInfo> GetAllConfigDataHAttackInfo()
        {
            return m_ConfigDataHAttackInfoData;
        }


        public void ClearConfigDataHAttackInfo()
        {
            m_ConfigDataHAttackInfoData.Clear();
        }


        public ConfigDataHAttackConditionInfo GetConfigDataHAttackConditionInfo(int key)
        {
            ConfigDataHAttackConditionInfo data;
            if (m_ConfigDataHAttackConditionInfoData.TryGetValue(key, out data))
            {
                return data;
            }
            return null;
        }


        public Dictionary<int, ConfigDataHAttackConditionInfo> GetAllConfigDataHAttackConditionInfo()
        {
            return m_ConfigDataHAttackConditionInfoData;
        }


        public void ClearConfigDataHAttackConditionInfo()
        {
            m_ConfigDataHAttackConditionInfoData.Clear();
        }


        public ConfigDataHPartFightInfo GetConfigDataHPartFightInfo(int key)
        {
            ConfigDataHPartFightInfo data;
            if (m_ConfigDataHPartFightInfoData.TryGetValue(key, out data))
            {
                return data;
            }
            return null;
        }


        public Dictionary<int, ConfigDataHPartFightInfo> GetAllConfigDataHPartFightInfo()
        {
            return m_ConfigDataHPartFightInfoData;
        }


        public void ClearConfigDataHPartFightInfo()
        {
            m_ConfigDataHPartFightInfoData.Clear();
        }


        public ConfigDataHSkillGroupInfo GetConfigDataHSkillGroupInfo(int key)
        {
            ConfigDataHSkillGroupInfo data;
            if (m_ConfigDataHSkillGroupInfoData.TryGetValue(key, out data))
            {
                return data;
            }
            return null;
        }


        public Dictionary<int, ConfigDataHSkillGroupInfo> GetAllConfigDataHSkillGroupInfo()
        {
            return m_ConfigDataHSkillGroupInfoData;
        }


        public void ClearConfigDataHSkillGroupInfo()
        {
            m_ConfigDataHSkillGroupInfoData.Clear();
        }


        #endregion


        #region 数据定义

        private Dictionary<int, ConfigDataHAttackPoolInfo> m_ConfigDataHAttackPoolInfoData = new Dictionary<int, ConfigDataHAttackPoolInfo>();

        private Dictionary<int, ConfigDataHAttackInfo> m_ConfigDataHAttackInfoData = new Dictionary<int, ConfigDataHAttackInfo>();

        private Dictionary<int, ConfigDataHAttackConditionInfo> m_ConfigDataHAttackConditionInfoData = new Dictionary<int, ConfigDataHAttackConditionInfo>();

        private Dictionary<int, ConfigDataHPartFightInfo> m_ConfigDataHPartFightInfoData = new Dictionary<int, ConfigDataHPartFightInfo>();

        private Dictionary<int, ConfigDataHSkillGroupInfo> m_ConfigDataHSkillGroupInfoData = new Dictionary<int, ConfigDataHSkillGroupInfo>();

        #endregion

    }
}
