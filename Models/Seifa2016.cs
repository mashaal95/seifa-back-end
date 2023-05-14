using System.ComponentModel.DataAnnotations;

namespace Seifa2011_2016_API.Models;

public partial class Seifa2016
{
    [Key]
    public int LgaCode { get; set; }

    public string? LgaName { get; set; }

    public int? IndexOfRelativeSocioEconomicDisadvantageScore { get; set; }

    public int? IndexOfRelativeSocioEconomicDisadvantageDecile { get; set; }

    public int? IndexOfRelativeSocioEconomicAdvantageDisadvantageScore { get; set; }

    public int? IndexOfRelativeSocioEconomicAdvantageDisadvantageDecile { get; set; }

    public int? IndexOfRelativeEconomicScore { get; set; }

    public int? IndexOfRelativeEconomicDecile { get; set; }

    public int? IndexOfEducationAndOccupationScore { get; set; }

    public int? IndexOfEducationAndOccupationDecile { get; set; }

    public int? UsualResidentPopulation { get; set; }
}
