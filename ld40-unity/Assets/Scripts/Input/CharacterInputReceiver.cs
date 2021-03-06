﻿using UnityEngine;

public class CharacterInputReceiver : BaseInputReceiver
{
    private static readonly int MAX_NUM_RAYCAST_HITS = 3;
    
    public Character character = null;
    public float raycastDistance = 100;
    public LayerMask raycastLayerMask = new LayerMask();

    private Vector3 _initialPosition = Vector3.zero;
    private RaycastHit2D[] _raycastResults = null;
    private VisitorSpawner _visitorSpawner = null;
    private Room _characterPreviousRoom = null;

    private void Awake()
    {
        _visitorSpawner = FindObjectOfType<VisitorSpawner>();
    }

    private void Start()
    {
        _raycastResults = new RaycastHit2D[MAX_NUM_RAYCAST_HITS];
    }

    public override void OnTouchBegan(TouchEventInfo eventInfo)
    {
        if (null != character.currentRoom)
        {
            _characterPreviousRoom = character.currentRoom;
            character.currentRoom.RemoveCharacter(character);
        }
        _initialPosition = character.transform.position;
        character.isDragged = true;
    }

    public override void OnTouchMoved(TouchEventInfo eventInfo)
    {
        Vector3 newPosition = Camera.main.ScreenToWorldPoint(eventInfo.touchPositionPix);
        newPosition.z = character.transform.position.z;
        character.transform.position = newPosition;
    }

    public override void OnTouchEnded(TouchEventInfo eventInfo)
    {
        // Check if there's a room below
        Ray ray = Camera.main.ScreenPointToRay(eventInfo.touchPositionPix);
        int numHits = Physics2D.RaycastNonAlloc(ray.origin, ray.direction, _raycastResults, raycastDistance, raycastLayerMask);
        Room droppedToRoom = null;
        for (int hitIdx = 0; hitIdx < numHits; ++hitIdx)
        {
            Room room = _raycastResults[hitIdx].collider.GetComponentInParent<Room>();
            if (null != room)
            {
                droppedToRoom = room;
                break;
            }
        }

        if ((null != droppedToRoom) && (droppedToRoom.HasCapacity()))
        {
            droppedToRoom.AddCharacter(character);
            Vector3 targetPosition = ray.origin;
            targetPosition.z = character.transform.position.z;
            character.targetPosition = droppedToRoom.GetRandomFreeDancePosition();
            character.transform.position = targetPosition;

            if (null != _visitorSpawner)
            {
                _visitorSpawner.SpawnReplacementForCharacter(character);
            }
        }
        else
        {
            character.transform.position = _initialPosition;
            if (null != _characterPreviousRoom)
            {
                _characterPreviousRoom.AddCharacter(character);
            }
        }

        character.isDragged = false;
    }
}
