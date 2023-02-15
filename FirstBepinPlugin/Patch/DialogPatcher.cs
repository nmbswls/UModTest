using FirstBepinPlugin.Config;
using HarmonyLib;
using SkySwordKill.Next;
using SkySwordKill.Next.DialogEvent;
using SkySwordKill.Next.DialogSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstBepinPlugin.Patch
{
    [DialogEvent("MyCustom")]
    public class MyCustom : IDialogEvent
    {
        public void Execute(DialogCommand command, DialogEnvironment env, Action callback)
        {
            Main.LogInfo("fucking get info" + SecretsSystem.Instance.m_SecretsInfo.Level);
            

            callback?.Invoke();
        }
    }

    [DialogEvent("SecretJieSuan")]
    public class SecretJieSuan : IDialogEvent
    {
        public void Execute(DialogCommand command, DialogEnvironment env, Action callback)
        {
            //Tools.instance.getPlayer().addItem(id, count, null, showText);
            int partType = command.GetInt(0);
            int addNum = command.GetInt(1);

            SecretsSystem.Instance.AddJingYuan((EnumPartType)partType, addNum);

            callback?.Invoke();
        }
    }


    [DialogEvent("FightEndCallback")]
    public class FightEndCallback : IDialogEvent
    {
        public void Execute(DialogCommand command, DialogEnvironment env, Action callback)
        {

            env.tmpArgs.TryGetValue("callbackParam", out var callbackParam);
            if(SecretsSystem.FightManager.IsInBattle)
            {
                SecretsSystem.FightManager.OnFightTalkFinish(callbackParam);
            }
            callback?.Invoke();
        }
    }

    /// <summary>
    /// 对话事件扩展
    /// </summary>
    [HarmonyPatch(typeof(DialogAnalysis), "Init")]
    public class DialogAnalysisPatch
    {
        [HarmonyPostfix]
        public static void InitEx()
        {
            Main.LogInfo("InitExInitExInitExInitExInitExInitExInitExInitExInitExInitExInitExInitExInitExInitExInitExInitExInitExInitExInitExInitExInitExInitExInitEx");
            DialogAnalysis.RegisterCommand("MyCustom", Activator.CreateInstance(typeof(MyCustom)) as IDialogEvent);
            DialogAnalysis.RegisterCommand("SecretJieSuan", Activator.CreateInstance(typeof(SecretJieSuan)) as IDialogEvent);
            DialogAnalysis.RegisterCommand("FightEndCallback", Activator.CreateInstance(typeof(FightEndCallback)) as IDialogEvent);
            
        }
    }
}
