using System;
using UnityEngine;

public class SwitchBoard : Obstacle
{
    protected override void UniqueInteraction() //고유한 작용
    {
        base.UniqueInteraction(); //깨진다.
        //정전을 시킨다.
    }
}
