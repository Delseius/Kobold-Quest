using System;
using UnityEngine;

public class Floor : worldObject
{
    [SerializeField]
    Boolean isTillable;

    public Boolean getTill()
    {
        return isTillable;
    }
}
