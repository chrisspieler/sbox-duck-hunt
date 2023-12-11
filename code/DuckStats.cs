using System.Text.Json.Serialization;

namespace Sandbox;

public class DuckStats
{
	public int HighestPoints { get; set; } = 0;
	public int HighestDucksHunted { get; set; } = 0;
	public int HighestCombo { get; set; } = 0;
	public int TotalGamesPlayed { get; set; } = 0;

	[JsonIgnore]
	public const string StatsFile = "personalStats.json";

	public static DuckStats Load()
	{
		if ( !FileSystem.Data.FileExists( StatsFile ) )
		{
			// Create a new stats file if it doesn't exist
			var stats = new DuckStats();
			stats.Save();
			return stats;
		}

		return FileSystem.Data.ReadJson<DuckStats>( "personalStats.json" );
	}

	public void Save()
	{
		FileSystem.Data.WriteJson( StatsFile, this );
	}
}
