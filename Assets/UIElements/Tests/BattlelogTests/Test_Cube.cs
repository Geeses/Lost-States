using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Cube : MonoBehaviour
{
    private Renderer _renderer;
    

    private void Start()
    {
        _renderer = GetComponent<Renderer>();
    }

    private void OnMouseDown()
    {
        LogData.shared.AddLog("I'm a cube!");
    }
}
