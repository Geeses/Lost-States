using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class BattlelogCellController {
    Label m_LogLabel;

    //This function retrieves a reference to the 
    //character name label inside the UI element.
    public void SetVisualElement(VisualElement visualElement)
    {
        m_LogLabel = visualElement.Q<Label>("battlelog-cell");
    }

    public void SetLogData(string log)
    {
        m_LogLabel.text = log;
    }
}
