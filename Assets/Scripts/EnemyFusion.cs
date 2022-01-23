using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFusion : MonoBehaviour
{
    public void DestroyInstance()
    {
        Destroy(transform.parent.gameObject);
    }
}
