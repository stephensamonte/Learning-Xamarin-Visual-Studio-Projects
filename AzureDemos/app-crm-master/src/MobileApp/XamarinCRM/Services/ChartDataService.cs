﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using XamarinCRM.Services;
using XamarinCRM.Models;
using XamarinCRM.Models.Local;
using System.Globalization;

[assembly: Dependency(typeof(ChartDataService))]

namespace XamarinCRM.Services
{
    public class ChartDataService : IChartDataService
    {
        IDataService _DataClient;

        const int defaultNumberOfWeeks = 6;

        public ChartDataService()
        {
            _DataClient = DependencyService.Get<IDataService>();
        }

        #region IChartDataService implementation

        public async Task<IEnumerable<WeeklySalesDataPoint>> GetWeeklySalesDataPointsAsync(IEnumerable<Order> orders, int numberOfWeeks = defaultNumberOfWeeks, OrderStatusOption statusOption = OrderStatusOption.Both)
        {
            var now = DateTime.UtcNow;

            var today = DateTime.SpecifyKind(new DateTime(now.Year, now.Month, now.Day, 0, 0, 0), DateTimeKind.Utc);

            int delta = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek - today.DayOfWeek;

            if(delta > 0)
                delta -= 7;
            
            var firstDayOfCurrentWeek = today.AddDays(delta);

            var weekStart = firstDayOfCurrentWeek;

            var weekEnd = weekStart.AddDays(7);

            var enumerableOrders = orders as IList<Order> ?? orders.ToList();

            var weeklySalesDataPoints = new List<WeeklySalesDataPoint>();

            double weekTotal = 0;

            for (int i = 0; i < numberOfWeeks; i++)
            {
                weekStart = weekStart.AddDays(-7);
                weekEnd = weekEnd.AddDays(-7);
                weekTotal = GetOrderTotalForPeriod(enumerableOrders, weekStart, weekEnd);
                weeklySalesDataPoints.Add(new WeeklySalesDataPoint() { DateStart = weekStart, DateEnd = weekEnd, Amount = weekTotal });
            }

            return weeklySalesDataPoints;
        }

        public async Task<IEnumerable<IGrouping<string, CategorySalesDataPoint>>> GetCategorySalesDataPointsAsync(IEnumerable<Order> orders, int numberOfWeeks = defaultNumberOfWeeks, OrderStatusOption statusOption = OrderStatusOption.Both)
        {
            var now = DateTime.UtcNow;

            var today = DateTime.SpecifyKind(new DateTime(now.Year, now.Month, now.Day, 0, 0, 0), DateTimeKind.Utc);

            int delta = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek - today.DayOfWeek;

            if(delta > 0)
                delta -= 7;

            var firstDayOfCurrentWeek = today.AddDays(delta);

            var dateEnd = firstDayOfCurrentWeek;

            var dateStart = dateEnd.AddDays(numberOfWeeks * -7);

            var enumerableOrders = orders as IList<Order> ?? orders.ToList();

            var categorySalesDataPoints = new List<CategorySalesDataPoint>();

            IEnumerable<Order> dateRangedOrders;

            switch (statusOption)
            {
                case OrderStatusOption.Open:
                    dateRangedOrders = enumerableOrders.Where(
                        order => 
                        order.IsOpen &&
                        order.OrderDate >= dateStart &&
                        order.OrderDate < dateEnd);
                    break;
                case OrderStatusOption.Closed:
                    dateRangedOrders = enumerableOrders.Where(
                        order => 
                        !order.IsOpen &&
                        order.OrderDate >= dateStart &&
                        order.OrderDate < dateEnd);
                    break;
                default:
                    dateRangedOrders = enumerableOrders.Where(
                        order => 
                        order.OrderDate >= dateStart &&
                        order.OrderDate < dateEnd);
                    break;
            }

            foreach (var r in dateRangedOrders)
            {
                categorySalesDataPoints.Add(new CategorySalesDataPoint() { CategoryName = (await _DataClient.GetTopLevelCategory((await _DataClient.GetProductByNameAsync(r.Item)).CategoryId)).Name, Amount = r.Price });
            }

            var groupedCategorySalesDataPoints = categorySalesDataPoints.GroupBy(x => x.CategoryName);

            return groupedCategorySalesDataPoints;
        }

        #endregion

        static double GetOrderTotalForPeriod(IEnumerable<Order> orders, DateTime dateStart, DateTime dateEnd, OrderStatusOption statusOption = OrderStatusOption.Both)
        {
            double total = 0;

            IEnumerable<Order> results;

            switch (statusOption)
            {
                case OrderStatusOption.Open:
                    results = orders.Where(
                        order => 
                        order.IsOpen &&
                        order.OrderDate >= dateStart &&
                        order.OrderDate < dateEnd);
                    break;
                case OrderStatusOption.Closed:
                    results = orders.Where(
                        order => 
                        !order.IsOpen &&
                        order.OrderDate >= dateStart &&
                        order.OrderDate < dateEnd);
                    break;
                default:
                    results = orders.Where(
                        order => 
                        order.OrderDate >= dateStart &&
                        order.OrderDate < dateEnd);
                    break;
            }

            foreach (var order in results)
            {
                total += order.Price;
            }

            return total;
        }
    }
}

