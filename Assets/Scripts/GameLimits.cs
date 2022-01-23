using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLimits : MonoBehaviour
{

    [SerializeField] Vector2 limit;
    public static Vector2 limitRoom;


    private void Awake()
    {
        limitRoom = limit;
    }

    private void OnDrawGizmos()
    {

        DrawSquare();

    }

    void DrawSquare()
    {

        Vector3[] positions = new Vector3[4]
        {
            new Vector3(-limit.x, limit.y, 0),
            new Vector3(limit.x, limit.y, 0),
            new Vector3(limit.x, -limit.y, 0),
            new Vector3(-limit.x, -limit.y, 0)
        };

        for (int i = 0; i < 4; i++)
        {
            if (i != 3)
                Debug.DrawLine(positions[i], positions[i + 1], Color.blue);
            else
                Debug.DrawLine(positions[i], positions[0], Color.blue);
        }

    }

    public static bool IsInLimits(Vector3 position)
    {
        bool insideLimits = true;

        if (Mathf.Abs(position.x) > limitRoom.x || Mathf.Abs(position.z) > limitRoom.y)
            insideLimits = false;

        return insideLimits;

    }

    public static Vector2 GetRandomPositionInLimits()
    {

        Vector2 position = new Vector2(Random.Range(-limitRoom.x, limitRoom.x), Random.Range(-limitRoom.y, limitRoom.y));
        return position;

    }


}
