using System;

namespace Module.InteractiveEditor.Saves
{
    public interface ISavable : IEquatable<ISavable>
    {
        public string SaveKey { get; }
        public void PostLoad();
    }
}