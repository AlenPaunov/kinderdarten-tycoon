using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Job {
	
	public Tile tile{ get; protected set;}
	float jobTime;

	Action<Job> cbJobComplete;
	Action<Job> cbJobCancel;

	public Job(Tile tile,  Action<Job> cbJobComplete, float jobTime = 1f){
		this.tile = tile;
		this.jobTime = jobTime;
		this.cbJobComplete += cbJobComplete;
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

	public void RegisterJobCompleteCallBack(Action<Job> callback){
		this.cbJobComplete += callback;
	}

	public void UnregisterJobCompleteCallBack(Action<Job> callback){
		this.cbJobComplete -= callback;
	}

	public void RegisterJobCancelCallBack(Action<Job> callback){
		this.cbJobComplete += callback;
	}

	public void UnregisterJobCancelCallBack(Action<Job> callback){
		this.cbJobComplete -= callback;
	}
}
