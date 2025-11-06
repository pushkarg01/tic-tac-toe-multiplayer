#region

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#endregion

    [AddComponentMenu("")]
    [Serializable]
    public class CustomButton : Button
    {
        public enum TransitionEffectType
        {
            None,
            Scale,
            ScaleSpriteChange,
            ColorTransition
        }

        [Header("Transition Settings")]
        public TransitionEffectType TransitionType = TransitionEffectType.None;
        public float ScaleAmount = 0.9f;
        public float ScaleDuration = 0.1f;
        private Sprite _normalSprite;

        private Vector3 _originalScale;

        protected override void Awake()
        {
            base.Awake();
            _originalScale = transform.localScale;
            UpdateNormalSprite();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (!interactable && TransitionType == TransitionEffectType.ColorTransition)
            {
                ApplyColorTransition(colors.disabledColor);
            }
            HandleTransition(PointerState.Exit);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (interactable && TransitionType == TransitionEffectType.ColorTransition)
            {
                ApplyColorTransition(colors.normalColor);
            }
        }
#if UNITY_EDITOR

        protected override void OnValidate()
        {
            base.OnValidate();
            UpdateNormalSprite();
            UpdatedInteractableState();
        }
#endif

        /// <summary>
        ///     Change the interactable state of the button
        /// </summary>
        /// <param name="state">true: Interactable; false: Non Interactable/Diabled</param>
        public void SetInteractable(bool state = true)
        {
            interactable = state;
            UpdatedInteractableState();
        }
        private void UpdateNormalSprite()
        {
            if (targetGraphic is Image buttonImage)
            {
                _normalSprite = buttonImage.sprite;
            }
        }

        private void ApplyColorTransition(Color color, bool instant = false)
        {
            foreach (var graphic in GetComponentsInChildren<Graphic>())
            {
                graphic.CrossFadeColor(color, instant ? 0f : colors.fadeDuration, true, true);
            }
        }

        private void HandleTransition(PointerState state, Action onClick = null)
        {
            if (!interactable) return;

            StopAllCoroutines();
            switch (TransitionType)
            {
                case TransitionEffectType.Scale:
                case TransitionEffectType.ScaleSpriteChange:
                    var targetScale = state == PointerState.Down ? _originalScale * ScaleAmount : _originalScale;
                    StartCoroutine(Scale(transform, targetScale, ScaleDuration, onClick));

                    if (TransitionType == TransitionEffectType.ScaleSpriteChange && targetGraphic is Image targetImage)
                    {
                        // Only apply sprite transitions if the corresponding sprite is assigned
                        targetImage.overrideSprite = state switch
                        {
                            PointerState.Enter when spriteState.highlightedSprite != null => spriteState.highlightedSprite,
                            PointerState.Down when spriteState.pressedSprite != null => spriteState.pressedSprite,
                            PointerState.Up when _normalSprite != null => _normalSprite,
                            PointerState.Exit when _normalSprite != null => _normalSprite,
                            _ => targetImage.overrideSprite // Keep the current sprite if no transition occurs
                        };
                    }
                    break;

                case TransitionEffectType.ColorTransition:
                    var targetColor = state switch
                    {
                        PointerState.Enter => colors.highlightedColor,
                        PointerState.Exit => colors.normalColor,
                        PointerState.Down => colors.pressedColor,
                        _ => colors.normalColor
                    };
                    ApplyColorTransition(targetColor);
                    onClick?.Invoke();
                    break;
            }
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            HandleTransition(PointerState.Enter);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            HandleTransition(PointerState.Exit);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            //base.OnPointerDown(eventData);
            HandleTransition(PointerState.Down, () => base.OnPointerDown(eventData));
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            //base.OnPointerUp(eventData);
            HandleTransition(PointerState.Up, () => base.OnPointerUp(eventData));
        }

        private void UpdatedInteractableState()
        {
            switch (TransitionType)
            {
                case TransitionEffectType.ScaleSpriteChange when targetGraphic is Image targetImage:
                    {
                        targetImage.overrideSprite = interactable ? _normalSprite : spriteState.disabledSprite == null ? _normalSprite : spriteState.disabledSprite;
                        break;
                    }
                case TransitionEffectType.ColorTransition:
                    ApplyColorTransition(interactable ? colors.normalColor : colors.disabledColor);
                    break;
                case TransitionEffectType.None:
                case TransitionEffectType.Scale:
                default:
                    break;
            }
        }

        private IEnumerator Scale(Transform transform, Vector3 finalScale, float duration, Action onClick = null)
        {
            var originalScale = transform.localScale;
            float elapsedTime = 0f;
            while (elapsedTime <= duration)
            {
                float t = elapsedTime / duration;
                t = EaseOutBack(t);
                transform.localScale = Vector3.LerpUnclamped(originalScale, finalScale, t);
                yield return null;
                elapsedTime += Time.unscaledDeltaTime;
            }
            transform.localScale = finalScale;
            yield return null;
            onClick?.Invoke();
        }

        private float EaseOutBack(float t, float overshoot = 3f)
        {
            t -= 1f;
            return t * t * ((overshoot + 1f) * t + overshoot) + 1f;
        }

        private enum PointerState
        {
            Enter,
            Exit,
            Down,
            Up
        }
    }