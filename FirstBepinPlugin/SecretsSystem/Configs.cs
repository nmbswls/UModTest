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
