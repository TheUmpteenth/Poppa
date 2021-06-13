///---------------------------------------------------------------------------------------------------------------------
/// Copyright Davie Farrell - 2021
///---------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplaySceneController : MonoBehaviour
{
    public void OnStartButtonPressed()
    {
        SceneManager.LoadScene(SceneDefs.k_gameplaySceneIdx);
    }
}
