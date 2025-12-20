using UnityEngine;

namespace Modules
{
    /// <summary>
    /// Base class for simple HUD containers providing a unified Show/Hide mechanism.
    /// Extend and override lifecycle hooks as needed.
    /// </summary>
    public class BaseHUD : MonoBehaviour
    {
        [SerializeField] private GameObject root;

        protected virtual void Awake()
        {

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

        /// <summary>
        /// Shows or hides the root GameObject for this HUD.
        /// </summary>
        public virtual void ShowHUD(bool isShow)
        {
            root.SetActive(isShow);
        }
    }
}
