namespace Module.InteractiveEditor.Saves
{
    public static class SaveProviderFactory
    {
        public static SaveProvider Create()
        {
            
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE
            return new CommonSaveProvider();

#endif
            return new CommonSaveProvider();
        }
    }
}