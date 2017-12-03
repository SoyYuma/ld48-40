﻿using UnityEngine;

public enum CharacterType
{
    A,
    B,
    C
}

public class Character : MonoBehaviour
{
    [Header("Character Settings")]
    public CharacterType characterType = CharacterType.A;
    public float speed = 1f;

    [Header("Happiness")]
    public float peopleTolerance = 1f;

    [Header("Character Face")]
    public SpriteRenderer faceRenderer = null;
    public Sprite happyFace = null;
    public Sprite neutralFace = null;
    public Sprite sadFace = null;

    public float happiness
    {
        get;
        private set;
    }

    public Transform targetPosition
    {
        get;
        set;
    }

    public bool isDragged
    {
        get;
        set;
    }

    public Room currentRoom
    {
        get;
        set;
    }

    private void Update()
    {
        if (isDragged)
        {
            return;
        }

        Vector3 displacementDirection = targetPosition.position - transform.position;
        displacementDirection.z = 0f;
        float distance = displacementDirection.magnitude;
        displacementDirection.Normalize();

        float frameMaxDisplacement = speed * Time.deltaTime;
        distance = Mathf.Min(distance, frameMaxDisplacement);
        Vector3 newPosition = transform.position + displacementDirection * distance;
        transform.position = newPosition;
    }

    public void EvaluateRoomSituation(RoomSituation roomSituation)
    {
        int numA = roomSituation.numCharacters[(int)CharacterType.A];
        int numB = roomSituation.numCharacters[(int)CharacterType.B];
        int numC = roomSituation.numCharacters[(int)CharacterType.C];
        switch (characterType)
        {
            case CharacterType.A:
                {
                    happiness = (numA - (numB + numC));
                }
                break;

            case CharacterType.B:
                {
                    happiness = (numB - 2f * numC);
                }
                break;

            case CharacterType.C:
                {
                    happiness = (numC > 1) ? ((numA + numB) - 5f * (numC - 1)) : 0;
                }
                break;
        }
        happiness = Mathf.Clamp(1 + happiness * peopleTolerance, -1f, 1f);
        UpdateFaceSprite();
    }

    private void UpdateFaceSprite()
    {
        HappinessState happinessState = HappinessUtils.GetHappinessStateFromValue(happiness);

        Sprite currentSprite = neutralFace;
        switch (happinessState)
        {
            case HappinessState.Happy:
                currentSprite = happyFace; 
                break;
            case HappinessState.Unhappy:
                currentSprite = sadFace;
                break;
        }

        faceRenderer.sprite = currentSprite;
    }
}
