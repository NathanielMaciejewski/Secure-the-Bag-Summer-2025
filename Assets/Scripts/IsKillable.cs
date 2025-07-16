using UnityEngine;
using UnityEngine.SceneManagement;

public class IsKillable : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
        // If this object collides with a cop or bottomless pit
        if (collision.gameObject.CompareTag("Destroyer"))
        {
            // Show death animation
            // Play death SFX
            // Reload scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            Debug.Log("You Died");
        }
    }

}
