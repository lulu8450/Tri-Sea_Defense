using UnityEngine;

public class CreditScroll : MonoBehaviour
{
    public float speed = 50f;
    public float stopY = 800f;          // Y où le scroll doit s'arrêter
    public AudioSource audioSource;     // AudioSource attaché au panel ou texte

    private bool finished = false;

    void Update()
    {
        if (!finished)
        {
            transform.Translate(Vector3.up * speed * Time.deltaTime);

            if (transform.position.y >= stopY)
            {
                finished = true;

                // Stop le son quand le texte a fini
                if (audioSource != null)
                {
                    audioSource.Stop();
                }
            }
        }
    }
}
