using CsvHelper;
using Data.ViewModel;
using Repository.ImportData.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Repository.ImportData.SeedingData
{
	public class ScoreImportStrategy : IScoreImportStrategy
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
						csvReader.SkipHeaders(1);

						State state = new State();
						while (csvReader.Read())
						{
							var stateName = csvReader.GetString(0);
							
							var locationName = csvReader.GetString(1);
							if (!string.IsNullOrWhiteSpace(locationName))
							{
								if (!string.IsNullOrWhiteSpace(stateName))
								{
									state = new State() { StateName = stateName };
									context.States.AddOrUpdate(state);
								}

								Location location = new Location() { PlaceName = locationName, State = state };
								location = context.Locations.AddOrUpdate(location);

								var disadvantage = csvReader.GetInt(2);
								var advantage = csvReader.GetInt(3);

								Score score = new Score()
								{
									DisadvantageScore = disadvantage,
									AdvantageDisadvantageScore = advantage,
									Year = year,
									Location = location
								};

								context.Scores.AddOrUpdate(score);
							}
						}
					}
					// Todo: Create exception class 
					catch (Exception e) { }
				}
			}
		}
	}
}
