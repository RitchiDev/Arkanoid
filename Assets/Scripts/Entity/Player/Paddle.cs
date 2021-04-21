using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Paddle : Entity
{
    [Header("Components")]
    [SerializeField] private Rigidbody2D m_Rigidbody; //Serialized vanwege OnDrawGizmo
    private Animator m_Animator;
    private BoxCollider2D m_PaddleCollider;
    private bool m_IsLong;

    [Header("Movement")]
    private Vector2 m_MovementInput;
    private float m_MovementSpeed;
    private bool m_OnWall;
    private bool m_PressedShoot;

    [Header("Effect")]
    [SerializeField] private GameObject m_Trail;

    [Header("Effect")]
    private IEnumerator m_SpeedCoroutine;
    private IEnumerator m_SizeCoroutine;
    protected override void Awake()
    {
        base.Awake();
        m_Rigidbody = GetComponent<Rigidbody2D>();
        m_PaddleCollider = GetComponent<BoxCollider2D>();
        m_Animator = GetComponent<Animator>();
        m_Trail.transform.localScale = new Vector2(m_Trail.transform.localScale.x * 3, m_Trail.transform.localScale.y);
    }

    protected override void Start()
    {
        base.Start();
        VariableDebug();
        UpdateHeartSprite();
    }

    private void Update()
    {
        UpdateAnimation();
    }

    protected void FixedUpdate()
    {
        PaddleMovement();
        UpdateOverlapBox();
    }

    private void OnEnable()
    {
        UpdateHeartSprite();
        ResetPaddlePowerUps();
        UpdateHeartSprite();
    }

    private void PaddleMovement()
    {
        float moveLimit = m_PaddleData.m_BaseMoveLimit - (m_PaddleCollider.bounds.size.x) * 0.5f;
        Vector2 position = m_Rigidbody.position + (Vector2.right * m_MovementInput.x * m_MovementSpeed * Time.fixedDeltaTime);
        position.x = Mathf.Clamp(position.x, -moveLimit, moveLimit);

        m_Rigidbody.MovePosition(position);
    }

    public void MovementInputInfo(InputAction.CallbackContext context)
    {
        m_MovementInput = context.ReadValue<Vector2>();
    }

    public void ShootTriggerInfo(InputAction.CallbackContext context)
    {
        m_PressedShoot = context.performed;
    }

    public IEnumerator SpeedUp(float time)
    {
        m_MovementSpeed =  Mathf.Lerp(m_PaddleData.m_DefaultSpeed, m_PaddleData.m_PoweredUpSpeed, 1.5f);
        m_Trail.SetActive(true);

        yield return new WaitForSeconds(time);

        m_MovementSpeed = Mathf.Lerp(m_PaddleData.m_PoweredUpSpeed, m_PaddleData.m_DefaultSpeed, 1.5f);
        m_Trail.SetActive(false);
    }

    public IEnumerator SizeUp(float time)
    {
        m_PaddleCollider.size = Vector2.Lerp(m_PaddleData.m_ShortPaddleSize, m_PaddleData.m_LongPaddleSize, 1);
        if(!m_IsLong)
        {
            AudioSource.PlayClipAtPoint(m_PaddleData.m_StretchSound, m_Rigidbody.position);
        }

        m_IsLong = true;

        yield return new WaitForSeconds(time);

        m_PaddleCollider.size = Vector2.Lerp(m_PaddleData.m_LongPaddleSize, m_PaddleData.m_ShortPaddleSize, 1);
        AudioSource.PlayClipAtPoint(m_PaddleData.m_ShrinkSound, m_Rigidbody.position);
        m_IsLong = false;
    }

    public void ResetPaddlePowerUps()
    {
        StopCoroutine(SpeedUp(0));
        StopCoroutine(SizeUp(0));
        m_MovementSpeed = m_PaddleData.m_DefaultSpeed;
        m_PaddleCollider.size = m_PaddleData.m_ShortPaddleSize;
        m_IsLong = false;
        m_Trail.SetActive(false);
    }

    public float GetHorizontalInput()
    {
        return m_MovementInput.x;
    }

    public bool GetIfShot()
    {
        return m_PressedShoot;
    }

    public bool GetIfHurt()
    {
        return m_Health < 2;
    }

    private void UpdateOverlapBox()
    {
        bool onRightWall = Physics2D.OverlapBox(m_Rigidbody.position + m_PaddleData.m_RightOffset, m_PaddleData.m_WallDetectSize, 0, m_PaddleData.m_WallLayer); //Rechter OverlapBox
        bool onLeftWall = Physics2D.OverlapBox(m_Rigidbody.position + m_PaddleData.m_LeftOffset, m_PaddleData.m_WallDetectSize, 0, m_PaddleData.m_WallLayer); //Linker OverlapBox
        m_OnWall = onLeftWall || onRightWall;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(m_Rigidbody.position + m_PaddleData.m_RightOffset, m_PaddleData.m_WallDetectSize); //Rechts
        Gizmos.DrawWireCube(m_Rigidbody.position + m_PaddleData.m_LeftOffset, m_PaddleData.m_WallDetectSize); //Links
    }

    public void UpdateHeartSprite()
    {
        for (int i = 0; i < GameManager.m_Instance.m_Hearts.Count; i++)
        {
            if (i < m_Health)
            {
                GameManager.m_Instance.m_Hearts[i].sprite = m_PaddleData.m_FullHeart;
            }
            else
            {
                GameManager.m_Instance.m_Hearts[i].sprite = m_PaddleData.m_EmptyHeart;
            }
        }
    }

    private void UpdateAnimation()
    {
        m_Animator.SetFloat("Movement", m_MovementInput.x);
        m_Animator.SetBool("OnWall", m_OnWall);
        m_Animator.SetBool("IsLong", m_IsLong);
    }

    private void VariableDebug()
    {
        if (m_MovementSpeed == 0)
        {
            Debug.LogError("Hey jij daar! De paddle zijn movement speed staat op 0!");
        }
        if (m_PaddleData.m_BaseMoveLimit == 0)
        {
            Debug.LogError("Hey jij daar! De paddle zijn base move limit staat op 0!");
        }
        if (m_PaddleData == null)
        {
            Debug.LogError("Je moet de ScriptableObject nog in de prefab slepen!");
        }
    }
}


//m_IsMoving = m_MovementInput.x != 0 ? true : false;
//if(m_MovementInput.x != 0)
//{
//    m_IsMoving = true;
//print("update " + i);
//}
//else
//{
//    m_IsMoving = false;
//}
