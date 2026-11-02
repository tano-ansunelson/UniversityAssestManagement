using Microsoft.AspNetCore.Mvc;
using UniversityAssestManagement.Models;
using UniversityAssestManagement.Models.ViewModels;
using UniversityAssetsManagement.Repositories;

namespace UniversityAssetsManagement.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _repo;

        public DashboardService(IDashboardRepository repo)
        {
            _repo = repo;
        }

        public async Task<DashboardViewModel> GetDashboardAsync()
        {
           var totalAssets = await _repo.GetTotalAssetsAsync();
           var totalColleges = await _repo.GetTotalCollegeAsync();
            var damaged = await _repo.GetDamagedAssetsAsync();
            var underMaintenance = await _repo.GetUnderMaintenanceAsync();
           var recentAssets = await _repo.GetRecentAssetsAsync(6);
            var colleges = await _repo.GetCollegesWithAssetsAsync();


            var MaxAssets = colleges.Any()
                ? colleges.Max(c => c.Assets.Count)
                : 1;

            var summaries =colleges
                .OrderByDescending(c =>  c.Assets.Count)
                .Select(c => new CollegeAssetsSummary
                {
                    College = c.Name.Replace("College of", ""),
                    Total = c.Assets.Count,

                    Active = c.Assets.Count(a => a.Status == AssetStatus.Active),
                    Damaged = c.Assets.Count(a => a.Status == AssetStatus.Damaged),
                    Maintenance = c.Assets.Count(a => a.Status == AssetStatus.UnderMaintenance),
                    BarPercentage = MaxAssets ==0
                    ?0 : (int)((double)c.Assets.Count/MaxAssets * 100),


                }). ToList();


            return new DashboardViewModel
            {
                TotalAssets = totalAssets,
                TotalColleges = totalColleges,
                DamagedAssets = damaged,
                UnderMaintenance = underMaintenance,
                RecentAssets = recentAssets,
                CollegeSummaries = summaries,
            };






        }

       
    }
}
