using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Web.ViewModels;

namespace WhiteLagoon.Application.Services.Interface
{
    public interface IDashboardService
    {
         Task<RadialBarChartDTO> GetTotalBookingRadialChartData();
        Task<RadialBarChartDTO> GetRegisteredUsersRadialChartData();
        Task<RadialBarChartDTO> GetRevenueRadialChartData();
        Task<PieChartDTO> GetCustomerBookingPieChartData();
    }
}
