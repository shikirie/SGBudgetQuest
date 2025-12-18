using UnityEngine;

public class BaseController : MonoBehaviour
{
    [SerializeField] protected GameObject root;

    protected virtual void Awake()
    {
        Hide();
    }

    protected virtual void Start()
    {
    }

    protected virtual void Update()
    {
    }

    protected virtual void OnDestroy()
    {
    }

    public virtual void Show()
    {
        root.SetActive(true);
    }

    public virtual void Hide()
    {
        root.SetActive(false);
    }
}
