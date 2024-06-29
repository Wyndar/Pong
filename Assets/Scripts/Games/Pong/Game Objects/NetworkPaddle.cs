using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkPaddle : NetworkBehaviour
{
    private InputManager InputManager;
    private bool isTouch1;
    public int clientID;
    private float time;

    public PongManager PongManager;
    public Vector2 resetPosition;
    public float PaddleSpeed { get; private set; }
    public Rigidbody2D rb;
    public Vector2 startPos;
    public Vector2 endPos;
    public float startTime;
    public float endTime;
    public bool allowMovement;
    public bool hasGameStarted;
    public PowerBar powerBar;
    public void ResetPosition()
    {
        rb.velocity = Vector2.zero;
        rb.position = resetPosition;
        GetComponent<RectTransform>().localScale = new(1, 0.125f);
        PaddleSpeed = 5;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Ball"))
            return;
        if (PongManager.gameType == GameType.VSOnline)
            PongManager.PowerBarChargeRpc(5, true, IsHost);
        else
            powerBar.PowerPercentChange(5, true);
    }
    public void ScaleSize(float scale) => GetComponent<RectTransform>().localScale = new(scale, 0.125f);
    public void ChangeSpeed(float speed) => PaddleSpeed = speed;
    public void DisableScript() => enabled = IsOwner;
    //bloody americans 
    public void SetColor(Color color) => GetComponent<SpriteRenderer>().color = color;
    public override void OnNetworkSpawn()
    {
        PongManager = FindObjectOfType<PongManager>();
        rb = GetComponent<Rigidbody2D>();
        if (!IsOwner)
            return;
        SetPlayerPaddleRpc(NetworkManager.Singleton.LocalClientId);
        InputManager = FindObjectOfType<InputManager>();
        InputManager.OnStartTouch += TouchStart;
        InputManager.OnEndTouch += TouchEnd;
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void SetPlayerPaddleRpc(ulong id) => clientID = (int)id;

    private void TouchStart(Vector2 position, float time, bool isFirstTouch)
    {
        if (!hasGameStarted)
            return;
        startPos = position;
        startTime = time;
        allowMovement = true;
    }

    private void Update()
    {
        if (!hasGameStarted)
            rb.position = PongManager.offscreenPosition.position;
        if (allowMovement && PongManager.gameInputType != InputType.Gyro)
        {
            rb.velocity = name == "Blue Player"
                ? PaddleSpeed * new Vector2(0, 0) { x = startPos.x > Screen.width / 2 ? 1f : -1f }
                : PaddleSpeed * new Vector2(0, 0) { x = startPos.x > Screen.width / 2 ? -1f : 1f };
        }
        else if (PongManager.gameInputType != InputType.Touchscreen)
        {
            rb.velocity = name == "Blue Player"
                ? PaddleSpeed * new Vector2(0, 0) { x = Accelerometer.current.acceleration.ReadValue().x > 0 ? 1f : -1f }
                : PaddleSpeed * new Vector2(0, 0) { x = Accelerometer.current.acceleration.ReadValue().x < 0 ? -1f : 1f };
            time += Time.deltaTime;
            if (time >= 1)
            {
                PongManager.PowerBarChargeRpc(2, true, IsHost);
                time = 0;
            }
        }
        else
            rb.velocity = Vector2.zero;
    }
    private void TouchEnd(Vector2 touchPosition, float time, bool isFirstTouch)
    {
        if (!hasGameStarted)
            return;
        endPos = touchPosition;
        endTime = time;
        allowMovement = false;
        PongManager.PowerBarChargeRpc(2 * (endTime - startTime), true, IsHost);
    }
}

