using FirstBepinPlugin.Config;
using FirstBepinPlugin.MonoScripts;
using Fungus;
using GUIPackage;
using KBEngine;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using YSGame.Fight;

namespace FirstBepinPlugin
{

    /// <summary>
    /// 玩家信息
    /// </summary>
    public class SecretsPlayerInfo
    {
        public int Level;
        public int Exp;
        public int Status;

        /// <summary>
        /// 
        /// </summary>
        public List<SecretsPlayerPartInfo> PartsList = new List<SecretsPlayerPartInfo>();

        public static SecretsPlayerInfo CreateNew()
        {
            SecretsPlayerInfo ret = new SecretsPlayerInfo();
            ret.Level = 1;
            ret.Exp = 0;
            ret.Status = 0;

            for(int i=(int)EnumPartType.Invalid+1; i< (int)EnumPartType.Max;i++)
            {
                ret.PartsList.Add(new SecretsPlayerPartInfo()
                {
                    Type = i,
                    Level = 1
                }); 
            }
            return ret;
        }
    }


    public class SecretsPlayerPartInfo
    {
        public int Type;
        public int Level;

        public int Exp;
        public int JingLeft;
        public List<int> Perks = new List<int>();
    }

    public class SecretsSystem
    {

        #region 单例相关

        private static SecretsSystem m_instance;
        public static SecretsSystem Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new SecretsSystem();
                }
                return m_instance;
            }
        }
        private SecretsSystem(){}

        #endregion

        public int GetPlayerLevel()
        {
            return m_SecretsInfo.Level;
        }

        public bool IsSecretOpen()
        {
           var player = Tools.instance.getPlayer();
            if(player == null)
            {
                return false;
            }
            if(player.SelectTianFuID.HasItem(PluginMain.Main.config.EntryTalentId))
            {
                return true;
            }
            return false;
        }

        public void Reset(SecretsPlayerInfo newInfo)
        {
            this.m_SecretsInfo = newInfo;

            isSecretsLunDao = false;

            var player = Tools.instance.getPlayer();

            PluginMain.Main.LogInfo("Fake add needed skills");
            player.addHasSkillList(99710);
            player.addHasSkillList(99711);
        }


        public bool AddJingYuan(EnumPartType partType, int jingyuan)
        {
            if (!IsSecretOpen())
            {
                return false;
            }

            PluginMain.Main.LogInfo("AddJingYuan " + partType);
            var part = m_SecretsInfo.PartsList.Find((info) => info.Type == (int)partType);
            if(part == null)
            {
                return false;
            }
            var levelInfo = PluginMain.Main.config.PartInfo[part.Type].LevelInfoGet(part.Level);
            if(levelInfo == null)
            {
                return false;
            }
            part.JingLeft += jingyuan;
            if(part.JingLeft > levelInfo.JingStorage)
            {
                part.JingLeft = levelInfo.JingStorage;
            }
            return true;
        }

        public void AbsorbByTime(int dayDiff)
        {
            if(!IsSecretOpen())
            {
                return;
            }

            PluginMain.Main.LogInfo("AbsorbByTime get exp");
            double totalExp = 0;
            foreach(var partInfo in m_SecretsInfo.PartsList)
            {
                if(partInfo.JingLeft == 0)
                {
                    continue;
                }
                int left = partInfo.JingLeft;
                var levelInfo = PluginMain.Main.config.PartInfo[partInfo.Type].LevelInfoGet(partInfo.Level);
                int absorbVal = dayDiff * levelInfo.JingAbsorbSpeed;
                if(absorbVal > left)
                {
                    absorbVal = left;
                }
                totalExp += (absorbVal * levelInfo.JingAbsorbRate * 0.01);
                partInfo.JingLeft -= absorbVal;
            }

            if(totalExp > 0)
            {
                Tools.instance.getPlayer().addEXP((int)totalExp);
                UIPopTip.Inst.Pop($"转换修为{(int)totalExp}", PopTipIconType.叹号);
            }
        }

        /// <summary>
        /// 为空表示未开启功能
        /// </summary>
        public SecretsPlayerInfo m_SecretsInfo;

        #region ctx信息

        /// <summary>
        /// context
        /// </summary>
        public bool isSecretsLunDao;

        #endregion



        #region 战斗相关

        public class HFightCtx
        {
            public bool m_isInHMode;
            public string m_hState = "";

            public Dictionary<int, GUIPackage.Skill> m_stateSkillCache = new Dictionary<int, GUIPackage.Skill>();
            public List<int> m_nonHSkillCache = new List<int>();
        }


        public HFightCtx m_ctx;
        /// <summary>
        /// 切换技能
        /// </summary>
        /// <param name="oldState"></param>
        /// <param name="newState"></param>
        public void FightOnSwitchState(KBEngine.Avatar player, string oldState, string newState)
        {
            //if(m_stateSkillCache.TryGetValue(newState, out var skillList))
            //{

            //}
        }


        /// <summary>
        /// 检查是否能进入HMode
        /// </summary>
        /// <returns></returns>
        public bool CheckFightEnterHMode(KBEngine.Avatar avatar)
        {
            PluginMain.Main.LogInfo("CheckFightEnterHMode");
            var player = Tools.instance.getPlayer();
            int buffNum = avatar.buffmag.GetBuffSum(Consts.BuffId_YinYuWang);

            if (buffNum <= 20)
            {
                PluginMain.Main.LogInfo("层数不足 不进入Hzhan");
                return false;
            }

            int rate10000 = (buffNum - 20) * 80;
            if (rate10000 >= 10000)
            {
                return true;
            }
            int randVal = UnityEngine.Random.Range(0, 10000);
            if (randVal < rate10000)
            {
                return true;
            }
            else
            {
                return true;
            }
        }

        public void FightHInit(KBEngine.Avatar player)
        {
            m_ctx = new HFightCtx();

            player.spell.addBuff(Consts.BuffId_YinTili, 100);
            player.spell.addBuff(Consts.BuffId_YinJingli, 100);

            player.spell.addBuff(Consts.BuffId_YinYuWang, 10);

            player.OtherAvatar.spell.addBuff(9971009, 1);
        }

        public void FightEnterHMode()
        {
            PluginMain.Main.LogInfo("try enter h mode.");

            if(m_ctx == null || m_ctx.m_isInHMode)
            {
                return;
            }
            var player = Tools.instance.getPlayer();
            m_ctx.m_isInHMode = true;

            m_ctx.m_hState = "";

            var skillList = new List<int>();
            // skil list
            switch(m_ctx.m_hState)
            {
                case "": // 原始形态
                    {
                        skillList.Add(997201);
                        skillList.Add(997211);
                        skillList.Add(997221);
                    }
                    break;
                case "WuLi":
                    {
                        skillList.Add(997231);
                        skillList.Add(997241);
                        skillList.Add(997251);
                    }
                    break;
                case "GaoChao":
                    {
                        skillList.Add(997261);
                        skillList.Add(997271);
                        skillList.Add(997281);
                    }
                    break;
            }

            // 清空技能
            player.FightClearSkill(0, 10);

            // 赋予技能
            foreach (var skillId in skillList)
            {
                var skillItem = player.skill.Find(delegate(GUIPackage.Skill s){ return s.skill_ID == skillId; });
                if(skillItem == null)
                {
                    skillItem = new GUIPackage.Skill(skillId, 0, 10);
                }
                player.skill.Add(skillItem);
                int num = 0;
                foreach (UIFightSkillItem fightSkill in UIFightPanel.Inst.FightSkills)
                {
                    if (num >= 0 && num < 10 && !fightSkill.HasSkill)
                    {
                        fightSkill.SetSkill(skillItem);
                        break;
                    }
                    num++;
                }
            }
        }


        public void FightExitHMode()
        {
            PluginMain.Main.LogInfo("try exit h mode.");

            if (m_ctx == null || !m_ctx.m_isInHMode)
            {
                return;
            }

            var player = Tools.instance.getPlayer();
            m_ctx.m_isInHMode = false;

            // 清空技能
            player.FightClearSkill(0, 10);

            foreach(var oldSkillId in m_ctx.m_nonHSkillCache)
            {
                var skillItem = player.skill.Find(delegate (GUIPackage.Skill s) { return s.skill_ID == oldSkillId; });
                
                // 一定在列表中
                if(skillItem == null)
                {
                    continue;
                }
                int num = 0;
                foreach (UIFightSkillItem fightSkill in UIFightPanel.Inst.FightSkills)
                {
                    if (num >= 0 && num < 10 && !fightSkill.HasSkill)
                    {
                        fightSkill.SetSkill(skillItem);
                        break;
                    }
                    num++;
                }
            }
        }


        #endregion


            /// <summary>
            /// 
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
        public Sprite SecretsLunDaoSpriteGet(int id)
        {
            string spritePath = "";
            //ResManager.inst.LoadSprite("Skill Icon/" + SkillIconPatch.GetSkillIconByKey(__instance.Id));
            return null;
        }

        #region 临时ui

        #endregion

        public SecretsDetailPanel panel;
        public void ShowNpcSecretsUI()
        {
            if(panel == null)
            {
                return;
            }
            PluginMain.Main.LogInfo("ShowNpcSecretsUI");
            panel.Show();
        }

    }
}
