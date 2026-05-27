using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BossRoomBound : MonoBehaviour
{
    public static BossRoomBound instance;
    public GameObject bossHealthPanel;

    private Collider2D _roomCol;
    [SerializeField] private Boss1AI _bossAI;
    [SerializeField] private Boss1Health _bossHealth;
    private BlockingWay blockingWay;

    public float minX { get; private set; }
    public float maxX { get; private set; }
    public float minY { get; private set; }
    public float maxY { get; private set; }

    private void Awake()
    {
        instance = this;
        _roomCol = GetComponent<Collider2D>();
        blockingWay = gameObject.GetComponent<BlockingWay>();

        minX = _roomCol.bounds.min.x;
        maxX = _roomCol.bounds.max.x;
        minY = _roomCol.bounds.min.y;
        maxY = _roomCol.bounds.max.y;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _bossAI.IsPlayerInBound(false);
    }

    private void OnEnable()
    {
        PlayerHealth.OnPlayerDeath += ResetBossRoom;
    }

    private void OnDisable()
    {
        PlayerHealth.OnPlayerDeath -= ResetBossRoom;
    }

    private void ResetBossRoom()
    {
        StartCoroutine(WaitToResetBossRoom());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            AudioManager.Instance.PlayMusic("BossRoom");
            AudioManager.Instance.Stop("InGame");

            bossHealthPanel.SetActive(true);
            _bossAI.IsPlayerInBound(true);
            blockingWay.SetStateWay(true);
        }
    }

    private IEnumerator WaitToResetBossRoom()
    {
        yield return new WaitForSeconds(3.5f);
        bossHealthPanel.SetActive(false);
        _bossAI.IsPlayerInBound(false);
        _bossAI.ResetPos();
        blockingWay.SetStateWay(false);
        _bossHealth.resetBossHealth();

        AudioManager.Instance.Stop("BossRoom");
        AudioManager.Instance.PlayMusic("InGame");
    }
}
