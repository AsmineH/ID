using Data.ViewModel;
using Repository.ImportData;
using Repository.ImportData.SeedingData;
using System.Data.Entity;
using System.IO;
using System.Reflection;
using MathNet.Numerics.Statistics;
using System.Linq;
using Repository.ImportData.Extensions;

namespace Repository
{
	public class Seeding : DropCreateDatabaseIfModelChanges<LGAContext>
	{
		protected override void Seed(LGAContext context)
		{
			SeedingContext seedingContext = new SeedingContext();
			SeedingAdapter adapter = new SeedingAdapter();

			SeedSEIFA_2011(seedingContext);
			SeedSEIFA_2016(seedingContext);

			CalculateMedianByState(seedingContext);

			adapter.MergeContext(context, seedingContext);
			base.Seed(context);
		}

		private void CalculateMedianByState(SeedingContext context)
		{
			foreach (var state in context.States)
			{
				var data = context.Scores
					.Where(p=> p.DisadvantageScore.HasValue
						&&
						p.Location != null
						&& p.Location.State !=null 
						&& p.Location.State.StateName.Equals
						(state.StateName,System.StringComparison.InvariantCultureIgnoreCase))
						.Select(p=> (int)p.DisadvantageScore)
						.ToList();
				if (data.Count > 0)
				{ state.Median = data.GetMedian(); }
			}
		}

		private void SeedSEIFA_2016(SeedingContext context)
		{
			string resourceName = @"Repository.SeedData.SEIFA_2016.csv";

			IScoreImportStrategy strategy = new ScoreDetailImportStrategy();
			Stream stream = GetStream(resourceName);

            strategy.SeedToContext(stream, context, 2016);
        }

		private void SeedSEIFA_2011(SeedingContext context)
		{
			string resourceName = @"Repository.SeedData.SEIFA_2011.csv";

			IScoreImportStrategy strategy = new ScoreImportStrategy();
			Stream stream = GetStream(resourceName);

			strategy.SeedToContext(stream, context, 2011);
		}

		private Stream GetStream(string resourceName)
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
			return assembly.GetManifestResourceStream(resourceName);
		}
	}
}
