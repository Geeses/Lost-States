using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class cardsTests : MonoBehaviour
{
    private VisualElement _card1;
    private VisualElement _card2;
    private VisualElement _card3;
    private VisualElement _card4;
    public void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        _card1 = root.Q<VisualElement>("card1");
        _card2 = root.Q<VisualElement>("card2");
        _card3 = root.Q<VisualElement>("card3");
        _card4 = root.Q<VisualElement>("card4");

        _card1.RegisterCallback<PointerOverEvent>(OnPointerOver1);
        _card1.RegisterCallback<PointerOutEvent>(OnPointerOut1);
        _card2.RegisterCallback<PointerOverEvent>(OnPointerOver2);
        _card2.RegisterCallback<PointerOutEvent>(OnPointerOut2);
        _card3.RegisterCallback<PointerOverEvent>(OnPointerOver3);
        _card3.RegisterCallback<PointerOutEvent>(OnPointerOut3);
        _card4.RegisterCallback<PointerOverEvent>(OnPointerOver4);
        _card4.RegisterCallback<PointerOutEvent>(OnPointerOut4);
    }

    public void OnPointerOver1(PointerOverEvent evt)
    {
        _card1.style.translate = new StyleTranslate(new Translate(0, -10, 0));
        _card1.BringToFront();
    }
    public void OnPointerOut1(PointerOutEvent evt)
    {
        _card1.style.translate = new StyleTranslate(new Translate(0, 10, 0));
    }
    public void OnPointerOver2(PointerOverEvent evt)
    {
        _card2.style.translate = new StyleTranslate(new Translate(0, -10, 0));
        _card2.BringToFront();
    }
    public void OnPointerOut2(PointerOutEvent evt)
    {
        _card2.style.translate = new StyleTranslate(new Translate(0, 10, 0));
    }
    public void OnPointerOver3(PointerOverEvent evt)
    {
        _card3.style.translate = new StyleTranslate(new Translate(0, -10, 0));
        _card3.BringToFront();
    }
    public void OnPointerOut3(PointerOutEvent evt)
    {
        _card3.style.translate = new StyleTranslate(new Translate(0, 10, 0));
    }
    public void OnPointerOver4(PointerOverEvent evt)
    {
        _card4.style.translate = new StyleTranslate(new Translate(0, -10, 0));
        _card4.BringToFront();
    }
    public void OnPointerOut4(PointerOutEvent evt)
    {
        _card4.style.translate = new StyleTranslate(new Translate(0, 10, 0));
    }
}
