//
// Auto Generated Code By excel2json
// 1. 定义Config Loader 提供加载方法


using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;
using My.Framework.ConfigData;
namespace My.ConfigData {

    public partial class ConfigDataLoader : ConfigDataLoaderBase
    {

        protected override HashSet<string> GetAllInitLoadConfigDataAssetPath()
        {

            var set = new HashSet<string>();

            set.Add($"HAttackPoolInfo");

            set.Add($"HAttackInfo");

            set.Add($"HAttackConditionInfo");

            set.Add($"HPartFightInfo");

            return set;
        }
        protected override void InitDeserializFunc4ConfigData()
        {

            m_deserializFuncDict["HAttackPoolInfo"] = DeserializFunc4ConfigDataHAttackPoolInfo;

            m_deserializFuncDict["HAttackInfo"] = DeserializFunc4ConfigDataHAttackInfo;

            m_deserializFuncDict["HAttackConditionInfo"] = DeserializFunc4ConfigDataHAttackConditionInfo;

            m_deserializFuncDict["HPartFightInfo"] = DeserializFunc4ConfigDataHPartFightInfo;

            m_deserializFuncDict["MultiLangInfo_CN"] = DeserializFunc4ConfigDataMultiLangInfo_CN;

            m_deserializFuncDict["MultiLangInfo_EN"] = DeserializFunc4ConfigDataMultiLangInfo_EN;

        }
        protected override Dictionary<string, LazyLoadConfigAssetInfo> GetAllLazyLoadConfigDataAssetPath()
        {

            Dictionary<string, LazyLoadConfigAssetInfo> assetInfoDict = new Dictionary<string, LazyLoadConfigAssetInfo>();
            LazyLoadConfigAssetInfo assetInfo;

            assetInfo = new LazyLoadConfigAssetInfo();
            assetInfo.m_configAssetName = $"HAttackPoolInfo";
            assetInfo.m_state = LazyLoadConfigAsssetState.Unload;
            assetInfoDict.Add("HAttackPoolInfo", assetInfo);

            assetInfo = new LazyLoadConfigAssetInfo();
            assetInfo.m_configAssetName = $"HAttackInfo";
            assetInfo.m_state = LazyLoadConfigAsssetState.Unload;
            assetInfoDict.Add("HAttackInfo", assetInfo);

            assetInfo = new LazyLoadConfigAssetInfo();
            assetInfo.m_configAssetName = $"HAttackConditionInfo";
            assetInfo.m_state = LazyLoadConfigAsssetState.Unload;
            assetInfoDict.Add("HAttackConditionInfo", assetInfo);

            assetInfo = new LazyLoadConfigAssetInfo();
            assetInfo.m_configAssetName = $"HPartFightInfo";
            assetInfo.m_state = LazyLoadConfigAsssetState.Unload;
            assetInfoDict.Add("HPartFightInfo", assetInfo);

            assetInfo = new LazyLoadConfigAssetInfo();
            assetInfo.m_configAssetName = $"MultiLangInfo_CN";
            assetInfo.m_state = LazyLoadConfigAsssetState.Unload;
            assetInfoDict.Add("MultiLangInfo_CN", assetInfo);

            assetInfo = new LazyLoadConfigAssetInfo();
            assetInfo.m_configAssetName = $"MultiLangInfo_EN";
            assetInfo.m_state = LazyLoadConfigAsssetState.Unload;
            assetInfoDict.Add("MultiLangInfo_EN", assetInfo);

            return assetInfoDict;
        }
        public void DeserializFunc4ConfigDataHAttackPoolInfo(string content)
        {
            var data = JsonConvert.DeserializeObject<Dictionary<int, ConfigDataHAttackPoolInfo>>(content);
            if(data != null)
            {
                foreach(var pair in data)
                {
                    m_ConfigDataHAttackPoolInfoData[pair.Key] = pair.Value;
                }
            }
        }
        public void DeserializFunc4ConfigDataHAttackInfo(string content)
        {
            var data = JsonConvert.DeserializeObject<Dictionary<int, ConfigDataHAttackInfo>>(content);
            if(data != null)
            {
                foreach(var pair in data)
                {
                    m_ConfigDataHAttackInfoData[pair.Key] = pair.Value;
                }
            }
        }
        public void DeserializFunc4ConfigDataHAttackConditionInfo(string content)
        {
            var data = JsonConvert.DeserializeObject<Dictionary<int, ConfigDataHAttackConditionInfo>>(content);
            if(data != null)
            {
                foreach(var pair in data)
                {
                    m_ConfigDataHAttackConditionInfoData[pair.Key] = pair.Value;
                }
            }
        }
        public void DeserializFunc4ConfigDataHPartFightInfo(string content)
        {
            var data = JsonConvert.DeserializeObject<Dictionary<int, ConfigDataHPartFightInfo>>(content);
            if(data != null)
            {
                foreach(var pair in data)
                {
                    m_ConfigDataHPartFightInfoData[pair.Key] = pair.Value;
                }
            }
        }
        public void DeserializFunc4ConfigDataMultiLangInfo_CN(string content)
        {
            var data = JsonConvert.DeserializeObject<Dictionary<int, ConfigDataMultiLangInfo_CN>>(content);
            if(data != null)
            {
                foreach(var pair in data)
                {
                    m_ConfigDataMultiLangInfo_CNData[pair.Key] = pair.Value;
                }
            }
        }
        public void DeserializFunc4ConfigDataMultiLangInfo_EN(string content)
        {
            var data = JsonConvert.DeserializeObject<Dictionary<int, ConfigDataMultiLangInfo_EN>>(content);
            if(data != null)
            {
                foreach(var pair in data)
                {
                    m_ConfigDataMultiLangInfo_ENData[pair.Key] = pair.Value;
                }
            }
        }


#region 获取方法


