using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoolScrapper
{
    public class MatchMasterModel
    {
        public string matchDate { get; set; }
        public string roundno { get; set; }
        public string fieldno { get; set; }
        public string divisionName { get; set; }
        public string poolName { get; set; }
        public string teamA { get; set; }
        public string versus { get; set; }
        public string teamB { get; set; }
        public string halveA { get; set; }
        public string halveB { get; set; }
    }

    public class RootObject
    {
        public string PoolIDHome { get; set; }
        public string Match { get; set; }
        public string DetailedResultsURL { get; set; }
        public string HomeID { get; set; }
        public string smlFixID { get; set; }
        public int isBye { get; set; }
        public string HomeLinkRaw { get; set; }
        public string HomeScore { get; set; }
        public string MatchStatus { get; set; }
        public string HomeClubLogoSml { get; set; }
        public string HomeTeamLogo { get; set; }
        public string Pool { get; set; }
        public string FixtureID { get; set; }
        public string ExtKey { get; set; }
        public string AwayClubLogo { get; set; }
        public int FutureGame { get; set; }
        public string TimeDateRaw { get; set; }
        public string Date { get; set; }
        public string MySportLink { get; set; }
        public string AwayID { get; set; }
        public string AwayClubName { get; set; }
        public string TimeRaw { get; set; }
        public string FinalisationString { get; set; }
        public string AwayTeamLogoSml { get; set; }
        public string OLRLink { get; set; }
        public int PastGame { get; set; }
        public string HomeClubLogo { get; set; }
        public string AssocID { get; set; }
        public string Round { get; set; }
        public string HomeLinkSFIX { get; set; }
        public string AwayClubLogoSml { get; set; }
        public string DateTime { get; set; }
        public string AwayTeamLogo { get; set; }
        public string WinnerLink { get; set; }
        public string DetailedResults { get; set; }
        public string HomeAbbrev { get; set; }
        public string AwayNameFMT { get; set; }
        public string BothTeamsID { get; set; }
        public string HomeTeamLogoSml { get; set; }
        public string WinnerID { get; set; }
        public string HomeClubID { get; set; }
        public string AssocName { get; set; }
        public string Client { get; set; }
        public string SportID { get; set; }
        public string AwayLinkRaw { get; set; }
        public string DateRaw { get; set; }
        public string PoolIDAway { get; set; }
        public string VersusString { get; set; }
        public string HomeLink { get; set; }
        public string AwayClubID { get; set; }
        public string Time { get; set; }
        public string AwayLink { get; set; }
        public string AwayName { get; set; }
        public string AwayLinkSFIX { get; set; }
        public string CompName { get; set; }
        public string HomeNameFMT { get; set; }
        public string VenueName { get; set; }
        public string CompID { get; set; }
        public string HomeName { get; set; }
        public string Venue { get; set; }
        public int MatchToday { get; set; }
        public string HomeClubName { get; set; }
        public string AwayAbbrev { get; set; }
        public string MatchName { get; set; }
        public string VenueURL { get; set; }
        public string AwayScore { get; set; }
    }
}
