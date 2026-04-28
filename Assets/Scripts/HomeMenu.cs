using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeMenu : MonoBehaviour
{
    [SerializeField] private string _gameSceneName = string.Empty;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void StartGame()
    {
        SceneManager.LoadScene(_gameSceneName);
        Debug.Log("Start Game");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
