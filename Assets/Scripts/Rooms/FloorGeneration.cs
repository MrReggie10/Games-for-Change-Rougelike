//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorGeneration : MonoBehaviour
{
    [SerializeField] private GameObject mainGuy;

    private int[,] floorLayout = new int[10,10];
    //private List<Room> placedRooms = new List<Room>();

    Dictionary<string, Room> placedRooms1 = new Dictionary<string, Room>();
    private int[] currentTile = new int[2];

    [SerializeField] private List<GameObject> roomPrefabs = new List<GameObject>();

    [SerializeField] private int roomsToEnd;
    [SerializeField] private int extraRooms;

    private bool generationFailed = false;
    
    void Start()
    {
        //GenerateFloor(roomsToEnd, extraRooms);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            GenerateFloor(roomsToEnd, extraRooms);
        }
    }

    private void GenerateFloor(int minRoomsToEnd, int extraRooms)
    {
        foreach(KeyValuePair<string, Room> room in placedRooms1)
        {
            Destroy(room.Value.gameObject);
        }
        placedRooms1.Clear();
        System.Array.Clear(floorLayout, 0, floorLayout.Length);

        currentTile[0] = Random.Range(2, 7);
        currentTile[1] = Random.Range(2, 7);

        PlaceRoom(0);

        mainGuy.transform.position = new Vector3(currentTile[0] * 36, currentTile[1] * 20, 0);

        for (int i = 0; i < minRoomsToEnd; i++)
        {
            GenerateAdjacentRoom();
            if(generationFailed)
            {
                generationFailed = false;
                foreach (KeyValuePair<string, Room> room in placedRooms1)
                {
                    Destroy(room.Value.gameObject);
                }
                placedRooms1.Clear();
                System.Array.Clear(floorLayout, 0, floorLayout.Length);

                GenerateFloor(minRoomsToEnd, extraRooms);
                return;
            }

            if(i == minRoomsToEnd - 1)
            {
                foreach(SpriteRenderer renderer in placedRooms1[currentTile[0] + " " + currentTile[1]].renderers)
                {
                    renderer.color = Color.red;
                }
            }
        }

        for(int i = 0; i < extraRooms; i++)
        {
            while(true)
            {
                List<string> keyList = new List<string>(placedRooms1.Keys);

                int randPosition = Random.Range(0, keyList.Count);
                currentTile[0] = System.Convert.ToInt32(keyList[randPosition].Substring(0, 1));
                currentTile[1] = System.Convert.ToInt32(keyList[randPosition].Substring(2));

                if(placedRooms1[currentTile[0] + " " + currentTile[1]].renderers[0].GetComponent<SpriteRenderer>().color != Color.red )
                {
                    GenerateAdjacentRoom();
                }
                else
                {
                    generationFailed = true;
                }
                if (!generationFailed) {  break; } else { generationFailed = false; }
            }
        }
    }

    private void GenerateAdjacentRoom()
    {
        List<int> availableDirections = new List<int>();

        if (currentTile[0] != 9) { if (floorLayout[currentTile[0] + 1, currentTile[1]] == 0) { availableDirections.Add(0); } }
        if (currentTile[1] != 9) { if (floorLayout[currentTile[0], currentTile[1] + 1] == 0) { availableDirections.Add(1); } }
        if (currentTile[0] != 0) { if (floorLayout[currentTile[0] - 1, currentTile[1]] == 0) { availableDirections.Add(2); } }
        if (currentTile[1] != 0) { if (floorLayout[currentTile[0], currentTile[1] - 1] == 0) { availableDirections.Add(3); } }

        if (availableDirections.Count == 0)
        {
            generationFailed = true;
            return;
        }

        int selectedDirection = availableDirections[Random.Range(0, availableDirections.Count)];

        foreach(Room room in placedRooms1.Values)
        {
            if(room.mapPos.x == currentTile[0] && room.mapPos.y == currentTile[1])
            {
                if (selectedDirection == 0)
                {
                    room.rightBarrier.SetActive(false);
                    currentTile[0] = currentTile[0] + 1;
                    PlaceRoom(Random.Range(0, roomPrefabs.Count)).leftBarrier.SetActive(false);
                }
                else if (selectedDirection == 1)
                {
                    room.topBarrier.SetActive(false);
                    currentTile[1] = currentTile[1] + 1;
                    PlaceRoom(Random.Range(0, roomPrefabs.Count)).bottomBarrier.SetActive(false);
                }
                else if (selectedDirection == 2)
                {
                    room.leftBarrier.SetActive(false);
                    currentTile[0] = currentTile[0] - 1;
                    PlaceRoom(Random.Range(0, roomPrefabs.Count)).rightBarrier.SetActive(false);
                }
                else
                {
                    room.bottomBarrier.SetActive(false);
                    currentTile[1] = currentTile[1] - 1;
                    PlaceRoom(Random.Range(0, roomPrefabs.Count)).topBarrier.SetActive(false);
                }
                break;
            }
        }
    }

    private Room PlaceRoom(int roomNumber)
    {
        Room currentRoom = Instantiate(roomPrefabs[roomNumber], new Vector3(currentTile[0] * 36, currentTile[1] * 20, 0), Quaternion.identity).GetComponent<Room>();
        currentRoom.SetLocation(new Vector2Int(currentTile[0], currentTile[1]));
        floorLayout[currentTile[0], currentTile[1]] = 1;
        placedRooms1.Add(currentTile[0] + " " + currentTile[1] , currentRoom);
        return currentRoom;
    }
}
