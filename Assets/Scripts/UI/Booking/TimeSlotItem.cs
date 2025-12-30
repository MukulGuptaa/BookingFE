using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Booking.Data;

namespace Booking.UI
{
    public class TimeSlotItem : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text timeText;
        [SerializeField] private Image statusBackground; 

        private string _time;
        private Action<TimeSlotItem> _onClick;
        private Color _defaultBackgroundColor;
        private Color _defaultTextColor;

        public string Time => _time;

        public void Setup(SlotData data, Action<TimeSlotItem> onClick)
        {
            _time = data.time;
            _onClick = onClick;
            timeText.text = data.time;
            _defaultTextColor = timeText.color;

            // Status Logic
            bool isAvailable = data.status == "AVAILABLE";
            button.interactable = isAvailable;

            if (data.status == "BOOKED_BY_ME")
            {
                _defaultBackgroundColor = Color.green;
            }
            else if (data.status == "BOOKED_BY_OTHERS")
            {
                _defaultBackgroundColor = Color.red; 
            }
            else if (data.status == "PAYMENT_PENDING")
            {
                _defaultBackgroundColor = Color.yellow; 
            }
            else
            {
                _defaultBackgroundColor = new Color32(249, 250, 251, 255); // Default Gray/White
            }

            statusBackground.color = _defaultBackgroundColor;

            button.onClick.RemoveAllListeners();
            if (isAvailable)
            {
                button.onClick.AddListener(() => _onClick?.Invoke(this));
            }
        }

        public void SetSelected(bool isSelected)
        {
            if (isSelected)
            {
                statusBackground.color = new Color32(31, 41, 55, 255); // Dark Blue/Black
                timeText.color = Color.white;
            }
            else
            {
                statusBackground.color = _defaultBackgroundColor;
                timeText.color = _defaultTextColor;
            }
        }
    }
}
