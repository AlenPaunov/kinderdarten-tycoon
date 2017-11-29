using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Baby {
	Tile currTile;
	Tile destTile;
	float movementPercentage;
	float speed = 1f; //tile per sec

	public float X {
		get{
			return Mathf.Lerp (currTile.X, destTile.X, movementPercentage);
		}
	}

	public float Y {
		get{
			return Mathf.Lerp (currTile.Y, destTile.Y, movementPercentage);
		}
	}

	public Baby (Tile tile){
		currTile = destTile = tile;
	}

	public void SetDestination(Tile tile){
		if (currTile.IsNeighbour(tile) == false) {
			Debug.Log ("Dest tile isn`t neighbour");
		}
		destTile = tile;
	}

	public void Update(float deltaTime){
		// we don`t move;
		if (currTile==destTile) return;
		//total distance
		float distToTravel = Mathf.Pow (currTile.X - destTile.X, 2) + Mathf.Pow (currTile.Y - destTile.Y, 2);
		//
		float distThisFrame = speed * deltaTime;

		float percThisFrame = distToTravel / distThisFrame;

		movementPercentage += percThisFrame;
		if (movementPercentage>=1) {
			currTile = destTile;
			movementPercentage = 0;
			// FIXME => overshotmovement?????
		}

	}

}
