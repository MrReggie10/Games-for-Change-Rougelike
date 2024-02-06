using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorGeneration : MonoBehaviour
{
    [SerializeField] private GameObject mainGuy;

    private int[,] floorLayout = new int[10,10];
    private List<Room> placedRooms = new List<Room>();
    private int[] currentTile = new int[2];

    [SerializeField] private List<GameObject> roomPrefabs = new List<GameObject>();

    private bool generationFailed = false;
    
    void Start()
    {
        GenerateFloor(7, 15);
    }

    private void GenerateFloor(int minRoomsToEnd, int extraRooms)
    {
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
                placedRooms.Clear();
                floorLayout.Initialize();

                GenerateFloor(minRoomsToEnd, extraRooms);
                return;
            }
        }

        for(int i = 0; i < extraRooms; i++)
        {
            while(true)
            {
                int randomRoomNum = Random.Range(0, placedRooms.Count);
                currentTile[0] = placedRooms[randomRoomNum].mapXPos;
                currentTile[1] = placedRooms[randomRoomNum].mapYPos;

                GenerateAdjacentRoom();
                if(!generationFailed) { break; } else { generationFailed = false; }
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

        foreach(Room room in placedRooms)
        {
            if(room.mapXPos == currentTile[0] && room.mapYPos == currentTile[1])
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
        GameObject currentRoom = Instantiate(roomPrefabs[roomNumber], new Vector3(currentTile[0] * 36, currentTile[1] * 20, 0), Quaternion.identity);
        currentRoom.GetComponent<Room>().SetLocation(currentTile[0], currentTile[1]);
        floorLayout[currentTile[0], currentTile[1]] = 1;
        placedRooms.Add(currentRoom.GetComponent<Room>());
        return currentRoom.GetComponent<Room>();
    }
}
