using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class JobQueue {
	
	Queue<Job> jobQueue;

	Action<Job> cb_JobCreated;

	public JobQueue(){

		jobQueue = new Queue<Job> ();
	}

	public void Enqueue(Job j){
		jobQueue.Enqueue (j);

		if (cb_JobCreated != null) {
			cb_JobCreated (j);
		}
	}

	public Job Dequeue(){
		if (jobQueue.Count == 0) {
			return null;
		}
		return jobQueue.Dequeue ();
	}

	public void RegisterJobCreationCallback(Action<Job> cb){
		cb_JobCreated += cb;
	}

	public void UnregisterJobCreationCallback(Action<Job> cb){
		cb_JobCreated -= cb;
	}
}
