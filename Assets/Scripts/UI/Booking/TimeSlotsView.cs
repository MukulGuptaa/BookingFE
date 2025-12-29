using System;
using System.Collections.Generic;
using UnityEngine;
using Booking.Data;

namespace Booking.UI
{
    public class TimeSlotsView : MonoBehaviour
    {
        public event Action<string> OnSlotSelected;

        [SerializeField] private Transform slotsContainer;
        [SerializeField] private TimeSlotItem timeSlotItemPrefab;
        [SerializeField] private GameObject noSlotsMessage; // Optional "No slots available" text

        public void DisplaySlots(List<SlotData> slots)
        {
            foreach (Transform child in slotsContainer)
                Destroy(child.gameObject);

            if (slots == null || slots.Count == 0)
            {
                if(noSlotsMessage) noSlotsMessage.SetActive(true);
                return;
            }
            
            if(noSlotsMessage) noSlotsMessage.SetActive(false);

            foreach (var slot in slots)
            {
                TimeSlotItem item = Instantiate(timeSlotItemPrefab, slotsContainer);
                item.Setup(slot, OnSlotClicked);
            }
        }

        private void OnSlotClicked(string time)
        {
            OnSlotSelected?.Invoke(time);
        }
    }
}
