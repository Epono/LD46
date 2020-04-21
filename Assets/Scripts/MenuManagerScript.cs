using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Video;

public class MenuManagerScript : MonoBehaviour
{
    [SerializeField]
    Text keep;

    [SerializeField]
    Text it;

    [SerializeField]
    Text aflame;

    [SerializeField]
    Text pressAnyKey;

    [SerializeField]
    VideoPlayer videoPlayer;

    [SerializeField]
    RawImage video;


    // Start is called before the first frame update
    void Start()
    {
        video.gameObject.SetActive(false);

        keep.color = new Color(keep.color.r, keep.color.g, keep.color.b, 0.0f);
        it.color = new Color(it.color.r, it.color.g, it.color.b, 0.0f);
        aflame.color = new Color(aflame.color.r, aflame.color.g, aflame.color.b, 0.0f);
        pressAnyKey.color = new Color(pressAnyKey.color.r, pressAnyKey.color.g, pressAnyKey.color.b, 0.0f);

        IEnumerator coKeep = FadeInAfterDelay(keep, 1.0f, 1.5f);
        IEnumerator coIt = FadeInAfterDelay(it, 2.5f, 1.5f);
        IEnumerator coAflame = FadeInAfterDelay(aflame, 4.0f, 3.0f);
        IEnumerator coPress = FadeInPress(6.0f);
        IEnumerator coVideo = WaitForVideo(4.0f);
        StartCoroutine(coKeep);
        StartCoroutine(coIt);
        StartCoroutine(coAflame);
        StartCoroutine(coPress);
        StartCoroutine(coVideo);
    }

    IEnumerator FadeInAfterDelay(Text text, float delay, float duration)
    {
        yield return new WaitForSeconds(delay);
        DOTween.To(() => text.color, x => text.color = x, new Color(text.color.r, text.color.g, text.color.b, 1), duration);
    }

    IEnumerator WaitForVideo(float delay)
    {
        yield return new WaitForSeconds(delay);
        videoPlayer.Play();
        yield return new WaitForSeconds(0.3f);
        video.gameObject.SetActive(true);
    }

    IEnumerator FadeInPress(float delay)
    {
        yield return new WaitForSeconds(delay);
        DOTween.To(() => pressAnyKey.color, x => pressAnyKey.color = x, new Color(pressAnyKey.color.r, pressAnyKey.color.g, pressAnyKey.color.b, 1), 3).SetLoops(-1, LoopType.Yoyo);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        else if (Input.anyKeyDown)
        {
            StartCoroutine(LoadScene());
            SoundManagerScript.instance.PlayOneShotSound(SoundManagerScript.AudioClips.MainMenu);
        }
    }

    IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene("MainScene");
    }


}
