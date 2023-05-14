using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Seifa2011_2016_API.Models;

namespace Seifa2011_2016_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeifaController : ControllerBase
    {
        private readonly TestDbContext _context;

        public SeifaController(TestDbContext context)
        {
            _context = context;
        }


        // GET: api/Seifa2011/5
        [HttpGet]
        [Route("allStates")]
        public async Task<string> GetStates()
        {
            //if (_context.Seifa2011s == null)
            //{
            //    return NotFound();
            //}
            var states = (from s in _context.Seifa2011s
                select s.LocalGovtAreas).Distinct();

            string json = JsonConvert.SerializeObject(states);

            //if (seifa2011 == null)
            //{
            //    return NotFound();
            //}

            return json;
        }

        [HttpGet]
        [Route("allData")]
        public async Task<string> GetAllSeifaRecordsForEachState(string state)
        {

            var seifa2011Filtered = _context.Seifa2011s.Where(x => x.LocalGovtAreas != null && x.LocalGovtAreas.Equals(state));


            var s = (from x in seifa2011Filtered
                
                    select new
                    {
                        Year = "2011",
                        StateName = x.LocalGovtAreas,
                        PlaceName = x.Locations,
                        DisadvantageScore = x.RelativeDisadvantage,
                        AdvantageScore = x.RelativeAdvantage
                    })
                .Union
                (
                    from seifa2016 in _context.Seifa2016s
                    join z in seifa2011Filtered on seifa2016.LgaName equals z.Locations
                    select new
                    {
                        Year = "2016",
                        StateName = z.LocalGovtAreas,
                        PlaceName = seifa2016.LgaName,
                        DisadvantageScore = seifa2016.IndexOfRelativeSocioEconomicDisadvantageScore,
                        AdvantageScore = seifa2016.IndexOfRelativeSocioEconomicAdvantageDisadvantageScore
                    }
                );

            string json = JsonConvert.SerializeObject(s);
            return json;
        }




        // GET: api/Seifa2011/Victoria
        [HttpGet("{state}")]
        public async Task<string> GetSeifaRecordsForEachState(string state)
        {
            //if (_context.Seifa2011s == null)
            //{
            //    return NotFound();
            //}

            var seifa2011Filtered = _context.Seifa2011s.Where(x => x.LocalGovtAreas != null && x.LocalGovtAreas.Equals(state));
   

            var medianDisadvantage2011 = seifa2011Filtered.Select(x => x.RelativeDisadvantage).OrderBy(x => x).Skip(seifa2011Filtered.Count() / 2).FirstOrDefault();
            var medianDisadvantage2016 = _context.Seifa2016s.Select(x => x.IndexOfRelativeSocioEconomicDisadvantageScore).OrderBy(x => x).Skip(_context.Seifa2016s.Count() / 2).FirstOrDefault();

            var s = from x in seifa2011Filtered
                    join z in _context.Seifa2016s on x.Locations equals z.LgaName
                    where x.RelativeDisadvantage > medianDisadvantage2011 && z.IndexOfRelativeSocioEconomicDisadvantageScore > medianDisadvantage2016
                    select new
                    {
                        Disadvantage2011 = x.RelativeDisadvantage,
                        Disadvantage2016 = z.IndexOfRelativeSocioEconomicDisadvantageScore,
                        Comparison =  z.IndexOfRelativeSocioEconomicDisadvantageScore - x.RelativeDisadvantage,
                        PlaceName = z.LgaName,
                        StateName = x.LocalGovtAreas
                    };

            string json = JsonConvert.SerializeObject(s);
            return json;

            //if (seifa2011 == null)
            //{
            //    return NotFound();
            //}

        }

       
        private bool Seifa2011Exists(int id)
        {
            return (_context.Seifa2011s?.Any(e => e.SeifaId == id)).GetValueOrDefault();
        }
    }
}
