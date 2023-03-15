using FirstBepinPlugin.Config;
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

        #region 战斗常量

        public const int YuWang2ExtraYinLingqi = 10;
        public const int FaQingThreshold = 20;


        public const string HDialogId_Defeated = "HFight_Defeated"; // 搞
        public const string HDialogId_EnterTiwei = "HFight_EnterTiwei"; // 进入体位 哪些变量
        public const string HDialogId_WantEnterTiwei = "HFight_WantEnterTiwei"; // 尝试进入体位 给出选项
        #endregion

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

        public const int BuffId_FlagFaQing = 9971200; // 发情 
        public const int BuffId_FlagTiWeiShou   = 9971211;
        public const int BuffId_FlagTiWeiKou = 9971212;
        public const int BuffId_FlagTiWeiRu = 9971213;
        public const int BuffId_FlagTiWeiXue = 9971214;
        public const int BuffId_FlagTiWeiGang = 9971215;

        public const int BuffId_FlagJingJin = 9971220; // jingjin
        public const int BuffId_FlagJueDing = 9971221; // jueding
        public const int BuffId_FlagWuLi = 9971222; // 无力

        public const int BuffId_FlagWantShou  = 9971231; // flag 准备进入 Shou
        public const int BuffId_FlagWantKou   = 9971232; // flag 准备进入 Kou
        public const int BuffId_FlagWantRu    = 9971233; // flag 准备进入 Ru
        public const int BuffId_FlagWantXue   = 9971234; // flag 准备进入 Xue
        public const int BuffId_FlagWantGang  = 9971235; // flag 准备进入 Gang

        public const int BuffId_GuideShou     = 9971236; // flag 准备进入 Shou 2
        public const int BuffId_GuideKou      = 9971237; // flag 准备进入 Kou 2
        public const int BuffId_GuideRu       = 9971238; // flag 准备进入 Ru 2
        public const int BuffId_GuideXue      = 9971239; // flag 准备进入 Xue 2
        public const int BuffId_GuideGang     = 9971240; // flag 准备进入 Gang 2

        public const int BuffId_FlagYiZhuang0 = 9971241; // flag 衣装0
        public const int BuffId_FlagYiZhuang1 = 9971242; // flag 衣装1
        public const int BuffId_FlagYiZhuang2 = 9971243; // flag 衣装2
        public const int BuffId_FlagYiZhuang3 = 9971244; // flag 衣装3

        public const int BuffId_FlagDefeated = 9971250; // flag 已战败

        public const int BuffId_FlagMeiLi0 = 9971261; // flag 魅力等级 1
        public const int BuffId_FlagMeiLi1 = 9971262; // flag 魅力等级 2
        public const int BuffId_FlagMeiLi2 = 9971263; // flag 魅力等级 3
        public const int BuffId_FlagMeiLi3 = 9971264; // flag 魅力等级 4
        public const int BuffId_FlagMeiLi4 = 9971265; // flag 魅力等级 5

        
        #endregion

        #region 机制 9971400 - 9971600

        public const int BuffId_JiaoHe = 9971400; // 交合类技能 减少行动点
        public const int BuffId_JingYu = 9971401; // 精浴覆盖

        #endregion

        #region 非技能状态 9971600 - 9971800
        public const int BuffId_BasicYinLingGen = 9971605; //进战后的基础淫灵根比率
        public const int BuffId_TurnModYuWang = 9971607; // 回合开始改变欲望
        public const int BuffId_SelfCostYiZhuang = 9971608; // 自身使用技能时损耗衣装
        public const int BuffId_SelfRecoverYiZhuang = 9971609; // 自身回合结束弃牌时恢复衣装
        public const int BuffId_SelfCalmDown = 9971610; // 自身衣装完好时 回合结束时降低欲望
        public const int BuffId_HDefeatProtect = 9971611; // 战败时恢复血量 并播放动画
        #endregion

        #region 一次性效果 9971800 - 9972000
        public const int BuffId_AddYinLingQi = 9971800; // 一次性效果 增加等同于层数的淫灵气
        public const int BuffId_AddCertainWuXingLingQi = 9971801; // 一次性效果 增加等同于层数的自选灵气
        public const int BuffId_AddYuWang = 9971802; // 一次性效果 增加等同于层数的欲望
        public const int BuffId_StealLingQi = 9971803; // 一次性效果 偷取层数的灵气
        
        public const int BuffId_AddXingFen_Kou = 9971804; // 一次性效果 增加等同于层数的兴奋 口
        public const int BuffId_AddXingFen_Ru = 9971805; // 一次性效果 增加等同于层数的兴奋 口
        public const int BuffId_AddXingFen_Xue = 9971806; // 一次性效果 增加等同于层数的兴奋 口
        public const int BuffId_AddXingFen_Gang = 9971807; // 一次性效果 增加等同于层数的兴奋 口
        #endregion


        #region 战斗技能
        public const int SkillId_Base = 9971000;
        #endregion

        //public const int BuffTriggerId_CheckLayerBetween = 9971; // 检查层数要求 其具体参数定义在seid 9971文件中

        public const int BuffSeId_CheckYiZhuang = 520; // 检查衣装 p1 值 p2 比较方式

        //public const int BuffSeId_CheckIntoHMode = 599; // 特殊检查 判断进入yinzhan
        //public const int BuffSeId_SwitchIntoHMode = 598; // 特殊效果 进入Hmode
        //public const int BuffSeId_CheckOutHMode = 597; // 特殊seid 检查退出yinzhan的条件
        //public const int BuffSeId_SwitchOutHMode = 596; // 特殊效果 进入Hmode
        public const int BuffSeId_ModYiZhuang = 594; // 改变衣装 p1 每层加多少 p2 非0时固定增加 无关层数
        public const int BuffSeId_ModYuWang = 595; // 增加欲望 p1 每层加多少 p2 非0时固定增加 无关层数
        public const int BuffSeId_ModXingFen = 596; // 增加欲望 part 部位 value1 每层加多少 value2 非0时固定增加 无关层数

        //public const int BuffSeId_CheckIntoYinNormal = 901; // 特殊seid 检查进入yinzhan的条件
        //public const int BuffSeId_CheckIntoYinShou = 902; // 特殊seid 检查进入yinzhan手的条件


        // bonus
        public const int BuffSeId_ModMaxYizhuang = 540; // 改变衣装最大值
        public const int BuffSeId_ModMeiLi = 541; // 改变魅力 value1 每层加值 value1 固定加值（倒计时类）
        public const int BuffSeId_ModMaxTili = 542; // 改变最大体力
        public const int BuffSeId_ModFaQingThreshold = 543; // 改变发情阈值

        // 改变衣装
        public const int SkillSeId_ModYiZhuang = 550; // 移除衣装 p1 固定值 p2 最大百分比
        public const int SkillSeId_ModYuWang = 551; // 改变欲望 p1 固定值
        public const int SkillSeId_ModXingFen = 552; // 改变部位兴奋度 p1 固定值


        public const int SkillSeId_SwitchTiWei = 560; // 切换体味 p1 体位id
        public const int SkillSeId_ModKuaiGan = 561; // 改变对象 p1:1自身2敌方 p2:数值
        public const int SkillSeId_YinYi = 562; // 淫意
        public const int SkillSeId_DiscardNonYinQiAddBuff = 563; // 弃置X点非淫灵气 给予X*Y层Zbuff


        public const int SkillSeId_ApplyHAttack = 590; // 实施h伤害 

        public const int SkillSeId_MultiTriggerByUsedTimee = 598; // 根据本回合使用次数改变后续触发次数 初始触发p1次 每次连续使用效果减少触发p2次 最低触发p3次 最高触发p4次
        public const int SkillSeId_CheckNotWuLi = 597; // 特殊seid 检查退出HMode的条件
        public const int SkillSeId_CheckFirstUse = 596; // 检查 是否第一次使用

        public const int SkillSeId_EnterHMode = 599;
    }
}
