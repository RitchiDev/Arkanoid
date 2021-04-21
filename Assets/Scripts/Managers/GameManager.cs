using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager m_Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject m_PauseMenu;
    [SerializeField] private GameObject m_HudButtons;

    [Header("Game")]
    [SerializeField] private float m_RestartDelay;

    [Header("Paddle")]
    [SerializeField] private Transform m_PaddleSpawnpoint;
    public List<Image> m_Hearts = new List<Image>(); //Gebruikt in Paddle
    public GameObject m_CurrentPaddle { get; private set; }

    [Header("Ball")]
    [SerializeField] private AudioClip m_ShootSound;
    [SerializeField] private float m_TimeBeforeAutoShoot = 3f;
    [SerializeField] private Vector2 m_SpawnOffset;
    private GameObject m_CurrentBall;
    private float m_ShootTimer;
    private Ball m_BallToHold;

    [Header("Brick")]
    [SerializeField] private Transform m_BricksSpawnpoint; //Voor nu de Bricks parent in de hierarchy
    [SerializeField] private int m_Columns;
    [SerializeField] private int m_Rows;
    [SerializeField] private Vector2 m_BrickSize;
    private List<GameObject> m_Bricks = new List<GameObject>();
    private GameObject m_CurrentBrick;

    private void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
        }
        else if (m_Instance != null)
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        Time.timeScale = 1;
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            m_PauseMenu.SetActive(false);
            m_HudButtons.SetActive(true);
            AudioManager.m_Instance.SetVolume();
            Restart();
        }
    }

    public void FixedUpdate()
    {
        CheckWhenToShootBall();
    }

    public void Restart() //Word ook gebruikt als button
    {
        if (m_CurrentPaddle == null)
        {
            m_CurrentPaddle = ObjectPooler.m_Instance.SetActiveFromPool("Paddle", m_PaddleSpawnpoint.position, Quaternion.identity); //De speler
        }

        m_CurrentPaddle.GetComponent<Paddle>().ResetPaddlePowerUps();
        RemovePowerUps();
        RemoveBall();

        StartCoroutine(SpawnBallWithDelay());

        ResetBricks();
    }

    private void ResetBricks()
    {
        if (m_CurrentBrick != null)
        {
            m_CurrentBrick = null;
        }

        foreach (GameObject brick in m_Bricks)
        {
            brick.SetActive(false);
        }

        m_Bricks.Clear();

        for (int x = 0; x < m_Columns; x++)
        {
            for (int y = 0; y < m_Rows; y++)
            {
                Vector2 spawnPosition = (Vector2)m_BricksSpawnpoint.position + new Vector2(x * m_BrickSize.x, -y * m_BrickSize.y);
                if (y <= 0)
                {
                    m_CurrentBrick = ObjectPooler.m_Instance.SetActiveFromPool("GoldBrick", spawnPosition, Quaternion.identity);
                }
                else if (y > 0 && y <= 2 && m_Rows >= 2)
                {
                    m_CurrentBrick = ObjectPooler.m_Instance.SetActiveFromPool("SilverBrick", spawnPosition, Quaternion.identity);
                }
                else
                {
                    m_CurrentBrick = ObjectPooler.m_Instance.SetActiveFromPool("Brick", spawnPosition, Quaternion.identity);
                }
                m_Bricks.Add(m_CurrentBrick);
            }
        }
    }
    private void CheckWhenToShootBall()
    {
        if (m_ShootTimer > 0)
        {
            m_ShootTimer = Mathf.Clamp(m_ShootTimer -= Time.fixedDeltaTime, 0, m_TimeBeforeAutoShoot);
        }

        if (m_BallToHold != null)
        {
            if (m_BallToHold.GetBallState() == BallState.OnPaddle)
            {
                if (m_CurrentPaddle.GetComponent<Paddle>().GetIfShot() || m_ShootTimer <= 0) //Als de speler schiet of als de timer 0 is
                {
                    AudioSource.PlayClipAtPoint(m_ShootSound, m_BallToHold.transform.position);
                    m_BallToHold.ShootBall(m_CurrentPaddle.GetComponent<Paddle>().GetHorizontalInput());
                    m_BallToHold = null;
                }
            }
        }
    }

    public void GivePowerUp(PowerUpType type, float time)
    {
        switch (type)
        {
            case PowerUpType.SpeedUp:
                StartCoroutine(m_CurrentPaddle.GetComponent<Paddle>().SpeedUp(time));
                break;
            case PowerUpType.SizeUp:
                StartCoroutine(m_CurrentPaddle.GetComponent<Paddle>().SizeUp(time));
                break;
            default:
                break;
        }
    }

    private void CheckForWin()
    {
        if (m_Bricks.Count <= 0)
        {
            m_CurrentBall.SetActive(false);
            StartCoroutine(RestartWithDelay());
        }
    }

    private IEnumerator RestartWithDelay()
    {
        yield return new WaitForSeconds(m_RestartDelay);
        Restart();
        StopCoroutine(RestartWithDelay());
    }

    public IEnumerator SpawnBallWithDelay()
    {
        RemoveBall();
        yield return new WaitForSeconds(m_RestartDelay);
        if (m_CurrentBall == null)
        {
            m_CurrentBall = ObjectPooler.m_Instance.SetActiveFromPool("Ball", GetSpawnOffSet(), Quaternion.identity); //De bal

            if(m_CurrentBall.GetComponent<Ball>().GetBallType() == BallType.Ball)
            {
                m_BallToHold = m_CurrentBall.GetComponent<Ball>();
                m_ShootTimer = m_TimeBeforeAutoShoot;
            }
        }
    }

    private void RemoveBall()
    {
        if (m_CurrentBall != null)
        {
            m_CurrentBall.SetActive(false);
            m_CurrentBall = null;
        }
    }

    private void RemovePowerUps()
    {
        GameObject[] foundPowerUps = GameObject.FindGameObjectsWithTag("PowerUp");
        for (int i = 0; i < foundPowerUps.Length; i++)
        {
            foundPowerUps[i].SetActive(false);
        }
    }

    public Vector2 GetSpawnOffSet()
    {
        return (Vector2)m_CurrentPaddle.transform.position + m_SpawnOffset;
    }

    public void RemoveBrick(GameObject brick)
    {
        m_Bricks.Remove(brick);
        CheckForWin();
    }

    public void RemovePaddle()
    {
        m_CurrentPaddle = null;
        StartCoroutine(RestartWithDelay());
    }

    public void Pause() //Wordt ook als button gebruikt
    {
        if (Time.timeScale > 0)
        {
            m_HudButtons.SetActive(false);
            m_PauseMenu.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            m_PauseMenu.SetActive(false);
            m_HudButtons.SetActive(true);
            Time.timeScale = 1;
        }

        AudioManager.m_Instance.SetVolume();
    }

    public void HealPaddle() //Wordt ook als button gebruikt
    {
        m_CurrentPaddle.GetComponent<Paddle>().ChangeHealth(100);
    }

    public void Play() //Wordt ook als button gebruikt
    {
        SceneManager.LoadScene(1); //Arkanoid scene (level)
    }

    public void ReloadScene() //Wordt ook als button gebruikt
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMenu() //Wordt ook als button gebruikt
    {
        SceneManager.LoadScene(0); //Menu scene
    }

    public void Quit() //Wordt ook als button gebruikt
    {
        Application.Quit();
    }
}

//ObjectPooler.m_ObjectPoolerInstance.SetActiveFromPool("Paddle", m_PaddleSpawnpoint.position, Quaternion.identity).SetActive(true);
//m_Paddle.SetActive(true);
//ObjectPooler.m_ObjectPoolerInstance.GetObjectFromPool().SetActive(true);
//Vector2 spawnPosition = (Vector2)m_BricksSpawnpoint.position + new Vector2(x * (m_BrickPrefab.GetComponent<Brick>().GetBrickSize().x * 2f), -y + m_BrickPrefab.GetComponent<Brick>().GetBrickSize().y);
//print("Restart Game");
//print("Ball was null has now spawned");
//yay gewonnen
//m_Bricks.Remove(brick);
//print("Spawn ball");
//Vector2 spawnPosition = (Vector2)m_BricksSpawnpoint.position + new Vector2(x * (m_BrickPrefab.GetComponent<Brick>().GetBoxCollider2D().bounds.size.x + m_PositionOffset), -y * (m_BrickPrefab.GetComponent<Brick>().GetBoxCollider2D().bounds.size.y + m_PositionOffset * 0.5f));