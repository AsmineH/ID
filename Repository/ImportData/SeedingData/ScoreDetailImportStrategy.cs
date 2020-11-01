using CsvHelper;
using Data.ViewModel;
using Logic.Extensions;
using Repository.ImportData.Extensions;
using System;
using System.IO;
using System.Text;

namespace Repository.ImportData.SeedingData
{
	public class ScoreDetailImportStrategy : IScoreImportStrategy
	{
		public void SeedToContext(Stream stream, SeedingContext context, int year)
		{
			using (stream)
			{
				using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
				{
					CsvReader csvReader = new CsvReader(reader);
					try
					{
						csvReader.SkipHeaders(6);

						while (csvReader.Read())
						{
							var locationCode = csvReader.GetInt(0);
							var locationName = csvReader.GetString(1);

							if (!string.IsNullOrWhiteSpace(locationName))
							{
								Location location = new Location() { PlaceName = locationName, Code = locationCode, State = null };

								location = context.Locations.AddOrUpdate(location);
								var disadvantage = csvReader.GetInt(2);
								var advantage = csvReader.GetInt(4);

								Score score = new Score()
								{
									DisadvantageScore = disadvantage,
									AdvantageDisadvantageScore = advantage,
									Year = year,
									Location = location
								};

								score = context.Scores.AddOrUpdate(score);

								var disadvantageDecile = csvReader.GetInt(3);
								var advantageDecile = csvReader.GetInt(5);
								var indexOfEconomicResourcesScore = csvReader.GetInt(6);
								var indexOfEconomicResourcesDecile = csvReader.GetInt(7);
								var indexOfEducationAndOccupationScore = csvReader.GetInt(8);
								var indexOfEducationAndOccupationDecile = csvReader.GetInt(9);
								var usualResedantPopulation = csvReader.GetDecimal(10);

								ScoreDetail scoreDetail = new ScoreDetail()
								{
									Score = score,
									DisadvantageDecile = disadvantageDecile,
									AdvantageDisadvantageDecile = advantageDecile,
									IndexOfEconomicResourcesScore = indexOfEconomicResourcesScore,
									IndexOfEconomicResourcesDecile = indexOfEconomicResourcesDecile,
									IndexOfEducationAndOccupationScore = indexOfEducationAndOccupationScore,
									IndexOfEducationAndOccupationDecile = indexOfEducationAndOccupationDecile,
									UsualResedantPopulation = usualResedantPopulation
								};

								scoreDetail.Score = score;
								context.ScoreDetails.Add(scoreDetail);
							}
						}
					}
					// Todo: Create exception class to be caught and logged the right logging
					catch (Exception e)
					{
					}
				}
			}
		}

	}
}

