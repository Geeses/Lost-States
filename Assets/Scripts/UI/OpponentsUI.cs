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

    private bool isHidden = true;
    public void Initialize(Player player)
    {
        Debug.Log("OpponentsUI.Initialize was called");
        player.moveCount.OnValueChanged += UpdateRemainingMovesText;
        player.movementCards.OnListChanged += UpdateMovementCardsText;
        player.inventoryChestCards.OnListChanged += UpdateChestCardsText;
        player.coinCount.OnValueChanged += UpdateCoinsText;
        player.inventoryRessources.OnListChanged += UpdateRessourcesText;
        player.savedRessources.OnListChanged += UpdateSafeText;
        initialPosition = showButton.transform.localPosition;
        showButton.onClick.AddListener(MoveCharBar);
    }

    private void MoveCharBar()
    {
        Debug.Log("OpponentsUI.OnMouseDown isShowing: " + isHidden);
        Debug.Log("OpponentsUI.OnMouseDown initialPosition: " + initialPosition);
        if (isHidden)
        {
            isHidden = false;
            showButton.transform.localPosition = showButton.transform.localPosition - new Vector3(409, 0, 0); ;
            Debug.Log("OpponentsUI.OnMouseDown currentPosition: " + transform.position);
        }
        else
        {
            isHidden = true;
            showButton.transform.localPosition = showButton.transform.localPosition + new Vector3(409, 0, 0);
            Debug.Log("OpponentsUI.OnMouseDown currentPosition: " + showButton.transform.localPosition);
        }
    }

    private void UpdateSafeText(NetworkListEvent<int> changeEvent)
    {
        Debug.Log("OpponentsUI.UpdateSafeText changeEvent: " + changeEvent);
        safeCount.text = changeEvent.ToString();
    }

    private void UpdateRessourcesText(NetworkListEvent<int> changeEvent)
    {
        Debug.Log("OpponentsUI.UpdateRessourcesText changeEvent: " + changeEvent);
        ressourcesCount.text = changeEvent.ToString();
    }

    private void UpdateCoinsText(int previousValue, int newValue)
    {
        coinsCount.text = newValue.ToString();
    }

    private void UpdateChestCardsText(NetworkListEvent<int> changeEvent)
    {
        Debug.Log("OpponentsUI.UpdateChestCardsText changeEvent: " + changeEvent);
        chestCardsCount.text = changeEvent.ToString();
    }

    private void UpdateMovementCardsText(NetworkListEvent<int> changeEvent)
    {
        Debug.Log("OpponentsUI.UpdateMovementCardsText changeEvent: " + changeEvent);
        movementCardsCount.text = changeEvent.ToString();
    }

    private void UpdateRemainingMovesText(int previousValue, int newValue)
    {
        Debug.Log("OpponentsUI.UpdateRemainingMovesText was called");
        remainingMovesCount.text = newValue.ToString();
    }
}
