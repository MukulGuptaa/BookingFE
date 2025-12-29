using System;
using System.Collections.Generic;
using UnityEngine;
using Booking.Data;

public class BookingManager
{
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
        
        // Ensure userId is present if logged in, otherwise it might be null/empty string
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
            amount = 100 // Default or dynamic amount
        };

        APIManager.Instance.PostRequest<BookingResponse>("/bookings", request, onSuccess, onError);
    }
}
