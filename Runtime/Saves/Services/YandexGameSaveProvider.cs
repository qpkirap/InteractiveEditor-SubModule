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
        
        private readonly JsonSerializerSettings serializerSettings = new()
        {
            TypeNameHandling = TypeNameHandling.Objects
        };

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

            YandexGame.savesData.Saves = save;
            
            YandexGame.SaveProgress();
        }

        internal override async UniTask LoadAsync()
        {
            if (!YandexGame.SDKEnabled)
            {
                await UniTask.WaitUntil(() => YandexGame.SDKEnabled);
            }
            
            var save = YandexGame.savesData.Saves;

            if (string.IsNullOrEmpty(save)) return;

            var jsonArray = JSONArray.Parse(save) as JSONArray;
            
            if (jsonArray == null) return;
            
            foreach (var savable in saves)
            {
                if (savable == default) continue;

                JSONNode saveNode = null;
                
                foreach (var kvArrayItem in jsonArray)
                {
                    if (!kvArrayItem.Value.HasKey(savable.SaveKey)) continue;
                    
                    saveNode = kvArrayItem.Value;
                    
                    break;
                }
                    
                Deserialize(savable, saveNode.ToString());
            }
            
            foreach (var savable in saves)
            {
                if (savable == null) continue;
                
                savable.PostLoad();
            }
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