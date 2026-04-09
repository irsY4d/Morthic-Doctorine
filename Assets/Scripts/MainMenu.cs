using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public void OnClickPlay()
    {
        SceneManager.LoadScene("0_PrologScene");
    }

    public void OnClickCredit()
    {
        print("Creeedit");
    }

    public void OnClickExit()
    {

        Application.Quit();
    }
}
