using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level0 : Level
{
    public override int LevelNumber => 0;

    public override void StartLevel()
    {
        base.StartLevel();
    }

    public override void EndLevel()
    {

    }

    private void Awake()
    {
        StartLevel();
    }
}
