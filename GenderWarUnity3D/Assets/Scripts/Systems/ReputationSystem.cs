using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Tracks player reputation based on choices across all dates
/// </summary>
public class ReputationSystem : MonoBehaviour
{
    public bool LeftEarly { get; private set; }
    public bool StayedTooLong { get; private set; }
    public bool BoundarySetter { get; private set; }
    public bool HumorDeflect { get; private set; }
    public bool EngagedInSpreadsheet { get; private set; }
    public bool HighSpend { get; private set; }
    public bool ChaosAgent { get; private set; }

    private int boundaryCount = 0;
    private int humorCount = 0;
    private int engagementCount = 0;
    private int chaosCount = 0;

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

        if (ContainsAny(label, earlyExitKeywords) || nodeId.Contains("leave") || nodeId.Contains("exit"))
            LeftEarly = true;

        if (ContainsAny(label, stayKeywords) || nodeId.Contains("stay"))
            StayedTooLong = true;

        if (ContainsAny(label, boundaryKeywords))
        {
            boundaryCount++;
            if (boundaryCount >= 2) BoundarySetter = true;
        }

        if (ContainsAny(label, humorKeywords))
        {
            humorCount++;
            if (humorCount >= 2) HumorDeflect = true;
        }

        if (ContainsAny(label, spreadsheetKeywords) || nodeId.Contains("algorithm") || nodeId.Contains("spreadsheet"))
        {
            engagementCount++;
            if (engagementCount >= 1) EngagedInSpreadsheet = true;
        }

        if (ContainsAny(label, chaosKeywords))
        {
            chaosCount++;
            if (chaosCount >= 1) ChaosAgent = true;
        }

        if (entry.Effects != null && entry.Effects.MoneyChange <= -40)
            HighSpend = true;

        if (entry.Effects?.Tags != null)
        {
            foreach (var tag in entry.Effects.Tags)
                ApplyTag(tag);
        }
    }

    private void ApplyTag(string tag)
    {
        switch (tag.ToLower())
        {
            case "leftearly": LeftEarly = true; break;
            case "stayedtoolong": StayedTooLong = true; break;
            case "boundarysetter": BoundarySetter = true; break;
            case "humordeflect": HumorDeflect = true; break;
            case "engagedinspreadsheet": EngagedInSpreadsheet = true; break;
            case "highspend": HighSpend = true; break;
            case "chaosagent": ChaosAgent = true; break;
        }
    }

    private bool ContainsAny(string text, string[] keywords)
    {
        foreach (var keyword in keywords)
            if (text.Contains(keyword)) return true;
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

        if (LeftEarly) rumors.Add("I heard you ghost people mid-date.");
        if (StayedTooLong) rumors.Add("I heard you're... patient.");
        if (BoundarySetter) rumors.Add("I heard you have 'boundaries.'");
        if (HumorDeflect) rumors.Add("I heard you use humor as a defense mechanism.");
        if (HighSpend) rumors.Add("I heard you're generous.");
        if (ChaosAgent) rumors.Add("I heard you caused a scene.");
        if (EngagedInSpreadsheet) rumors.Add("I heard you understood the algorithm.");

        if (rumors.Count == 0) rumors.Add("I haven't heard much about you.");

        return rumors[Random.Range(0, rumors.Count)];
    }
}
