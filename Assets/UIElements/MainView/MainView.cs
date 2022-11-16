using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class MainView : MonoBehaviour
{
    public Button shopButton;
    public Button alertButton;
    public Button battlelogButton;
    public Button settingsButton;
    public VisualElement charFigures;
    public GroupBox charBarGroup;
    public void Start()
    {
        // Each editor window contains a root VisualElement object
        var root = GetComponent<UIDocument>().rootVisualElement;

        shopButton = root.Q<Button>("shop-button");
        settingsButton = root.Q<Button>("settings-button");
        alertButton = root.Q<Button>("alert-button");
        battlelogButton = root.Q<Button>("battlelog-button");
        charFigures = root.Q<VisualElement>("char-fig");
        charBarGroup = root.Q<GroupBox>("char-bar");
    }

    private void Update()
    {

    }
}
