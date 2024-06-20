using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;
public class GameManager : NetworkBehaviour
{
    public GameObject loadingScreenPrefab;
    public GameObject canvas;

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
