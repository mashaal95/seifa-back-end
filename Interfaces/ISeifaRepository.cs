﻿namespace Seifa2011_2016_API.Interfaces
{
    public interface ISeifaRepository
    {
        Task<IEnumerable<string>> GetDistinctLocalGovtAreas();
        Task<IEnumerable<object>> GetAllSeifaRecordsForEachState(string state);
        Task<IEnumerable<object>> GetSeifaRecordsForEachState(string state);

        Task<IEnumerable<string>> GetDashboardStats();
        bool Seifa2011Exists(int id);
    }
}
