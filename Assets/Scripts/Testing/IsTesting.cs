public sealed class TestManager
{
    public bool isTesting = false;

    // Lock Object for thread safety
    private static readonly object instanceLock = new object();
    private static TestManager instance = null;

    // Init
    private TestManager() { }

    public static TestManager Instance
    {
        get
        {
            if (instance == null)
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new TestManager();
                    }
                }
            }
            return instance;
        }
    }
}
