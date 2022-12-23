using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterModel
{
    List<Resource> resources = new List<Resource>();
    List<Kistenkarten> kistenkarten = new List<Kistenkarten>();
    List<Bewegungskarten> bewegungskarten = new List<Bewegungskarten>();
    // int munzenanzahl = 0;
    Character char_type;
}

class Bewegungskarten {
    int move_counter;
}

class Kistenkarten {

}

enum Character{
    Marco,
    Amberah,
    Monokel,
    Sonja
}

enum Resource{
    Schrott,
    Holz,
    Kleber,
    Baureste
};
