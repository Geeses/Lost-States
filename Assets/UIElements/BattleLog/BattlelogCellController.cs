using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class BattlelogCellController {
    Label logLabel;
    public void SetVisualElement(VisualElement visualElement)
    {
        logLabel = visualElement.Q<Label>("battlelog-cell");
    }

    public void SetLogData(string log)
    {
        logLabel.text = log;
    }
}
