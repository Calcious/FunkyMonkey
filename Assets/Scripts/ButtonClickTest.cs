using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonClickTest : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"Button {gameObject.name} was CLICKED!");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse clicked somewhere!");
        }
    }
}
