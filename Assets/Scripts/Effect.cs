using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Keyword
{
    add,
    when,
    move,
}

[Serializable]
public class Effect
{
    public List<Keyword> keyword;
}
