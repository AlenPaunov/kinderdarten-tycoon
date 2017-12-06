using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Character {

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

	public Tile currTile{ get; protected set; }
	Tile destTile; // if we aren`t moving destTile = currTile;
	float movementPercentage;

	float speed = 1f; //tile per sec

	Job myJob;
	Action<Character> cb_CharacterChanged;

	public Character (Tile tile){
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
		if (myJob == null) {
			//grab a job
			myJob = currTile.World.jobQueue.Dequeue ();
		}
		if (myJob != null) {
			destTile = myJob.Tile;
			myJob.RegisterJobCompleteCallBack (OnJobEnded);
			myJob.RegisterJobCancelCallBack	(OnJobEnded);
		}


		if (currTile == destTile) {
			if (myJob!=null) {
				myJob.DoWork (deltaTime);
			}
			return;
		}
			
		//total distance
		float distToTravel = Mathf.Sqrt(Mathf.Pow (currTile.X - destTile.X, 2) + Mathf.Pow (currTile.Y - destTile.Y, 2));
		//
		float distThisFrame = speed * deltaTime;

		float percThisFrame = distThisFrame/distToTravel;

		movementPercentage += percThisFrame;

		if (movementPercentage>=1) {
			currTile = destTile;
			movementPercentage = 0;
			// FIXME => overshotmovement?????
		}

		if (cb_CharacterChanged != null) {
			cb_CharacterChanged (this);
		}
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
