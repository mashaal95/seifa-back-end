using System;
using System.Collections.Generic;

namespace Seifa2011_2016_API.Models;

public partial class Seifa2011
{
    public string? LocalGovtAreas { get; set; }

    public string? Locations { get; set; }

    public int? RelativeDisadvantage { get; set; }

    public int? RelativeAdvantage { get; set; }

    public int SeifaId { get; set; }
}
