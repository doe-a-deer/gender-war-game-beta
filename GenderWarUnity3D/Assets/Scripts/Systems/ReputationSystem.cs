using UnityEngine;
using System.Collections.Generic;
using GenderWar.Core;

namespace GenderWar.Systems
{
    /// <summary>
    /// Tracks player reputation based on choices across all dates
    /// </summary>
    [System.Serializable]
    public class ReputationSystem
    {
        // Reputation tags
        public bool LeftEarly { get; private set; }
        public bool StayedTooLong { get; private set; }
        public bool BoundarySetter { get; private set; }
        public bool HumorDeflect { get; private set; }
        public bool EngagedInSpreadsheet { get; private set; }
        public bool HighSpend { get; private set; }
        public bool ChaosAgent { get; private set; }

        // Tag counts for nuanced reputation
        private int boundaryCount = 0;
        private int humorCount = 0;
        private int engagementCount = 0;
        private int chaosCount = 0;

        // Choice keywords for tag detection
        private readonly string[] earlyExitKeywords = { "leave", "go", "escape", "run", "ghost", "bathroom" };
        private readonly string[] stayKeywords = { "stay", "wait", "listen", "okay", "sure", "fine" };
        private readonly string[] boundaryKeywords = { "no", "stop", "boundary", "refuse", "won't", "don't" };
        private readonly string[] humorKeywords = { "joke", "laugh", "funny", "kidding", "ironic" };
        private readonly string[] spreadsheetKeywords = { "spreadsheet", "algorithm", "data", "analyze", "system" };
        private readonly string[] chaosKeywords = { "scene", "yell", "throw", "flip", "chaos" };

        public void Reset()
        {
            LeftEarly = false;
            StayedTooLong = false;
            BoundarySetter = false;
            HumorDeflect = false;
            EngagedInSpreadsheet = false;
            HighSpend = false;
            ChaosAgent = false;

            boundaryCount = 0;
            humorCount = 0;
            engagementCount = 0;
            chaosCount = 0;
        }

        public void ProcessChoice(ChoiceLogEntry entry)
        {
            if (entry == null) return;

            string label = entry.ChoiceLabel?.ToLower() ?? "";
            string nodeId = entry.NodeId?.ToLower() ?? "";

            // Check for early exit
            if (ContainsAny(label, earlyExitKeywords) || nodeId.Contains("leave") || nodeId.Contains("exit"))
            {
                LeftEarly = true;
            }

            // Check for staying
            if (ContainsAny(label, stayKeywords) || nodeId.Contains("stay"))
            {
                StayedTooLong = true;
            }

            // Check for boundaries
            if (ContainsAny(label, boundaryKeywords))
            {
                boundaryCount++;
                if (boundaryCount >= 2)
                {
                    BoundarySetter = true;
                }
            }

            // Check for humor/deflection
            if (ContainsAny(label, humorKeywords))
            {
                humorCount++;
                if (humorCount >= 2)
                {
                    HumorDeflect = true;
                }
            }

            // Check for spreadsheet/framework engagement
            if (ContainsAny(label, spreadsheetKeywords) || nodeId.Contains("algorithm") || nodeId.Contains("spreadsheet"))
            {
                engagementCount++;
                if (engagementCount >= 1)
                {
                    EngagedInSpreadsheet = true;
                }
            }

            // Check for chaos
            if (ContainsAny(label, chaosKeywords))
            {
                chaosCount++;
                if (chaosCount >= 1)
                {
                    ChaosAgent = true;
                }
            }

            // Check for high spending
            if (entry.Effects != null && entry.Effects.MoneyChange <= -40)
            {
                HighSpend = true;
            }

            // Process explicit tags from effects
            if (entry.Effects?.Tags != null)
            {
                foreach (var tag in entry.Effects.Tags)
                {
                    ApplyTag(tag);
                }
            }
        }

        private void ApplyTag(string tag)
        {
            switch (tag.ToLower())
            {
                case "leftearly":
                    LeftEarly = true;
                    break;
                case "stayedtoolong":
                    StayedTooLong = true;
                    break;
                case "boundarysetter":
                    BoundarySetter = true;
                    break;
                case "humordeflect":
                    HumorDeflect = true;
                    break;
                case "engagedinspreadsheet":
                    EngagedInSpreadsheet = true;
                    break;
                case "highspend":
                    HighSpend = true;
                    break;
                case "chaosagent":
                    ChaosAgent = true;
                    break;
            }
        }

        private bool ContainsAny(string text, string[] keywords)
        {
            foreach (var keyword in keywords)
            {
                if (text.Contains(keyword))
                {
                    return true;
                }
            }
            return false;
        }

        public List<string> GetTags()
        {
            var tags = new List<string>();

            if (LeftEarly) tags.Add("leftEarly");
            if (StayedTooLong) tags.Add("stayedTooLong");
            if (BoundarySetter) tags.Add("boundarySetter");
            if (HumorDeflect) tags.Add("humorDeflect");
            if (EngagedInSpreadsheet) tags.Add("engagedInSpreadsheet");
            if (HighSpend) tags.Add("highSpend");
            if (ChaosAgent) tags.Add("chaosAgent");

            return tags;
        }

        public string GenerateRumor()
        {
            var rumors = new List<string>();

            if (LeftEarly)
            {
                rumors.Add("I heard you ghost people mid-date.");
                rumors.Add("Word is you're a flight risk.");
                rumors.Add("Someone said you climbed out a bathroom window.");
            }

            if (StayedTooLong)
            {
                rumors.Add("I heard you're... patient.");
                rumors.Add("Word is you stick around even when you shouldn't.");
                rumors.Add("Someone said you're a glutton for punishment.");
            }

            if (BoundarySetter)
            {
                rumors.Add("I heard you have 'boundaries.'");
                rumors.Add("Word is you actually say no to things.");
                rumors.Add("Someone said you're difficult.");
            }

            if (HumorDeflect)
            {
                rumors.Add("I heard you use humor as a defense mechanism.");
                rumors.Add("Word is you can't take anything seriously.");
                rumors.Add("Someone said you're 'funny.'");
            }

            if (HighSpend)
            {
                rumors.Add("I heard you're generous.");
                rumors.Add("Word is money isn't an issue for you.");
                rumors.Add("Someone said you picked up the check without flinching.");
            }

            if (ChaosAgent)
            {
                rumors.Add("I heard you caused a scene.");
                rumors.Add("Word is you're... unpredictable.");
                rumors.Add("Someone said security had to get involved.");
            }

            if (EngagedInSpreadsheet)
            {
                rumors.Add("I heard you understood the algorithm.");
                rumors.Add("Word is you appreciate data-driven approaches.");
                rumors.Add("Someone said you got really into spreadsheets.");
            }

            // Default rumors if no tags
            if (rumors.Count == 0)
            {
                rumors.Add("I haven't heard much about you.");
                rumors.Add("You're kind of a mystery.");
                rumors.Add("Nobody seems to have a read on you yet.");
            }

            // Return random rumor
            return rumors[Random.Range(0, rumors.Count)];
        }
    }
}
