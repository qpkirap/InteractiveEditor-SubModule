using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using SimpleJSON;

namespace Module.InteractiveEditor.Saves
{
    public class CommonSaveProvider : SaveProvider
    {
        private readonly List<ISavable> saves = new();
        
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
                
                var serializeString = JsonConvert.SerializeObject(savable);

                var jsonObject = new JSONObject
                {
                    [savable.SaveKey] = JSONNode.Parse(serializeString)
                };
                
                array.Add(jsonObject);
            }

            var save = array.ToString();
        }

        internal override async UniTask Load()
        {
            foreach (var savable in saves)
            {
                if (savable == null) continue;

                var save = "asdf";
                
                JsonConvert.PopulateObject(save, savable);
            }
        }
    }
}