using Module.InteractiveEditor.Configs;
using UnityEngine.UIElements;

namespace Module.InteractiveEditor.Editor
{
    public class EpisodeViewItem : Button
    {
        public EpisodeData EpisodeData { get; private set; }
        
        public EpisodeViewItem()
        {
            AddToClassList("episode-item");
        }

        public void InjectData(EpisodeData episodeData)
        {
            this.EpisodeData = episodeData;
            
            text = episodeData.Title;
        }
    }
}