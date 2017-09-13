using UnityEngine;
using UnityEngine.UI;

/*
 * 로그인 씬 GUI 관리자 클래스.
 */
public partial class LoginSceneManager : MonoBehaviour
{
    public InputField _idField;
    public InputField _pwField;
    public string     _id;
    public string     _pw;
    public string     _reqUserId;

    private void Start()
    {
        #region MAKE BACKGROUND

        var bgPrefab = Resources.Load("Prefabs/Background") as GameObject;
        var bgInstance = Instantiate(bgPrefab).GetComponent<Background>();

        #endregion

        #region INITIALIZE INPUT FIELDS

        _idField.text = "ID";
        _pwField.text = "Password";

        var idEvent = new InputField.SubmitEvent();
        idEvent.AddListener(GetId);
        _idField.onEndEdit = idEvent;

        var pwEvent = new InputField.SubmitEvent();
        pwEvent.AddListener(GetPw);
        _pwField.onEndEdit = pwEvent;

        #endregion
    }

    private void OnGUI()
    {
        ButtonCheck();
    }

    // 버튼이 눌렸는지 체크해주는 메소드.
    private void ButtonCheck()
    {
        if (GUI.Button(new Rect((Screen.width / 2) - 155, Screen.height * 2 / 3, 150, 50), "Login"))
        {
            TryLogin(_id, _pw);
        }

        // TODO :: 이걸 회원가입 버튼으로 바꿔야 될 것 같은데?
        if (GUI.Button(new Rect((Screen.width / 2) + 5, Screen.height * 2 / 3, 150, 50), "Exit"))
        {
            Application.Quit();
        }
    }

    // Input Field에 적힌 string을 가져오는 콜백 메소드.
    #region INPUT FIELD CALL BACK
    private void GetId(string arg0)
    {
        _id = arg0;
    }

    private void GetPw(string arg0)
    {
        _pw = arg0;
    }
    #endregion

}
