﻿namespace Valorant.API
{
    public enum Region
    {
        NA,
        EU,
        AP,
        KO
    }
    public class Ranks
    {
        public static string GetRankFormatted(int rank)
        {
            string[] ranksfor = new string[] { "Unrated", "none", "none", "Iron 1", "Iron 2", "Iron 3", "Bronze 1", "Bronze 2", "Bronze 3", "Silver 1", "Silver 2", "Silver 3", "Gold 1", "Gold 2", "Gold 3", "Platinum 1", "Platinum 2", "Platinum 3", "Diamond 1", "Diamond 2", "Diamond 3", "Immortal", "none", "none", "Radiant" };
            return ranksfor[rank];
        }
    }
}
