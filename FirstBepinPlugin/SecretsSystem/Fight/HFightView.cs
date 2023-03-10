using SkySwordKill.Next.DialogSystem;
using SuperScrollView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FirstBepinPlugin
{

    #region UI
    public class FightUIPointerListener : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public event Action EventOnPointerEnter;
        public event Action EventOnPointerExit;
        public void OnPointerEnter(PointerEventData eventData)
        {
            EventOnPointerEnter?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            EventOnPointerExit?.Invoke();
        }
    }

    public class FightHudRootController : MonoBehaviour
    {
        public HModeFightManager Owner;

        public FightUISkillTabController m_skillTabController;
        public FightUIHRecordController m_recordController;
        public FightUIStatusController m_statusController;
        public FightUISpecialActionController m_specialActionController;
        public FightHShowController m_hShowController;

        public void InitUI(HModeFightManager owner)
        {
            this.Owner = owner;

            var fightSpecialAction = transform.Find("FightSpecialAction");
            m_specialActionController = fightSpecialAction.gameObject.AddComponent<FightUISpecialActionController>();
            m_specialActionController.Init(owner);

            var fightHRecordPanel = transform.Find("FightHRecordPanel");
            m_recordController = fightHRecordPanel.gameObject.AddComponent<FightUIHRecordController>();
            m_recordController.Init(owner);

            var fightHStatus = transform.Find("FightHStatus");
            m_statusController = fightHStatus.gameObject.AddComponent<FightUIStatusController>();
            m_statusController.Init(owner);

            var fightHShow = transform.Find("FightHShow");
            m_hShowController = fightHShow.gameObject.AddComponent<FightHShowController>();
            m_hShowController.Init(owner);
        }

        public void OnDestroy()
        {
            SecretsSystem.FightManager.m_fightHud = null;
        }
    }

    public class FightUISkillTabController : MonoBehaviour
    {
        public HModeFightManager Owner;
        public UnityEngine.Transform TabRoot;
        public UnityEngine.GameObject ToggleTemplate;

        private class ToggleWrapper
        {
            public int TabIdx;
            public int SkillGroupId;
            public UnityEngine.GameObject Go;
            public Toggle CompToggle;
            public Text CompText;
        }

        private List<ToggleWrapper> m_currToggleList = new List<ToggleWrapper>();
        private Queue<ToggleWrapper> m_cachedToggles = new Queue<ToggleWrapper>();


        public int m_currSelectIdx = -1;

        public bool m_isWuLi = false;
        public void Init(HModeFightManager owner)
        {
            this.Owner = owner;
            TabRoot = gameObject.transform.GetChild(0);
            ToggleTemplate = gameObject.transform.Find("TabTemplate").gameObject;
        }

        public void RefreshUI()
        {

            while (m_currToggleList.Count > 0)
            {
                var firstWrapper = m_currToggleList[0];
                firstWrapper.Go.SetActive(false);
                m_cachedToggles.Enqueue(firstWrapper);
                m_currToggleList.RemoveAt(0);
            }

            var skillGroupList = Owner.GetCurrSkillGroupList();
            for (int i = 0; i < skillGroupList.Count; i++)
            {
                ToggleWrapper toggleWrapper;
                if (m_cachedToggles.Count > 0)
                {
                    toggleWrapper = m_cachedToggles.Dequeue();
                }
                else
                {
                    var newGo = UnityEngine.GameObject.Instantiate(ToggleTemplate, TabRoot);
                    var toggle = newGo.GetComponent<Toggle>();
                    var textComp = newGo.GetComponentInChildren<Text>();
                    textComp.font = PluginMain.Main.font_YaHei;
                    toggleWrapper = new ToggleWrapper()
                    {
                        SkillGroupId = 0,
                        CompToggle = toggle,
                        CompText = textComp,
                        Go = newGo,
                    };
                    var listener = newGo.AddComponent<FightUIPointerListener>();
                    listener.EventOnPointerEnter += delegate ()
                    {
                        OnTogglePointerEnter(toggleWrapper);
                    };

                    listener.EventOnPointerExit += delegate ()
                    {
                        OnTogglePointerExit(toggleWrapper);
                    };

                    toggle.onValueChanged.AddListener(delegate (bool isOn)
                    {
                        OnToggleValueChange(toggleWrapper);
                    });
                }
                toggleWrapper.TabIdx = i;
                toggleWrapper.SkillGroupId = skillGroupList[i];
                toggleWrapper.CompText.text = skillGroupList[i] + "技能组";
                toggleWrapper.CompToggle.isOn = false;
                toggleWrapper.Go.SetActive(true);
                m_currToggleList.Add(toggleWrapper);
            }

            m_currSelectIdx = 0;

            m_currToggleList[m_currSelectIdx].CompToggle.isOn = true;
        }

        private void OnToggleValueChange(ToggleWrapper toggleWrapper)
        {
            if (!toggleWrapper.CompToggle.isOn)
            {
                return;
            }
            m_currSelectIdx = toggleWrapper.TabIdx;
            SwitchSkillGroup(toggleWrapper.SkillGroupId);
        }

        private void SwitchSkillGroup(int skillGroupId)
        {
            if (skillGroupId == -1)
            {
                return;
            }
            var newSkills = HFightUtils.HSkillListGetByGroup(skillGroupId);

            SecretsSystem.FightManager.SwitchSkill(newSkills);
        }

        private void OnTogglePointerEnter(ToggleWrapper toggleWrapper)
        {
            var hintStr = "";
            hintStr = $"切换技能组：{toggleWrapper.SkillGroupId}技能";
            UToolTip.Show(hintStr, 150f);
        }

        private void OnTogglePointerExit(ToggleWrapper toggleWrapper)
        {
            UToolTip.Close();
        }
    }

    public class FightUIHRecordController : MonoBehaviour
    {
        public HModeFightManager Owner;

        public LoopListView2 MLoopListView;
        public int MRecordCount = 5;
        public UnityEngine.GameObject TextTemplage;
        public List<string> m_records = new List<string>() { "<color=#FF1493>im record 0.</color>", "im record 1.", "im record 2.", "im record 3.", "im record 4." };
        public void Init(HModeFightManager owner)
        {
            this.Owner = owner;

            TextTemplage = transform.Find("TextTemplate").gameObject;
            var loopListViewGo = transform.Find("ContentScroll").gameObject;
            MLoopListView = loopListViewGo.AddComponent<LoopListView2>();
            MLoopListView.ItemPrefabDataList.Add(new ItemPrefabConfData() { mItemPrefab = TextTemplage, mInitCreateCount = 6, mStartPosOffset = 10 });
            MLoopListView.ArrangeType = ListItemArrangeType.BottomToTop;
            MLoopListView.InitListView(MRecordCount, OnGetItemByIndex);

        }
        protected LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int rowIndex)
        {
            if (rowIndex < 0)
            {
                return null;
            }
            LoopListViewItem2 loopListViewItem = listView.NewListViewItem("TextTemplate");
            Text component = loopListViewItem.GetComponent<Text>();
            loopListViewItem.gameObject.SetActive(true);
            PluginMain.Main.LogInfo("virtual list OnGetItemByIndex " + rowIndex + "val " + m_records[rowIndex] + " sibling indx " + loopListViewItem.CachedRectTransform.localPosition);
            if (!loopListViewItem.IsInitHandlerCalled)
            {
                loopListViewItem.IsInitHandlerCalled = true;
                //component.Init();
            }

            //for (int i = 0; i < mItemCountPerRow; i++)
            //{
            //    int num = rowIndex * mItemCountPerRow + i;
            //    component.mItemList[i].SetAccptType(CanSlotType.全部物品);
            //    if (num >= MItemTotalCount)
            //    {
            //        component.mItemList[i].SetNull();
            //        continue;
            //    }
            //    BaseItem slotData = BaseItem.Create(ItemList[num].itemId, (int)ItemList[num].itemCount, ItemList[num].uuid, ItemList[num].Seid);
            //    component.mItemList[i].SetSlotData(slotData);
            //}
            component.text = m_records[rowIndex];
            return loopListViewItem;
        }

        public void AddRecord(string record)
        {
            m_records.Insert(0, "<color=#FF1493>" + record + "</color>");
            MLoopListView.SetListItemCount(m_records.Count);
            MLoopListView.RefreshAllShownItemWithFirstIndex(0);
        }

        /// <summary>
        /// 移除战斗记录
        /// </summary>
        public void RemoveOldRecords()
        {

        }
    }

    public class FightUIStatusController : MonoBehaviour
    {
        public HModeFightManager Owner;

        public Image m_tiliImage;
        public Text m_tiliText;
        public void Init(HModeFightManager owner)
        {
            this.Owner = owner;
            gameObject.transform.GetChild(0);

            var barTili = transform.Find("BarTili");
            m_tiliImage = barTili.GetComponentInChildren<Image>();
            m_tiliText = barTili.GetComponentInChildren<Text>();

            m_tiliImage.fillAmount = 0.7f;
        }
    }

    public class FightUISpecialActionController : MonoBehaviour
    {
        public HModeFightManager Owner;
        public UnityEngine.Transform Root;
        public UnityEngine.GameObject ActionTemplate;

        private class ActionWrapper
        {
            public UnityEngine.GameObject Go;
        }

        private List<ActionWrapper> m_currSkillList = new List<ActionWrapper>();

        public void Init(HModeFightManager owner)
        {
            this.Owner = owner;
            Root = gameObject.transform.GetChild(0);
            ActionTemplate = gameObject.transform.Find("ActionTemplate").gameObject;
        }
    }

    public class FightHShowController : MonoBehaviour
    {
        public HModeFightManager Owner;

        public UnityEngine.GameObject m_showImage;
        public Animator m_animator;

        public event Action<bool> EventOnAnimEnd;

        private float m_playTime;
        private float m_timer;


        private bool m_isPlaying;

        public void Init(HModeFightManager owner)
        {
            this.Owner = owner;

            m_showImage = transform.Find("ShowImage").gameObject;
            m_animator = m_showImage.GetComponentInChildren<Animator>();

            m_showImage.SetActive(false);
        }

        public void PlayHAnim(string stateName, Action<bool> onEnd = null)
        {
            if (m_isPlaying)
            {
                // 缓存或暂不处理？
                return;
            }
            EventOnAnimEnd = onEnd;
            m_showImage.SetActive(true);

            PluginMain.Main.LogInfo($"PlayHAnim {stateName}");
            m_animator.Play(stateName, 0, 0f);
            m_isPlaying = true;
            m_timer = 0;
            m_playTime = 1f;
        }

        public void Update()
        {
            if (!m_isPlaying)
            {
                return;
            }
            m_timer += Time.deltaTime;

            var currStateInfo = m_animator.GetCurrentAnimatorStateInfo(0);
            if (currStateInfo.normalizedTime < 1.0)
            {
                return;
            }
            //if (m_timer > m_playTime)
            {
                EventOnAnimEnd?.Invoke(true);
                EventOnAnimEnd = null;
                m_isPlaying = false;
                m_showImage.SetActive(false);
            }
        }
    }
    #endregion



    #region process

    public abstract class FightProcessBase
    {
        public HModeFightManager Owner;
        public ulong m_id;
        public bool m_isEnd;
        public bool m_isStart;

        public FightProcessBase(HModeFightManager owner)
        {
            Owner = owner;
            m_isEnd = false;
            m_isStart = false;
        }

        public virtual void OnStart()
        {
        }
        public abstract void Tick(float dt);

        public event Action EventOnEnd;
        public virtual void OnEnd()
        {
            EventOnEnd?.Invoke();
        }
    }

    public class FightProcessWait : FightProcessBase
    {
        public float m_lastTime;
        public float m_tickTimer;

        public FightProcessWait(HModeFightManager owner, float lastTime) : base(owner)
        {
            m_lastTime = lastTime;
            m_tickTimer = 0;
        }

        public override void Tick(float dt)
        {
            m_tickTimer += dt;
            if (m_tickTimer > m_lastTime)
            {
                m_isEnd = true;
            }
        }
    }

    public class FightProcessWaitHHint : FightProcessWait
    {
        private string m_hintContent;
        public FightProcessWaitHHint(HModeFightManager owner, float lastTime, string hintContent) : base(owner, lastTime)
        {
            m_hintContent = hintContent;
        }

        public override void OnStart()
        {
            base.OnStart();
            UIPopTip.Inst.Pop(m_hintContent, (PopTipIconType)12);
        }
    }

    public class FightProcessWaitAnimation : FightProcessBase
    {
        public string m_animationName;
        public bool m_isLoop;
        public bool m_loopTime;

        private float m_tickTimer;

        public FightProcessWaitAnimation(HModeFightManager owner, string animationName) : base(owner)
        {
            m_animationName = animationName;
            m_tickTimer = 0;
        }

        public override void OnStart()
        {
            base.OnStart();
            Owner.m_fightHud.m_hShowController?.PlayHAnim(m_animationName, OnAnimationEnd);
        }

        public override void Tick(float dt)
        {
            if (!m_isLoop)
            {
                return;
            }
        }

        /// <summary>
        /// 是否播放完成
        /// </summary>
        /// <param name="isFinish"></param>
        public void OnAnimationEnd(bool isFinish)
        {
            m_isEnd = true;
        }
    }

    public class FightProcessImmediate : FightProcessBase
    {
        public FightProcessImmediate(HModeFightManager owner, Action onEnd = null) : base(owner)
        {
            this.EventOnEnd += onEnd;
        }

        public override void OnStart()
        {
            base.OnStart();
        }

        public override void Tick(float dt)
        {
            m_isEnd = true;
        }
    }

    public class FightProcessWaitDialog : FightProcessBase
    {
        protected string m_eventId;
        private DialogEnvironmentEx m_dialogEnv;

        public int Ret1;
        public FightProcessWaitDialog(HModeFightManager owner, string eventId) : base(owner)
        {
            m_dialogEnv = new DialogEnvironmentEx();
            m_eventId = eventId;
        }

        public void SetArg(string argName, int argVal)
        {
            m_dialogEnv.tmpArgs[argName] = argVal;
        }

        public override void OnStart()
        {
            base.OnStart();
            DialogAnalysis.OnDialogComplete += delegate {
                Ret1 = m_dialogEnv.GetInt("Ret1");
                m_isEnd = true;
            };
            DialogAnalysis.StartDialogEvent(m_eventId, m_dialogEnv);
        }

        public override void Tick(float dt)
        {

        }
    }


    #endregion

}
