using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    private const string SceneName = "GameScene";
    
    public void LoadScene()
    {
        SceneManager.LoadScene(SceneName);
    }
}