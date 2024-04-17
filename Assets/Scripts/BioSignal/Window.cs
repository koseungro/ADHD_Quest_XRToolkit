using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window<T>{
	private Queue<T> windowQ;
	private Queue<T[]> WindowsQ;
	private int overlapLength;
	private int overlapCounter;
	private int length;

	public Window(int _length, float overlapPercent){
		length = _length;
		overlapLength = (int)(length * (1 - overlapPercent * 0.01f));
		overlapCounter = 0;

		windowQ = new Queue<T> ();
		WindowsQ = new Queue<T[]>();
	}

	public T[] GetCurrentWindow(){
		return windowQ.ToArray ();
	}

	public T[][] GetWindows(){
		T[][] returnT = WindowsQ.ToArray ();
		WindowsQ.Clear ();
		return returnT;
	}

	public void InputData(T chData){
		if (windowQ.Count < length) {
			windowQ.Enqueue (chData);
			return;
		}
		windowQ.Enqueue(chData);
		windowQ.Dequeue ();

		overlapCounter++;
		if (overlapCounter > overlapLength) {
			overlapCounter = 0;
			WindowsQ.Enqueue (windowQ.ToArray ());
		}
	}
}
