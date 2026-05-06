using UnityEngine;
using UnityEngine.UI;

public class BossRoomBound : MonoBehaviour
{
    public static BossRoomBound instance;
    public GameObject bossHealthPanel;

    private Collider2D _roomCol;
    [SerializeField] private Boss1AI _bossAI;
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


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            bossHealthPanel.SetActive(true);
            _bossAI.IsPlayerInBound(true);
            blockingWay.SetStateWay(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {

            _bossAI.IsPlayerInBound(false);
            Debug.Log("stop");

        }
    }
}
