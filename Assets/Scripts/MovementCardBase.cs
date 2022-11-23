using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementCardBase : CardBase
{
    [Header("Movement Card References")]
    public TMPro.TextMeshProUGUI moveCountText;

    [Header("Movement Card Options")]
    public int baseMoveCount;
    public int id;

}
