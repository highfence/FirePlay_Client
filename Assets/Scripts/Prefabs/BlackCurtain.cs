using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackCurtain : MonoBehaviour
{
    public SpriteRenderer _spriteRenderer;

    public void Init()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.color = new Color(0, 0, 0, 0);

        Vector2 spriteSize = _spriteRenderer.sprite.rect.size;
        Vector3 scale = new Vector3(Screen.width * 5f / spriteSize.x, Screen.height * 5f / spriteSize.y, 1f);

        transform.localScale = scale;
    }

    public void StartFadeIn()
    {
        StartCoroutine("FadeIn");
    }

    IEnumerator FadeOut()
    {
        for (var i = 1f; i >= 0f; i -= 0.1f)
        {
            Color color = new Color(1, 1, 1, i);
            _spriteRenderer.color = color;
            yield return 0;
        }
    }

    public void StartFadeOut()
    {
        StartCoroutine("FadeOut");
    }

    IEnumerator FadeIn()
    {
        for (var i = 0f; i <= 1f; i += 0.1f)
        {
            Color color = new Color(1, 1, 1, i);
            _spriteRenderer.color = color;
            yield return 0;
        }
    }
}
