using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Relay;
using Unity.Networking.Transport.Relay;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

[RequireComponent(typeof(GameManager))]
public class LobbyManager : MonoBehaviour
{
    [SerializeField] private TMP_Text lobbyHeaderText;
    [SerializeField] private GameObject lobbyIDPrefab;
    [SerializeField] private GameObject lobbyPanel, relaySelectPanel, lobbyClients, loadingPanel, lobbyStartButton;
    [SerializeField] private TMP_Text joinCodeText;
    [SerializeField] private InputField joinCodeInput;
    [SerializeField] private GameManager GameManager;
    private void Awake() => GameManager = GetComponent<GameManager>();
    public void ToggleLobby(bool shouldShow)
    {
        lobbyHeaderText.text = GameManager.IsHost ? "Hosting" : "Joining";
        lobbyStartButton.SetActive(GameManager.IsHost);
        lobbyPanel.SetActive(shouldShow);
        if (!shouldShow)
            DisconnectRelay();
    }
    public void ToggleRelaySelection(bool shouldShow) => relaySelectPanel.SetActive(shouldShow);

    public void StartGame() => GameManager.StartGame();
    public void LobbyUpdate()
    {
        List<Transform> l = new();
        foreach (Transform transform in lobbyClients.transform)
            l.Add(transform);
        foreach (Transform transform in l)
            Destroy(transform.gameObject);
        foreach (ulong PlayerID in NetworkManager.Singleton.ConnectedClientsIds)
        {
            GameObject p = Instantiate(lobbyIDPrefab, lobbyClients.transform);
            p.GetComponentInChildren<Toggle>().isOn = false;
            string v = PlayerID == 0 ? "Host" : "Client";
            p.GetComponentInChildren<TMP_Text>().text = $"Player {PlayerID} ({v})";
        }
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
        GameManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private async Task<string> StartRelayHost(int maxConnections = 2)
    {
        Allocation allocation;
        loadingPanel.SetActive(true);
        try
        {
            allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
            loadingPanel.SetActive(false);
        }
        catch
        {
            Debug.Log("Error in allocation.");
            loadingPanel.SetActive(false);
            throw;
        }
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));
        string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        return NetworkManager.Singleton.StartHost() ? joinCode : null;
    }
    private async Task<bool> StartRelayClient(string joinCode)
    {
        JoinAllocation joinAllocation;
        loadingPanel.SetActive(true);
        try
        {
            joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            loadingPanel.SetActive(false);
        }
        catch
        {
            Debug.Log("Error in allocation.");
            loadingPanel.SetActive(false);
            throw;
        }

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
        return !string.IsNullOrEmpty(joinCode) && NetworkManager.Singleton.StartClient();
    }
}
