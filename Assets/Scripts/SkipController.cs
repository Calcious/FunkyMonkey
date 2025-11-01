using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using System;

public class SkipController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject skipUI;
    public TextMeshProUGUI skipText;
    public Image skipBarFill;

    [Header("Settings")]
    public float holdDurationToSkip = 1.5f;
    public Color barColor = Color.white;

    private float holdProgress = 0f;
    private bool isHolding = false;
    private bool hasSkipped = false;

    public event Action OnSkipComplete;

    private void Start()
    {
        if (skipUI != null)
        {
            skipUI.SetActive(true);
        }

        if (skipBarFill != null)
        {
            skipBarFill.fillAmount = 0f;
            skipBarFill.color = barColor;
        }

        if (skipText != null)
        {
            skipText.text = "Hold to Skip";
        }
    }

    private void Update()
    {
        if (hasSkipped) return;

        bool isMouseDown = Mouse.current != null && Mouse.current.leftButton.isPressed;

        if (isMouseDown)
        {
            isHolding = true;
            holdProgress += Time.deltaTime;

            if (skipBarFill != null)
            {
                skipBarFill.fillAmount = holdProgress / holdDurationToSkip;
            }

            if (holdProgress >= holdDurationToSkip)
            {
                Skip();
            }
        }
        else
        {
            if (isHolding)
            {
                holdProgress -= Time.deltaTime * 2f;
                holdProgress = Mathf.Max(0f, holdProgress);

                if (skipBarFill != null)
                {
                    skipBarFill.fillAmount = holdProgress / holdDurationToSkip;
                }

                if (holdProgress <= 0f)
                {
                    isHolding = false;
                }
            }
        }
    }

    private void Skip()
    {
        hasSkipped = true;
        OnSkipComplete?.Invoke();

        if (skipUI != null)
        {
            skipUI.SetActive(false);
        }
    }

    public void DisableSkip()
    {
        hasSkipped = true;

        if (skipUI != null)
        {
            skipUI.SetActive(false);
        }
    }
}
