using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SpriteColorChanger : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Color[] color;

    public Image ImageComponent => image;

    private void Awake()
    {
        if (image == null)
            image = GetComponent<Image>();
    }

    public void ChangeColor(int index)
    {
        if (image == null)
            image = GetComponent<Image>();

        if (index < 0 || index >= color.Length)
            return;

        if (image != null)
        {
            image.color = color[index];
        }
    }
}
