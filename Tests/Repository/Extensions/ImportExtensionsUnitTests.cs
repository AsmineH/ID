using NUnit.Framework;
using Repository.ImportData.Extensions;
using System;
using System.Collections.Generic;
using Data.ViewModel;

namespace Tests.Repository.Extensions
{
    [TestFixture]
    public class ImportExtensionsUnitTests
    {
        private static State initialState = new State()
        {
            StateName = "Victoria"
        };
        private static Location initialLocation = new Location()
        {
            PlaceName = "Somewhere",
            State = initialState
        };
        private static Score initialScore = new Score()
        {
            Year = 2016,
            Location = initialLocation
        };

        private static State stateToAdd = new State()
        {
            StateName = "Queensland"
        };
        private static Location locationToAdd = new Location()
        {
            PlaceName = "Somewhere else",
            Code = 10,
            State = initialState
        };
        private static Score scoreToAdd = new Score()
        {
            DisadvantageScore = 110, 
            AdvantageDisadvantageScore = 200,
            Year = 2016,
            Location = initialLocation
        };

        //Score

        [Test]
        public void ScoreAddOrUpdate_EmptyScores_NullScore()
        {
            var input = new List<Score>();
            Assert.IsNull(input.AddOrUpdate(null));
            Assert.IsEmpty(input);
        }


        [Test]
        public void ScoreAddOrUpdate_NullScore()
        {
            var input = new List<Score>()
            {
                initialScore
            };
            Assert.AreEqual(null, input.AddOrUpdate(null));
            Assert.Contains(initialScore, input);
            Assert.AreEqual(1, input.Count);
        }

        [Test]
        public void ScoreAddOrUpdate_EmptyScores_AddNewScore()
        {
            var input = new List<Score>();
            Assert.AreEqual(scoreToAdd, input.AddOrUpdate(scoreToAdd));
            Assert.AreEqual(1, input.Count);
            Assert.Contains(scoreToAdd, input);
        }

        [Test]
        public void ScoreAddOrUpdate_AddNewScore()
        {
            // Build a list with a variation on all the properties taken into account when finding a new score
            var input = new List<Score>()
            {
                new Score()
                {
                    Year = 2011,
                    Location = initialLocation
                },
                new Score()
                {
                    Year = 2016,
                    Location = locationToAdd
                },
                new Score()
                {
                    Year = 2016,
                    Location = new Location()
                    {
                        PlaceName = "Somewhere",
                        State = stateToAdd
                    }
                }
            };

            Assert.AreEqual(scoreToAdd, input.AddOrUpdate(scoreToAdd));
            Assert.AreEqual(4, input.Count);
            Assert.Contains(scoreToAdd, input);
        }

        [Test]
        public void ScoreAddOrUpdate_ThrowsForDuplicates()
        {
            var input = new List<Score>()
            {
                new Score()
                {
                    Year = 2016,
                    Location = new Location()
                    {
                        PlaceName = "Somewhere",
                        State = initialState
                    }
                },
                new Score()
                {
                    Year = 2016,
                    Location = new Location()
                    {
                        PlaceName = "SOMEWHERE",
                        State = initialState
                    }
                }
            };
            Assert.Throws<InvalidOperationException>(() => input.AddOrUpdate(scoreToAdd));
        }

        [Test]
        public void ScoreAddOrUpdate_UpdateScore()
        {
            var input = new List<Score>()
            {
                initialScore
            };
            var result = input.AddOrUpdate(scoreToAdd);
            Assert.AreEqual(1, input.Count);
            Assert.AreEqual(scoreToAdd.AdvantageDisadvantageScore, result.AdvantageDisadvantageScore);
            Assert.AreEqual(scoreToAdd.DisadvantageScore, result.DisadvantageScore);
        }

        [Test]
        public void ScoreAddOrUpdate_UpdateScoreWithNullState()
        {
            initialLocation.State = null;
            scoreToAdd.Location.State = null;
            var input = new List<Score>()
            {
                new Score()
                {
                    Year = 2011,
                    Location = initialLocation
                },
                new Score()
                {
                    Year = 2016,
                    Location = locationToAdd
                }
            };

            Assert.AreEqual(scoreToAdd, input.AddOrUpdate(scoreToAdd));
            Assert.AreEqual(3, input.Count);
            Assert.Contains(scoreToAdd, input);
        }

        [Test]
        public void ScoreAddOrUpdate_DoesntUpdateScore()
        {
            var input = new List<Score>()
            {
                new Score(){
                    DisadvantageScore = 200,
                    AdvantageDisadvantageScore = 300,
                    Year = 2016,
                    Location = initialLocation
                }
            };
            var result = input.AddOrUpdate(scoreToAdd);
            Assert.AreEqual(input[0].AdvantageDisadvantageScore, result.AdvantageDisadvantageScore);
            Assert.AreEqual(input[0].DisadvantageScore, result.DisadvantageScore);
            Assert.AreEqual(1, input.Count);
        }

