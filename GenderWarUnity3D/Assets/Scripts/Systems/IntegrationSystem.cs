using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Handles Part 3 integration calculations
/// </summary>
public class IntegrationSystem : MonoBehaviour
{
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
        if (tags == null) tags = new List<string>();

        if (tags.Contains("stayedTooLong")) Legibility += 2;
        if (tags.Contains("engagedInSpreadsheet")) Legibility += 1;
        if (tags.Contains("humorDeflect")) Legibility -= 2;
        if (tags.Contains("chaosAgent")) Legibility -= 2;

        if (tags.Contains("boundarySetter")) Friction += 2;
        if (tags.Contains("leftEarly")) Friction += 1;
        if (tags.Contains("chaosAgent")) Friction += 1;
        if (tags.Contains("stayedTooLong")) Friction -= 1;
        if (tags.Contains("engagedInSpreadsheet")) Friction -= 1;
        if (tags.Contains("highSpend")) Friction -= 1;

        if (tags.Contains("stayedTooLong")) ReframingAcceptance += 2;
        if (tags.Contains("boundarySetter")) ReframingAcceptance -= 1;
        if (tags.Contains("leftEarly")) ReframingAcceptance -= 2;
        if (tags.Contains("engagedInSpreadsheet")) ReframingAcceptance += 1;

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
            if (Legibility >= 2) return "You're very readable. We like that. You fit our patterns.";
            if (Friction <= -1) return "You don't cause problems. That's valuable here.";
            if (ReframingAcceptance >= 2) return "You let us tell the story. That's all we ask.";
            return "You meet our criteria. Welcome to the group.";
        }
        else
        {
            if (Legibility <= -2) return "You're too unpredictable. We can't categorize you.";
            if (Friction >= 3) return "You create too much friction. You're not worth the effort.";
            if (ReframingAcceptance <= -2) return "You won't let us tell your story. That's a dealbreaker.";
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
}
