using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.CurrencyBarUISystem
{
    public class CurrencyBarUI : MonoBehaviour
    {
        [Header("Component")]
        [SerializeField] private Image currencyImage;
        [SerializeField] private Sprite currencySprite;
        [SerializeField] private TMP_Text valueText;
        [SerializeField] private TMP_SpriteAsset valueChangeSpriteAsset;
        [SerializeField] private Transform valueChangeTransform;
        [SerializeField] private TMP_Text valueChangeTextPrefab;

        [Header("Config")]
        [SerializeField] private int poolSize = 5;
        [SerializeField] private float popupSpacing = 0.03f;
        [SerializeField] private bool showValueAnimationChange;
        [SerializeField] private bool showColorChange;
        [SerializeField] private Color validColor;
        [SerializeField] private Color invalidColor;
        [SerializeField] private Color defaultColor;
        [SerializeField] private float animationDuration = 0.5f;
        [SerializeField] private bool useUnscaledTime = true;
        [SerializeField] private string suffix = "";

        private Queue<float> changeQueue;
        private float currentValue;
        private Coroutine valueAnimationCoroutine;

        private readonly Queue<TMP_Text> popupPool = new Queue<TMP_Text>();
        private readonly List<TMP_Text> activePopups = new List<TMP_Text>();
        private bool isPlaying = false;

        private void Awake()
        {
            changeQueue = new Queue<float>();
            
            if (valueChangeTextPrefab == null || valueChangeTransform == null)
            {
                Debug.LogWarning($"CurrencyBarUI on {gameObject.name}: Missing prefab or transform references", this);
                return;
            }
            
            for (int i = 0; i < poolSize; i++)
            {
                TMP_Text popup = Instantiate(valueChangeTextPrefab, valueChangeTransform);
                popup.spriteAsset = valueChangeSpriteAsset;
                popup.color = defaultColor;
                popup.gameObject.SetActive(false);
                popupPool.Enqueue(popup);
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (currencySprite != null && currencyImage != null)
            {
                currencyImage.sprite = currencySprite;
            }
        }
#endif

        public void SetValue(float value)
        {
            if (valueText == null) return;
            
            if (valueAnimationCoroutine != null)
                StopCoroutine(valueAnimationCoroutine);

            if (showValueAnimationChange)
            {
                valueAnimationCoroutine = StartCoroutine(AnimateValue(currentValue, value));
            }
            else
            {
                if (string.IsNullOrEmpty(suffix))
                {
                    valueText.text = $"{(int)value}";
                }
                else
                {
                    valueText.text = value > 0 ? $"{value:##,###}{suffix}" : $"0{suffix}";
                }

                if (showColorChange && value != currentValue)
                    StartCoroutine(FlashColor(value >= currentValue));
            }

            currentValue = value;

            if (currentValue <= 0)
            {
                currentValue = 0;
            }
        }

        public void SetValueImmediate(float value)
        {
            if (valueText == null) return;
            
            currentValue = value;

            if (currentValue <= 0)
            {
                currentValue = 0;
            }

            if (string.IsNullOrEmpty(suffix))
            {
                valueText.text = $"{(int)currentValue}";
            }
            else
            {
                valueText.text = currentValue > 0 ? $"{currentValue:##,###}{suffix}" : $"0{suffix}";
            }
        }

        public float GetValue()
        {
            return currentValue;
        }

        public void ShowChange(float delta)
        {
            if (delta == 0) return;

            if (currentValue <= 0 && delta < 0)
                return;

            if (currentValue + delta <= 0 && delta < 0)
                return;

            changeQueue.Enqueue(delta);
            if (!isPlaying)
                StartCoroutine(ProcessQueue());
        }

        private IEnumerator FlashColor(bool isPositive)
        {
            if (valueText == null) yield break;
            
            valueText.color = isPositive ? validColor : invalidColor;

            if (useUnscaledTime)
                yield return new WaitForSecondsRealtime(popupSpacing);
            else
                yield return new WaitForSeconds(popupSpacing);

            valueText.color = defaultColor;
        }

        private IEnumerator AnimateValue(float fromValue, float toValue)
        {
            if (valueText == null) yield break;

            bool isPositive = toValue >= fromValue;

            if (showColorChange && toValue != fromValue)
                valueText.color = isPositive ? validColor : invalidColor;

            float elapsed = 0f;
            while (elapsed < animationDuration)
            {
                elapsed += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / animationDuration);
                float easedT = 1f - Mathf.Pow(1f - t, 2f);
                float current = Mathf.Lerp(fromValue, toValue, easedT);

                int currentInt = Mathf.FloorToInt(current);
                valueText.text = currentInt > 0 ? $"{currentInt:##,###}{suffix}" : $"0{suffix}";

                yield return null;
            }

            int finalValue = Mathf.FloorToInt(toValue);
            valueText.text = finalValue > 0 ? $"{finalValue:##,###}{suffix}" : $"0{suffix}";

            if (showColorChange && toValue != fromValue)
                valueText.color = defaultColor;
        }

        private IEnumerator ProcessQueue()
        {
            isPlaying = true;
            while (changeQueue.Count > 0)
            {
                float delta = changeQueue.Dequeue();
                StartCoroutine(PlayPopup(delta));

                if (useUnscaledTime)
                    yield return new WaitForSecondsRealtime(popupSpacing);
                else
                    yield return new WaitForSeconds(popupSpacing);
            }
            isPlaying = false;
        }

        private IEnumerator PlayPopup(float delta)
        {
            TMP_Text popup = GetPopupFromPool();

            if (popup == null)
            {
                Debug.LogWarning("Popup pool exhausted! Consider increasing pool size.");
                yield break;
            }

            if (valueChangeSpriteAsset != null && (popup.spriteAsset == null || popup.spriteAsset.name != valueChangeSpriteAsset.name))
            {
                popup.spriteAsset = valueChangeSpriteAsset;
            }

            bool isPositive = delta > 0;
            popup.text = $"{(isPositive ? "+" : "-")}<sprite index=0>{Mathf.Abs(delta):N1}";

            if (showColorChange)
                popup.color = isPositive ? validColor : invalidColor;

            popup.gameObject.SetActive(true);

            Animator anim = popup.GetComponent<Animator>();
            float duration = anim != null ? anim.GetCurrentAnimatorStateInfo(0).length : 1f;

            yield return new WaitForSecondsRealtime(duration);

            if (showColorChange)
                popup.color = defaultColor;

            ReturnToPool(popup);
        }

        private TMP_Text GetPopupFromPool()
        {
            if (popupPool.Count > 0)
            {
                TMP_Text popup = popupPool.Dequeue();
                activePopups.Add(popup);
                return popup;
            }

            return null;
        }

        private void ReturnToPool(TMP_Text popup)
        {
            popup.gameObject.SetActive(false);

            if (!popupPool.Contains(popup))
                popupPool.Enqueue(popup);

            activePopups.Remove(popup);
        }

        public void CustomSuffix(string suffix)
        {
            this.suffix = suffix;
        }
    }
}
