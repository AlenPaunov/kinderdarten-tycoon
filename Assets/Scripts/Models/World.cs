using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World {
	Tile[,] tiles;
	List<Baby> babies;
	public int Width{ get; protected set; }
	public int Height{ get; protected set; }

	/// <summary>
	/// Initializes a new instance of the World class.
	/// </summary>
	/// <param name="width">Width.</param>
	/// <param name="height">Height.</param>
	public World(int width = 20, int height = 20){
		this.Width = width;
		this.Height = height;

		tiles = new Tile[width, height];
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				tiles [x, y] = new Tile (this, x, y);
			}
		}
		babies = new List<Baby>();
		Baby a = new Baby(tiles[Width/2,Height/2]);
	}

	/// <summary>
	/// Gets the tile at x and y.
	/// </summary>
	/// <returns>The <see cref="Tile"/>.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public Tile GetTileAt(int x, int y) {
		if (x>Width||x<0||y>Height||y<0) {
			//Debug.LogError ("wrong coordinates for a tile" + x + " " + y);
			return null;
		}
		return tiles [x, y];
	}
		
	/// <summary>
	/// Randomizes the tiles.
	/// </summary>
	public void RandomizeTiles(){
		for (int x = 0; x < Width; x++) {
			for (int y = 0; y < Height; y++) {
				int random = Random.Range (0, 3);
				if ( random == 0) {
					tiles [x, y].Type = Tile.TileType.Soil;
				} else if (random == 1) {
					tiles [x, y].Type = Tile.TileType.Sand;
				} else if (random  == 2) {
					tiles [x, y].Type = Tile.TileType.RoughStone;
				} else if (random == 3) {
					tiles [x, y].Type = Tile.TileType.Gravel;
				} else {
					Debug.LogError ("Error in setting type of tyle with random = " + random);
				}

			}
		}
	}

}
