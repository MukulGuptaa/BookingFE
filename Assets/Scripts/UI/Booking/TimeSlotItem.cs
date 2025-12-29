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
        [SerializeField] private Image statusBackground; // To change color based on status

        private string _time;
        private Action<string> _onClick;

        public void Setup(SlotData data, Action<string> onClick)
        {
            _time = data.time;
            _onClick = onClick;
            timeText.text = data.time;

            // Check Status from Backend
            // AVAILABLE, BOOKED_BY_ME, BOOKED_BY_OTHERS, PAYMENT_PENDING
            
            bool isAvailable = data.status == "AVAILABLE";
            button.interactable = isAvailable;

            if (data.status == "BOOKED_BY_ME")
            {
                statusBackground.color = Color.green; 
            }
            else if (data.status == "BOOKED_BY_OTHERS")
            {
                statusBackground.color = Color.red; // Example color
            }
            else if (data.status == "PAYMENT_PENDING")
            {
                statusBackground.color = Color.yellow; // Example color
            }
            else
            {
                statusBackground.color = new Color32(249, 250, 251, 255);
            }

            button.onClick.RemoveAllListeners();
            if (isAvailable)
            {
                button.onClick.AddListener(() => _onClick?.Invoke(_time));
            }
        }
    }
}
