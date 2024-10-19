using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum View
{
    FirstPerson,
    ThirdPerson
}

[CreateAssetMenu(fileName = "ViewEvent", menuName = "Game Event/View")]
public sealed class ViewEvent : GameEvent<View>
{
}
