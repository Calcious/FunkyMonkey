using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class EmblemButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Emblem Info")]
    public string emblemName;
    public string levelName;

    [Header("References")]
    public TextMeshProUGUI statsText;

    [Header("Hover Effect")]
    public float hoverScale = 1.1f;
    public float transitionSpeed = 10f;

    private Vector3 originalScale;
    private Image buttonImage;

    private void Awake()
    {
        originalScale = transform.localScale;
        buttonImage = GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = originalScale * hoverScale;

        if (statsText != null)
        {
            if (emblemName == "Close")
            {
                statsText.text = "CLOSE MENU";
            }
            else
            {
                bool isCompleted = LevelCompletionManager.IsLevelCompleted(levelName);
                statsText.text = $"{emblemName}\n{(isCompleted ? "COMPLETED" : "NOT COMPLETED")}";
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = originalScale;

        if (statsText != null)
        {
            statsText.text = "STATS TBD";
        }
    }

    public void OnEmblemClicked()
    {
        Debug.Log($"{emblemName} clicked! Level: {levelName}");
    }
}
