using DG.Tweening;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class OpponentsUI : NetworkBehaviour
{
    [SerializeField] private TMPro.TMP_Text movementCardsCount;
    [SerializeField] private TMPro.TMP_Text chestCardsCount;
    [SerializeField] private TMPro.TMP_Text coinsCount;
    [SerializeField] private TMPro.TMP_Text ressourcesCount;
    [SerializeField] private TMPro.TMP_Text safeCount;
    [SerializeField] private TMPro.TMP_Text remainingMovesCount;
    [SerializeField] private Button showButton;

    private Vector3 initialPosition;
    private Player _player;
    private RectTransform _rectTransform;

    private bool isHidden = true;

    public Player Player { get => _player; set => _player = value; }

    public void Initialize(Player player)
    {
        Debug.Log("OpponentsUI.Initialize was called", player);
        Player = player;
        player.moveCount.OnValueChanged += UpdateRemainingMovesText;
        player.movementCards.OnListChanged += UpdateMovementCardsText;
        player.inventoryChestCards.OnListChanged += UpdateChestCardsText;
        player.coinCount.OnValueChanged += UpdateCoinsText;
        player.inventoryRessources.OnListChanged += UpdateRessourcesText;
        player.savedRessources.OnListChanged += UpdateSafeText;
        initialPosition = showButton.transform.localPosition;
        showButton.onClick.AddListener(MoveCharBar);
        _rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        //movementCardsCount.text = Player.movementCards.Count.ToString();
    }

    private void MoveCharBar()
    {
        Debug.Log("OpponentsUI.OnMouseDown isShowing: " + isHidden);
        Debug.Log("OpponentsUI.OnMouseDown initialPosition: " + initialPosition);
        if (isHidden)
        {
            isHidden = false;
            _rectTransform.DOBlendableLocalMoveBy(new Vector2(-409, 0), 0.2f);
            //showButton.transform.localPosition = showButton.transform.localPosition - new Vector3(409, 0, 0); ;
            Debug.Log("OpponentsUI.OnMouseDown currentPosition: " + transform.position);
        }
        else
        {
            isHidden = true;
            _rectTransform.DOBlendableLocalMoveBy(new Vector2(409, 0), 0.2f);
            //showButton.transform.localPosition = showButton.transform.localPosition + new Vector3(409, 0, 0);
            Debug.Log("OpponentsUI.OnMouseDown currentPosition: " + showButton.transform.localPosition);
        }
    }

    private void UpdateSafeText(NetworkListEvent<int> changeEvent)
    {
        Debug.Log("OpponentsUI.UpdateSafeText changeEvent: " + changeEvent.Type);
        safeCount.text = Player.savedRessourceCount.ToString();
    }

    private void UpdateRessourcesText(NetworkListEvent<int> changeEvent)
    {
        //Debug.Log("OpponentsUI.UpdateRessourcesText changeEvent: " + changeEvent.Type);
        ressourcesCount.text = Player.inventoryRessourceCount.ToString();
    }

    private void UpdateCoinsText(int previousValue, int newValue)
    {
        coinsCount.text = newValue.ToString();
    }

    private void UpdateChestCardsText(NetworkListEvent<int> changeEvent)
    {
        Debug.Log("OpponentsUI.UpdateChestCardsText changeEvent: " + changeEvent.Type);
        chestCardsCount.text = Player.inventoryChestCards.Count.ToString();
    }

    private void UpdateMovementCardsText(NetworkListEvent<int> changeEvent)
    {
        Debug.Log("OpponentsUI.UpdateMovementCardsText changeEvent: " + changeEvent.Type);
        movementCardsCount.text = Player.movementCards.Count.ToString();
    }

    private void UpdateRemainingMovesText(int previousValue, int newValue)
    {
        Debug.Log("OpponentsUI.UpdateRemainingMovesText was called");
        remainingMovesCount.text = newValue.ToString();
    }
}
