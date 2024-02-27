using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TriggerTactics.Gameplay.Spawn
{
    public interface IBaseSpawnable
    {
        GameObject GameObject { get; }

        int SpawnableTypeID { get; }  

    }
}

