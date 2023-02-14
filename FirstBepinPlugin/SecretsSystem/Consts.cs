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
        public const int SecretEquipType = 997;
        public const string sLianqi_SecretTabName = "Toggle_Sex";
        //public const int YinLingQiTypeInt = 9;
        //public const LingQiType YinLingQiType = (LingQiType)9;


        public const int ItemId_MineLu = 99790;

        // 战斗buff
        public const int BuffId_YinTili = 9971000;
        public const int BuffId_YinNaili = 9971001;
        public const int BuffId_YinKuaiGan = 9971002;
        public const int BuffId_YinYuWang = 9971003;
        public const int BuffId_YinYiZhuang = 9971004;

        public const int BuffId_BasicYinLingen = 9971005; //进战后的基础淫灵根比率

        //public const int BuffTriggerId_CheckLayerBetween = 9971; // 检查层数要求 其具体参数定义在seid 9971文件中

        public const int BuffSeId_CheckIntoHMode = 599; // 特殊检查 判断进入yinzhan
        public const int BuffSeId_SwitchIntoHMode = 598; // 特殊效果 进入Hmode
        public const int BuffSeId_CheckOutHMode = 597; // 特殊seid 检查退出yinzhan的条件
        public const int BuffSeId_SwitchOutHMode = 596; // 特殊效果 进入Hmode


        //public const int BuffSeId_CheckIntoYinNormal = 901; // 特殊seid 检查进入yinzhan的条件
        //public const int BuffSeId_CheckIntoYinShou = 902; // 特殊seid 检查进入yinzhan手的条件

        public const int SkillSeId_CheckOutHMode = 597; // 特殊seid 检查退出HMode的条件
        public const int SkillSeId_SwitchOutHMode = 596; // 特殊效果 脱离Hmode

        // bonus
        public const int BuffSeId_ModMaxYizhuang = 540; // 改变衣装最大值

        // 改变衣装
        public const int SkillSeId_ModYiZhuang = 550; // 移除衣装 p1 固定值 p2 最大百分比

        public const int SkillSeId_EnterHMode = 599;
    }
}
