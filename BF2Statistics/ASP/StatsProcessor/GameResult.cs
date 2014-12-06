using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BF2Statistics.ASP.StatsProcessor
{
    public abstract class GameResult
    {
        /// <summary>
        /// The snapshot Date
        /// </summary>
        public DateTime Date { get; protected set; }

        /// <summary>
        /// The epoch Timestamp of when this snapshot was recieved by the ASP server
        /// for processing
        /// </summary>
        public int TimeStamp { get; protected set; }

        /// <summary>
        /// Returns the timespan of time the round lasted from start to finish
        /// </summary>
        public TimeSpan RoundTime { get; protected set; }

        /// <summary>
        /// Snapshot Server prefix
        /// </summary>
        public string ServerPrefix { get; protected set; }

        /// <summary>
        /// Snapshot Server Name
        /// </summary>
        public string ServerName { get; protected set; }

        /// <summary>
        /// Snapshot Server connection Port
        /// </summary>
        public int ServerPort { get; protected set; }

        /// <summary>
        /// Snapshot Server Gamespy Query Port
        /// </summary>
        public int QueryPort { get; protected set; }

        /// <summary>
        /// Map ID Played during this round
        /// </summary>
        public int MapId { get; protected set; }

        /// <summary>
        /// Is this a custom map?
        /// </summary>
        public bool IsCustomMap { get; protected set; }

        /// <summary>
        /// Map name played this round
        /// </summary>
        public string MapName { get; protected set; }

        /// <summary>
        /// Epoch timestamp of when the Round Started (Python's time.time() function)
        /// </summary>
        public int RoundStart { get; protected set; }

        /// <summary>
        /// Epoch timestamp of when the Round ended (Python's time.time() function)
        /// </summary>
        public int RoundEnd { get; protected set; }

        /// <summary>
        /// Total amount of kills from all players in the round
        /// </summary>
        public int MapKills { get; protected set; }

        /// <summary>
        /// Total amount of deaths from all players in the round (includes suicides and freak deaths)
        /// </summary>
        public int MapDeaths { get; protected set; }

        /// <summary>
        /// Sum of all the players scorein the given round
        /// </summary>
        public int MapScore { get; protected set; }

        /// <summary>
        /// The gamemode ID of this round (Cq, Coop, SP)
        /// </summary>
        public int GameMode { get; protected set; }

        /// <summary>
        /// Mod name that was played
        /// </summary>
        public string Mod { get; protected set; }

        /// <summary>
        /// The winning team ID. Value is set to 0 if no team won (tie)
        /// </summary>
        public int WinningTeam { get; protected set; }

        /// <summary>
        /// Winning Army ID. Value is set to -1 if no army won the round
        /// </summary>
        public int WinningArmyId { get; protected set; }

        /// <summary>
        /// Team 1's Army Id
        /// </summary>
        public int Team1ArmyId { get; protected set; }

        /// <summary>
        /// Team 2's Army Id
        /// </summary>
        public int Team2ArmyId { get; protected set; }

        /// <summary>
        /// Remaining round tickets team 1
        /// </summary>
        public int Team1Tickets { get; protected set; }

        /// <summary>
        /// Remaining round tickets team 2
        /// </summary>
        public int Team2Tickets { get; protected set; }

        /// <summary>
        /// The number of team 1 players
        /// </summary>
        public int Team1Players { get; protected set; }

        /// <summary>
        /// The number of team 2 players
        /// </summary>
        public int Team2Players { get; protected set; }

        /// <summary>
        /// The number of remaining team 1 players at the end of the round
        /// </summary>
        public int Team1PlayersEnd { get; protected set; }

        /// <summary>
        /// The number of remaining team 2 players at the end of the round
        /// </summary>
        public int Team2PlayersEnd { get; protected set; }

        /// <summary>
        /// A Count of how many played connected
        /// </summary>
        public int PlayersConnected { get; protected set; }

        /// <summary>
        /// The list of players and their data that played this round
        /// </summary>
        public List<Player> Players { get; protected set; }

        /// <summary>
        /// Returns the player object by PID.
        /// </summary>
        /// <param name="Pid"></param>
        /// <returns></returns>
        public Player GetPlayerById(int Pid)
        {
            return this.Players.FirstOrDefault(p => p.Pid == Pid);
        }

        /// <summary>
        /// Fetches all players into a list on the specfied team
        /// </summary>
        /// <param name="Team"></param>
        /// <returns></returns>
        public List<Player> GetPlayersByTeam(int Team)
        {
            return new List<Player>(from player in this.Players where player.Team == Team select player);
        }
    }
}
