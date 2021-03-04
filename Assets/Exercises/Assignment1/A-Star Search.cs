using System;
using System.Collections.Generic;
using UnityEngine;
using AfGD.Execise3;

namespace AfGD.Assignment1
{
    public static class AStarSearch
    {
        public static void Execute(Graph graph, Node start, Node goal, Dictionary<Node, Node> cameFrom)
        {
            PriorityQueue<Node> frontier = new PriorityQueue<Node>();
            List<Node> neighbours = new List<Node>();
            Dictionary<Node, float> exploredCost = new Dictionary<Node, float>();
            Dictionary<Node, float> heuristicCost = new Dictionary<Node, float>();

            frontier.Enqueue(start, 0);
            exploredCost[start] = 0;
            heuristicCost[start] = Vector3.Distance(start.Position, goal.Position);
            //cameFrom.Add(start, null);

            while (frontier.Count > 0)
            {
                Node current = frontier.Dequeue();
                if (current == goal) break;
                graph.GetNeighbours(current, neighbours);

                foreach (Node next in neighbours)
                {
                    float cost = exploredCost[current] + graph.GetCost(current, next);
                    if (!exploredCost.ContainsKey(next) || cost < exploredCost[next])
                    {
                        float heuCost = Vector3.Distance(next.Position, goal.Position);
                        heuCost += cost;
                        heuristicCost[next] = heuCost;
                        exploredCost[next] = cost;
                        frontier.Enqueue(next, heuCost);
                        cameFrom[next] = current;
                    }
                }
            }
        }
    }
}