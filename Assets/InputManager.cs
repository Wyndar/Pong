using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public delegate void StartTouch(Vector2 position, float time);
    public event StartTouch OnStartTouch;
    public event StartTouch OnStartSecondaryTouch;
    public delegate void EndTouch(Vector2 position, float time);
    public event EndTouch OnEndTouch;
    public event EndTouch OnEndSecondaryTouch;

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
    private void StartedTouch(InputAction.CallbackContext callbackContext) => OnStartTouch?.Invoke(playerInputs.Player.Position.ReadValue<Vector2>(), (float)callbackContext.startTime);

    private void EndedTouch(InputAction.CallbackContext callbackContext) => OnEndTouch?.Invoke(playerInputs.Player.Position.ReadValue<Vector2>(), (float)callbackContext.time);
    private void StartedSecondaryTouch(InputAction.CallbackContext callbackContext) => OnStartSecondaryTouch?.Invoke(playerInputs.Player.Position2.ReadValue<Vector2>(), (float)callbackContext.startTime);

    private void EndedSecondaryTouch(InputAction.CallbackContext callbackContext) => OnEndSecondaryTouch?.Invoke(playerInputs.Player.Position2.ReadValue<Vector2>(), (float)callbackContext.time);

    public Vector2 CurrentFingerPosition => playerInputs.Player.Position.ReadValue<Vector2>();
    public Vector2 CurrentSecondFingerPosition => playerInputs.Player.Position2.ReadValue<Vector2>();
}



