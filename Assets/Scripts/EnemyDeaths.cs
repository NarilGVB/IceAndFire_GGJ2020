using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeaths : MonoBehaviour
{
    public void DestroyInstance()
    {
        Destroy(gameObject);
    }
}
