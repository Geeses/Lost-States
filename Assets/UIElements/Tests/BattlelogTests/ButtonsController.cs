using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class ButtonsController : MonoBehaviour
{
    Button m_Button1;
    Button m_Button2;
    Button m_Button3;
    
    void OnEnable() {
        var root = GetComponent<UIDocument>().rootVisualElement;

        m_Button1 = root.Q<Button>("button-one");
        m_Button2 = root.Q<Button>("button-two");
        m_Button3 = root.Q<Button>("button-three");

        m_Button1.RegisterCallback<ClickEvent>(ButtonOneAction);
        m_Button2.RegisterCallback<ClickEvent>(ButtonTwoAction);
        m_Button3.RegisterCallback<ClickEvent>(ButtonThreeAction);
    }

    void ButtonOneAction(ClickEvent evt) {
        LogData log = LogData.shared;
        log.AddLog("Button1 was pressed");
    }

    void ButtonTwoAction(ClickEvent evt) {
        LogData log = LogData.shared;
        log.AddLog("Button2 was pressed");
    }

    void ButtonThreeAction(ClickEvent evt) {
        LogData log = LogData.shared;
        log.AddLog("Button3 was pressed");
    }
}
