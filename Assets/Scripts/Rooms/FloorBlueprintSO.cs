using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Room blueprints are used as a constraint for what type of room can be placed where (the specific room used won't be baked into the floor)
public class RoomBlueprint
{
    public RectInt bounds;
    public int route;
    public RoomType type;
    // the int is used as an index for what walls need to be free. 0 = upper-left-most corner on the northern wall, indices go clockwise from there.
    public List<int> unblockedWalls;
}

//Room prototypes are used as identifiers that signify one complete room, and includes important information
public class RoomPrototype
{
    public RectInt bounds;
    public int route;
    public RoomType type;
    public List<RoomBlueprint> blueprints;
}

public class FloorBlueprint
{
    public Vector2Int floorBounds;
    public int numSideRoutes;

    public List<Vector2Int> horizontalWallData;
    public int numHorizontalDoors;
    public bool?[,] horizontalWallMatrix;
    public List<Vector2Int> verticalWallData;
    public int numVerticalDoors;
    public bool?[,] verticalWallMatrix;
    /// <summary>
    /// ignore
    /// </summary>
    public List<RoomPrototype> roomData;
    public List<RoomBlueprint> roomBlueprints;
    public RoomPrototype[,] roomMatrix;

    public Vector2Int startingRoomPos;
    public Vector2Int bossRoomPos;
}

[CreateAssetMenu(menuName = "Floor Blueprint")]
public class FloorBlueprintSO : ScriptableObject
{
    [SerializeField] Vector2Int floorBounds;
    [SerializeField] int numSideRoutes;
    [Space]
    [Tooltip("Walls that are at the top & bottom of rooms (horizontal refers to orientation of walls, not position)\n" +
             "Position is based on what room (if it existed) would this wall be the bottom wall of.")]
    [SerializeField] List<Vector2Int> horizontalWalls;
    [Tooltip("How many walls have doors on them. Place all the walls with doors at the beginning of the list above. Don't repeat those walls afterward.")]
    [SerializeField] int numHorizontalDoors;
    [Space]
    [Tooltip("Walls that are at the left & right of rooms (vertical refers to orientation of walls, not position)\n" +
             "Position is based on what room (if it existed) would this wall be the left wall of.")]
    [SerializeField] List<Vector2Int> verticalWalls;
    [Tooltip("How many walls have doors on them. Place all the walls with doors at the beginning of the list above. Don't repeat those walls afterward.")]
    [SerializeField] int numVerticalDoors;
    [Space]
    [SerializeField] int numRooms;
    //--INGORE TOOLTIP HERE, COMPOUND ROOMS NOT IMPLEMENTED--
    //[Tooltip("If a room is non-rectangular, split the room into multiple data entries, with each one not exceeding the bounds of the room, and put the extra entries at the end.\n" +
    //         "Do not increase the room count variable for these extra entries, and make sure to have 1 (and only 1) entry before the end (it's recommended this be the biggest rectangle you can make with the room).")]
    [SerializeField] List<Vector2Int> roomSizes;
    [Tooltip("Origins are center cell in room, rounded down (towards bottom-left) if room has even dimensions.")]
    [SerializeField] List<Vector2Int> roomOrigins;
    [Tooltip("0 = no route, 1 = main route, 2+ = side route\n" +
             "Start, Boss, and Connector rooms should be no route\n" +
             "Battle rooms should be some route and only connected to either same route or no route\n" +
             "Treasure rooms should be same route as it's connected to (if it's connected to multiple routes, it should be no route)")]
    [SerializeField] List<int> roomRoutes;
    [SerializeField] List<RoomType> roomTypes;
    [Space]
    [SerializeField] Vector2Int startingRoomPos;
    [SerializeField] Vector2Int bossRoomPos;

    public FloorBlueprint ConstructData()
    {
        FloorBlueprint floor = new FloorBlueprint();
        //need to initialize: horizontalWallData, horizontalWallMatrix, verticalWallData, verticalWallMatrix, roomData, roomBlueprints, roomMatrix

        floor.horizontalWallData = new List<Vector2Int>(horizontalWalls);
        floor.numHorizontalDoors = numHorizontalDoors;
        //the top-most walls go beyond floorBounds because of how wall positions are handled.
        floor.horizontalWallMatrix = new bool?[floorBounds.x, floorBounds.y + 1];
        for(int i = 0; i < horizontalWalls.Count; i++)
        {
            bool isDoor = i < numHorizontalDoors;
            //each matrix item is a nullable bool, so don't simplify out the "!=null". "false" would mean there's wall there without a door.
            if (floor.horizontalWallMatrix[horizontalWalls[i].x, horizontalWalls[i].y] != null)
            {
                throw new InvalidOperationException($"Multiple horizontal walls placed at location {horizontalWalls[i]}.");
            }
            floor.horizontalWallMatrix[horizontalWalls[i].x, horizontalWalls[i].y] = isDoor;
        }

        floor.verticalWallData = new List<Vector2Int>(verticalWalls);
        floor.numVerticalDoors = numVerticalDoors;
        //the right-most walls go beyond floorBounds because of how wall positions are handled.
        floor.verticalWallMatrix = new bool?[floorBounds.x + 1, floorBounds.y];
        for (int i = 0; i < verticalWalls.Count; i++)
        {
            bool isDoor = i < numVerticalDoors;
            //each matrix item is a nullable bool, so don't simplify out the "!=null". "false" would mean there's wall there without a door.
            if (floor.verticalWallMatrix[verticalWalls[i].x, verticalWalls[i].y] != null)
            {
                throw new InvalidOperationException($"Multiple vertical walls placed at location {verticalWalls[i]}.");
            }
            floor.verticalWallMatrix[verticalWalls[i].x, verticalWalls[i].y] = isDoor;
        }
        floor.roomData = new List<RoomPrototype>();
        floor.roomBlueprints = new List<RoomBlueprint>();
        floor.roomMatrix = new RoomPrototype[floorBounds.x, floorBounds.y];
        for(int i = 0; i < numRooms; i++)
        {
            RoomBlueprint roomData = new RoomBlueprint()
            {
                bounds = new RectInt(roomOrigins[i], roomSizes[i]),
                route = roomRoutes[i],
                type = roomTypes[i],
                unblockedWalls = new List<int>()
            };
            RoomPrototype room = new RoomPrototype()
            {
                bounds = roomData.bounds,
                route = roomData.route,
                type = roomData.type,
                blueprints = new List<RoomBlueprint>() { roomData }
            };

            int x;
            int y;
            for(x = roomData.bounds.xMin; x < roomData.bounds.xMax; x++)
            {
                for(y = roomData.bounds.yMin; y < roomData.bounds.yMax; y++)
                {
                    if(floor.roomMatrix[x,y] != null)
                    {
                        throw new InvalidOperationException($"Multiple rooms overlapping at position {x}, {y}.");
                    }
                    floor.roomMatrix[x, y] = room;
                }
            }

            int doorIndex = 0;
            //first wall is upperleftmost horizontal wall, so point to cell just above upper-left-most corner
            for (x = roomData.bounds.xMin, y = roomData.bounds.yMax; x < roomData.bounds.xMax; x++, doorIndex++)
                if(floor.horizontalWallMatrix[x,y] != false)
                    roomData.unblockedWalls.Add(doorIndex);
            //right side, point to cell to right of upper-right-most corner
            for(x = roomData.bounds.xMax, y = roomData.bounds.yMax - 1; y >= roomData.bounds.yMin; y--, doorIndex++)
                if(floor.verticalWallMatrix[x,y] != false)
                    roomData.unblockedWalls.Add(doorIndex);
            //bottom side, point to cell above lower-right-most corner
            for(x = roomData.bounds.xMax - 1, y = roomData.bounds.yMin; x >= roomData.bounds.xMin; x--, doorIndex++)
                if (floor.horizontalWallMatrix[x, y] != false)
                    roomData.unblockedWalls.Add(doorIndex);
            //left side, point to cell to right of lower-left-most corner
            for(x = roomData.bounds.xMin, y = roomData.bounds.yMin; y < roomData.bounds.yMax; y++, doorIndex++)
                if (floor.verticalWallMatrix[x, y] != false)
                    roomData.unblockedWalls.Add(doorIndex);
        }

        //compound blueprints for non-rectangular rooms
        for (int i = numRooms; i < roomOrigins.Count; i++)
        {
            Debug.LogWarning("Compound rooms not implemented yet, please only use rectangular rooms. If only rectangular rooms were used, ensure that 'numRooms' is set to the right value.");
            //RoomBlueprint blueprint = new RoomBlueprint() { bounds = new RectInt(roomOrigins[i], roomSizes[i]) };
        }

        floor.floorBounds = floorBounds;
        floor.numSideRoutes = numSideRoutes;
        floor.startingRoomPos = startingRoomPos;
        floor.bossRoomPos = bossRoomPos;
        return floor;
    }
}