using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum DetectorMode{
	MinDetection,
	MaxDetection
}

public class PeakDetector {
	private int sampleFrq;
	private DetectorMode mode;
	private Queue<float> linearBpm;
	private Queue<KeyValuePair<uint, float>> peackbuff;

	private bool isFollow;
	private float threshold;
	private float slope;

	private uint clampMin;
	private uint clampMax;

	private uint sampleCount;
	private uint currPeakSampleNum;
	private uint prePeakSampleNum;
	private float prePeakValue;
	private float preBpm;

	public PeakDetector(Queue<float> _linearBpm, Queue<KeyValuePair<uint, float>> _peackbuff, int _sampleFrq){
		linearBpm = _linearBpm;
		sampleFrq = _sampleFrq;
		peackbuff = _peackbuff;

		mode = DetectorMode.MaxDetection;

		SetClampWithBpm (40f, 200f);
		Reset ();
	}

	private void SetClampWithBpm(float min, float max){
		clampMin = (uint)(60f * sampleFrq / max);
		clampMax = (uint)(60f * sampleFrq / min);
	}

	public void Reset(){
		isFollow = true;
		threshold = 0f;
		slope = 0f;
		sampleCount = 0;
		currPeakSampleNum = 0;
	}

	public float ApplyAndGetThreshold(float data){
		sampleCount++;
		if(mode == DetectorMode.MaxDetection){
			if (isFollow) {
				if (threshold < data) {
					threshold = data;
				} else {
					slope = Mathf.Abs (data * 0.01f);

					currPeakSampleNum = sampleCount;
					isFollow = false;
				}
			} else {
				threshold -= slope;
				if (threshold < data) {
					isFollow = true;
				}

				if (sampleCount - currPeakSampleNum > clampMax) {
					threshold = data;
					isFollow = true;
				}
			}

			if (sampleCount - currPeakSampleNum > clampMin) {
				if (prePeakSampleNum != currPeakSampleNum) {
                    Debug.Log(currPeakSampleNum + " - " + prePeakSampleNum);
                    if (prePeakSampleNum > currPeakSampleNum)
                        prePeakSampleNum = 0;
                    uint nnInterval = currPeakSampleNum - prePeakSampleNum;
                    float bpm = (60f * sampleFrq) / nnInterval;

//					Debug.LogFormat ("{0} {1} {2}", prePeakSampleNum, currPeakSampleNum, bpm);
					
					if (preBpm == 0)
						preBpm = bpm;
					for (int i = 0; i < nnInterval; i++) {
						linearBpm.Enqueue (Mathf.Lerp (preBpm, bpm, (float)i / nnInterval));
					}
					
					peackbuff.Enqueue (new KeyValuePair<uint, float> (sampleCount, data));
                    float averageBPM = (preBpm + bpm) / 2;
					preBpm = bpm;
					
					prePeakSampleNum = currPeakSampleNum;
				}
			}
		}
		return threshold;
	}


}