        public ConfigDataHAttackPoolInfo GetConfigDataHAttackPoolInfo(int key)
        {
            ConfigDataHAttackPoolInfo data;
            if(m_ConfigDataHAttackPoolInfoData.TryGetValue(key, out data))
            {
                return data;
            }
            return null;
        }


        public Dictionary<int,ConfigDataHAttackPoolInfo> GetAllConfigDataHAttackPoolInfo()
        {
            return m_ConfigDataHAttackPoolInfoData;
        }


        public void ClearConfigDataHAttackPoolInfo()
        {
            m_ConfigDataHAttackPoolInfoData.Clear();
        }


        public ConfigDataHAttackInfo GetConfigDataHAttackInfo(int key)
        {
            ConfigDataHAttackInfo data;
            if(m_ConfigDataHAttackInfoData.TryGetValue(key, out data))
            {
                return data;
            }
            return null;
        }


        public Dictionary<int,ConfigDataHAttackInfo> GetAllConfigDataHAttackInfo()
        {
            return m_ConfigDataHAttackInfoData;
        }


        public void ClearConfigDataHAttackInfo()
        {
            m_ConfigDataHAttackInfoData.Clear();
        }


        public ConfigDataHAttackConditionInfo GetConfigDataHAttackConditionInfo(int key)
        {
            ConfigDataHAttackConditionInfo data;
            if(m_ConfigDataHAttackConditionInfoData.TryGetValue(key, out data))
            {
                return data;
            }
            return null;
        }


        public Dictionary<int,ConfigDataHAttackConditionInfo> GetAllConfigDataHAttackConditionInfo()
        {
            return m_ConfigDataHAttackConditionInfoData;
        }


        public void ClearConfigDataHAttackConditionInfo()
        {
            m_ConfigDataHAttackConditionInfoData.Clear();
        }


        public ConfigDataHPartFightInfo GetConfigDataHPartFightInfo(int key)
        {
            ConfigDataHPartFightInfo data;
            if(m_ConfigDataHPartFightInfoData.TryGetValue(key, out data))
            {
                return data;
            }
            return null;
        }


        public Dictionary<int,ConfigDataHPartFightInfo> GetAllConfigDataHPartFightInfo()
        {
            return m_ConfigDataHPartFightInfoData;
        }


        public void ClearConfigDataHPartFightInfo()
        {
            m_ConfigDataHPartFightInfoData.Clear();
        }


        public ConfigDataMultiLangInfo_CN GetConfigDataMultiLangInfo_CN(int key)
        {
            ConfigDataMultiLangInfo_CN data;
            if(m_ConfigDataMultiLangInfo_CNData.TryGetValue(key, out data))
            {
                return data;
            }
            return null;
        }


        public Dictionary<int,ConfigDataMultiLangInfo_CN> GetAllConfigDataMultiLangInfo_CN()
        {
            return m_ConfigDataMultiLangInfo_CNData;
        }


