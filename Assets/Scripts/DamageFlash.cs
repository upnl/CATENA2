using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class DamageFlash : MonoBehaviour
{
    [ColorUsage(true, true)]
    [SerializeField] private Color _flashColor = Color.white;
    [SerializeField] private float _flashTime = 0.2f;

    private SpriteRenderer _spriteRenderer;
    private Material _material;

    [SerializeField] private Material _defaultMAT;
    [SerializeField] private Material _flashMAT;

    private Coroutine _damageFlashCoroutine;

    private void Start()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _material = _spriteRenderer.material;
    }

    public void Flash()
    {
        if (_damageFlashCoroutine != null) StopCoroutine(_damageFlashCoroutine);
        _damageFlashCoroutine = StartCoroutine(DamageFlasher());
    }

    private IEnumerator DamageFlasher()
    {
        _spriteRenderer.material = _flashMAT;
        
        _spriteRenderer.material.SetColor("_FlashColor", _flashColor);

        float flashTimeElapsed = 0f;
        float currentFlashAmount = 0f;

        while (flashTimeElapsed < _flashTime)
        {
            flashTimeElapsed += Time.deltaTime;
            currentFlashAmount = Mathf.Lerp(1f, 0f, flashTimeElapsed / _flashTime);

            _spriteRenderer.material.SetFloat("_FlashAmount", currentFlashAmount);

            yield return null;
        }

        _spriteRenderer.material = _defaultMAT;
    }
    
}
