using UnityEngine;
using UnityEngine.UI;
public class MyPlayerUI : UIManager
{
    [SerializeField] private TMPro.TMP_Text _myMoveCountText;
    [SerializeField] private Button _hostButton;
    [SerializeField] private VerticalLayoutGroup _oponentsLayoutGroup;
    public TMPro.TMP_Text playerIdText;
    public TMPro.TMP_Text currentTurnPlayerIdText;
    public TMPro.TMP_Text currentTurnPlayerMovesText;

    public Text myMovesText;
    private static MyPlayerUI s_instance;
    // private List<OponentsBar> _cardUis = new List<OponentsBar>();
    // private List<Player> _cards = new List<Player>();
    public VerticalLayoutGroup OponentsLayoutGroup { get => _oponentsLayoutGroup; set => _oponentsLayoutGroup = value; }
    // Singleton
    public static MyPlayerUI Instance { get { return s_instance; } }

    private void Awake()
    {
        // Singleton Pattern
        if (s_instance != null && s_instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            s_instance = this;
        }
        Player.MoveCount.ToString();
    }

    private void UpdateMyMoveCounter()
    {
        myMovesText.text = Player.MoveCount.ToString();
    }
}
