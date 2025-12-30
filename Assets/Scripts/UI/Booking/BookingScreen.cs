using System;
using System.Collections;
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
        [SerializeField] private Button confirmButton; 

        private DateTime _selectedDate;
        private string _selectedTime;

        private void Start()
        {
            backButton.onClick.AddListener(OnBackButtonClicked);
            confirmButton.onClick.AddListener(OnConfirmClicked);
            SetConfirmButtonState(false);

            calendarView.OnDateSelected += HandleDateSelected;
            timeSlotsView.OnSlotSelected += HandleSlotSelected;
            
            // Subscribe to Events
            BookingManager.Instance.OnBookingConfirmed += HandleBookingConfirmed;
            BookingManager.Instance.OnBookingFailed += HandleBookingFailed;

            ShowCalendar();
            HandleDateSelected(DateTime.Now); 
        }

        private void OnDestroy()
        {
            // Unsubscribe to avoid memory leaks
            if (BookingManager.Instance != null)
            {
                BookingManager.Instance.OnBookingConfirmed -= HandleBookingConfirmed;
                BookingManager.Instance.OnBookingFailed -= HandleBookingFailed;
            }
        }

        private void HandleBookingConfirmed(string bookingId)
        {
            Debug.Log($"Booking Confirmed Event Received! ID: {bookingId}");
            if(loadingIndicator) loadingIndicator.SetActive(false);
            
            FetchSlots(); // Refresh UI to show 'Booked by Me'
            
            // Optional: Show success popup here
        }

        private void HandleBookingFailed(string error)
        {
            Debug.LogError($"Booking Failed Event Received: {error}");
            if(loadingIndicator) loadingIndicator.SetActive(false);
            
            SetConfirmButtonState(true); // Allow retry
            
            // Optional: Show error popup
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
            
            calendarView.ShowMonth(DateTime.Now.Year, DateTime.Now.Month);
            
            SetConfirmButtonState(false);
            _selectedTime = null;
        }

        private void HandleDateSelected(DateTime date)
        {
            _selectedDate = date;
            timeSlotsView.gameObject.SetActive(true);
            
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

            BookingManager.Instance.BookSlot(_selectedDate, _selectedTime, 60,
                onSuccess: (response) => {
                    Debug.Log($"Booking Created! Opening Payment: {response.paymentUrl}");
                    Application.OpenURL(response.paymentUrl);
                    
                    // Start Polling (Async void that fires events)
                    BookingManager.Instance.StartPolling(response.booking._id);
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
        }
    }
}
