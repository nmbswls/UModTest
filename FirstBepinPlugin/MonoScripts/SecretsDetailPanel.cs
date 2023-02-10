using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace FirstBepinPlugin.MonoScripts
{
    public class SecretsDetailPanel : MonoBehaviour, IESCClose
    {
        public UINPCJiaoHu Owner;
        public RectTransform RT;

        public Button CloseBtn;
        public Text Info1;
        public Text Info2;
        public Text Info3;

        public void Awake()
        {
            RT = GetComponent<RectTransform>();

            CloseBtn = transform.Find("Close").GetComponent<Button>();
            CloseBtn.onClick.AddListener(delegate ()
            {
                TryEscClose();
            });
        }

        public void Show()
        {

            var npcData = UINPCJiaoHu.Inst.NowJiaoHuNPC;
            PluginMain.Main.LogInfo("now show " + npcData.ID);


            gameObject.SetActive(value: true);
            Owner.CommonMask.gameObject.SetActive(value: true);
            
            ESCCloseManager.Inst.RegisterClose(this);
        }

        public void OnDestroy()
        {
            if(SecretsSystem.Instance.panel == this)
            {
                SecretsSystem.Instance.panel = null;
            }
        }

        public bool TryEscClose()
        {
            gameObject.SetActive(value: false);
            Owner.CommonMask.gameObject.SetActive(value: false);
            ESCCloseManager.Inst.UnRegisterClose(this);

            return true;
        }
    }
}
