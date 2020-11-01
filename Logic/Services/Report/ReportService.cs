using Data.ViewModel;
using Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Logic.Services.Report
{
	public class ReportService
	{
		private readonly LGAContext db = new LGAContext();

		private int _VicStateId = -1;
		public int VicStateId
		{
			get
			{
				if (_VicStateId == -1)
				{
					var state = db.States.Single(p => p.StateName.Equals("Victoria", StringComparison.CurrentCultureIgnoreCase));
					_VicStateId = state != null ? state.StateId : -1;
				}
				return _VicStateId;
			}
		}
		private IEnumerable<State> _States = Enumerable.Empty<State>();
		public IEnumerable<State> States
		{
			get
			{
                if (_States.Count() == 0)
                {
                    _States = db.States.ToList();
                }

                return _States;
			}
		}


		public IEnumerable<DataModel> GetAllData(int? stateId, bool showAllData = false)
		{
			// Default state
			if (!stateId.HasValue)
			{
				stateId = VicStateId;
			}

			var data =
                 GetScoreData((int)stateId, showAllData)
                .Where(p => p.Location.State.StateId == stateId)
				.Join(db.Locations,
				s => s.Location.LocationId,
				l => l.LocationId,
				(s, l) => new
				{
					l.State.StateId,
					l.PlaceName,
					s.AdvantageDisadvantageScore,
					s.DisadvantageScore,
					s.Year
				}).Join(db.States,
				s => s.StateId,
				st => st.StateId,
				(s, st) => new DataModel()
				{
					StateName = st.StateName,
					PlaceName = s.PlaceName,
					Year = s.Year,
					AdvantageDisadvantageScore = s.AdvantageDisadvantageScore,
					DisadvantageScore = s.DisadvantageScore
				})
				.OrderBy(p => new { p.StateName, p.PlaceName })
				.ToList();
			return data;
		}

		public IEnumerable<ReportModel> GetDisadges(int? stateId, bool showAllData = false)
		{
			// Default state
			if (!stateId.HasValue)
			{
				stateId = VicStateId;
			}

			if (stateId != -1)
			{
				var data2011 = GetScoreData((int)stateId, showAllData)
                    .Where(p => p.Year == 2011
						&& p.Location.State.StateId == stateId)
					.Select(p => new { p.Location.LocationId, p.DisadvantageScore })
					.ToList();
				var data2016 = GetScoreData((int)stateId, showAllData)
                    .Where(p => p.Year == 2016
						&& p.Location.State.StateId == stateId)
                    .Select(p => new { p.Location.LocationId, p.DisadvantageScore })
					.ToList();
				var both = data2011.Join(
					data2016,
					d11 => d11.LocationId,
					d16 => d16.LocationId,
					(d11, d16) => new
					{
						d11.LocationId,
						Score2011 = d11.DisadvantageScore,
						Score2016 = d16.DisadvantageScore
					});
				var result = both.Join(db.Locations,
						s => s.LocationId,
						l => l.LocationId,
						(s, l) => new { s.Score2011, s.Score2016, s.LocationId, l.PlaceName, l.State.StateId })
						.Join(db.States,
						s => s.StateId,
						st => st.StateId,
						(s, st) => new ReportModel()
						{
							Disadvantage2011 = s.Score2011,
							Disadvantage2016 = s.Score2016,
							PlaceName = s.PlaceName,
							StateName = st.StateName
						});
				return result.ToList();
			}

			return Enumerable.Empty<ReportModel>();
		}

        private IQueryable<Score> GetScoreData(int stateId, bool showAllData)
        {
            var scores = db.Scores.AsQueryable();
            if (!showAllData)
            {
                var medianScore = db.States.Where(s => s.StateId == stateId).FirstOrDefault()?.Median ?? 0m;
                scores = scores.Where(s => s.DisadvantageScore > medianScore);
            }
            return scores;
        }
	}
}
