using HarmonyLib;
using KBEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstBepinPlugin.Patch
{
    /// <summary>
    /// 对话事件扩展
    /// </summary>
    [HarmonyPatch(typeof(Avatar), "AddTime")]
    public class AvatarPatcher
    {
        [HarmonyPostfix]
        public static void AddTime(Avatar __instance, int addday, int addMonth, int Addyear)
        {
            SecretsSystem.Instance.AbsorbByTime(addday);
        }
    }
}
