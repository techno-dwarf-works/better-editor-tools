namespace Better.EditorTools.SettingsTools
{
    public class BetterSettingsToolsContainer<THandler> where THandler : new()
    {
        private static THandler _instance;

        public static THandler Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new THandler();
                }

                return _instance;
            }
        }
    }
}