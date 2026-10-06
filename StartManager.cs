using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartManager : MonoBehaviour
{
    public void startgame()
    {
        SceneManager.LoadScene(1);
    }
}
