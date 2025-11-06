using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public interface IMessageBox
{
    void UpdateMessage(string msg);
}

public class MessageBoxHandler : MonoBehaviour, IMessageBox 
    {
        [SerializeField] TextMeshProUGUI _message;
        WaitForSeconds closeTime = new WaitForSeconds(2f);
        Coroutine closePopUp;
        private void OnEnable()
        {
            transform.DOScale(1f, 0.2f)
                 .SetEase(Ease.InQuint);
        }
        public void UpdateMessage(string msg)
        {
            _message.text = msg;
            gameObject.SetActive(true);
            if (closePopUp != null) StopCoroutine(closePopUp);
            closePopUp = StartCoroutine(ClosePopup());
        }
        public IEnumerator ClosePopup()
        {
            yield return closeTime;
            AnimateOnClose();
        }
        private void AnimateOnClose()
        {
            transform.DOScale(0f, 0.2f)
                .SetEase(Ease.InQuint)
                .OnComplete(DisableObject);
        }
        private void DisableObject()
        {
            gameObject.SetActive(false);
        }
    }

