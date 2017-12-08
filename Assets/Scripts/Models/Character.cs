using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Character {

	public float X {
		get{
			return Mathf.Lerp (currTile.X, nextTile.X, movementPercentage);
		}
	}

	public float Y {
		get{
			return Mathf.Lerp (currTile.Y, nextTile.Y, movementPercentage);
		}
	}

	public Tile currTile{ get; protected set; }
	Tile destTile; // if we aren`t moving destTile = currTile;
	Tile nextTile; // The next in the path from pathfinding;
	Path_AStar path;

	float movementPercentage;

	float speed = 2f; //tile per sec

	Job myJob;
	Action<Character> cb_CharacterChanged;

	public Character (Tile tile){
		currTile = destTile = nextTile = tile;
	}

	public void SetDestination(Tile tile){
		if (currTile.IsNeighbour(tile) == false) {
			Debug.Log ("Dest tile isn`t neighbour");
		}
		destTile = tile;
	}

	public void Update(float deltaTime){

		Update_DoJob (deltaTime);
		Update_Movement (deltaTime);

		if (cb_CharacterChanged != null) {
			cb_CharacterChanged (this);
		}

	}

	void Update_DoJob (float deltaTime)
	{
		// we don`t move;
		if (myJob == null) {
			//grab a job
			myJob = currTile.World.jobQueue.Dequeue ();
		}
		if (myJob != null) {

			// CHECK TO SEE IF JOB IS REACHABLE
			destTile = myJob.Tile;
			myJob.RegisterJobCompleteCallBack (OnJobEnded);
			myJob.RegisterJobCancelCallBack (OnJobEnded);
		}
		if (currTile == destTile) {
			if (myJob != null) {
				myJob.DoWork (deltaTime);
			}
		}

	}

	void Update_Movement (float deltaTime)
	{
		
		if (currTile == destTile) {
			path = null;	
			return; // we are where we want to be;
		}
		if (nextTile == null || nextTile == currTile) {
			//pathfind next tile
			if (path == null || path.Lenght() == 0) {
				path = new Path_AStar (currTile.World, currTile, destTile);
				if (path.Lenght() == 0) {
					Debug.Log ("no path to dest");
					//FIXME job maybe must be reenqued instead;
					AbandonJob();
					//FIXME cancel Job;
					return;
				}
			}
			//grab next tile
			nextTile = path.DequeueNextTile();
			if (nextTile == currTile) {
				Debug.LogError ("Update movement - next tile is currtile");
			}
		}
			
		//total distance
		float distToTravel = Mathf.Sqrt (Mathf.Pow (currTile.X - nextTile.X, 2) + Mathf.Pow (currTile.Y - nextTile.Y, 2));
		//
		float distThisFrame = speed * deltaTime;
		float percThisFrame = distThisFrame / distToTravel;
		movementPercentage += percThisFrame;
		if (movementPercentage >= 1) {
			currTile = nextTile;
			movementPercentage = 0;
			// FIXME => overshotmovement?????
		}

	}
		
	void AbandonJob(){
		nextTile = destTile = currTile;
		path = null;
		// FIXME BLUEPRINT CONTROLLER 25, JOBQUEUE 21; CHARACTER 68/85/115; WORLD 52; WORLDCOMTROLLER 26 - ELEMENT WITH THE SAME KEY ALLREADY EXISTS
		currTile.World.jobQueue.Enqueue (myJob);
		myJob = null;
	}

	public void RegisterCharacterChangedCallback(Action<Character> cb){
		cb_CharacterChanged += cb;
	}

	public void UnregisterCharacterChangedCallback(Action<Character> cb){
		cb_CharacterChanged -= cb;
	}

	void OnJobEnded(Job j){
		//completed or canceled
		if (j!=myJob) {
			//isnt mine job
			return;
		}

		myJob = null;
	}
}
