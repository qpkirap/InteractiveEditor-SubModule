using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using SimpleJSON;
using YG;

namespace Module.InteractiveEditor.Saves
{
    public class YandexGameSaveProvider : SaveProvider
    {
        private readonly List<ISavable> saves = new();
        private readonly Dictionary<string, string> saveCache = new();
        
        private readonly JsonSerializerSettings serializerSettings;

        public YandexGameSaveProvider()
        {
            serializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects
            };
        }
        
        internal override void Add(ISavable savable)
        {
            if (savable == null || saves.Contains(savable)) return;
            
            saves.Add(savable);
        }

        internal override async UniTask Save()
        {
            var array = new JSONArray();
            
            foreach (var savable in saves)
            {
                if (savable == null) continue;

                var serializeString = Serialize(savable);

                var jsonObject = new JSONObject
                {
                    [savable.SaveKey] = JSONNode.Parse(serializeString)
                };
                
                array.Add(jsonObject);
            }

            var save = array.ToString();

            YandexGame.savesData.SaveNodes = save;
            
            YandexGame.SaveProgress();
        }

        internal override async UniTask Load()
        {
            if (!YandexGame.SDKEnabled)
            {
                await UniTask.WaitUntil(() => YandexGame.SDKEnabled);
            }
            
            var save = YandexGame.savesData.SaveNodes;

            if (string.IsNullOrEmpty(save)) return;
            
            JsonConvert.PopulateObject(save, saves); //не будет работать скорее всего
        }
        
        private string Serialize(ISavable state)
        {
            return JsonConvert.SerializeObject(state, serializerSettings);
        }

        private void Deserialize(ISavable savable, string save)
        {
            JsonConvert.PopulateObject(save, savable, serializerSettings);
        }
    }
}