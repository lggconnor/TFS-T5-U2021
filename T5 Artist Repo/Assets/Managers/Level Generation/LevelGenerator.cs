using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    // an array of ints containing the tile index for each room position. Each index is the room number and the int at that index is the index of the tile within the tileset for the room.
    public int[] seed = null;

    // a wrapper class to store an array of gameobjects. 
    // This is only necessary because jagged arrays aren't serializable and writing an editor script would take more time than writing this wrapper script
    [Serializable]
    public class GameObjectArray
    {
        public GameObject[] Array;

        public GameObjectArray(GameObject[] array)
        {
            Array = array;
        }
    }

    // public variable declaration for the tileset wrapper
    public GameObjectArray[] TileSetsWrapper;

    // This is a jagged array containing tilesets for each part of the level. The level will be pieced together based on the order of the tilesets;
    [SerializeField] public GameObject[][] TileSets;

    // Start is called before the first frame update
    void Start()
    {
        // The section below writes the objects in the tileset wrapper to the jagged array for easy access. 
        // AFAIK the Tilesets Jagged Array will store references and not copies, so memory shouldn't be a concern. Let me know if I'm wrong though

        // initializes the Tilesets Jagged Array
        TileSets = new GameObject[TileSetsWrapper.Length][];
        
        // sets the arrays for each Tileset to the jagged array
        int j = 0;
        foreach (GameObjectArray arrayObject in TileSetsWrapper)
        {
            TileSets[j] = arrayObject.Array;
            // Debug.Log(arrayObject.Array.Length);
            j++;
        }

        // TODO: get seed from somewhere (probably save data class) 
        

        // checks to see if the Tilesets arrays are populated, otherwise skips generation;
        if (TileSets.Length > 0)
        {
            for (int i = 0; i < TileSets.Length; i++)
            {
                if (TileSets[i].Length < 1)
                {
                    Debug.LogError("There are no tiles in the Tileset at index " + i);
                    return;
                }

            }
        }
        else
        {
            Debug.LogError("There are no Tilesets to generate the level from!");
            return;
        }


        // if seed length less than TileSets.Length, initialize array and generate seed
        if (seed.Length < TileSets.Length)
        {
            seed = new int[TileSets.Length];
            for (int i = 0; i < TileSets.Length; i++)
            {
                // seed += i;
                // if (TileSets[i].Length > 1)
                seed[i] = UnityEngine.Random.Range(0, TileSets[i].Length);
                // else
                    // seed += 0;
            }
        }
        Debug.Log(seed);

        // Place starting room (index 0) at vector3.zero
        // get the exit node for the first room and the entrance node for the next room and instantiate the next room, placing the entrance node at the exit node's location
        // should be a loop based on the length of the jagged array

        Transform[] prevExit = new Transform[TileSets.Length];

        GameObject[] prevRoom = new GameObject[TileSets.Length];

        int[] currentIterations = new int[TileSets.Length];

        for (int i = 0; i < TileSets.Length;)
        {
            // declares and initializes value for the room from the seed
            int seedRoom = 0;

            // checks to see if the length of the seed is larger than the current index, so that an error isn't thrown when trying to access an invalid index.
            if (seed.Length > i)
            {
                seedRoom = seed[i];
                
            }
            else
            {
                Debug.LogError("The Seed for room " + i + " is not Valid! Defaulting to 0");
            }

            // checks to see if the tile at index seedroom exists
            if (seedRoom >= TileSets[i].Length)
            {                
                Debug.LogError("The tile at index " + seedRoom + " in Tileset Array " + i + " is doesn't exist! \n defaulting to zero");
                seedRoom = 0;
            }

            // a bool to keep track of if a room is blocked through the for loop
            // bool isBlocked = true;

            // check every room in the tileset to see if it fits at that position
            for (int k = 0; k < TileSets[i].Length; k++)
            {
                // variable to store the script of the room we want to check temporarily
                RoomScript roomCheck = TileSets[i][seedRoom].GetComponent<RoomScript>();
                // gets the rough size of the room specified in the script
                Vector3 size = roomCheck.roughDimensions;

                Quaternion rotation = Quaternion.identity;

                Vector3 center = Vector3.zero;

                if (prevExit[i])
                {
                    // gets a rotation based on the rotation of the exit relative to the rotation of the entrance
                    rotation = prevExit[i].rotation * Quaternion.Inverse(roomCheck.entrances[0].rotation);

                    // multiply the vector of the offset we wish to achieve by the rotation from above
                    center = prevExit[i].position - rotation * (roomCheck.entrances[0].position - roomCheck.center);
                }


                // Checks to see if there is anything occupying the space we want to put the room, the space being specified by the variables above;
                if (Physics.CheckBox(center, size / 2, rotation))
                {
                    Debug.Log("Room " + seedRoom + " in TileSet " + i + " does not fit... trying next tile");
                }
                else
                {
                    // breaks out of loop if the room fits
                    Debug.Log("Room " + seedRoom + " in TileSet " + i + " fit");
                    break;
                }

                // if the room doesn't fit, we cycle the room in the current tileset to check and update the seed to reflect the changes
                seedRoom = ++seedRoom % TileSets[i].Length;
                seed[i] = seedRoom;

                // we increment the value at the current iteration to keep track of what rooms we have cycled through already in a tileset
                currentIterations[i] = ++currentIterations[i];
            }
            
            // if the first room has been cycled through completely, abort room generation because there is no combination of tiles that fit
            if (currentIterations[0] >= TileSets[0].Length)
            {
                Debug.LogError("This combination of Tilesets cannot generate a complete level! \n Please check the values in each room generation script.");
                return;
            }
            // if the current tileset has been cycled through, reset the current tileset counter, remove the previous room, then decrement the current room and increment the tileset counter, updating the seed for the room
            else if (currentIterations[i] >= TileSets[i].Length)
            {
                Debug.Log("Rooms in TileSet " + i + " do not fit... changing previous tile");
                currentIterations[i] = currentIterations[i] % TileSets[i].Length;
                if (prevExit[i])
                {                
                // I must set the previous gameobject to be inactive, as unity does not destroy objects until the first update, however does mark gameobjects as not active in Start();
                // This means if the line below is not present, Physics.CheckBox will return true as the gameobject has not been destroyed yet
                    prevExit[i].root.gameObject.SetActive(false);
                    Destroy(prevExit[i].root.gameObject);
                }
                else if (prevRoom[i])
                {
                    prevRoom[i].SetActive(false);
                    Destroy(prevRoom[i]);
                }
                i--;
                seedRoom = ++seed[i] % TileSets[i].Length;
                seed[i] = seedRoom;
                currentIterations[i] = ++currentIterations[i];
            }
            // if the tileset has not been cycled through and there is space, spawn the gameobject
            else
            {
                Quaternion rotation = Quaternion.identity;

                Vector3 position = Vector3.zero;

                RoomScript roomTemp = TileSets[i][seedRoom].GetComponent<RoomScript>();

                Transform entrance = roomTemp.entrances[0];

                if (prevExit[i])
                {
                    // essentially subtracts the rotation of the entrance in worldspace from the rotation of the exit in worldspace and sets the rotation of the room to that difference
                    // room.transform.rotation = prevExit[i].rotation * Quaternion.Inverse(entrance.rotation);
                    rotation = prevExit[i].rotation * Quaternion.Inverse(entrance.rotation);

                    // AFTER rotating the piece to the correct rotation, we can then set its position such that the entrance is at the same position as the exit

                    // Haha, just kidding, I was running into issue with check box (due to me setting the transform after instantiation) so I decided to assign the position as variables used at instantiation.
                    // room.transform.position = prevExit[i].position - entrance.position;
                    position = prevExit[i].position - rotation * (roomTemp.entrances[0].position);
                }

                GameObject room = Instantiate(TileSets[i][seedRoom], position, rotation, null);

                i++;

                if (room)
                    prevRoom[i % prevRoom.Length] = room;

                if(room.GetComponent<RoomScript>().exits.Length > 0)
                    prevExit[(i) % prevExit.Length] = room.GetComponent<RoomScript>().exits[0];


            }
        }

    }

    public string ReplaceCharacterInString(int index, string character, string source)
    {
        string newString = "";
        for (int i = 0; i < index; i++)
        {
            newString += source[i];
        }
        newString += character;
        for (int j = index + 1; j < source.Length; j++)
        {
            newString += source[j];
        }
        return newString;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
