using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class BattlelogCellController {
    Label logLabel;

    //This function retrieves a reference to the 
    //character name label inside the UI element.
    public void SetVisualElement(VisualElement visualElement)
    {
        logLabel = visualElement.Q<Label>("battlelog-cell");
    }

    public void SetLogData(string log)
    {
        logLabel.text = log;
    }
}
