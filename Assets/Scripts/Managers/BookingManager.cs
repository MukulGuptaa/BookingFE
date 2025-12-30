using System;
using System.Collections.Generic;
using Booking.Data;
using System.Threading.Tasks;
using UnityEngine;

public class BookingManager
{
    // Events for Observer Pattern
    public event Action<string> OnBookingConfirmed;
    public event Action<string> OnBookingFailed; // contains error or failure reason

    private static BookingManager _instance;
    public static BookingManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new BookingManager();
            }
            return _instance;
        }
    }

    /// <summary>
    /// Fetches available slots for a given date.
    /// Calls API: GET /api/bookings/slots?date=YYYY-MM-DD&userId=...
    /// </summary>
    public void GetSlots(DateTime date, Action<List<SlotData>> onSuccess, Action<string> onError)
    {
        string dateStr = date.ToString("yyyy-MM-dd");
        string userId = UserData.Instance.UserId;
        
        string queryParams = $"?date={dateStr}";
        if (!string.IsNullOrEmpty(userId))
        {
            queryParams += $"&userId={userId}";
        }

        string endpoint = $"/bookings/slots{queryParams}";

        APIManager.Instance.GetRequest<List<SlotData>>(endpoint, onSuccess, onError);
    }

    /// <summary>
    /// Books a specific slot.
    /// Calls API: POST /api/bookings
    /// </summary>
    public void BookSlot(DateTime date, string time, int duration, Action<BookingResponse> onSuccess, Action<string> onError)
    {
        BookingRequest request = new BookingRequest
        {
            date = date.ToString("yyyy-MM-dd"),
            time = time,
            userId = UserData.Instance.UserId,
            duration = duration,
            // amount = 100 // Default or dynamic amount
        };

        APIManager.Instance.PostRequest<BookingResponse>("/bookings", request, onSuccess, onError);
    }
    
    /// <summary>
    /// Checks the status of a specific booking.
    /// Calls API: GET /api/payments/check-status/{bookingId}
    /// </summary>
    public void CheckBookingStatus(string bookingId, Action<BookingStatusResponse> onSuccess, Action<string> onError)
    {
        string endpoint = $"/payments/check-status/{bookingId}";
        APIManager.Instance?.GetRequest<BookingStatusResponse>(endpoint, onSuccess, onError);
    }

    /// <summary>
    /// Starts polling the booking status.
    /// Fires OnBookingConfirmed or OnBookingFailed events.
    /// </summary>
    public async void StartPolling(string bookingId, float intervalSeconds = 3f, float timeoutSeconds = 300f)
    {
        float timer = 0f;
        bool stopPolling = false;

        while (!stopPolling && timer < timeoutSeconds)
        {
            await Task.Delay((int)(intervalSeconds * 1000));
            timer += intervalSeconds;

            bool requestComplete = false;
            
            CheckBookingStatus(bookingId,
                onSuccess: (response) => {
                    if (response.status == "CONFIRMED")
                    {
                        OnBookingConfirmed?.Invoke(bookingId);
                        stopPolling = true;
                    }
                    else if (response.status == "CANCELLED" || response.status == "FAILED")
                    {
                        OnBookingFailed?.Invoke("Booking Failed or Cancelled");
                        stopPolling = true;
                    }
                    requestComplete = true;
                },
                onError: (error) => {
                    Debug.LogWarning($"Polling Error: {error}");
                    // Optionally stop on error or continue retrying
                    requestComplete = true;
                }
            );

            // Wait for callback to complete
            while (!requestComplete)
            {
                await Task.Delay(100);
            }
        }

        if (!stopPolling && timer >= timeoutSeconds)
        {
            OnBookingFailed?.Invoke("Polling Timed Out");
        }
    }
}
