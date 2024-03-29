using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FloorGeneration : MonoBehaviour
{
    //const int MAIN_ROUTE_NUM = -1;
    //const int MAP_SIZE = 64;

    //const int DIRECTION_NORTH = 0;
    //const int DIRECTION_EAST = 1;
    //const int DIRECTION_SOUTH = 2;
    //const int DIRECTION_WEST = 3;

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
        public readonly RouteType type;

        public SideRouteProperties(RouteType type)
        {
            this.type = type;
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
    [SerializeField] GameObject virtualCamera;
    [SerializeField] List<Room> roomPrefabs;
    [SerializeField] List<FloorBlueprintSO> floorBlueprints;
    [SerializeField] Door horizontalDoorPrefab;
    [SerializeField] Door verticalDoorPrefab;
    [SerializeField] Sprite wallSprite;

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

    const float WALLWIDTH = 1;
    const float DOORSIZE = 2;
    const float CELLSIZE = 10;

    public bool GenerateFloor(FloorProperties properties)
    {
        DeleteFloor();
        floorData = new FloorData(properties);
        Random.InitState(properties.seed);

        //STEP 1: Choose Floor and initialize basic data
        FloorBlueprint floorBlueprint;
        int floorAttempts = 0;
        do
        {
            floorAttempts++;
            floorBlueprint = floorBlueprints[Random.Range(0, floorBlueprints.Count)].ConstructData();
            //it'll probably be better to just have each floor pull from its own pool of blueprints, so checks won't be needed
            //TODO: Check more thoroughly whether the chosen floor blueprint fits the desired properties. We can make it so each floor draws from a different pool if this becomes a problem and we don't want to code more.
            //if (floorBlueprint.numSideRoutes != floorData.properties.sideRoutes.Count) continue;
            break;
        } while (floorAttempts < 20);
        if(floorAttempts >= 20)
        {
            Debug.LogError("Failed to generate floor. Unable to find compatible floor blueprint.");
            return false;
        }

        floorData.floorLayout = new RoomPrototype[floorBlueprint.floorBounds.x, floorBlueprint.floorBounds.y];
        floorData.startRoomLocation = floorBlueprint.startingRoomPos;
        floorData.bossRoomLocation = floorBlueprint.bossRoomPos;

        //STEP 2: Add rooms
        foreach (RoomBlueprint blueprint in floorBlueprint.roomBlueprints)
        {
            //Debug.Log("Creating room blueprint");
            RoomPrototype prototype = new RoomPrototype { routeNum = blueprint.route, type = blueprint.type };
            Room prefab;
            int numAttempts = 0;
            do
            {
                numAttempts++;
                prefab = roomPrefabs[Random.Range(0, roomPrefabs.Count)];
                //Debug.Log($"Room Candidate: Size: {prefab.size}. Room Type: {prefab.roomType}. Route: {prefab.routeType}.");
                if (prefab.size != blueprint.bounds.size) continue; //check room size
                if (prefab.roomType != blueprint.type) continue; //check room type
                switch (blueprint.route) //check route type
                {
                    case 0: //no route
                    case 1: //main route
                        if (prefab.routeType != RouteType.Normal)
                            continue;
                        break;
                    default: //side route
                        if (prefab.routeType != floorData.properties.sideRoutes[blueprint.route - 2].type)
                            continue;
                        break;
                }
                break;
                //TODO: Implement check for wall availability in room prefab. Otherwise, make all rooms have all walls available (i.e. don't block any of them)
            } while (numAttempts < 200);
            if (numAttempts >= 200)
            {
                Debug.LogError("Failed to generate floor. Desired room could not be found.");
                return false;
            }
            //Debug.Log($"Generating room {prefab.gameObject.name}");
            //room fits parameters, create it
            prototype.room = Instantiate(prefab); //Unity automatically clones the gameobject and returns the room component like this
            prototype.room.mapPos = blueprint.bounds.position;
            prototype.room.transform.position = blueprint.bounds.center * CELLSIZE;
            for(int x = blueprint.bounds.xMin; x < blueprint.bounds.xMax; x++)
            {
                for(int y = blueprint.bounds.yMin; y < blueprint.bounds.yMax; y++)
                {
                    floorData.floorLayout[x, y] = prototype;
                }
            }
            prototype.room.Init();
        }

        //STEP 3: Create walls/doors
        for (int i = 0; i < floorBlueprint.numHorizontalDoors; i++)
        {
            Vector2Int wallGridPos = floorBlueprint.horizontalWallData[i];
            Vector2 wallLocation = (Vector2)wallGridPos * CELLSIZE; //bottom-left corner of room
            wallLocation += Vector2.right * CELLSIZE / 2; //move to bottom-center of room
            Door door = Instantiate(horizontalDoorPrefab, wallLocation, Quaternion.identity);

            door.room1 = floorData.floorLayout[wallGridPos.x, wallGridPos.y - 1].room;
            door.room2 = floorData.floorLayout[wallGridPos.x, wallGridPos.y].room;
            if(door.room1 == null || door.room2 == null)
            {
                Debug.LogError($"Floor generation failed. Invalid door index {i}.");
                return false;
            }
            door.room1.connectedDoors.Add(door);
            door.room2.connectedDoors.Add(door);

            GameObject wall1 = new GameObject("Horizontal Wall", typeof(BoxCollider2D), typeof(SpriteRenderer));
            wall1.GetComponent<SpriteRenderer>().sprite = wallSprite;
            wall1.transform.localScale = new Vector3((CELLSIZE - DOORSIZE + WALLWIDTH) / 2, WALLWIDTH);
            wall1.transform.position = wallLocation + Vector2.left * (CELLSIZE + DOORSIZE + WALLWIDTH) / 4;
            GameObject wall2 = new GameObject("Horizontal Wall", typeof(BoxCollider2D), typeof(SpriteRenderer));
            wall2.GetComponent<SpriteRenderer>().sprite = wallSprite;
            wall2.transform.localScale = new Vector3((CELLSIZE - DOORSIZE + WALLWIDTH) / 2, WALLWIDTH);
            wall2.transform.position = wallLocation + Vector2.right * (CELLSIZE + DOORSIZE + WALLWIDTH) / 4;
        }
        for (int i = floorBlueprint.numHorizontalDoors; i < floorBlueprint.horizontalWallData.Count; i++)
        {
            Vector2 wallLocation = (Vector2)floorBlueprint.horizontalWallData[i] * CELLSIZE; //bottom-left corner of room
            wallLocation += Vector2.right * CELLSIZE / 2; //move to bottom-center of room
            GameObject wall = new GameObject("Horizontal Wall", typeof(BoxCollider2D), typeof(SpriteRenderer));
            wall.GetComponent<SpriteRenderer>().sprite = wallSprite;
            wall.transform.localScale = new Vector3(CELLSIZE + WALLWIDTH, WALLWIDTH);
            wall.transform.position = wallLocation;
        }

        for (int i = 0; i < floorBlueprint.numVerticalDoors; i++)
        {
            Vector2Int wallGridPos = floorBlueprint.verticalWallData[i];
            Vector2 wallLocation = (Vector2)wallGridPos * CELLSIZE; //bottom-left corner of room
            wallLocation += Vector2.up * CELLSIZE / 2; //move to center-left of room
            Door door = Instantiate(verticalDoorPrefab, wallLocation, Quaternion.identity);

            door.room1 = floorData.floorLayout[wallGridPos.x - 1, wallGridPos.y].room;
            door.room2 = floorData.floorLayout[wallGridPos.x, wallGridPos.y].room;
            if (door.room1 == null || door.room2 == null)
            {
                Debug.LogError($"Floor generation failed. Invalid door index {i}.");
                return false;
            }
            door.room1.connectedDoors.Add(door);
            door.room2.connectedDoors.Add(door);

            GameObject wall1 = new GameObject("Vertical Wall", typeof(BoxCollider2D), typeof(SpriteRenderer));
            wall1.GetComponent<SpriteRenderer>().sprite = wallSprite;
            wall1.transform.localScale = new Vector3(WALLWIDTH, (CELLSIZE - DOORSIZE + WALLWIDTH) / 2);
            wall1.transform.position = wallLocation + Vector2.down * (CELLSIZE + DOORSIZE + WALLWIDTH) / 4;
            GameObject wall2 = new GameObject("Vertical Wall", typeof(BoxCollider2D), typeof(SpriteRenderer));
            wall2.GetComponent<SpriteRenderer>().sprite = wallSprite;
            wall2.transform.localScale = new Vector3(WALLWIDTH, (CELLSIZE - DOORSIZE + WALLWIDTH) / 2);
            wall2.transform.position = wallLocation + Vector2.up * (CELLSIZE + DOORSIZE + WALLWIDTH) / 4;
        }
        for (int i = floorBlueprint.numVerticalDoors; i < floorBlueprint.verticalWallData.Count; i++)
        {
            Vector2 wallLocation = (Vector2)floorBlueprint.verticalWallData[i] * CELLSIZE; //bottom-left corner of room
            wallLocation += Vector2.up * CELLSIZE / 2; //move to center-left of room
            GameObject wall = new GameObject("Vertical Wall", typeof(BoxCollider2D), typeof(SpriteRenderer));
            wall.GetComponent<SpriteRenderer>().sprite = wallSprite;
            wall.transform.localScale = new Vector3(WALLWIDTH, CELLSIZE + WALLWIDTH);
            wall.transform.position = wallLocation;
        }

        //it would probably be better to have this be done in GameManager and have that script call this method first
        mainGuy.transform.position = (floorData.startRoomLocation + Vector2.one * 0.5f) * CELLSIZE;
        //virtualCamera.transform.position = (floorData.startRoomLocation + Vector2.one * 0.5f) * CELLSIZE;
        return true;
    }
        /* OLD GENERATION CODE
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
