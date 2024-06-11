using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public delegate void StartTouch(Vector2 position, float time, bool isFirstTouch);
    public event StartTouch OnStartTouch;
    public delegate void EndTouch(Vector2 position, float time, bool isFirstTouch);
    public event EndTouch OnEndTouch;

    private PlayerInputList playerInputs;

    private void Awake() => playerInputs = new PlayerInputList();

    private void OnEnable() => playerInputs.Enable();

    private void OnDisable() => playerInputs.Disable();

    private void Start()
    {
        playerInputs.Player.Tap.started += context => StartedTouch(context);
        playerInputs.Player.Tap.canceled += context => EndedTouch(context);
        playerInputs.Player.Tap2.started += context => StartedSecondaryTouch(context);
        playerInputs.Player.Tap2.canceled += context => EndedSecondaryTouch(context);
    }
    private void StartedTouch(InputAction.CallbackContext callbackContext) => OnStartTouch?.Invoke(CurrentFingerPosition, (float)callbackContext.startTime, true);

    private void EndedTouch(InputAction.CallbackContext callbackContext) => OnEndTouch?.Invoke(CurrentFingerPosition, (float)callbackContext.time, true);
    private void StartedSecondaryTouch(InputAction.CallbackContext callbackContext) => OnStartTouch?.Invoke(CurrentSecondFingerPosition, (float)callbackContext.startTime, false);

    private void EndedSecondaryTouch(InputAction.CallbackContext callbackContext) => OnStartTouch?.Invoke(CurrentSecondFingerPosition, (float)callbackContext.time, false);

    public Vector2 CurrentFingerPosition => playerInputs.Player.Position.ReadValue<Vector2>();
    public Vector2 CurrentSecondFingerPosition => playerInputs.Player.Position2.ReadValue<Vector2>();
}



