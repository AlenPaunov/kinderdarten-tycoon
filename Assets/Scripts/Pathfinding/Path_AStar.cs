using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Priority_Queue;
using System.Linq;

public class Path_AStar
{

	Queue<Tile> path;

	public Path_AStar (World world, Tile tileStart, Tile tileEnd)
	{

		if (world.PathfindingGraph == null) {
			world.PathfindingGraph = new Path_TileGraph (world);
		}
			
		Dictionary <Tile, Path_Node<Tile>> nodes = world.PathfindingGraph.nodes;
		Path_Node<Tile> start = nodes [tileStart];
		Path_Node<Tile> goal = nodes [tileEnd];


		if (nodes.ContainsKey (tileStart) == false) {
			Debug.Log ("Error, missing tile in pathgrid - INVALID STARTING POS");
		}

		if (nodes.ContainsKey (tileEnd) == false) {
			Debug.Log ("Error, missing tile in pathgrid - INVALID ending POS");
		}

		List<Path_Node<Tile>> ClosedSet = new List<Path_Node<Tile>> ();

		// use priority queue  to enter the start tile
		SimplePriorityQueue<Path_Node<Tile>> OpenedSet = new SimplePriorityQueue<Path_Node<Tile>> ();
		OpenedSet.Enqueue (start, 0);

		Dictionary<Path_Node<Tile>, Path_Node<Tile>> Came_From = new Dictionary<Path_Node<Tile>, Path_Node<Tile>> ();

		Dictionary<Path_Node<Tile>, float> g_score = new Dictionary<Path_Node<Tile>, float> ();
		foreach (var n in nodes.Values) {
			g_score [n] = Mathf.Infinity;
		}
		g_score [start] = 0;

		Dictionary<Path_Node<Tile>, float> f_score = new Dictionary<Path_Node<Tile>, float> ();
		foreach (var n in nodes.Values) {
			f_score [n] = Mathf.Infinity;
		}
		f_score [start] = heuristic_cost_estimate (start, goal);

		while (OpenedSet.Count > 0) {
			Path_Node<Tile> current = OpenedSet.Dequeue ();

			if (current == goal) {
				// goal reached - create a path -> public member 
				reconstruct_path (Came_From, current);
				return;
			}
			ClosedSet.Add (current);

			foreach (Path_Edge<Tile> edge_neighbour in current.Edges) {
				Path_Node<Tile> neighbour = edge_neighbour.Node;

				if (ClosedSet.Contains (neighbour)) {
					continue; // ignore the completed neighbour
				}		

				float	tentative_g_score = g_score [current] + dist_between (current, neighbour);

				if (OpenedSet.Contains(neighbour) && tentative_g_score >= g_score[neighbour]) {
					continue;
				}

				Came_From [neighbour] = current;
				g_score [neighbour] = tentative_g_score;
				f_score [neighbour] = g_score [neighbour] + heuristic_cost_estimate(neighbour, goal);

				if (OpenedSet.Contains(neighbour) == false) {
					OpenedSet.Enqueue (neighbour, f_score [neighbour]);
				}

			} // foreach neighbour
		} // end of loop
		// if we reach here - there is no path
		// we dont have a failstate - implement???

	}

	float dist_between (Path_Node<Tile> a, Path_Node<Tile> b)
	{

		//HORI VERT NEIGHBOURS - 1
		if (Mathf.Abs (a.data.X - b.data.X) + Mathf.Abs (a.data.Y - b.data.Y) == 1) {
			return 1f;
		}

		// DIAGONAL - DIST OF 1.41421356237
		if (Mathf.Abs (a.data.X - b.data.X) == 1 && Mathf.Abs (a.data.Y - b.data.Y) == 1) {
			return 1.41421356237f;
		}

		// do the math
		return Mathf.Sqrt (
			Mathf.Pow (a.data.X - b.data.X, 2) +
			Mathf.Pow (a.data.Y - b.data.Y, 2)
		);

	}

	float heuristic_cost_estimate (Path_Node<Tile> a, Path_Node<Tile> b)
	{
		return Mathf.Sqrt (
			Mathf.Pow (a.data.X - b.data.X, 2) +
			Mathf.Pow (a.data.Y - b.data.Y, 2)
		);
	}

	void reconstruct_path(Dictionary<Path_Node<Tile>, Path_Node<Tile>> Came_From, Path_Node<Tile> current){
		// current is the goal so we walk backwards
		Queue<Tile> total_path = new Queue<Tile>();

		total_path.Enqueue (current.data);

		while (Came_From.ContainsKey(current)) {

			current = Came_From [current];
			total_path.Enqueue (current.data);
		}
		// here total path is a Queue that is running backwards from the end tile to the start tile - reverse
		path = new Queue<Tile>( total_path.Reverse());
	}

	public Tile GetNextTile ()
	{
		return path.Dequeue ();
	}
}
