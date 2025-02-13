
using Unity.Netcode;
using UnityEngine;

public class NetworkPaddle : NetworkBehaviour
{
    private InputManager InputManager;
    public int clientID;

    public PongManager PongManager;
    public Vector2 resetPosition;
    public float PaddleSpeed { get; private set; }
    public Rigidbody2D rb;
    public float startTime;
    public float endTime;
    public bool allowMovement;
    public bool hasGameStarted;
    public PowerBar powerBar;
    public void ResetPosition()
    {
        rb.linearVelocity = Vector2.zero;
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
    public void SetColor(Color color) => GetComponent<SpriteRenderer>().color = color;
    //bloody americans 
    public void InitializePaddle()
    {
        PongManager = FindFirstObjectByType<PongManager>();
        rb = GetComponent<Rigidbody2D>();
        if (!IsOwner)
            return;
        SetPlayerPaddleRpc(NetworkManager.Singleton.LocalClientId);
        InputManager = FindFirstObjectByType<InputManager>();
        InputManager.OnStartTouch += TouchStart;
        InputManager.OnEndTouch += TouchEnd;
    }
    private void TouchStart(Vector2 position, float time, bool isFirstTouch)
    {
        if (!hasGameStarted)
            return;
        startTime = time;
        allowMovement = true;
    }
    private void TouchEnd(Vector2 touchPosition, float time, bool isFirstTouch)
    {
        if (!hasGameStarted)
            return;
        allowMovement = false;
        endTime = time;
        powerBar.PowerPercentChange(2 * (endTime - startTime), true);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void SetPlayerPaddleRpc(ulong id)
    {
        clientID = (int)id;
        if (clientID == 1)
            PongManager.clientPaddle = this;
        else
            PongManager.hostPaddle = this;
    }
    private void FixedUpdate()
    {
        if (!hasGameStarted || !IsOwner) return;
        int moveInput = GetTouchDirection();
        if(!IsHost) moveInput *= -1;
        ApplyMovement(moveInput);
    }

    private int GetTouchDirection()
    {
        Vector2 touchPosition = allowMovement ? InputManager.CurrentFingerPosition : Vector2.zero;
        float screenCenterX = Screen.width / 2;
        return touchPosition.x == 0 ? 0 : touchPosition.x < screenCenterX ? -1 : 1;
    }
    private void ApplyMovement(int input)
    {
        Vector2 newPosition = rb.position + new Vector2(input * PaddleSpeed * Time.deltaTime, 0);
        rb.MovePosition(newPosition);
    }
}
