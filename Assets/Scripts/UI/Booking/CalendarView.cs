using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Booking.UI
{
    public class CalendarView : MonoBehaviour
    {
        public event Action<DateTime> OnDateSelected;

        [Header("UI References")]
        [SerializeField] private Transform daysContainer; // Grid Layout Group
        [SerializeField] private DayItem dayItemPrefab;
        [SerializeField] private GameObject emptyDayItem;
        [SerializeField] private TMP_Text monthYearText;
        [SerializeField] private Button nextMonthBtn;
        [SerializeField] private Button prevMonthBtn;

        private int _currentYear;
        private int _currentMonth;
        private List<GameObject> _instantiatedDays;

        private void Awake()
        {
            nextMonthBtn.onClick.AddListener(NextMonth);
            prevMonthBtn.onClick.AddListener(PreviousMonth);
        }

        public void ShowMonth(int year, int month)
        {
            _currentYear = year;
            _currentMonth = month;
            monthYearText.text = new DateTime(year, month, 1).ToString("MMMM yyyy");

            GenerateDays();
        }

        private void EmptyInstantiatedDays()
        {
            if (_instantiatedDays == null) return;
            foreach (var days in _instantiatedDays)
                Destroy(days);
        }

        private void GenerateDays()
        {
            // Clear existing items
            EmptyInstantiatedDays();

            _instantiatedDays = new List<GameObject>();

            DateTime firstDayOfMonth = new DateTime(_currentYear, _currentMonth, 1);
            int daysInMonth = DateTime.DaysInMonth(_currentYear, _currentMonth);
            
            // DayOfWeek returns 0 for Sunday. If your calendar starts on Sunday, this is fine.
            int startDayOffset = (int)firstDayOfMonth.DayOfWeek; 

            // 1. Create empty spacers for days before the 1st of the month
            for (int i = 0; i < startDayOffset; i++)
            {
                var spacer = Instantiate(emptyDayItem, daysContainer);
                spacer.gameObject.SetActive(true); // Invisible but takes space in Grid
                _instantiatedDays.Add(spacer.gameObject);
                // Alternatively, use a separate empty prefab or CanvasGroup alpha=0
            }

            // 2. Create actual day buttons
            DateTime today = DateTime.Today;

            for (int day = 1; day <= daysInMonth; day++)
            {
                DateTime date = new DateTime(_currentYear, _currentMonth, day);
                DayItem item = Instantiate(dayItemPrefab, daysContainer);
                _instantiatedDays.Add(item.gameObject);
                
                // Logic: Disable if date is strictly before today
                bool isPast = date < today;
                bool isToday = date == today;

                item.Setup(date, isPast, isToday, OnDayClicked);
            }
        }

        private void OnDayClicked(DateTime date)
        {
            OnDateSelected?.Invoke(date);
        }

        private void NextMonth()
        {
            DateTime target = new DateTime(_currentYear, _currentMonth, 1).AddMonths(1);
            ShowMonth(target.Year, target.Month);
        }

        private void PreviousMonth()
        {
            DateTime target = new DateTime(_currentYear, _currentMonth, 1).AddMonths(-1);
            
            // Optional: Don't allow navigating to months before the current month
            if (target.Year < DateTime.Now.Year || (target.Year == DateTime.Now.Year && target.Month < DateTime.Now.Month))
            {
                return;
            }
            
            ShowMonth(target.Year, target.Month);
        }
    }
}
