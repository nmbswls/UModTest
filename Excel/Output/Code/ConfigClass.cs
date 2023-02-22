//
// Auto Generated Code By excel2json
// https://neil3d.gitee.io/coding/excel2json.html
// 1. 每个 Sheet 形成一个 Struct 定义, Sheet 的名称作为 Struct 的名称
// 2. 表格约定：第一行是变量名称，第二行是变量类型

using System;
using System.Collections.Generic;

namespace My.ConfigData {

#region subtypes

    public class Sample
    {
        public Int32 SampleField; // This is a int field
    }

#endregion



#region enums


    public enum Sample
    {
    }


#endregion



#region datas

    public partial class ConfigDataHAttackPoolInfo
    {

        public Int32 ID; // 池ID

        public List<int> AttackIdList= new List<int>(); // 攻击Id列表

    }


    public partial class ConfigDataHAttackInfo
    {

        public Int32 ID; // 主键

        public String Name; // 名称

        public List<int> TargetPart= new List<int>(); // 部位 1口 2乳 3穴 4尻

        public Int32 AttackType; // 攻击类型 1 轻 2 重 3 淫

        public List<int> Conditions= new List<int>(); // 使用条件列表

        public Int32 DamageRate; // 伤害率

        public Int32 KuaiGanRate; // 快感率

        public List<int> Tags= new List<int>(); // Tags

        public Int32 DefaultWeight; // 基础权重

        public String HintContent; // 提示语

        public String AnimName; // 动画名

    }


    public partial class ConfigDataHAttackConditionInfo
    {

        public Int32 ID; // 主键

        public Int32 Type; // 条件类型 1 衣装 2 自身欲望 3 主角欲望

        public List<int> Params= new List<int>(); // 参数列表

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


    public partial class ConfigDataMultiLangInfo_CN
    {

        public Int32 ID; // 主键

        public String StrKey; // 多语言键

        public String OriginContent; // 原始内容

        public String ReplaceContent; // 替换内容

    }


    public partial class ConfigDataMultiLangInfo_EN
    {

        public Int32 ID; // 主键

        public String StrKey; // 多语言键

        public String OriginContent; // 原始内容

        public String ReplaceContent; // 替换内容

    }

#endregion

}


// End of Auto Generated Code
