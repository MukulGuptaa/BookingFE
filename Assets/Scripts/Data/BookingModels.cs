using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Booking.Data
{
    [Serializable]
    public class SlotData
    {
        public string time;     // "09:00"
        public string status;   // AVAILABLE, BOOKED_BY_ME, BOOKED_BY_OTHERS, PAYMENT_PENDING
        public string bookingId;
    }

    [Serializable]
    public class BookingRequest
    {
        public string date;     // "YYYY-MM-DD"
        public string time;     // "HH:mm"
        public string userId;
        public int duration;    // minutes
        // public int amount;      // optional
    }

    [Serializable]
    public class BookingResponse
    {
        public BookingData booking;
        public string paymentUrl;
    }

    [Serializable]
    public class BookingData
    {
        public string _id;
        public string date;
        public string time;
        public string status;
    }
    
    [Serializable]
    public class BookingStatusResponse
    {
        public string status;        // CONFIRMED, PENDING, CANCELLED
        public string paymentStatus; // SUCCESS, FAILED, PENDING
    }
}
