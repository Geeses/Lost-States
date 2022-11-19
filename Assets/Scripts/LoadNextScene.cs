using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class LoadNextScene : NetworkBehaviour
{
    private void Start()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
