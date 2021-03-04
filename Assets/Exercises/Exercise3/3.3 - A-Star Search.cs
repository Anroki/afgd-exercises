using System;
using System.Collections.Generic;
using UnityEngine;

namespace AfGD.Execise3
{
    public static class AStarSearch
    {
        // Exercise 3.3 - Implement A* search
        // Explore the graph and fill the _cameFrom_ dictionairy with data using uniform cost search.
        // Similar to Exercise 3.1 PathFinding.ReconstructPath() will use the data in cameFrom  
        // to reconstruct a path between the start node and end node. 
        //
        // Notes:
        //      Use the data structures used in Exercise 3.1 and 3.2
        //
        public static void Execute(Graph graph, Node start, Node goal, Dictionary<Node, Node> cameFrom)
        {
            PriorityQueue<Node> frontier = new PriorityQueue<Node>();
            List<Node> neighbours = new List<Node>();
            Dictionary<Node, float> exploredCost = new Dictionary<Node, float>();
            Dictionary<Node, float> heuristicCost = new Dictionary<Node, float>();

            frontier.Enqueue(start, 0);
            exploredCost[start] = 0;
            heuristicCost[start] = Vector3.Distance(start.Position, goal.Position);
            cameFrom.Add(start, null);

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