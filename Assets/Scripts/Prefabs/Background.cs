using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
	private Sprite _backgroundSprite = null;
	private SpriteRenderer _renderer = null;

	public void Init()
	{
		_backgroundSprite = Resources.Load("GUI/FirePlayBackground", typeof(Sprite)) as Sprite;
		_renderer = this.GetComponent<SpriteRenderer>();

		_renderer.sprite = _backgroundSprite;

		Vector2 spriteSize = _backgroundSprite.rect.size;
		Vector3 scale = new Vector3(Screen.width * 2f / spriteSize.x, Screen.height * 2f / spriteSize.y, 1f);

		transform.localScale = scale;
		//transform.position -= new Vector3(0, 1.0f, 0);
	}

}