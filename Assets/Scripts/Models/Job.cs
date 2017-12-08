using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Job {
	
	public Tile Tile{ get; protected set;}
	float jobTime;

	public string jobObjectType { get; protected set;}

	Action<Job> cbJobComplete;
	Action<Job> cbJobCancel;

	public Job(Tile tile, string jobObjType, Action<Job> cbJobComplete, float jobTime = 1f){
		this.Tile = tile;
		this.jobObjectType = jobObjType;
		this.cbJobComplete += cbJobComplete;
		this.jobTime = jobTime;
	}

	public void RegisterJobCompleteCallBack(Action<Job> callback){
		this.cbJobComplete += callback;
	}

	public void UnregisterJobCompleteCallBack(Action<Job> callback){
		this.cbJobComplete -= callback;
	}

	public void RegisterJobCancelCallBack(Action<Job> callback){
		this.cbJobCancel += callback;
	}

	public void UnregisterJobCancelCallBack(Action<Job> callback){
		this.cbJobCancel -= callback;
	}

	public void DoWork(float workTime){
		jobTime -= workTime;

		if (jobTime<=0) {
			if (cbJobComplete!=null) {
				cbJobComplete(this);
			}
		}
	}

	public void CancelJob(){
		if (cbJobCancel!=null) {
			cbJobCancel(this);
		}
	}


}
