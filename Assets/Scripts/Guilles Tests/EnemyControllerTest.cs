//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Random = UnityEngine.Random;

public class EnemyControllerTest : MonoBehaviour
{
    [SerializeField] public float speed = 2f;
    [SerializeField] public float temperature = 10;

    [SerializeField] public enum MoveType
    {
        Chase,
        RunAway,
        Straight
    };

    [SerializeField] MoveType _moveType;
    
    private Transform _target;
    private Rigidbody2D _rigidbody;


    public static EnemyControllerTest Spawn(GameObject prefab, Vector3 position, 
        Transform parent, Transform target, Vector3 scale, MoveType moveType, float temperature = 0)
    {
        EnemyControllerTest ins = Instantiate(prefab, position, Quaternion.identity, parent).GetComponent<EnemyControllerTest>();

        ins._target = target;
        ins._rigidbody = ins.GetComponent<Rigidbody2D>();
        ins._moveType = moveType;

        return ins;
    }

    void FixedUpdate()
    {

        
        switch (_moveType)
        {

            case MoveType.Chase:
                ChaseTarget();
                break;

            case MoveType.Straight:
                MoveStraight();
                break;

            case MoveType.RunAway:
                RunAway();
                break;

        }
        
        
    }

    //abstract public void Movement();

    private void OnCollisionEnter2D(Collision2D collision)
    {
        print(collision.collider.name);
    }

    #region RUN AWAY ENEMY

    Vector3 target;

    void RunAway()
    {

        

        print("TAR = " + target);

        if (Vector3.Distance(_target.position, transform.position) < 1f)
        {
            print("ESTA A RANGO");
            Vector2 direction = (_target.position - transform.position).normalized * -1;
            _rigidbody.MovePosition((Vector2)transform.position + direction * (speed * Time.deltaTime));
        }
        else if (Vector3.Distance(_target.position, transform.position) > 1.2f)
        {
            MoveRandom();
        }
    }


    void MoveRandom()
    {
        if (target == Vector3.zero)
        {
            target = NewTarget();
        }
        else
        {
            if (Vector3.Distance(transform.position, target) < 0.5f)
            {
                print("LLEGO A LA POSICIÖN");
                target = NewTarget();
            }
            else
            {
                Vector2 direction = (target - transform.position).normalized;
                _rigidbody.MovePosition((Vector2)transform.position + direction * (speed * Time.deltaTime));
                //print(_rigidbody.velocity);
            }
        }
    }

    Vector2 NewTarget()
    {

        target = Random.insideUnitCircle * 5;
        print("NEW TARGET = " + target);
        //while (!GameLimits.IsInLimits(target))
        if (!GameLimits.IsInLimits(target))
        {
            Debug.Log("NO ESTÄ EN LOS LIMITES");
            target = Random.insideUnitCircle * 5;
        }

        return target;
    }

    #endregion

    #region CHASE ENEMY

    void ChaseTarget()
    {
        Vector2 direction = (_target.position - transform.position).normalized;
        _rigidbody.MovePosition((Vector2)transform.position + direction * (speed * Time.deltaTime));
    }


    #endregion

    #region STRAIGHT ENEMY

    void MoveStraight()
    {

    }

    #endregion

}
