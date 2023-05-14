using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Seifa2011_2016_API.Interfaces;

namespace Seifa2011_2016_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeifaController : ControllerBase

    {
        private readonly ISeifaRepository _seifaRepository;

        public SeifaController(ISeifaRepository seifaRepository)
        {
            _seifaRepository = seifaRepository;
        }

        // GET: api/Seifa/allStates
        [HttpGet]
        [Route("allStates")]
        public async Task<string> GetStates()
        {
            var states = await _seifaRepository.GetDistinctLocalGovtAreas();
            string json = JsonConvert.SerializeObject(states);
            return json;
        }

        // GET: api/Seifa/allData
        [HttpGet]
        [Route("allData")]
        public async Task<string> GetAllSeifaRecordsForEachState(string state)
        {
            var seifaRecords = await _seifaRepository.GetAllSeifaRecordsForEachState(state);
            string json = JsonConvert.SerializeObject(seifaRecords);
            return json;
        }

        // GET: api/Seifa/{state}
        [HttpGet("{state}")]
        public async Task<string> GetSeifaRecordsForEachState(string state)
        {
            var seifaRecords = await _seifaRepository.GetSeifaRecordsForEachState(state);
            string json = JsonConvert.SerializeObject(seifaRecords);
            return json;
        }

        private bool Seifa2011Exists(int id)
        {
            return _seifaRepository.Seifa2011Exists(id);
        }
    }
}
