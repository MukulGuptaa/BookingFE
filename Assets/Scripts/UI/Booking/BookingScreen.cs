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
        [SerializeField] private Button confirmButton; // New Confirm Button

        private DateTime _selectedDate;
        private string _selectedTime;

        private void Start()
        {
            // Initial State
            backButton.onClick.AddListener(OnBackButtonClicked);
            
            // Setup Confirm Button
            confirmButton.onClick.AddListener(OnConfirmClicked);
            SetConfirmButtonState(false);// Initially disabled

            calendarView.OnDateSelected += HandleDateSelected;
            timeSlotsView.OnSlotSelected += HandleSlotSelected;

            ShowCalendar();
            // Removed generic HandleDateSelected(DateTime.Now) call to ensure clean start state
            // If you want to start on today's slots immediately, uncomment next line:
            HandleDateSelected(DateTime.Now); 
        }

        public void SetConfirmButtonState(bool enabled)
        {
            if (enabled)
            {
                confirmButton.image.color = new Color32(31, 42, 57, 255);
                confirmButton.interactable = true;
            }
            else
            {
                confirmButton.image.color = new Color32(229, 231, 234, 255);
                confirmButton.interactable = false;
            }
        }

        private void ShowCalendar()
        {
            calendarView.gameObject.SetActive(true);
            
            // Show current month
            calendarView.ShowMonth(DateTime.Now.Year, DateTime.Now.Month);
            
            // Reset selection state
            SetConfirmButtonState(false);
            _selectedTime = null;
        }

        private void HandleDateSelected(DateTime date)
        {
            _selectedDate = date;
            timeSlotsView.gameObject.SetActive(true);
            
            // Reset selection for new date
            SetConfirmButtonState(false);
            _selectedTime = null;

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
                }
            );
        }

        private void HandleSlotSelected(string time)
        {
            _selectedTime = time;
            
            SetConfirmButtonState(true);
        }

        private void OnConfirmClicked()
        {
            if (string.IsNullOrEmpty(_selectedTime)) return;

            Debug.Log($"Confirming Booking: {_selectedDate:yyyy-MM-dd} at {_selectedTime}");
            
            if(loadingIndicator) loadingIndicator.SetActive(true);
            SetConfirmButtonState(false);

            // Default duration 60 mins for now
            BookingManager.Instance.BookSlot(_selectedDate, _selectedTime, 60,
                onSuccess: (response) => {
                    if(loadingIndicator) loadingIndicator.SetActive(false);
                    Debug.Log($"Booking Created! Opening Payment: {response.paymentUrl}");
                    Application.OpenURL(response.paymentUrl);
                    // Reset or show success UI
                },
                onError: (error) => {
                    if(loadingIndicator) loadingIndicator.SetActive(false);
                    SetConfirmButtonState(true);
                    Debug.LogError($"Booking Failed: {error}");
                }
            );
        }

        private void OnBackButtonClicked()
        {
            // If we are in TimeSlots view, go back to Calendar
        }
    }
}
