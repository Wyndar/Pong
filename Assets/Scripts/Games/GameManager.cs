using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
public class GameManager : NetworkBehaviour
{
    public GameObject NetworkManagerPrefab;
    public InputType gameInputType;
    public GameObject loadingScreenPrefab;
    public GameObject canvas;
    public List<ulong> PlayerIDs = new();
    public SaveManager SaveManager;

    //allowing for other games while preserving the singleton nature
    public virtual void Awake()
    {
        if (NetworkManager.Singleton == null)
        {
            Instantiate(NetworkManagerPrefab);
            NetworkManager.Singleton.gameObject.name = NetworkManagerPrefab.name;
        }
        else if (NetworkManager.Singleton.gameObject.name != NetworkManagerPrefab.name)
        {
            Destroy(NetworkManager.Singleton.gameObject);
            Instantiate(NetworkManagerPrefab);
        }
        SaveManager = GetComponent<SaveManager>();
    }
    public void LoadScene(int sceneNum)
    {
       SceneHandler sceneHandler = Instantiate(loadingScreenPrefab, canvas.transform).GetComponent<SceneHandler>();
        sceneHandler.LoadScene(sceneNum);
    }
    public virtual void StartGame()
    {
        Debug.Log("Failed Override, please check for proper implementation of inheritance.");
    }
}