        //Location

        [Test]
        public void LocationAddOrUpdate_ThrowsForDuplicates()
        {
            var input = new List<Location>()
            {
                new Location()
                {
                    PlaceName = "Somewhere",
                    State = initialState
                },
                new Location()
                {
                    PlaceName = "SOMEWHERE",
                    State = initialState
                },
            };
            Assert.Throws<InvalidOperationException>(() => input.AddOrUpdate(initialLocation));
        }

        [Test]
        public void LocationAddOrUpdate_EmptyLocation_NullLocation()
        {
            var input = new List<Location>();
            Assert.IsNull(input.AddOrUpdate(null));
            Assert.IsEmpty(input);
        }

        [Test]
        public void LocationAddOrUpdate_NullLocation()
        {
            var input = new List<Location>()
            {
                initialLocation
            };
            Assert.AreEqual(null, input.AddOrUpdate(null));
            Assert.Contains(initialLocation, input);
        }

        [Test]
        public void LocationAddOrUpdate_EmptyLocation()
        {
            var input = new List<Location>();
            Assert.AreEqual(locationToAdd, input.AddOrUpdate(locationToAdd));
            Assert.AreEqual(1, input.Count);
            Assert.Contains(locationToAdd, input);
        }

        [Test] 
        public void LocationAddOrUpdate_AddLocation()
        {
            var input = new List<Location>
            {
                initialLocation
            };
            input.AddOrUpdate(locationToAdd);

            Assert.AreEqual(2, input.Count);
            Assert.Contains(locationToAdd, input);
        }

        [Test]
        public void LocationAddOrUpdate_UpdateLocationWithNullState()
        {
            var input = new List<Location>
            {
                new Location ()
                {
                    PlaceName = "Somewhere else"
                }
            };
            input.AddOrUpdate(locationToAdd);

            Assert.AreEqual(1, input.Count);
            Assert.AreEqual(locationToAdd.Code, input[0].Code);
            Assert.AreEqual(locationToAdd.State, input[0].State);
        }

        [Test]
        public void LocationAddOrUpdate_UpdateLocationStateAndCode()
        {
            var input = new List<Location>
            {
                new Location ()
                {
                    PlaceName = "Somewhere else"
                }
            };
            input.AddOrUpdate(locationToAdd);

            Assert.AreEqual(1, input.Count);
            Assert.AreEqual(locationToAdd.Code, input[0].Code);
            Assert.AreEqual(locationToAdd.State, input[0].State);
        }

        [Test]
        public void LocationAddOrUpdate_UpdateLocationCode()
        {
            var input = new List<Location>
            {
                new Location ()
                {
                    PlaceName = "Somewhere else", 
                    State = initialState
                }
            };
            input.AddOrUpdate(locationToAdd);

            Assert.AreEqual(1, input.Count);
            Assert.AreEqual(locationToAdd.Code, input[0].Code);
        }

        //State

        [Test]
        public void StateAddOrUpdate_EmptyStates_NullState()
        {
            var input = new List<State>();
            Assert.AreEqual(null, input.AddOrUpdate(null));
            Assert.IsEmpty(input);
        }

        [Test]
        public void StateAddOrUpdate_NullState()
        {
            var input = new List<State>()
            {
                initialState
            };
            Assert.AreEqual(null, input.AddOrUpdate(null));
            Assert.Contains(initialState, input);
        }

        [Test]
        public void StateAddOrUpdate_EmptyStates_AddNewState()
        {
            var input = new List<State>();
            Assert.AreEqual(stateToAdd, input.AddOrUpdate(stateToAdd));
            Assert.Contains(stateToAdd, input);
            Assert.AreEqual(1, input.Count);
        }

        [Test]
        public void StateAddOrUpdate_AddNewState()
        {
            var input = new List<State>()
            { 
                initialState
            };
            Assert.AreEqual(stateToAdd, input.AddOrUpdate(stateToAdd));
            Assert.Contains(stateToAdd, input);
            Assert.AreEqual(2, input.Count);
        }

        [Test]
        public void StateAddOrUpdate_DoNothing()
        {
            var input = new List<State>()
            {
                initialState
            };
            initialState.StateName = initialState.StateName.ToUpper();
            Assert.AreEqual(initialState, input.AddOrUpdate(initialState));
            Assert.Contains(initialState, input);
            Assert.AreEqual(1, input.Count);
        }
    }
}
