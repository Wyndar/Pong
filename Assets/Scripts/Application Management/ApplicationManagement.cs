using UnityEngine;
using UnityEngine.SceneManagement;

public class ApplicationManagement : MonoBehaviour
{ 
    public void LoadGame(string gameName)=>SceneManager.LoadSceneAsync(gameName);
    public void ExitApp() => Application.Quit();
}
