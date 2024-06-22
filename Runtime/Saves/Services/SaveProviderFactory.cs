namespace Module.InteractiveEditor.Saves
{
    public static class SaveProviderFactory
    {
        public static SaveProvider Create()
        {
            
#if UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE
            return new CommonSaveProvider();
#endif

#if UNITY_WEBGL || UNITY_EDITOR
            return new YandexGameSaveProvider();
#endif
            return new CommonSaveProvider();
        }
    }
}