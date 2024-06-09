using UnityEngine;
using Unity.Netcode;

public class Paddle : NetworkBehaviour
{
    [SerializeField] private GameManager GameManager;
    [SerializeField] private InputManager InputManager;
    [SerializeField] private float paddleSpeed;
    private Rigidbody2D rb;
    private Vector2 startPos;
    private Vector2 endPos;
    private float startTime;
    private float endTime;
    private bool allowMovement;
    private Vector3 resetPosition;
    private void OnEnable()
    {
        GameManager = FindObjectOfType<GameManager>();
        InputManager = FindObjectOfType<InputManager>();
        rb = GetComponent<Rigidbody2D>();
        resetPosition = rb.position;
        InputManager.OnStartTouch += TouchStart;
        InputManager.OnEndTouch += TouchEnd;
    }
    private void OnDisable()
    {
        InputManager.OnEndTouch -= TouchEnd;
        InputManager.OnStartTouch -= TouchStart;
    }
    private void TouchStart(Vector2 touchPosition , float time)
    {
        //can add trail effects using the params here
        startPos = touchPosition;
        startTime = time;
        allowMovement = true;
    }

    private void Update()
    {
        if (!IsOwner)
            return;
        if (allowMovement)
            rb.velocity = (paddleSpeed * new Vector2(0, 0) { x = startPos.x > Screen.width / 2 ? 1f : -1f });
        else
            rb.velocity = Vector2.zero;
    }
    private void TouchEnd(Vector2 touchPosition, float time)
    {
        endPos = touchPosition;
        endTime = time;
        allowMovement = false;
    }
    public void ResetPosition()
    {
        rb.velocity = Vector2.zero;
        rb.position = resetPosition;
    }
}
