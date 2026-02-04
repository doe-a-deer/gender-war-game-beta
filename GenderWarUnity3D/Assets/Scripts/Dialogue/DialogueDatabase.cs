using UnityEngine;
using System.Collections.Generic;

namespace GenderWar.Dialogue
{
    /// <summary>
    /// ScriptableObject containing all dialogue nodes for a route
    /// </summary>
    [CreateAssetMenu(fileName = "New Dialogue Route", menuName = "Gender War/Dialogue Database")]
    public class DialogueDatabase : ScriptableObject
    {
        [Header("Route Info")]
        public string RouteName;
        public Core.RouteType RouteType;

        [Header("Dialogue Nodes")]
        public List<DialogueNode> Nodes = new List<DialogueNode>();

        private Dictionary<string, DialogueNode> nodeCache;

        public void OnEnable()
        {
            BuildCache();
        }

        public void BuildCache()
        {
            nodeCache = new Dictionary<string, DialogueNode>();
            foreach (var node in Nodes)
            {
                if (!string.IsNullOrEmpty(node.Id))
                {
                    nodeCache[node.Id] = node;
                }
            }
        }

        public DialogueNode GetNode(string nodeId)
        {
            if (nodeCache == null) BuildCache();

            if (nodeCache.TryGetValue(nodeId, out DialogueNode node))
            {
                return node;
            }
            return null;
        }

        public DialogueNode GetStartNode()
        {
            return GetNode("start");
        }

        public List<DialogueNode> GetAllEndings()
        {
            var endings = new List<DialogueNode>();
            foreach (var node in Nodes)
            {
                if (node.IsEnding)
                {
                    endings.Add(node);
                }
            }
            return endings;
        }
    }
}
