using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
public class GameManager : NetworkBehaviour
{
    public GameObject loadingScreenPrefab;
    public GameObject canvas;
    public List<ulong> PlayerIDs = new();
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
