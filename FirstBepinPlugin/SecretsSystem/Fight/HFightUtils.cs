using FirstBepinPlugin.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstBepinPlugin
{
    public class HFightUtils
    {

        public static bool CustomCompare<T>(T val, int cmpType, T targetVal) where T : IComparable
        {
            switch ((EConditionCompareType)cmpType)
            {
                case EConditionCompareType.Gt:
                    return val.CompareTo(targetVal) > 0;
                case EConditionCompareType.Gte:
                    return val.CompareTo(targetVal) >= 0;
                case EConditionCompareType.Equal:
                    return val.CompareTo(targetVal) == 0;
                case EConditionCompareType.Le:
                    return val.CompareTo(targetVal) < 0;
                case EConditionCompareType.Lte:
                    return val.CompareTo(targetVal) <= 0;
                case EConditionCompareType.NotEqual:
                    return val.CompareTo(targetVal) != 0;
            }
            return false;
        }

        private static List<int> s_emptySkillList = new List<int>();
        /// <summary>
        /// 获取对应组的技能
        /// </summary>
        /// <returns></returns>
        public static List<int> HSkillListGetByGroup(int skillGroup)
        {
            //var groupConf = PluginMain.Main.ConfigDataLoader.GetConfigDataHSkillGroupInfo(skillGroup);
            // 直接从内存里读
            if(!StaticConfigContainer.s_SkillGroupSkills.TryGetValue(skillGroup, out var skills))
            {
                return s_emptySkillList;
            }
            // 裁剪已装备
            return skills;
        }

        /// <summary>
        /// 通过weight获取id
        /// </summary>
        /// <param name="weights"></param>
        /// <returns></returns>
        public static int RandomValueByWeight(List<KeyValuePair<int, int>> weights)
        {
            int maxWeighht = 0;
            for (int i = 0; i < weights.Count; i++)
            {
                maxWeighht += weights[i].Value;
            }
            if (maxWeighht == 0)
            {
                return -1;
            }
            int randVal = UnityEngine.Random.Range(0, maxWeighht);
            int currWeight = 0;
            for (int i = 0; i < weights.Count; i++)
            {
                currWeight += weights[i].Value;
                if (randVal < currWeight)
                {
                    return weights[i].Key;
                }
            }
            return -1;
        }

        /// <summary>
        /// 通过属性获取seid
        /// </summary>
        /// <returns></returns>
        public static int ModSeidGetByAttribute(HModeAttributeType type)
        {
            switch (type)
            {
                case HModeAttributeType.HMaxClothes:
                    return Consts.BuffSeId_ModMaxYizhuang;
                case HModeAttributeType.HMeiLi:
                    return Consts.BuffSeId_ModMeiLi;
                case HModeAttributeType.HMaxTili:
                    return Consts.BuffSeId_ModMaxTili;
                case HModeAttributeType.HFaQingThreshold:
                    return Consts.BuffSeId_ModFaQingThreshold;
            }
            return 0;
        }

        private static List<int> m_cacheList = new List<int>();

        public static List<int> GetHAttackWeightList(ref List<GUIPackage.Skill> skills)
        {
            m_cacheList.Clear();
            foreach(var skill in skills)
            {
                var atkInfo = PluginMain.Main.ConfigDataLoader.GetConfigDataHAttackInfo(skill.skill_ID);
                m_cacheList.Add(atkInfo.DefaultWeight);
            }
            return m_cacheList;
        }


        /// <summary>
        /// 带权重的洗牌
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rand"></param>
        /// <param name="items"></param>
        /// <param name="weights"></param>
        public static bool ShuffleWithWeight<T>(List<T> items, List<int> weights)
        {
            if (items.Count != weights.Count)
            {
                return false;
            }

            var totalWeight = 0;
            foreach (var w in weights)
            {
                if (w < 0)
                {
                    return false;
                }

                totalWeight += w;
            }

            for (var i = 0; i < items.Count; ++i)
            {
                if (totalWeight == 0)
                {
                    return false;
                }
                var weight = UnityEngine.Random.Range(0, totalWeight);
                for (var j = i; j < weights.Count; ++j)
                {
                    weight -= weights[j];
                    if (weight < 0)
                    {
                        totalWeight -= weights[j];

                        var temp_weight = weights[j];
                        weights[j] = weights[i];
                        weights[i] = temp_weight;

                        var temp_item = items[j];
                        items[j] = items[i];
                        items[i] = temp_item;
                        break;
                    }
                }
            }
            return true;
        }
        
        /// <summary>
        /// 获得姿态专属标记buff
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public static int FlagBuffIdGetByTiWei(HModeTiWei tiWei)
        {
            switch (tiWei)
            {
                case HModeTiWei.Shou:
                    return Consts.BuffId_FlagTiWeiShou;
                case HModeTiWei.Kou:
                    return Consts.BuffId_FlagTiWeiKou;
                case HModeTiWei.Ru:
                    return Consts.BuffId_FlagTiWeiRu;
                case HModeTiWei.Xue:
                    return Consts.BuffId_FlagTiWeiXue;
                case HModeTiWei.Gang:
                    return Consts.BuffId_FlagTiWeiGang;
            }
            return -1;
        }

        public HModeTiWei GetTiWeiByWantBuffId(int wantBuffId)
        {
            switch(wantBuffId)
            {
                case Consts.BuffId_WantShou:
                {
                    return HModeTiWei.Shou;
                }
                case Consts.BuffId_WantKou:
                {
                    return HModeTiWei.Kou;
                }
                case Consts.BuffId_WantRu:
                    {
                        return HModeTiWei.Ru;
                    }
                case Consts.BuffId_WantXue:
                    {
                        return HModeTiWei.Xue;
                    }
                case Consts.BuffId_WantGang:
                    {
                        return HModeTiWei.Gang;
                    }
            }
            return HModeTiWei.None;
        }
    }
}
