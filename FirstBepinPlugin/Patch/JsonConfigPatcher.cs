using Fungus;
using HarmonyLib;
using JSONClass;
using KBEngine;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstBepinPlugin.Patch
{

    /// <summary>
    /// 配置读取扩展 拓展额外的seid位 防止冲突
    /// </summary>
    [HarmonyPatch(typeof(jsonData), "InitLogic")]
    public class jsonDataPatcher_InitLogic
    {
        public static void Prefix(jsonData __instance)
        {
            if(__instance.SkillSeidJsonData == null || __instance.SkillSeidJsonData.Length < 600)
            {
                __instance.SkillSeidJsonData = new JSONObject[600];
            }
            if (__instance.BuffSeidJsonData == null || __instance.BuffSeidJsonData.Length < 600)
            {
                __instance.BuffSeidJsonData = new JSONObject[600];
            }
        }
    }


    /// <summary>
    /// 配置读取扩展 在读取特定json时 追加数据
    /// </summary>
    [HarmonyPatch(typeof(jsonData), "init")]
    public class jsonDataPatcher_init
    {
        public static void Postfix(jsonData __instance, string path, JSONObject jsondata)
        {
            // 追加技能
            if(jsondata == __instance._skillJsonData)
            {
                try
                {
                    // 初始化 SkillEx
                    var fileNameSkillEx = PluginMain.Main.Path + "/PatchConfig/Skill.json";
                    var jsonstringSkillEx = File.ReadAllText(fileNameSkillEx);
                    var jsondataSkillEx = new JSONObject(jsonstringSkillEx);

                    jsondata.Merge(jsondataSkillEx);

                    PluginMain.Main.LogError("jsonConfig init Skill Success. " + jsondata.Count);
                }
                catch (Exception e)
                {
                    PluginMain.Main.LogError("jsonConfig InitLogic Skill Fail ");
                }
            }

            // 追加Buff
            if(jsondata == __instance._BuffJsonData)
            {
                try
                {
                    // 初始化 BuffEx
                    var fileNameBuffEx = PluginMain.Main.Path + "/PatchConfig/Buff.json";
                    var jsonstringBuffEx = File.ReadAllText(fileNameBuffEx);
                    var jsondataBuffEx = new JSONObject(jsonstringBuffEx);

                    jsondata.Merge(jsondataBuffEx);

                    PluginMain.Main.LogError("jsonConfig init Buff Success." + jsondata.Count);
                }
                catch (Exception e)
                {
                    PluginMain.Main.LogError("jsonConfig init Buff Fail ");
                }
            }

            // 追加Item
            if (jsondata == __instance._ItemJsonData)
            {
                try
                {
                    // 初始化 ItemEx
                    var fileNameItemEx = PluginMain.Main.Path + "/PatchConfig/Item.json";
                    var jsonstringItemEx = File.ReadAllText(fileNameItemEx);
                    var jsondataItemEx = new JSONObject(jsonstringItemEx);

                    jsondata.Merge(jsondataItemEx);

                    PluginMain.Main.LogError("jsonConfig init Item Success." + jsondata.Count);
                }
                catch (Exception e)
                {
                    PluginMain.Main.LogError("jsonConfig init Item Fail ");
                }
            }


            // 追加炼器相关配置
            if (jsondata == __instance.LianQiHeCheng)
            {
                try
                {
                    // 初始化 LianQiHeCheng
                    var fileNameLianQiHeCheng = PluginMain.Main.Path + "/PatchConfig/LianQiHeCheng.json";
                    var jsonLianQiHeCheng = File.ReadAllText(fileNameLianQiHeCheng);
                    var jsondataLianQiHeCheng = new JSONObject(jsonLianQiHeCheng);

                    jsondata.Merge(jsondataLianQiHeCheng);
                }
                catch (Exception e)
                {
                    PluginMain.Main.LogError("jsonConfig init LianQiHeCheng Fail.");
                }
            }

            

        }
    }

    /// <summary>
    /// 配置读取扩展 在读取特定json时 追加数据
    /// </summary>
    [HarmonyPatch(typeof(jsonData), "InitJObject")]
    public class jsonDataPatcher_InitJObject
    {
        public static void Postfix(jsonData __instance, string path, JObject jsondata)
        {
            if(jsondata == __instance.LianQiEquipType)
            {
                // 追加炼器相关配置
                try
                {
                    // 初始化 LianQiEquipType
                    var fileNameLianQiEquipType = PluginMain.Main.Path + "/PatchConfig/LianQiEquipType.json";
                    var jsonLianQiEquipType = File.ReadAllText(fileNameLianQiEquipType);
                    var jsondataLianQiEquipType = JObject.Parse(jsonLianQiEquipType);

                    jsondata.Merge(jsondataLianQiEquipType);
                }
                catch (Exception e)
                {
                    PluginMain.Main.LogError("jsonConfig InitJObject LianQiEquipType Fail.");
                }
            }
        }
    }
    


    /// <summary>
    /// 配置读取扩展 追加seid扩展数据
    /// </summary>
    [HarmonyPatch(typeof(jsonData), "initSkillSeid")]
    public class jsonDataPatcher_initSkillSeid
    {
        public static void Postfix(jsonData __instance)
        {
            try
            {
                // 初始化 SkillJsonSeid
                DirectoryInfo d = new DirectoryInfo(PluginMain.Main.Path + "/PatchConfig/SkillSeidJsonData/");
                if (d.Exists)
                {
                    foreach (var fInfo in d.GetFiles("*"))
                    {
                        if (fInfo.Extension != ".json")
                        {
                            continue;
                        }
                        int.TryParse(fInfo.Name.Substring(0, fInfo.Name.Length - ".json".Length), out var seid);

                        var jsonSkillSeid = File.ReadAllText(fInfo.FullName);
                        var jsondataSkillSeid = new JSONObject(jsonSkillSeid);

                        if(jsonData.instance.SkillSeidJsonData[seid] == null)
                        {
                            jsonData.instance.SkillSeidJsonData[seid] = jsondataSkillSeid;
                        }
                        else
                        {
                            jsonData.instance.SkillSeidJsonData[seid].Merge(jsondataSkillSeid);
                        }
                    }
                }

            }
            catch (Exception e)
            {
                PluginMain.Main.LogError($"jsonConfig initSkillSeid Fail : {e.Message}");
            }
        }
    }

    /// <summary>
    /// 配置读取扩展 追加seid扩展数据
    /// </summary>
    [HarmonyPatch(typeof(jsonData), "initBuffSeid")]
    public class jsonDataPatcher_initBuffSeid
    {
        public static void Postfix(jsonData __instance)
        {
            try
            {
                // 初始化 BuffJsonSeid
                DirectoryInfo d = new DirectoryInfo(PluginMain.Main.Path + "/PatchConfig/BuffSeidJsonData/");
                if (d.Exists)
                {
                    foreach (var fInfo in d.GetFiles("*"))
                    {
                        if (fInfo.Extension != ".json")
                        {
                            continue;
                        }
                        int.TryParse(fInfo.Name.Substring(0, fInfo.Name.Length - ".json".Length), out var seid);

                        var jsonBuffSeid = File.ReadAllText(fInfo.FullName);
                        var jsondataBuffSeid = new JSONObject(jsonBuffSeid);

                        if (jsonData.instance.BuffSeidJsonData[seid] == null)
                        {
                            jsonData.instance.BuffSeidJsonData[seid] = jsondataBuffSeid;
                        }
                        else
                        {
                            jsonData.instance.BuffSeidJsonData[seid].Merge(jsondataBuffSeid);
                        }
                    }
                }

            }
            catch (Exception e)
            {
                PluginMain.Main.LogError($"jsonConfig initBuffSeid Fail : {e.Message}");
            }
        }
    }


    /// <summary>
    /// 配置读取扩展 追加seid扩展数据
    /// </summary>
    [HarmonyPatch(typeof(jsonData), "initEquipSeid")]
    public class jsonDataPatcher_initEquipSeid
    {
        public static void Postfix(jsonData __instance)
        {
            try
            {
                // 初始化 EquipJsonSeid
                DirectoryInfo d = new DirectoryInfo(PluginMain.Main.Path + "/PatchConfig/EquipSeidJsonData/");
                if (d.Exists)
                {
                    foreach (var fInfo in d.GetFiles("*"))
                    {
                        if (fInfo.Extension != ".json")
                        {
                            continue;
                        }
                        int.TryParse(fInfo.Name.Substring(0, fInfo.Name.Length - ".json".Length), out var seid);

                        var jsonEquipSeid = File.ReadAllText(fInfo.FullName);
                        var jsondataEquipSeid = new JSONObject(jsonEquipSeid);

                        if (jsonData.instance.EquipSeidJsonData[seid] == null)
                        {
                            jsonData.instance.EquipSeidJsonData[seid] = jsondataEquipSeid;
                        }
                        else
                        {
                            jsonData.instance.EquipSeidJsonData[seid].Merge(jsondataEquipSeid);
                        }

                    }
                }

            }
            catch (Exception e)
            {
                PluginMain.Main.LogError($"jsonConfig initEquipSeid Fail : {e.Message}");
            }
        }
    }

    /// <summary>
    /// 配置读取扩展
    /// </summary>
    [HarmonyPatch(typeof(YSJSONHelper), "InitJSONClassData")]
    public class YSJSONHelperPatcher_InitJSONClassData
    {
        public static void Prefix()
        {
            //try
            //{
            //    // 初始化 Item
            //    var fileNameItem = PluginMain.Main.Path + "/PatchConfig/Item.json";
            //    var jsonItem = File.ReadAllText(fileNameItem);
            //    var jsondataItem = new JSONObject(jsonItem);

            //    jsonData.instance._ItemJsonData.Merge(jsondataItem);
            //}
            //catch (Exception e)
            //{
            //    PluginMain.Main.LogError("jsonConfig InitLogic ItemJsonData Fail 2.");
            //}

            //// 再刷一遍 不会duplicate错
            //jsonData.instance.setYSDictionor(jsonData.instance._ItemJsonData, jsonData.instance.ItemJsonData);
            //jsonData.instance.setYSDictionor(jsonData.instance._BuffJsonData, jsonData.instance.BuffJsonData);
            //jsonData.instance.setYSDictionor(jsonData.instance._skillJsonData, jsonData.instance.skillJsonData);
        }


    }


}
