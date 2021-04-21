using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BallType
{
    Ball = 0,
    MultiBall
}

public enum BallState
{
    OnPaddle = 0,
    Shot
}

public class Ball : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioClip m_BounceSound;

    [Header("Components")]
    [SerializeField] private BallType m_BallType;
    private BallState m_BallState;
    private Rigidbody2D m_Rigidbody;

    [Header("Movement")]
    private float m_TravelSpeed = 8;
    [SerializeField] private float m_DefaultSpeed = 8;
    [SerializeField] private float m_MaxSpeed = 8;
    [SerializeField] private float m_AccelerationTime = 120;
    [SerializeField] private GameObject m_Trail;
    private float m_Time;

    [Header("Damage")]
    [SerializeField] private float m_DamageToBricks = 1;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_Trail.SetActive(false);
        m_Time = 0;
    }

    private void OnEnable()
    {
        m_Time = 0;

        switch (m_BallType)
        {
            case BallType.Ball:
                m_BallState = BallState.OnPaddle;
                break;
            case BallType.MultiBall:
                ShootMultiBall();
                break;
            default:
                break;
        }
    }

    private void FixedUpdate()
    {
        switch (m_BallState)
        {
            case BallState.OnPaddle:
                MoveWithPaddle();
                break;
            case BallState.Shot:
                IncreaseSpeed();
                break;
            default:
                break;
        }
    }
    private void IncreaseSpeed()
    {
        m_TravelSpeed = Mathf.SmoothStep(m_DefaultSpeed, m_MaxSpeed, m_Time / m_AccelerationTime);
        m_Rigidbody.position -= (Vector2)transform.forward * m_TravelSpeed * Time.fixedDeltaTime;
        m_Time += Time.fixedDeltaTime;
    }

    private void OnDisable()
    {
        m_Trail.SetActive(false);
        m_Rigidbody.position = Vector2.zero;
        m_Rigidbody.rotation = 0;
        m_TravelSpeed = m_DefaultSpeed;
    }

    private void MoveWithPaddle()
    {
        m_Rigidbody.position = GameManager.m_Instance.GetSpawnOffSet();
    }

    public void ShootBall(float direction)
    {
        if(direction == 0)
        {
            direction = Random.Range(-0.2f, 0.2f);
        }
        Vector2 newDirection = new Vector2(direction, 1).normalized;
        m_Rigidbody.velocity = newDirection * m_TravelSpeed;

        m_Trail.SetActive(true);
        m_BallState = BallState.Shot;
    }

    private void ShootMultiBall()
    {
        Vector2 newDirection = new Vector2(Random.Range(-3, 3), 1).normalized;
        if (newDirection.x == 0)
        {
            newDirection.x = 0.3f;
        }
        m_Rigidbody.velocity = newDirection * m_TravelSpeed;

        m_Trail.SetActive(true);
        m_BallState = BallState.Shot;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (m_BallState == BallState.Shot)
        {
            Paddle paddle = collision.gameObject.GetComponent<Paddle>();
            Brick brick = collision.gameObject.GetComponent<Brick>();

            if (paddle != null)
            {
                float x = (m_Rigidbody.position.x - paddle.transform.position.x) / (collision.collider.bounds.size.x * 1.5f);
                Vector2 direction = new Vector2(x * 3.5f, 1f).normalized;
                m_Rigidbody.velocity = direction * m_TravelSpeed;
            }

            if (brick != null)
            {
                brick.ChangeHealth(-m_DamageToBricks);
            }

            AudioSource.PlayClipAtPoint(m_BounceSound, transform.position);
        }
    }

    public BallType GetBallType()
    {
        return m_BallType;
    }

    public BallState GetBallState()
    {
        return m_BallState;
    }
}

//public IEnumerator AutoShootBall(float direction, float time)
//{
//    //print("Autoshoot Started");
//    //if(GameManager.m_Instance.m_CurrentPaddle.GetComponent<Paddle>().GetIfShot())
//    //{
//    //    ShootBall(direction);
//    //    yield break;
//    //}

//    yield return new WaitForSeconds(time);
//    if (direction == 0)
//    {
//        direction = Random.Range(-0.2f, 0.2f);
//    }
//    Vector2 newDirection = new Vector2(direction, 1).normalized;
//    m_Rigidbody.velocity = newDirection * m_ShootSpeed;
//    m_BallState = BallState.Shot;
//    print("Autoshoot Used");
//}