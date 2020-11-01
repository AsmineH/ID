using Data.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repository.ImportData.Extensions
{
    public static class ImportExtensions
    {
        public static Score AddOrUpdate(this List<Score> scores, Score score)
        {
            if (scores != null && score != null)
            {
                Score found = scores.SingleOrDefault(p =>
                    p.Year == score.Year
                    && p.Location != null && score.Location != null
                    && p.Location.PlaceName.Equals(score.Location.PlaceName, StringComparison.InvariantCultureIgnoreCase)
                    &&
                    (
                        (p.Location.State == null && score.Location.State == null)
                        || (
                            (p.Location.State != null && score.Location.State != null)
                            && p.Location.State.StateName.Equals(score.Location.State.StateName, StringComparison.InvariantCultureIgnoreCase)
                            )
                    )
                );


                if (found != null)
                {
                    // Update
                    found.AdvantageDisadvantageScore = found.AdvantageDisadvantageScore ?? score.AdvantageDisadvantageScore;
                    found.DisadvantageScore = found.DisadvantageScore ?? score.DisadvantageScore;
                    return found;
                }
                else
                {
                    // Add
                    scores.Add(score);
                    return score;
                }
            }

            return null;
        }

        public static Location AddOrUpdate(this List<Location> locations, Location location)
        {
            if (locations != null && location != null)
            {
                Location updateLocation = locations.SingleOrDefault(p => p.PlaceName.Equals(location.PlaceName, StringComparison.CurrentCultureIgnoreCase));
                // Update
                if (updateLocation != null)
                {
                    if (updateLocation.State != null
                        && location.State != null
                        && !updateLocation.State.StateName.Equals(location.State.StateName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        locations.Add(location);
                        return location;
                    }
                    else
                    {
                        if (updateLocation.State == null && location.State != null)
                        {
                            updateLocation.State = location.State;
                        }
                        updateLocation.Code = updateLocation.Code ?? location.Code;
                        return updateLocation;
                    }
                }
                // Add
                else
                {
                    locations.Add(location);
                    return location;
                }
            }
            return null;
        }

        public static State AddOrUpdate(this List<State> states, State state)
        {
            if (states != null && state != null)
            {
                bool found = states.Any(p => p.StateName.Equals(state.StateName, StringComparison.CurrentCultureIgnoreCase));
                if (!found)
                {
                    states.Add(state);
                }
                // Do nothing for doublicate regions
            }
            return state;
        }
    }
}
