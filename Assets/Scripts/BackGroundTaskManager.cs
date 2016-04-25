using UnityEngine;
using System.Collections;
using System;



// A delegate setup for adding IEnumerator methods as background tasks.
public delegate IEnumerator TaskToPerform (TaskStatus taskStatus);

// Basic status class to be manipulated by a background task.
public class TaskStatus {

	public bool isDone;
	public TaskStatus (bool done) {
		isDone = done;
	}
}

// A delegate setup for to be called when a task is done, passing back its taskStatus to provide info.
// could be more generic to tie into the overall messaging system?

public delegate void OnCompletionTask (TaskStatus taskStatus);



public class BackGroundTaskManager : MonoBehaviour
{
	// This handles background tasks, using IEnumerator / coroutines.
	// IEnumerator delegates can be added to a tasklist. These must implement setting the passed-in taskStatus to true upon completion.
	// On update the manager checks if any task was completed and removes it from the list.


	ArrayList backGroundTasks;
	ArrayList backGroundTasksStatus;
	ArrayList onCompletionTasks;

	TaskToPerform task01;
	TaskStatus task01status;


	void Start ()
	{
		backGroundTasks = new ArrayList ();
		backGroundTasksStatus = new ArrayList ();
		onCompletionTasks = new ArrayList ();
	}

	void Update ()
	{
		int i = 0;

		while (i < backGroundTasksStatus.Count) {
			TaskStatus theStatus = (TaskStatus) backGroundTasksStatus [i];
			if (theStatus.isDone) {
				Debug.Log ("BackGroundTaskManager: task done");

				OnCompletionTask theCompletionTask = (OnCompletionTask) onCompletionTasks [i];
				theCompletionTask (theStatus);

				backGroundTasks.RemoveAt (i);
				backGroundTasksStatus.RemoveAt (i);
				onCompletionTasks.RemoveAt (i);

			} else {
				i++;
			}

		}

	}


	public void addTask (TaskToPerform newTask, OnCompletionTask newCompletionTask)
	{
		Debug.Log ("BackGroundTaskManager: adding task");
		TaskStatus newStatus = new TaskStatus (false);

	

		backGroundTasks.Add (newTask);
		backGroundTasksStatus.Add (newStatus);
		onCompletionTasks.Add (newCompletionTask);
		StartCoroutine (newTask (newStatus));


	}



}


