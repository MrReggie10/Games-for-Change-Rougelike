using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FloorGeneration : MonoBehaviour
{
    const int MAIN_ROUTE_NUM = -1;
    const int MAP_SIZE = 64;

    const int DIRECTION_NORTH = 0;
    const int DIRECTION_EAST = 1;
    const int DIRECTION_SOUTH = 2;
    const int DIRECTION_WEST = 3;

    #region Data Structures
    /// <summary>
    /// Representation of a room within map generation. Contains information used for different stages of generation.
    /// </summary>
    class RoomPrototype
    {
        public int routeNum;
        public RoomType type;
        public Room room;

        public void Delete()
        {
            Destroy(room.gameObject);
        }
    }

    /// <summary>
    /// Representation of a floor used for map generation. Contains floor layout as well as other useful information
    /// </summary>
    class FloorData
    {
        public readonly FloorProperties properties;
        public RoomPrototype[,] floorLayout;
        public Vector2Int startRoomLocation;
        public Vector2Int bossRoomLocation;

        public FloorData(FloorProperties properties)
        {
            this.properties = properties;
            floorLayout = new RoomPrototype[MAP_SIZE, MAP_SIZE];
        }
    }

    public class MainRouteProperties
    {
        public readonly int minDistance;
        public readonly int maxDistance;

        public readonly int minRooms;
        public readonly int maxRooms;

        public MainRouteProperties(int minDistance, int maxDistance, int minRooms, int maxRooms)
        {
            this.minDistance = minDistance;
            this.maxDistance = maxDistance;
            this.minRooms = minRooms;
            this.maxRooms = maxRooms;
        }
    }

    public class SideRouteProperties
    {

        public SideRouteProperties()
        {

        }
    }

    public class FloorProperties
    {
        public readonly int floorNum;
        public readonly Vector2Int startRoom;
        public readonly MainRouteProperties mainRoute;
        public readonly List<SideRouteProperties> sideRoutes;
        public readonly int minArea;
        public readonly int seed;

        public FloorProperties(int floorNum, MainRouteProperties mainRoute, int minArea, Vector2Int? startRoom = null, List<SideRouteProperties> sideRoutes = null, int? seed = null)
        {
            this.floorNum = floorNum;
            this.mainRoute = mainRoute;
            this.minArea = minArea;
            //the ?? operator returns the left operand if it's non-null, otherwise it returns the right operand
            this.startRoom = startRoom ?? Vector2Int.zero;
            this.sideRoutes = sideRoutes ?? new List<SideRouteProperties>();
            this.seed = seed ?? new System.Random().Next(int.MinValue, int.MaxValue);
        }
    }
    #endregion

    [SerializeField] GameObject mainGuy;
    [SerializeField] List<GameObject> roomPrefabs;

    private FloorData floorData;

    public void DeleteFloor()
    {
        if (floorData == null)
            return;
        foreach (RoomPrototype room in floorData.floorLayout)
        {
            room.Delete();
        }
        floorData = null;
    }

    public void GenerateFloor(FloorProperties properties)
    {
        DeleteFloor();
        floorData = new FloorData(properties);
        Random.InitState(properties.seed);
        /*
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
                System.Array.Clear(floorData, 0, floorData.Length);

                GenerateFloor(minRoomsToEnd, extraRooms);
                return;
            }

            if(i == minRoomsToEnd - 1)
            {
                //this is not how it will work in the actual generation, but this is an easy placeholder.
                placedRooms1[currentTile[0] + " " + currentTile[1]].roomType = RoomType.Boss;
            }
        }

        for(int i = 0; i < extraRooms; i++)
        {
            while(true)
            {
                List<string> keyList = new List<string>(placedRooms1.Keys);

                int randPosition = Random.Range(0, keyList.Count);
                currentTile[0] = Convert.ToInt32(keyList[randPosition].Substring(0, 1));
                currentTile[1] = Convert.ToInt32(keyList[randPosition].Substring(2));

                if(placedRooms1[currentTile[0] + " " + currentTile[1]].roomType != RoomType.Boss)
                {
                    GenerateAdjacentRoom();
                }
                else
                {
                    generationFailed = true;
                }
                if (!generationFailed) { break; } else { generationFailed = false; }
            }
        }
        */
    }

    private void SetupMainRoute()
    {
        Vector2Int currentTile = floorData.properties.startRoom;
        floorData.startRoomLocation = currentTile;
        int distance = 0;
        float direction = Random.Range(0f, 360f);
        float turnAverage = Random.Range(-15f, 15f);
    }

    private int ChooseNextDirection(float direction)
    {
        //direction 0 = north, increasing = clockwise
        float northWeight, eastWeight, southWeight, westWeight;

        if (direction < 180)
            northWeight = Mathf.Max(0, 120 - direction);
        else
            northWeight = Mathf.Max(0, direction - 240);
        northWeight *= northWeight;

        direction -= 90;
        if (direction < 0)
            direction += 360;
        if (direction < 180)
            eastWeight = Mathf.Max(0, 120 - direction);
        else
            eastWeight = Mathf.Max(0, direction - 240);
        eastWeight *= eastWeight;

        direction -= 90;
        if (direction < 0)
            direction += 360;
        if (direction < 180)
            southWeight = Mathf.Max(0, 120 - direction);
        else
            southWeight = Mathf.Max(0, direction - 240);
        southWeight *= southWeight;

        direction -= 90;
        if (direction < 0)
            direction += 360;
        if (direction < 180)
            westWeight = Mathf.Max(0, 120 - direction);
        else
            westWeight = Mathf.Max(0, direction - 240);
        westWeight *= westWeight;
        float totalWeight = (northWeight + eastWeight + southWeight + westWeight);
        float selectedWeight = Random.value * totalWeight;
        //if the value is somehow 0, it will choose north always (even if it has a weight of 0) unless I do this
        if (selectedWeight == 0)
            selectedWeight = 1; //with how the weights are set up, this is always guaranteed to always be valid and almost always the first direction with a weight

        selectedWeight -= northWeight;
        if (selectedWeight <= 0)
            return DIRECTION_NORTH;
        selectedWeight -= eastWeight;
        if (selectedWeight <= 0)
            return DIRECTION_EAST;
        selectedWeight -= southWeight;
        if (selectedWeight <= 0)
            return DIRECTION_SOUTH;
        return DIRECTION_WEST;
    }
    /*
    private void GenerateAdjacentRoom()
    {
        List<int> availableDirections = new List<int>();

        if (currentTile[0] != 9) { if (floorData[currentTile[0] + 1, currentTile[1]] == 0) { availableDirections.Add(0); } }
        if (currentTile[1] != 9) { if (floorData[currentTile[0], currentTile[1] + 1] == 0) { availableDirections.Add(1); } }
        if (currentTile[0] != 0) { if (floorData[currentTile[0] - 1, currentTile[1]] == 0) { availableDirections.Add(2); } }
        if (currentTile[1] != 0) { if (floorData[currentTile[0], currentTile[1] - 1] == 0) { availableDirections.Add(3); } }

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
        floorData[currentTile[0], currentTile[1]] = 1;
        placedRooms1.Add(currentTile[0] + " " + currentTile[1] , currentRoom);
        return currentRoom;
    }
    */
}
