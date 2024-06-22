using Cysharp.Threading.Tasks;

namespace Module.InteractiveEditor.Saves
{
    public abstract class SaveProvider
    {
        internal abstract void Add(ISavable savable);
        
        internal abstract UniTask Save();
        internal abstract UniTask LoadAsync();
    }
}