using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YSGame.Fight;

namespace FirstBepinPlugin
{
    /// <summary>
    /// 常量
    /// </summary>
    public static class Consts
    {
        public static int Float2Int100 = 100;

        public const int SecretEquipType = 997;
        public const string sLianqi_SecretTabName = "Toggle_Sex";
        //public const int YinLingQiTypeInt = 9;
        //public const LingQiType YinLingQiType = (LingQiType)9;


        public const int ItemId_MineLu = 99790;

        // 战斗buff
        #region 基础状态显示 9971000 - 9971200

        public const int BuffId_YinTili = 9971000;
        public const int BuffId_YinNaili = 9971001;
        public const int BuffId_YinKuaiGan = 9971002;
        public const int BuffId_YinYuWang = 9971003;
        public const int BuffId_YinYiZhuang = 9971004;
        public const int BuffId_YinXingFen = 9971006; // 兴奋度

        #endregion

        #region 状态标记 9971200 - 9971400

        public const int BuffId_FlagFaQing = 9971010; // 敌方发情
        public const int BuffId_FlagNormal = 9972000;
        public const int BuffId_FlagShou = 9972001;

        public const int BuffId_FlagJingJin = 9971220; // jingjin
        public const int BuffId_FlagJueDing = 9971221; // jueding
        public const int BuffId_FlagWuLi = 9971222; // 无力

        #endregion

        #region 机制 9971400 - 9971600

        public const int BuffId_JiaoHe = 9971400; // 交合类技能 减少行动点

        #endregion

        #region 非技能状态 9971600 - 9971800
        public const int BuffId_BasicYinLingen = 9971005; //进战后的基础淫灵根比率
        public const int BuffId_TurnModYuWang = 9971007; // 回合开始改变欲望
        #endregion
        #region 一次性效果 9971800 - 9972000
        public const int BuffId_AddYinLingQi = 9971800; // 一次性效果 增加等同于层数的淫灵气
        public const int BuffId_AddCertainWuXingLingQi = 9971801; // 一次性效果 增加等同于层数的自选灵气
        #endregion

        //public const int BuffTriggerId_CheckLayerBetween = 9971; // 检查层数要求 其具体参数定义在seid 9971文件中

        //public const int BuffSeId_CheckIntoHMode = 599; // 特殊检查 判断进入yinzhan
        //public const int BuffSeId_SwitchIntoHMode = 598; // 特殊效果 进入Hmode
        //public const int BuffSeId_CheckOutHMode = 597; // 特殊seid 检查退出yinzhan的条件
        //public const int BuffSeId_SwitchOutHMode = 596; // 特殊效果 进入Hmode
        public const int BuffSeId_ModYuWang = 595; // 增加欲望 p1 每层加多少 p2 非0时固定增加 无关层数


        //public const int BuffSeId_CheckIntoYinNormal = 901; // 特殊seid 检查进入yinzhan的条件
        //public const int BuffSeId_CheckIntoYinShou = 902; // 特殊seid 检查进入yinzhan手的条件

        
        public const int SkillSeId_SwitchOutHMode = 596; // 特殊效果 脱离Hmode

        // bonus
        public const int BuffSeId_ModMaxYizhuang = 540; // 改变衣装最大值
        public const int BuffSeId_ModMeiLi = 541; // 改变衣装最大值
        public const int BuffSeId_ModMaxTili = 542; // 改变最大体力

        // 改变衣装
        public const int SkillSeId_ModYiZhuang = 550; // 移除衣装 p1 固定值 p2 最大百分比
        public const int SkillSeId_ModYuWang = 551; // 改变欲望 p1 固定值
        public const int SkillSeId_ModXingFen = 552; // 改变部位兴奋度 p1 固定值


        public const int SkillSeId_SwitchTiWei = 560; // 切换体味 p1 体位id
        public const int SkillSeId_ModKuaiGan = 561; // 改变对象 p1:1自身2敌方 p2:数值
        public const int SkillSeId_YinYi = 562; // 淫意
        public const int SkillSeId_DiscardNonYinQiAddBuff = 563; // 弃置X点非淫灵气 给予X*Y层Zbuff

        public const int SkillSeId_MultiTriggerByUsedTimee = 598; // 根据本回合使用次数改变后续触发次数 初始触发p1次 每次连续使用效果减少触发p2次 最低触发p3次 最高触发p4次
        public const int SkillSeId_CheckNotWuLi = 597; // 特殊seid 检查退出HMode的条件

        public const int SkillSeId_EnterHMode = 599;
    }
}
