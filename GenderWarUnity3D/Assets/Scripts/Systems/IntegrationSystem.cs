using UnityEngine;
using System.Collections.Generic;
using GenderWar.Core;

namespace GenderWar.Systems
{
    /// <summary>
    /// Handles Part 3 integration calculations - determines if player is "accepted" by the group
    /// Based on legibility, friction, and reframing acceptance metrics
    /// </summary>
    [System.Serializable]
    public class IntegrationSystem
    {
        // Integration metrics
        public int Legibility { get; private set; }
        public int Friction { get; private set; }
        public int ReframingAcceptance { get; private set; }

        public void Reset()
        {
            Legibility = 0;
            Friction = 0;
            ReframingAcceptance = 0;
        }

        public IntegrationResult Calculate(List<string> tags)
        {
            Reset();

            // Calculate Legibility (how predictable/categorizable the player is)
            // Higher is better for acceptance
            if (tags.Contains("stayedTooLong"))
            {
                Legibility += 2; // Consistent engagement
            }
            if (tags.Contains("engagedInSpreadsheet"))
            {
                Legibility += 1; // Engaged with frameworks
            }
            if (tags.Contains("humorDeflect"))
            {
                Legibility -= 2; // Defense mechanism = hard to read
            }
            if (tags.Contains("chaosAgent"))
            {
                Legibility -= 2; // Unpredictable
            }

            // Calculate Friction (how costly the player is to process)
            // Lower is better for acceptance
            if (tags.Contains("boundarySetter"))
            {
                Friction += 2; // Disruptive
            }
            if (tags.Contains("leftEarly"))
            {
                Friction += 1; // Rejected the process
            }
            if (tags.Contains("chaosAgent"))
            {
                Friction += 1; // Created problems
            }
            if (tags.Contains("stayedTooLong"))
            {
                Friction -= 1; // Compliant
            }
            if (tags.Contains("engagedInSpreadsheet"))
            {
                Friction -= 1; // Engaged
            }
            if (tags.Contains("highSpend"))
            {
                Friction -= 1; // Invested resources
            }

            // Calculate Reframing Acceptance (willingness to surrender narrative control)
            // Higher is better for acceptance
            if (tags.Contains("stayedTooLong"))
            {
                ReframingAcceptance += 2; // Let things happen
            }
            if (tags.Contains("boundarySetter"))
            {
                ReframingAcceptance -= 1; // Resisted framing
            }
            if (tags.Contains("leftEarly"))
            {
                ReframingAcceptance -= 2; // Rejected the whole thing
            }
            if (tags.Contains("engagedInSpreadsheet"))
            {
                ReframingAcceptance += 1; // Accepted external frameworks
            }

            return GetResult();
        }

        private IntegrationResult GetResult()
        {
            var result = new IntegrationResult
            {
                Legibility = this.Legibility,
                Friction = this.Friction,
                ReframingAcceptance = this.ReframingAcceptance
            };

            // Acceptance criteria:
            // - At least 2 of 3 criteria must be met:
            //   1. Legibility >= 0
            //   2. Friction <= 1
            //   3. Reframing >= 1
            // - No extreme negatives
            // - At least one positive axis

            int criteriamet = 0;
            if (Legibility >= 0) criteriamet++;
            if (Friction <= 1) criteriamet++;
            if (ReframingAcceptance >= 1) criteriamet++;

            bool hasExtremeNegative = Legibility <= -3 || Friction >= 4 || ReframingAcceptance <= -3;
            bool hasPositive = Legibility > 0 || Friction < 0 || ReframingAcceptance > 0;

            result.Accepted = criteriamet >= 2 && !hasExtremeNegative && hasPositive;
            result.ResultText = GenerateResultText(result);

            return result;
        }

        private string GenerateResultText(IntegrationResult result)
        {
            if (result.Accepted)
            {
                if (Legibility >= 2)
                {
                    return "You're very readable. We like that. You fit our patterns.";
                }
                if (Friction <= -1)
                {
                    return "You don't cause problems. That's valuable here.";
                }
                if (ReframingAcceptance >= 2)
                {
                    return "You let us tell the story. That's all we ask.";
                }
                return "You meet our criteria. Welcome to the group.";
            }
            else
            {
                if (Legibility <= -2)
                {
                    return "You're too unpredictable. We can't categorize you.";
                }
                if (Friction >= 3)
                {
                    return "You create too much friction. You're not worth the effort.";
                }
                if (ReframingAcceptance <= -2)
                {
                    return "You won't let us tell your story. That's a dealbreaker.";
                }
                return "You don't fit our criteria. It's not personal. It's systemic.";
            }
        }
    }

    [System.Serializable]
    public class IntegrationResult
    {
        public int Legibility;
        public int Friction;
        public int ReframingAcceptance;
        public bool Accepted;
        public string ResultText;

        public string GetDetailedBreakdown()
        {
            return $"INTEGRATION ASSESSMENT\n" +
                   $"---------------------\n" +
                   $"Legibility: {Legibility} {(Legibility >= 0 ? "✓" : "✗")}\n" +
                   $"  (How predictable you are)\n\n" +
                   $"Friction: {Friction} {(Friction <= 1 ? "✓" : "✗")}\n" +
                   $"  (How costly you are to process)\n\n" +
                   $"Reframing: {ReframingAcceptance} {(ReframingAcceptance >= 1 ? "✓" : "✗")}\n" +
                   $"  (Your willingness to be defined)\n\n" +
                   $"VERDICT: {(Accepted ? "ACCEPTED" : "REJECTED")}";
        }
    }
}
