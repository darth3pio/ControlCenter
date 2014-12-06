using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BF2Statistics.Web.Bf2Stats
{
    public struct Player
    {
        public int Pid;
        public string Name;
        public int Rank;
        public int Count;
        public string Value;
    }

    public struct PlayerResult
    {
        public int Pid;
        public string Name;
        public int Rank;

        public int Score;
        public double Kdr;
        public double Spm;

        public string LastOnline;
        public string TimePlayed;
        public string Country;
    }

    public struct RankingStats
    {
        public string Name;
        public string Desc;

        public List<Player> TopPlayers;
    }

    public struct ArmyMapStat
    {
        public int Id;
        public int Wins;
        public int Losses;
        public int Time;
        public int Best;
        public double Ratio
        {
            get
            {
                if (Losses > 0)
                    return Math.Round((double)Wins / Losses, 2);
                else
                    return Wins;
            }
        }
    }

    public struct ArmyMapSummary
    {
        // public static int TotalArmies;
        public int TotalTime;
        public int TotalWins;
        public int TotalLosses;
        public int TotalBest;

        public double AverageTime
        {
            get { return Math.Round((double)TotalTime / 14, 0); }
        }
        public double AverageWins
        {
            get { return Math.Round((double)TotalWins / 14, 0); }
        }
        public double AverageLosses
        {
            get { return Math.Round((double)TotalLosses / 14, 0); }
        }
        public double AverageBest
        {
            get { return Math.Round((double)TotalBest / 14, 0); }
        }

        public double Ratio
        {
            get
            {
                if (TotalLosses > 0)
                    return Math.Round((double)TotalWins / TotalLosses, 2);
                else
                    return TotalWins;
            }
        }
    }

    public struct TheaterStat
    {
        public string Name;
        public int Wins;
        public int Losses;
        public int Time;
        public int Best;
        public double Ratio
        {
            get
            {
                if (Losses > 0)
                    return Math.Round((double)Wins / Losses, 2);
                else
                    return Wins;
            }
        }
    }

    public struct ObjectStat
    {
        public int Time;
        public int Kills;
        public int Deaths;
        public int Fired;
        public int Hits;
        public int RoadKills;
        public int Deployed;

        public double KillsAcctFor;

        public double Ratio
        {
            get
            {
                if (Deaths > 0)
                    return Math.Round((double)(Kills + RoadKills) / Deaths, 4);
                else
                    return Kills;
            }
        }

        public double Accuracy
        {
            get
            {
                if (Hits > 0)
                    return Math.Round(100 * ((double)Hits / Fired), 2);
                else
                    return 0.00;
            }
        }
    }

    public struct ObjectSummary
    {
        public int TotalTime;
        public int TotalKills;
        public int TotalDeaths;
        public int TotalRoadKills;
        public int TotalFired;
        public int TotalHits;

        public double KillsAcctFor;

        public double AverageTime
        {
            get { return Math.Round((double)TotalTime / 7, 0); }
        }
        public double AverageKills
        {
            get { return Math.Round((double)TotalKills / 7, 0); }
        }
        public double AverageDeaths
        {
            get { return Math.Round((double)TotalDeaths / 7, 0); }
        }
        public double AverageRoadKills
        {
            get { return Math.Round((double)TotalRoadKills / 7, 0); }
        }

        public double AverageRatio
        {
            get
            {
                if (TotalDeaths > 0)
                    return Math.Round((double)(TotalKills + TotalRoadKills) / TotalDeaths, 4);
                else
                    return TotalKills; 
            }
        }

        public double AverageAccuracy
        {
            get
            {
                if (TotalHits > 0)
                    return Math.Round(100 * ((double) TotalHits / TotalFired), 2);
                else
                    return 0.00;
            }
        }

    }

    public class WeaponStat
    {
        public int Time;
        public int Kills;
        public int Deaths;
        public int Fired;
        public int Hits;

        public double KillsAcctFor;

        public double Ratio
        {
            get
            {
                if (Deaths > 0)
                    return Math.Round((double)Kills / Deaths, 4);
                else
                    return Kills;
            }
        }

        public double Accuracy
        {
            get
            {
                if (Hits > 0)
                    return Math.Round(100 * ((double)Hits / Fired), 2);
                else
                    return 0.00;
            }
        }
    }

    public class WeaponSummary
    {
        public int TotalTime;
        public int TotalKills;
        public int TotalDeaths;
        public int TotalFired;
        public int TotalHits;

        public double KillsAcctFor;

        public double AverageTime(int NumWeapons)
        {
            return Math.Round((double)TotalTime / NumWeapons, 0);
        }

        public double AverageKills(int NumWeapons)
        {
            return Math.Round((double)TotalKills / NumWeapons, 0);
        }

        public double AverageDeaths(int NumWeapons)
        {
            return Math.Round((double)TotalDeaths / NumWeapons, 0);
        }

        public double AverageFired(int NumWeapons)
        {
            return Math.Round((double)TotalFired / NumWeapons, 0);
        }

        public double AverageHits(int NumWeapons)
        {
            return Math.Round((double)TotalHits / NumWeapons, 0);
        }

        public double AverageRatio
        {
            get
            {
                if (TotalDeaths > 0)
                    return Math.Round((double)TotalKills / TotalDeaths, 4);
                else
                    return TotalKills;
            }
        }

        public double AverageAccuracy
        {
            get
            {
                if (TotalHits > 0)
                    return Math.Round(100 * ((double)TotalHits / TotalFired), 2);
                else
                    return 0.00;
            }
        }

    }

    public struct WeaponUnlock
    {
        public int Id;
        public string Name;
        public string State;
    }

    public class Award
    {
        public int Id = 0;
        public int Level = 0;
        public int Earned = 0;
        public int First = 0;
    }

    public class Rank : ICloneable
    {
        // Variables that are always set
        public int Id;
        public int[] ReqRank;
        public int MinPoints;

        /// <summary>
        /// AwardId => Level
        /// </summary>
        public Dictionary<string, int> ReqAwards;

        // Variables that are set afterwards, on a per player basis
        public Dictionary<string, int> MissingAwards;
        public int PointsNeeded;
        public double PercentComplete;
        public double TimeToComplete;
        public int DaysToComplete;

        public string MissingDesc = String.Empty;

        public Object Clone()
        {
            return new Rank
            {
                Id = this.Id,
                ReqRank = this.ReqRank,
                MinPoints = this.MinPoints,
                ReqAwards = new Dictionary<string, int>(this.ReqAwards),
                MissingAwards = new Dictionary<string,int>(),
                PointsNeeded = 0,
                PercentComplete = 0.0,
                TimeToComplete = 0.0
            };
        }
    }
}
