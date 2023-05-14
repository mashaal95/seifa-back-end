using Microsoft.EntityFrameworkCore;
using Seifa2011_2016_API.Interfaces;
using Seifa2011_2016_API.Models;

namespace Seifa2011_2016_API.Repositories
{
    public class SeifaRepository : ISeifaRepository
    {
        private readonly TestDbContext _context;

        public SeifaRepository(TestDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<string>> GetDistinctLocalGovtAreas()
        {
            var states = await _context.Seifa2011s
                .Select(s => s.LocalGovtAreas)
                .Distinct()
                .ToListAsync();

            return states;
        }

        public async Task<IEnumerable<object>> GetAllSeifaRecordsForEachState(string state)
        {
            var seifaRecords = await (
                from seifa2011 in _context.Seifa2011s
                where seifa2011.LocalGovtAreas == state
                select new
                {
                    Year = "2011",
                    StateName = seifa2011.LocalGovtAreas,
                    PlaceName = seifa2011.Locations,
                    DisadvantageScore = seifa2011.RelativeDisadvantage,
                    AdvantageScore = seifa2011.RelativeAdvantage
                })
                .Union(
                    from seifa2016 in _context.Seifa2016s
                    join seifa2011 in _context.Seifa2011s on seifa2016.LgaName equals seifa2011.Locations
                    where seifa2011.LocalGovtAreas == state
                    select new
                    {
                        Year = "2016",
                        StateName = seifa2011.LocalGovtAreas,
                        PlaceName = seifa2016.LgaName,
                        DisadvantageScore = seifa2016.IndexOfRelativeSocioEconomicDisadvantageScore,
                        AdvantageScore = seifa2016.IndexOfRelativeSocioEconomicAdvantageDisadvantageScore
                    })
                .ToListAsync();

            return seifaRecords;
        }

        public async Task<IEnumerable<object>> GetSeifaRecordsForEachState(string state)
        {
            var seifa2011Filtered = _context.Seifa2011s.Where(x => x.LocalGovtAreas != null && x.LocalGovtAreas.Equals(state));


            var medianDisadvantage2011 = seifa2011Filtered.Select(x => x.RelativeDisadvantage).OrderBy(x => x).Skip(seifa2011Filtered.Count() / 2).FirstOrDefault();
            var medianDisadvantage2016 = _context.Seifa2016s.Select(x => x.IndexOfRelativeSocioEconomicDisadvantageScore).OrderBy(x => x).Skip(_context.Seifa2016s.Count() / 2).FirstOrDefault();


            var seifaRecords = await (
                from seifa2011 in _context.Seifa2011s
                join seifa2016 in _context.Seifa2016s on seifa2011.Locations equals seifa2016.LgaName
                where seifa2011.RelativeDisadvantage > medianDisadvantage2011 && seifa2016.IndexOfRelativeSocioEconomicDisadvantageScore > medianDisadvantage2016
                select new
                {
                    Disadvantage2011 = seifa2011.RelativeDisadvantage,
                    Disadvantage2016 = seifa2016.IndexOfRelativeSocioEconomicDisadvantageScore,
                    Comparison = seifa2016.IndexOfRelativeSocioEconomicDisadvantageScore - seifa2011.RelativeDisadvantage,
                    PlaceName = seifa2016.LgaName,
                    StateName = seifa2011.LocalGovtAreas
                })
                .ToListAsync();

            return seifaRecords;
        }

        public bool Seifa2011Exists(int id)
        {
            return _context.Seifa2011s.Any(e => e.SeifaId == id);
        }
    }
}
