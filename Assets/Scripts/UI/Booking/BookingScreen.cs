using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Booking.UI
{
    public class BookingScreen : MonoBehaviour
    {
        [Header("Views")]
        [SerializeField] private CalendarView calendarView;
        [SerializeField] private TimeSlotsView timeSlotsView;
        
        [Header("UI Elements")]
        [SerializeField] private GameObject loadingIndicator;
        [SerializeField] private Button backButton;

        private DateTime _selectedDate;


        private void Start()
        {
            // Initial State
            backButton.onClick.AddListener(OnBackButtonClicked);
            
            calendarView.OnDateSelected += HandleDateSelected;
            timeSlotsView.OnSlotSelected += HandleSlotSelected;

            ShowCalendar();
            HandleDateSelected(DateTime.Now);
        }

        private void ShowCalendar()
        {
            
            calendarView.gameObject.SetActive(true);
            
            // Show current month
            calendarView.ShowMonth(DateTime.Now.Year, DateTime.Now.Month);
        }

        private void HandleDateSelected(DateTime date)
        {
            _selectedDate = date;
            timeSlotsView.gameObject.SetActive(true);
            
            FetchSlots();
        }

        private void FetchSlots()
        {
            if(loadingIndicator) loadingIndicator.SetActive(true);

            BookingManager.Instance.GetSlots(_selectedDate, 
                onSuccess: (slots) => {
                    if(loadingIndicator) loadingIndicator.SetActive(false);
                    timeSlotsView.DisplaySlots(slots);
                },
                onError: (error) => {
                    if(loadingIndicator) loadingIndicator.SetActive(false);
                    Debug.LogError($"Error fetching slots: {error}");
                    // TODO: Show UI Popup for error
                }
            );
        }

        private void HandleSlotSelected(string time)
        {
            // Proceed to Booking Logic
            Debug.Log($"Booking Slot: {_selectedDate:yyyy-MM-dd} at {time}");
            
            if(loadingIndicator) loadingIndicator.SetActive(true);

            // Default duration 60 mins for now
            BookingManager.Instance.BookSlot(_selectedDate, time, 60,
                onSuccess: (response) => {
                    if(loadingIndicator) loadingIndicator.SetActive(false);
                    Debug.Log($"Booking Created! Opening Payment: {response.paymentUrl}");
                    Application.OpenURL(response.paymentUrl);
                    // TODO: Show success message or refresh
                },
                onError: (error) => {
                    if(loadingIndicator) loadingIndicator.SetActive(false);
                    Debug.LogError($"Booking Failed: {error}");
                    // TODO: Show UI Popup for error
                }
            );
        }

        private void OnBackButtonClicked()
        {
            // If we are in TimeSlots view, go back to Calendar
            
        }
    }
}
