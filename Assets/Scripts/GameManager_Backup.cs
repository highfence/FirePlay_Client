using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager_Backup : MonoBehaviour
{
    private Player _player1;
    private Player _player2;

    void Start()
    {
        //BackGroundCreate();
        PlatformCreate();
        PlayerCreate();
    }

    // TODO :: 전 씬에서 고른 PlayerType 기억하고 그에 맞추어 Player 생성하기
    private void PlayerCreate()
    {
        var player1Text = Resources.Load<TextAsset>("Data/Archer1").text;
        var player1Spec = PlayerSpec.CreateFromText(player1Text);

        _player1 = Player.Factory.Create(player1Spec);
        _player1.transform.position = CreatePosition();

        //var player2Text = Resources.Load<TextAsset>("Data/Archer1").text;
        //var player2Spec = PlayerSpec.CreateFromText(player2Text);

        //_player2 = Player.Factory.Create(player2Spec);
        //_player2.transform.position = new Vector3(1.0f, 0.0f, 0.0f);
    }

    private void PlatformCreate()
    {
        var platform = Resources.Load<GameObject>("Prefabs/Platform");
        Instantiate(platform);
    }

    private void BackGroundCreate()
    {
        var background = Resources.Load<GameObject>("Prefabs/Background");
        Instantiate(background);
    }

    private Vector3 CreatePosition()
    {
        return new Vector3(Random.Range(-2.0f, 2.0f), 0.0f, 0.0f);
    }
}