        public void ClearConfigDataMultiLangInfo_CN()
        {
            m_ConfigDataMultiLangInfo_CNData.Clear();
        }


        public ConfigDataMultiLangInfo_EN GetConfigDataMultiLangInfo_EN(int key)
        {
            ConfigDataMultiLangInfo_EN data;
            if(m_ConfigDataMultiLangInfo_ENData.TryGetValue(key, out data))
            {
                return data;
            }
            return null;
        }


        public Dictionary<int,ConfigDataMultiLangInfo_EN> GetAllConfigDataMultiLangInfo_EN()
        {
            return m_ConfigDataMultiLangInfo_ENData;
        }


        public void ClearConfigDataMultiLangInfo_EN()
        {
            m_ConfigDataMultiLangInfo_ENData.Clear();
        }



#endregion


#region 数据定义

        private Dictionary<int, ConfigDataHAttackPoolInfo> m_ConfigDataHAttackPoolInfoData = new Dictionary<int, ConfigDataHAttackPoolInfo>();

        private Dictionary<int, ConfigDataHAttackInfo> m_ConfigDataHAttackInfoData = new Dictionary<int, ConfigDataHAttackInfo>();

        private Dictionary<int, ConfigDataHAttackConditionInfo> m_ConfigDataHAttackConditionInfoData = new Dictionary<int, ConfigDataHAttackConditionInfo>();

        private Dictionary<int, ConfigDataHPartFightInfo> m_ConfigDataHPartFightInfoData = new Dictionary<int, ConfigDataHPartFightInfo>();

        private Dictionary<int, ConfigDataMultiLangInfo_CN> m_ConfigDataMultiLangInfo_CNData = new Dictionary<int, ConfigDataMultiLangInfo_CN>();

        private Dictionary<int, ConfigDataMultiLangInfo_EN> m_ConfigDataMultiLangInfo_ENData = new Dictionary<int, ConfigDataMultiLangInfo_EN>();

#endregion


    }



    public class StringTableManager : StringTableManagerBase
    {
        public StringTableManager(ConfigDataLoader configLoader):base(configLoader)
        {
            m_localizationList.Add("CN");
            m_localizationList.Add("EN");
        }
        protected override void InitDefaultStringTable()
        {
            switch (m_currLocalization)
            {

                case "CN":
                {
                    string configDataName = "MultiLangInfo_CN";
                    bool res = m_configLoader.LoadLazyLoadConfigDataAsset(configDataName);
                }
                break;

                case "EN":
                {
                    string configDataName = "MultiLangInfo_EN";
                    bool res = m_configLoader.LoadLazyLoadConfigDataAsset(configDataName);
                }
                break;

            }
        }
        protected override void ClearCurrentLocalizeion()
        {
            if(m_currLocalization == null)
            {
                m_currLocalization = "";
                return;
            }

            switch (m_currLocalization)
            {

                case "CN":
                {
                    string configDataName = "MultiLangInfo_CN";
                    var assetInfo = m_configLoader.GetLazyLoadConfigAssetInfo(configDataName);
                    if (assetInfo != null) assetInfo.m_state = ConfigDataLoaderBase.LazyLoadConfigAsssetState.Unload;
                    ((ConfigDataLoader)m_configLoader).ClearConfigDataMultiLangInfo_CN();
                }
                break;

                case "EN":
                {
                    string configDataName = "MultiLangInfo_EN";
                    var assetInfo = m_configLoader.GetLazyLoadConfigAssetInfo(configDataName);
                    if (assetInfo != null) assetInfo.m_state = ConfigDataLoaderBase.LazyLoadConfigAsssetState.Unload;
                    ((ConfigDataLoader)m_configLoader).ClearConfigDataMultiLangInfo_EN();
                }
                break;

            }
        }
        public string GetStringInStringTable(int keyId)
        {
            return GetStringInST(keyId);
        }
        public string GetStringInST(int keyId)
        {
            switch (m_currLocalization)
            {

                case "CN":
                {
                    var configData = ((ConfigDataLoader)m_configLoader).GetConfigDataMultiLangInfo_CN(keyId);
                    if(configData != null)
                        return configData.ReplaceContent;
                }
                break;

                case "EN":
                {
                    var configData = ((ConfigDataLoader)m_configLoader).GetConfigDataMultiLangInfo_EN(keyId);
                    if(configData != null)
                        return configData.ReplaceContent;
                }
                break;

            }
            return "";
        }
    }
}


// End of Auto Generated Code
