using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    public void LoadScene()
    {
        SceneManager.LoadScene("GameScene");
    }
}