using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonTextHighlight : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Button button;
    private Text buttonText;

    private Color originalColor;
    private Color highlightedColor;

    private void Awake()
    {
        button = GetComponent<Button>();
        buttonText = GetComponentInChildren<Text>();

        // 버튼의 원래 색상과 Highlighted 색상 지정
        originalColor = button.colors.normalColor;
        highlightedColor = button.colors.highlightedColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 버튼 텍스트 알파값을 0.5로 변경
        buttonText.color = new Color(buttonText.color.r, buttonText.color.g, buttonText.color.b, 0.5f);

        // 버튼 Highlighted 상태로 변경
        ColorBlock colors = button.colors;
        colors.normalColor = highlightedColor;
        button.colors = colors;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 버튼 텍스트 알파값을 원래대로 변경
        buttonText.color = new Color(buttonText.color.r, buttonText.color.g, buttonText.color.b, 1f);

        // 버튼 Normal 상태로 변경
        ColorBlock colors = button.colors;
        colors.normalColor = originalColor;
        button.colors = colors;
    }
}