using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path_TileGraph {

	public Dictionary<Tile,Path_Node<Tile>> nodes; 

	// Initializes a new instance of the <see cref="Path_TileGraph"/> class.
	public Path_TileGraph(World world){
		
		nodes = new Dictionary<Tile, Path_Node<Tile>> ();
		//loop the tiles - ncrate a node
		for (int x = 0; x < world.Width; x++) {
			for (int y = 0; y < world.Height; y++) {

				Tile t = world.GetTileAt (x, y);

				//if (t.movementCost > 0) {
					Path_Node<Tile> n = new Path_Node<Tile> ();
					n.data = t;
					nodes.Add (t, n);
				//}
			}
		}

		//loop the tiles - create edges
		foreach (Tile t in nodes.Keys) {
			Path_Node<Tile> n = nodes [t];

			List<Path_Edge<Tile>> edges = new List<Path_Edge<Tile>> ();
			//get list of neighbours
			Tile [] neighbours = t.GetNeighbours(true); // check if null
				
			for (int i = 0; i < neighbours.Length; i++) {
				Tile nbTile = neighbours [i];
				if (nbTile != null && nbTile.movementCost > 0 ) { // is walkable SO CREATE EDGE

					if (isClippingCorner(t, nbTile)) {
						continue;
					}

					Path_Edge<Tile> e = new Path_Edge<Tile>();
					e.Cost = nbTile.movementCost;
					e.Node = nodes [nbTile];
					//ADD THE EDGE TO TEMP LIST
					edges.Add (e);
				}
			}

			n.Edges = edges.ToArray ();

		}
	}

	bool isClippingCorner(Tile curr, Tile neighbour)
	{
		if (Mathf.Abs(curr.X - neighbour.X) + Mathf.Abs(curr.Y - neighbour.Y) == 2) {
			//we are diagonal
			int dX = curr.X - neighbour.X;
			int dY = curr.Y - neighbour.Y;

			if (curr.World.GetTileAt(curr.X - dX, curr.Y).movementCost == 0) {
				// East or west is unwalkable
				return true;
			}

			if (curr.World.GetTileAt(curr.X, curr.Y - dY).movementCost == 0) {
				// nORTH or south is unwalkable;
				return true;
			}
		}

		return false;
	}
}
