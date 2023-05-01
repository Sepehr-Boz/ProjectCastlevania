using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainManager : MonoBehaviour
{

    #region singleton

    private static MainManager _instance;
    public static MainManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }

    #endregion



    public void StartGame()
    {
        SceneManager.LoadScene("StartScene");
    }
}
