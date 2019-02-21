using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Refresh : MonoBehaviour
{
    public Button btn;
    private void Start()
    {
        //btn.onClick.AddListener(TaskOnClick);
    }
    // Start is called before the first frame update
    public void TaskOnClick()
    {
        SceneManager.LoadScene("Game");
    }
}
