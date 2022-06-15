using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroHandler : MonoBehaviour
{
    public void EndIntro()
    {
        Debug.Log("Entering actual game finally lol");
        SceneManager.LoadScene("OutsideTest", LoadSceneMode.Single);
    }
}
