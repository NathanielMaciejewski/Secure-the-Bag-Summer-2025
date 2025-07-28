using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IsKillable : MonoBehaviour
{
    public bool isVulnerable = true;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        // If this object collides with a cop or bottomless pit
        if (collision.gameObject.CompareTag("Destroyer"))
        {
            Kill();
        }
    }

    public void Kill()
    {
        Debug.Log("You Died");

        // Show death animation
        AudioManager.instance.PlayOneShot(FMODEvents.instance.playerDeath, this.transform.position);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }
}
