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

            YandexGame.savesData.Saves = array.ToString();
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

            var jsonArray = JSONNode.Parse(save).AsArray;
            
            if (jsonArray == null) return;

            foreach (var kv in jsonArray)
            {
                foreach (var (key, node) in kv.Value)
                {
                    var saveItem = saves.Find(s => s.SaveKey == key);
                    if (saveItem == null) continue;

                    Deserialize(saveItem, node.ToString());
                }
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