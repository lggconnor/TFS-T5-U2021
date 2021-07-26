using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;

public class SaveData
{
    public class EnemyData
    {
        public Vector3 position;
        public Quaternion rotation;
        public float health;
    }

    [XmlAttribute("Variables")]



    // stores data for each enemy on the current level 
    public EnemyData[] enemies;

    // stores the player's current items
    public LinkedList<GameObject> itemList;

    // stores the player's current item
    public LinkedListNode<GameObject> currentItem;

    // stores the player's position
    public Vector3 position;

    // stores the player's rotation
    public Quaternion rotation;
}
