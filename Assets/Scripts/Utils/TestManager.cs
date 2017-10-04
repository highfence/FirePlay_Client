using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestManager : MonoBehaviour
{
    public Player _player;
    public Player _enemy;

    private void Start()
    {
        PlatformCreate();
        PlayerCreate();
    }

    private void PlayerCreate()
    {
        // 내 캐릭터 생성.
        var playerSpecText = Resources.Load<TextAsset>("Data/Archer1").text;
        var playerSpec = PlayerSpec.CreateFromText(playerSpecText);

        _player = Player.Factory.Create(playerSpec);
        _player._isMyTurn = true;

        // 적 캐릭터 생성.
        var enemySpecText = Resources.Load<TextAsset>("Data/Archer2").text;
        var enemySpec = PlayerSpec.CreateFromText(enemySpecText);

        _enemy = Player.Factory.Create(enemySpec);
        _enemy.SetEnemy();

        var playerPosition = Camera.main.ScreenToWorldPoint(new Vector3(50, 300, 0));
        playerPosition.z = 10;
        _player.transform.position = playerPosition;

        DataContainer.GetInstance().SetPlayerPosition(_player.transform.position);
        DataContainer.GetInstance().SetEnemyPosition(_enemy.transform.position);
    }

    private void PlatformCreate()
    {
        var platform = Resources.Load<GameObject>("Prefabs/Platform");
        Instantiate(platform);
    }
}
