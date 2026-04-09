using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class UIToBeContinueScene : MonoBehaviour
{
    [Header("UI")]
    public GameObject tobecontinueText;
    public GameObject ThankyouText;
    public GameObject button;

    void Start()
    {
        StartCoroutine(GameSequence());
    }

    IEnumerator GameSequence()
    {
        tobecontinueText.SetActive(true);
        ThankyouText.SetActive(true);
        button.SetActive(false);

        yield return new WaitForSecondsRealtime(3f);

        tobecontinueText.SetActive(false);
        ThankyouText.SetActive(false);
        button.SetActive(true);
    }

    public void Exit()
    {
        SceneManager.LoadScene("00_MainMenuScene");
    }
}
