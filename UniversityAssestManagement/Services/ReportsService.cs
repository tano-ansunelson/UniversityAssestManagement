using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.Json;
using UniversityAssestManagement.Models;
using UniversityAssestManagement.Models.ViewModels;
using UniversityAssetsManagement.Repositories;

namespace UniversityAssetsManagement.Services
{
    public class ReportsService : IReportsService
    {
        private readonly IReportsRepository _repo;

        public ReportsService(IReportsRepository repo)
        {
            _repo = repo;
        }

        public async Task<ReportsViewModel> GetReportsAsync()
        {
           var colleges = await _repo.GetCollegeWithAssetsAsync();
           var damagedAssets = await _repo.GetDamagedAssetsAsync();
           var totalValue = await _repo.GetTotalAssetValueAsync();
           var uniqueValue = await _repo.GetValueByTypeAsync(AssetType.Unique);
           var bulkValue = await _repo.GetValueByTypeAsync(AssetType.Bulk);

            var byCollege = colleges.Select( c => new CollegeAssetsSummary
            {
              College =c.Name.Replace("College of", ""),
              Total = c.Assets.Count,
              Active = c.Assets.Count(a => a.Status == AssetStatus.Active),
              Damaged = c.Assets.Count(a => a.Status == AssetStatus.Damaged),

            }).OrderByDescending(c => c.Total).ToList();

            var topCollege = byCollege.FirstOrDefault();


            var damagedRows = damagedAssets.Select(a => new DamagedAssetRow
            {
                AssetName = a.Name,
                CollegeName = a.College.Name.Replace("College of", ""),
                Severity = GetSeverity(a.Value),
                ReportedDate = a.CreatedAt,
            }).ToList();



            var breakdowm = new AssetValueBreakdown
            {
                UniqueValue = uniqueValue,
                BulkValue = bulkValue,
                Total = totalValue,
            };

            return new ReportsViewModel
            {
                TotalAssetValue = totalValue,
                MostActiveCollege = topCollege?.College ?? "-",
                MostActiveCount = topCollege?.Total ?? 0,
                TotalDamaged =byCollege.Sum(c => c.Damaged),
                AssetByCollege =byCollege,
                GrandTotal =byCollege.Sum(c => c.Total),
                GrandActive = byCollege.Sum(c => c.Active),
                GrandDamaged= byCollege.Sum(c => c.Damaged),
                DamagedAssets =damagedRows,
                ValueBreakdown = breakdowm,

            };


        }

        private static string GetSeverity(decimal value) => value switch
        {
            >= 2000 => "High",
            >= 500 => "Medium",
            _ => "Low",
        };
    }
}
