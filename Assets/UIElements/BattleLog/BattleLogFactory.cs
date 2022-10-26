using UnityEngine;
using UnityEngine.UIElements;

public class BattleLogFactory : MonoBehaviour
{
    [SerializeField]
    VisualTreeAsset m_ListEntryTemplate;

    void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();

        var listController = new BattleLogListController();
        listController.InitializeList(uiDocument.rootVisualElement, m_ListEntryTemplate);
        var logger = LogData.shared;
        logger.setBattleLogListDependency(listController);
    }
}