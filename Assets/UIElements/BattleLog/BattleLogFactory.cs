using UnityEngine;
using UnityEngine.UIElements;

public class BattleLogFactory : MonoBehaviour
{
    [SerializeField]
    VisualTreeAsset listEntryTemplate;

    void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();

        var listController = new BattleLogListController();
        listController.InitializeList(uiDocument.rootVisualElement, listEntryTemplate);
        var logger = LogData.shared;
        logger.SetBattleLogListDependency(listController);
    }
}