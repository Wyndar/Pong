using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;
using System;
using UnityEngine.SceneManagement;
public class GameManager : NetworkBehaviour
{
    public virtual void StartGame()
    {
        Debug.Log("Failed Override, please check for proper implementation of inheritance.");
    }

    public void ReloadScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    public void ExitGame() => SceneManager.LoadSceneAsync("Home");
}
