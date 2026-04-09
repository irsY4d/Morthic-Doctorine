using UnityEngine;
using UnityEngine.SceneManagement;

public class ToBeContinue : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene("Z_ToBeContinue");    
        }
    }
}
