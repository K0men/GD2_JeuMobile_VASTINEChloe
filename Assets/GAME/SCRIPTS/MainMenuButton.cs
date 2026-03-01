using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _clickSound;
    [SerializeField] private string _sceneName;

    /// Il faut que ça joue le son PUIS lance l'autre scene
    public void OnClick()
    {
        StartCoroutine(PlaySoundThenLoad());
    }

    private IEnumerator PlaySoundThenLoad()
    {
        _audioSource.PlayOneShot(_clickSound);
        yield return new WaitForSeconds(_clickSound.length);
        SceneManager.LoadScene(_sceneName);
    }
}
