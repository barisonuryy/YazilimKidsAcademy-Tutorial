using UnityEngine;

public class PlayerSoundsProvider : MonoBehaviour
{
   public AudioClip audio;
   public AudioSource audioSource;

   public void PlayAttackSound()
    {
        audioSource.PlayOneShot(audio);
    }
}
