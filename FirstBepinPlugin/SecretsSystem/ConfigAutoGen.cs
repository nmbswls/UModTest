using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstBepinPlugin.Config
{
    #region subtypes

    public class Tuple2
    {
        public Int32 P1; // 
        public Int32 P2; // 
    }


    public class Tuple3
    {
        public Int32 P1; // 
        public Int32 P2; // 
        public Int32 P3; // 
    }


    public class Tuple4
    {
        public Int32 P1; // 
        public Int32 P2; // 
        public Int32 P3; // 
        public Int32 P4; // 
    }


    #endregion



    #region enums



    #endregion



    #region datas

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

        public List<Tuple4> Conditions = new List<Tuple4>(); // 使用条件列表

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


    public partial class ConfigDataHSkillGroupInfo
    {

        public Int32 ID; // 主键

        public List<Tuple4> ShowCondition = new List<Tuple4>(); // 条件列表

        public Int32 Priority; // 优先级（越低越靠前）

        public List<int> SkillList = new List<int>(); // 技能列表

    }

    #endregion
}
