using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFocus : MonoBehaviour
{
    [SerializeField] Vector2 gridSize;
    [SerializeField] Vector2 roomMargins;
    [SerializeField] Vector2 roomPadding;
    private new Rigidbody2D rigidbody;

    private AnimationCurve roomCameraXCurve;
    private AnimationCurve roomCameraYCurve;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        GameManager.instance.OnRoomEnter += battle => { if (battle) RecalculateCameraCurves(); };
    }

    void FixedUpdate()
    {
        if(GameManager.instance.playerInBattle)
        {
            Room room = GameManager.instance.currentRoom;
            Vector2 roomPos = room.transform.position;
            Vector2 playerRelativePos = (Vector2)PlayerSingleton.player.transform.position - roomPos;
            Vector2 cameraRelativePos;
            cameraRelativePos.x = Mathf.Max(0,roomCameraXCurve.Evaluate(Mathf.Abs(playerRelativePos.x))) * Mathf.Sign(playerRelativePos.x);
            cameraRelativePos.y = Mathf.Max(0,roomCameraYCurve.Evaluate(Mathf.Abs(playerRelativePos.y))) * Mathf.Sign(playerRelativePos.y);
            transform.position = cameraRelativePos + roomPos;
        }
        else
        {
            transform.position = PlayerSingleton.player.transform.position;
        }
    }

    private Rect GetRoomCameraBounds(Room room)
    {
        Rect rect = new Rect();
        rect.size = room.size * gridSize;
        rect.center = room.transform.position;
        return rect;
    }

    private void RecalculateCameraCurves()
    {
        Rect bounds = GetRoomCameraBounds(GameManager.instance.currentRoom);
        /*
        //for each Keyframe, time = player relative pos, value = camera relative pos
        roomCameraXCurve = new AnimationCurve();
        float xBound = bounds.width / 2;
        float xMargin = Mathf.Max(0, xBound - roomMargins.x);
        if (xMargin > 0)
            roomCameraXCurve.AddKey(new Keyframe() { time = 0, value = 0, outTangent = 1 });
        roomCameraXCurve.AddKey(new Keyframe() { time = xMargin, value = xMargin - roomPadding.x, inTangent = 1, outTangent = 0.8f });
        roomCameraXCurve.AddKey(new Keyframe() { time = xBound, value = xMargin + roomPadding.x, inTangent = 0.4f });

        roomCameraYCurve = new AnimationCurve();
        float yBound = bounds.height / 2;
        float yMargin = Mathf.Max(0, yBound - roomMargins.y);
        if (yMargin > 0)
            roomCameraYCurve.AddKey(new Keyframe() { time = 0, value = 0, outTangent = 1 });
        roomCameraYCurve.AddKey(new Keyframe() { time = yMargin, value = yMargin - roomPadding.y, inTangent = 1, outTangent = 0.8f });
        roomCameraYCurve.AddKey(new Keyframe() { time = yBound, value = yMargin + roomPadding.y, inTangent = 0.4f });
        */
        roomCameraXCurve = new AnimationCurve(new Keyframe() { time = 0, value = 0, outTangent = 0.7f }, new Keyframe() { time = bounds.width / 2, value = bounds.width / 2 - roomPadding.x, inTangent = 0f, inWeight = 0.1f });
        roomCameraYCurve = new AnimationCurve(new Keyframe() { time = 0, value = 0, outTangent = 0.7f }, new Keyframe() { time = bounds.height / 2, value = bounds.height / 2 - roomPadding.y, inTangent = 0f, inWeight = 0.1f });
    }
}
