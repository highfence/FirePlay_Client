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
        // 데이터 컨테이너 클래스 초기화.
        DataContainer.GetInstance();

        // 네트워크 클래스 초기화.
        NetworkManager.GetInstance();

        SceneManager.LoadScene("Login");
	}
}
