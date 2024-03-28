using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorGenerationDebug : MonoBehaviour
{
    [SerializeField] bool setSeed;
    [SerializeField] int seed;
    [SerializeField] List<RouteType> sideRoutes;

    [Header("The parameters below do nothing.")]
    [SerializeField] int floorNum;
    [SerializeField] int minArea;
    [SerializeField] int mainRouteMinDistance;
    [SerializeField] int mainRouteMaxDistance;
    [SerializeField] int mainRouteMinRooms;
    [SerializeField] int mainRouteMaxRooms;
    [SerializeField] bool setStartPos;
    [SerializeField] Vector2Int startRoomPos;

    void Start()
    {
        List<FloorGeneration.SideRouteProperties> sides = new List<FloorGeneration.SideRouteProperties>();
        foreach (RouteType type in sideRoutes)
            sides.Add(new FloorGeneration.SideRouteProperties(type));
        GetComponent<FloorGeneration>().GenerateFloor(new FloorGeneration.FloorProperties(
            floorNum,
            new FloorGeneration.MainRouteProperties(mainRouteMinDistance, mainRouteMaxDistance, mainRouteMinRooms, mainRouteMaxRooms),
            minArea,
            setStartPos ? startRoomPos : null,
            sides,
            setSeed ? seed : null));
    }
}
