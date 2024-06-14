using TMPro;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Relay;
using Unity.Networking.Transport.Relay;

public class RelayManager : MonoBehaviour
{
    [SerializeField] private TMP_Text joinCodeText;
    [SerializeField] private InputField joinCodeInput;
    [SerializeField] private GameManager gameManager;
    
    public async void StartRelay()
    {
        string joinCode = await StartRelayHost();
        joinCodeText.text = joinCode;
        gameManager.ToggleLobby(true);
    }
    public async void JoinRelay()
    {
        if (joinCodeInput.text.Length < 6)
            return;
        await StartRelayClient(joinCodeInput.text);
        joinCodeText.text = joinCodeInput.text;
        gameManager.ToggleLobby(true);
    }

    public async Task<string> StartRelayHost(int maxConnections = 2)
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
        string joinCode= await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        return NetworkManager.Singleton.StartHost() ? joinCode : null;
    }
    public async Task<bool> StartRelayClient(string joinCode)
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
