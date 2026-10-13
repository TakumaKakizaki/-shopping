using UnityEngine;
using UnityEngine.SceneManagement;

public class ReStartScript : MonoBehaviour
{
    [SerializeField] private string SceneName = ("Scene");
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void ChangeScene()
    {
        SceneManager.LoadScene("Scene");
    }
}
