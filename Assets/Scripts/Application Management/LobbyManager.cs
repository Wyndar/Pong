using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Relay;
using Unity.Networking.Transport.Relay;

[RequireComponent(typeof(GameManager))]
public class LobbyManager : MonoBehaviour
{
    [SerializeField] private TMP_Text lobbyHeaderText;
    [SerializeField] private GameObject lobbyIDPrefab;
    [SerializeField] private GameObject lobbyPanel, relaySelectPanel, lobbyClients;
    [SerializeField] private TMP_Text joinCodeText;
    [SerializeField] private InputField joinCodeInput;
    [SerializeField] private GameManager GameManager;

    private void Awake() => GameManager = GetComponent<GameManager>();
    public void ToggleLobby(bool shouldShow)
    {
        lobbyHeaderText.text = GameManager.IsHost ? "Hosting" : "Joining";
        lobbyPanel.SetActive(shouldShow);
        if (!shouldShow)
            DisconnectRelay();
    }
    public void ToggleRelaySelection(bool shouldShow) => relaySelectPanel.SetActive(shouldShow);

    public void StartGame() => GameManager.StartGame();

    public void LobbyUpdateOnJoin()
    {
        GameObject p = Instantiate(lobbyIDPrefab, lobbyClients.transform);
        int PlayerNumber = lobbyClients.transform.childCount;
        p.GetComponentInChildren<Toggle>().isOn = false;
        p.GetComponentInChildren<TMP_Text>().text = PlayerNumber == 1 ? "Player 1 (Host)" : $"Player {PlayerNumber} (Client)";
    }
    public async void StartRelay()
    {
        string joinCode = await StartRelayHost();
        joinCodeText.text = joinCode;
        ToggleLobby(true);
    }
    public async void JoinRelay()
    {
        if (joinCodeInput.text.Length < 6)
            return;
        await StartRelayClient(joinCodeInput.text);
        joinCodeText.text = joinCodeInput.text;
        ToggleLobby(true);
    }
    public void DisconnectRelay()
    {
        NetworkManager.Singleton.Shutdown();
        GameManager.ReloadScene();
    }

    private async Task<string> StartRelayHost(int maxConnections = 2)
    {
        Allocation allocation;
        try
        {
            allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
        }
        catch
        {
            Debug.Log("Error in allocation.");
            throw;
        }
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));
        string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        return NetworkManager.Singleton.StartHost() ? joinCode : null;
    }
    private async Task<bool> StartRelayClient(string joinCode)
    {
        JoinAllocation joinAllocation;
        try
        {
            joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        }
        catch
        {
            Debug.Log("Error in allocation.");
            throw;
        }

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
        return !string.IsNullOrEmpty(joinCode) && NetworkManager.Singleton.StartClient();
    }
}
