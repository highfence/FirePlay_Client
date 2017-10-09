using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * 게임 시작전에 처리해야할 것들이 있다면 처리해주는 클래스.
 * 싱글톤 클래스 초기화를 위해 제작.
 */
public class LaunchSceneManager : MonoBehaviour
{
	void Start ()
    {
        // 스크린 설정 초기화.
        Screen.SetResolution(1280, 800, false);

        StartCoroutine("OnClassLoad");

        // 데이터 컨테이너 클래스 초기화.
        DataContainer.GetInstance();

        // 네트워크 클래스 초기화.
        NetworkManager.GetInstance();

        // 이펙트 클래스 초기화.
        EffectManager.GetInstance();
	}

    IEnumerator OnClassLoad()
    {
        var loadingRenderer = GetComponent<SpriteRenderer>();

        var curColor = loadingRenderer.color;
        curColor.a = 0.0f;
        loadingRenderer.color = curColor;

        while (curColor.a < 1.0f)
        {
            curColor.a += 0.025f;
            loadingRenderer.color = curColor;

            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(0.5f);

        while (curColor.a > 0.0f)
        {
            curColor.a -= 0.025f;
            loadingRenderer.color = curColor;

            yield return new WaitForSeconds(0.05f);
        }

        SceneManager.LoadScene("1. Login");
    }
}
