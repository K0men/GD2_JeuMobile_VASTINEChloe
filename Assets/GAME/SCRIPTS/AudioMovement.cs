 using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioMovement : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _clickSound;

    public void OnClick()
    {
        StartCoroutine(PlaySoundThenLoad());
    }

    private IEnumerator PlaySoundThenLoad()
    {
        _audioSource.PlayOneShot(_clickSound);
        yield return new WaitForSeconds(_clickSound.length);
    }
}
