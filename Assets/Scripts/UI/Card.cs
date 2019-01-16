using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="New Card", menuName="Card")]
public class Card : ScriptableObject {

    public string title;
    public Sprite img;
    public int absolute;
    public string description;
    public string country;
    public string rarity;
    public int friendly;//1 è true
    public string effect;
    public bool progress;
    public int type; //1 truppa 2 struttura 3 spell  che sui appllica ai friendly 4 spell che si appplica agli enemys 5 insidia(si attiva come trigger ad un evento) 6 spell che si appplica agli enemys

}
