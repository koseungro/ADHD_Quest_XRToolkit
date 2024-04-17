using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Timers;

public class SignalMaker {
	private static Queue<float> output;
	private Timer interruptTimer;
	private static float[] signalArray;

	public enum WaveType{
		sin, rect, impulse, noise
	};

	public SignalMaker(float freq, float amp, int samplefreq, WaveType type, Queue<float> _output){
		output = _output;

		double timerTime = 1000d/samplefreq;
		signalArray = new float[(int)(samplefreq/freq)];

		for (int i = 0; i < signalArray.Length; i++) {
			switch (type) {
			//삼각파
			case WaveType.sin:
				signalArray [i] = Mathf.Sin (2 * Mathf.PI * i/ signalArray.Length) * amp;
				break;
			//사각파
			case WaveType.rect:
				signalArray [i] = (i > signalArray.Length/2)? amp : -1*amp;
				break;
			//임펄스파
			case WaveType.impulse:
				signalArray [i] = (i == 0) ? amp : 0;
				break;
			case WaveType.noise:
				signalArray [i] = Random.Range (-1 * amp, amp);
				break;
			default:
				break;
			}
		}


		interruptTimer = new Timer (timerTime);
		interruptTimer.Elapsed += OnTimedEvent;
		interruptTimer.AutoReset = true;
		interruptTimer.Enabled = false;
		signalPointer = 0;
	}

	public void Start(){
		interruptTimer.Enabled = true;
	}

	public void Stop(){
		interruptTimer.Enabled = false;
	}


	private static int signalPointer;
	private static void OnTimedEvent(object source, ElapsedEventArgs e){
		lock (output) {
			output.Enqueue (signalArray [signalPointer]);
		}
		signalPointer++;
		signalPointer = signalPointer % signalArray.Length;
	}

	public virtual void Dispose(){
		interruptTimer.Enabled = false;
		interruptTimer.Dispose ();
	}
}