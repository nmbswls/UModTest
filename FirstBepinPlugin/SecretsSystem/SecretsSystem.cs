using FirstBepinPlugin.Config;
using FirstBepinPlugin.MonoScripts;
using Fungus;
using GUIPackage;
using KBEngine;
using PaiMai;
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

            for(int i=(int)EPartType.Invalid+1; i< (int)EPartType.Max;i++)
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
            player.addHasSkillList(99711);

            if(!player.hasItem(99790))
            {
                int id = 99790;
                int count = 1;

                player.addItem(id, count, null, false);
            }
        }


        public bool AddJingYuan(EPartType partType, int jingyuan)
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

        private HModeFightManager m_fightManager = new HModeFightManager();
        public static HModeFightManager FightManager
        {
            get { return Instance.m_fightManager; }
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
