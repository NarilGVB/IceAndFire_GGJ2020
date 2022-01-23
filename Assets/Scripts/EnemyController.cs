using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    SpriteRenderer enemySprite;
    public GameObject enemyFusionPrefab;
    public GameObject iceEnemyFusionPrefab;
    public GameObject fireEnemyFusionPrefab;
    public GameObject fireEnemyDeathInstance;
    public GameObject iceEnemyDeathInstance;
    
    Animator enemyAnimator;
    public float speed = 2f;
    [SerializeField] public float temperature = 10;
    [SerializeField] public float mergeDelay = 0.5f;
    
    private Transform _target;
    private Rigidbody2D _rigidbody;
    private bool _isDead;
    float oldPosition;

    private float _coolDown;

    public bool canMove = true;

    //[SerializeField]
    public enum MoveType
    {
        Chase,
        RunAway,
        Straight,
        Oscilating
    };

    MoveType _moveType;

    private Vector2 direction;

    public static EnemyController Spawn(GameObject prefab, Vector3 position, 
        Transform parent, Transform target, Vector3 scale, MoveType moveType, float temperature = 0)
    {
        EnemyController ins = Instantiate(prefab, position, Quaternion.identity, parent).GetComponent<EnemyController>();

        ins._target = target;
        ins._rigidbody = ins.GetComponent<Rigidbody2D>();
        ins.transform.localScale = scale;
        if (temperature != 0) ins.temperature = temperature;
        ins._moveType = moveType;

        return ins;
    }

    private void Start()
    {
        _coolDown = 1;

        enemySprite = transform.GetChild(1).GetComponent<SpriteRenderer>();
        if ((int)_moveType == 2)
        {
            direction = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f)).normalized;
        }
        enemyAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (_coolDown > 0) _coolDown -= Time.deltaTime;

        if (transform.position.x > oldPosition)
        {
            enemySprite.flipX = true;
        }

        if (transform.position.x < oldPosition)
        {
            enemySprite.flipX = false;
        }
    }
    void LateUpdate()
    {
        oldPosition = transform.position.x;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        SetOrderInLayer();

        if (!canMove || GameManager.IsDead)
            return;

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

    void SetOrderInLayer()
    {
        //enemySprite.sortingOrder = Mathf.RoundToInt(transform.position.y * -10);
        Vector3 pos = transform.position;
        pos.z = pos.y;
        transform.position = pos;
    }

    public void DestroyEnemy()
    {
        if (_isDead) return;
        
        _isDead = true;
        Destroy(gameObject);
    }

    public void instantiateDeathAnimation()
    {
        if (tag == "FireEnemy")
        {
            GameObject fireDeathInstance = Instantiate(fireEnemyDeathInstance, transform.position, Quaternion.identity);
            fireDeathInstance.transform.localScale = GetScale(temperature);
        }
        else if (tag == "IceEnemy")
        {
            GameObject iceDeathInstance = Instantiate(iceEnemyDeathInstance, transform.position, Quaternion.identity);
            iceDeathInstance.transform.localScale = GetScale(temperature);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ManageCollision(other);

        if (other.transform.tag == "Wall")
        {

            if ((int)_moveType == 2)
            {
                float x = other.transform.position.x;
                float y = other.transform.position.y;

                if (Mathf.Abs(x) > Mathf.Abs(y))
                    direction.x *= -1;
                else
                    direction.y *= -1;
            }

            if ((int)_moveType == 3)
            {
                speed *= -1;
            }

        }

    }

    private void OnTriggerStay2D(Collider2D other)
    {
        ManageCollision(other);
    }

    private void ManageCollision(Collider2D other)
    {
        if (_isDead || _coolDown > 0) return;

        EnemyController otherController = other.gameObject.GetComponent<EnemyController>();

        if (otherController == null || otherController._coolDown > 0) return;

        _coolDown = mergeDelay;
        float finalTemp = temperature + otherController.temperature;

        if (!CompareTag(otherController.tag))
        {
            if (Math.Abs(temperature) > Math.Abs(otherController.temperature))
            {
                Instantiate(fireEnemyDeathInstance, transform.position, Quaternion.identity);
                otherController.DestroyEnemy();
                temperature = finalTemp;
                transform.localScale = GetScale(temperature);
            }
            else if (Math.Abs(temperature) < Math.Abs(otherController.temperature))
            {
                Instantiate(iceEnemyDeathInstance, transform.position, Quaternion.identity);
                otherController.temperature = finalTemp;
                otherController.transform.localScale = GetScale(otherController.temperature);
                DestroyEnemy();
            }
            else
            {
                Debug.Log("Destroyed spirits: " + finalTemp);
                GameObject fusedEnemies = Instantiate(enemyFusionPrefab, transform.position + ((otherController.transform.position - transform.position) / 2), Quaternion.identity);
                fusedEnemies.transform.localScale = GetScale(temperature);
                otherController.DestroyEnemy();
                DestroyEnemy();
            }
        }
        else
        {
            /*instantiateDeathAnimation();
            otherController.DestroyEnemy();
            temperature = finalTemp;
            transform.localScale = GetScale(temperature);*/

            StartCoroutine(SpiritFusion(otherController, finalTemp));
        }
    }

    IEnumerator SpiritFusion(EnemyController otherController,float finalTemp)
    {
        otherController.DestroyEnemy();
        temperature = finalTemp;

        if (temperature > 20)
            temperature = 20;

        enemySprite.enabled = false;

        if(tag == "FireEnemy")
        {
            GameObject fusedEnemy = Instantiate(fireEnemyFusionPrefab, transform.position, Quaternion.identity);
            fusedEnemy.transform.localScale = GetScale(temperature);
        }
        else if (tag == "IceEnemy")
        {
            GameObject fusedEnemy = Instantiate(iceEnemyFusionPrefab, transform.position, Quaternion.identity);
            fusedEnemy.transform.localScale = GetScale(temperature);
        }

        yield return new WaitForSeconds(0.45f);
        enemySprite.enabled = true;


        transform.localScale = GetScale(temperature);

        //print(transform.localScale + " " + temperature);

    }

    private static Vector3 GetScale(float temperature)
    {
        float scale = 1f + (2f * Math.Abs(temperature)) / 25f;
        
        return new Vector3(scale, scale, scale);
    }


    #region MOVEMENT

    #region RUN AWAY ENEMY

    Vector3 target;

    void RunAway()
    {

        if (Vector3.Distance(_target.position, transform.position) < 2f)
        {
            direction = (_target.position - transform.position).normalized * -1;
            CheckBorders();
            _rigidbody.MovePosition((Vector2)transform.position + direction * (speed * 0.5f * Time.deltaTime));
        }
        else if (Vector3.Distance(_target.position, transform.position) > 2.2f)
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
                target = NewTarget();
            }
            else
            {
                direction = (target - transform.position).normalized;

                //CheckBorders();

                _rigidbody.MovePosition((Vector2)transform.position + direction * (speed * Time.deltaTime));
            }
        }
    }

    void CheckBorders()
    {

        if (transform.position.x < -GameLimits.limitRoom.x || transform.position.x > GameLimits.limitRoom.x)
        {
            direction.x = 0;
        }
        if (transform.position.y < -GameLimits.limitRoom.y || transform.position.y > GameLimits.limitRoom.y)
        {
            direction.y = 0;
        }

    }

    Vector2 NewTarget()
    {

        target = GameLimits.GetRandomPositionInLimits();
        
        /*if (!GameLimits.IsInLimits(target))
        {
            target = Random.insideUnitCircle * 5;
        }
        */
        return target;
    }

    #endregion

    #region CHASE ENEMY

    void ChaseTarget()
    {
        direction = (_target.position - transform.position).normalized;
        _rigidbody.MovePosition((Vector2)transform.position + direction * (speed * Time.deltaTime));
    }


    #endregion

    #region STRAIGHT ENEMY

    void MoveStraight()
    {

        _rigidbody.MovePosition((Vector2)transform.position + direction * (speed * Time.deltaTime));

    }

    #endregion

    #region OSCILATING

    void MoveOscilating()
    {
        transform.Translate(Mathf.Sin(Time.time) * Time.deltaTime * speed, speed, 0);
    }

    #endregion

    #endregion

}
