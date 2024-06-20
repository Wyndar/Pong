using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ApplicationManagement : MonoBehaviour
{
    public GameObject loadingScreenPrefab;
    public GameObject canvas;
    public void LoadScene(int sceneNum)
    {
        SceneHandler sceneHandler = Instantiate(loadingScreenPrefab, canvas.transform).GetComponent<SceneHandler>();
        sceneHandler.LoadScene(sceneNum);
    }
    public void ExitApp() => Application.Quit();
}
