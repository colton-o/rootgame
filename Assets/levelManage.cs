using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class levelManage : MonoBehaviour
{
    public void reset()
    {
        SceneManager.LoadScene(0);
    }

}
