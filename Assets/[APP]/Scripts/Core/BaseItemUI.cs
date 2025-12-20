using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseItemUI : MonoBehaviour
{
    [SerializeField] protected Toggle toggle;
    [SerializeField] protected TMP_Text textTitle;
    [SerializeField] protected Image imageIcon;
    [SerializeField] protected TMP_Text textPrice;

    protected virtual void Awake()
    {
        toggle.onValueChanged.AddListener(HandleOnToggleValueChanged);
    }

    protected virtual void OnDestroy()
    {
        toggle.onValueChanged.RemoveListener(HandleOnToggleValueChanged);
    }

    public virtual void SetToggleGroup(ToggleGroup group)
    {
        toggle.group = group;
    }

    protected virtual void HandleOnToggleValueChanged(bool isOn)
    {
    }
}
