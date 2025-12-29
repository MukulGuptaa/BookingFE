using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Booking.UI
{
    public class DayItem : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text dayText;
        [SerializeField] private Image selectionHighlight; // Optional visual for selected state

        private DateTime _date;
        private Action<DateTime> _onClick;

        public void Setup(DateTime date, bool isPast, bool isToday, Action<DateTime> onClick)
        {
            _date = date;
            _onClick = onClick;
            dayText.text = date.Day.ToString();

            button.interactable = !isPast;
            
            // Optional: Highlight today
            if(isToday) 
            {
                dayText.color = new Color32(255, 255, 255, 255);
                selectionHighlight.color = new Color32(31, 42, 57, 255);
                // Maybe set a specific color for 'Today'
            }
            else
            {
                dayText.color = new Color32(108, 114, 127, 255);
                selectionHighlight.color = new Color32(255, 255, 255, 0);
            }

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => _onClick?.Invoke(_date));
        }
    }
}
