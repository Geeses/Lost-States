using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Specialized;
using Unity.Netcode;
public class Inventory : MonoBehaviour
{
    private Label water_bag;
    private Label steel_bag;
    private Label wood_bag;
    private Label coins_bag;
    private Label fruit_bag;
    private Label water_safe;
    private Label steel_safe;
    private Label wood_safe;
    private Label fruit_safe;
    private Player CurrentPlayer;

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        water_bag = root.Q<Label>("water-bag");
        steel_bag = root.Q<Label>("steel-bag");
        wood_bag = root.Q<Label>("wood-bag");
        coins_bag = root.Q<Label>("coins-bag");
        fruit_bag = root.Q<Label>("fruit-bag");
        water_safe = root.Q<Label>("water-safe");
        steel_safe = root.Q<Label>("steel-safe");
        wood_safe = root.Q<Label>("wood-safe");
        fruit_safe = root.Q<Label>("fruit-safe");

        var playerID = NetworkManager.Singleton.LocalClientId;
        CurrentPlayer = NetworkManager.Singleton.ConnectedClients[playerID].PlayerObject.GetComponent<Player>();
        CurrentPlayer.inventoryRessources.OnListChanged += UpdateRessourceLabels;
    }

    public void UpdateRessourceLabels(NetworkListEvent<int> changeEvent)
    {
        var water = 0;
        var steel = 0;
        var wood = 0;
        var fruit = 0;

        foreach (Ressource ressource in CurrentPlayer.inventoryRessources) {
            switch(ressource) {
                case Ressource.water:
                    water += 1;
                    break;
  
                case Ressource.steel:
                    steel += 1;
                    break;
  
                case Ressource.wood:
                    wood += 1;
                    break;
                case Ressource.fruit:
                    fruit += 1;
                    break;
                default:
                    break;
            }
        }

        water_bag.text = water.ToString();
        steel_bag.text = steel.ToString();
        wood_bag.text = wood.ToString();
        fruit_bag.text = fruit.ToString();
    }

}
