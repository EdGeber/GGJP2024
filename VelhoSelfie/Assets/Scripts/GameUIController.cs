using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUIController : MonoBehaviour
{
    public void ReloadScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    public void PlayerDied()
    {
        GameObject.Find("Player").SetActive(false);
        transform.GetChild(0).gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
    }
}
