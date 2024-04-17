using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EEGPower {
	private int sampleFrq;
	private int windowSize;

	private struct Band
	{
		public float startF;
		public float endF;
		public string name;

		public Band(float _startF, float _endF, string _name){
			startF = _startF;
			endF = _endF;
			name = _name;
		}
	}
	private List<Band> Bands;

	public int getBandNum{
		get { return Bands.Count; }
	}

	public string[] getBandNames{
		get { 
			string[] bandNames = new string[getBandNum];
			for (int i = 0; i < bandNames.Length; i++) {
				bandNames [i] = Bands [i].name;
			}
			return bandNames;
		}
	}

	public EEGPower(int _sampleFrq, int _windowSize){
		Bands = new List<Band> ();
		sampleFrq = _sampleFrq;
		windowSize = _windowSize;
	}

	public void ClearBand(){
		Bands.Clear ();
	}

	public void AddBand(string BandName, float StartFrq, float EndFrq){
		Bands.Add (new Band (StartFrq, EndFrq, BandName));
	}

	public float[] GetBandPw(double[] pw){
		float[] powerRatio = new float[Bands.Count];
		float frqPerindex = (float)sampleFrq / windowSize;
		for (int i = 0; i < Bands.Count; i++) {
			int startIndex = (int)(Bands [i].startF / frqPerindex);
			int endIndex = (int)(Bands [i].endF / frqPerindex);

			double bandTotal = 0d;
			for (int j = startIndex; j < endIndex + 1; j++) {
				bandTotal += pw [j];
			}
			powerRatio [i] = (float)bandTotal;
		}

		float bandtotal = 0f;
		for (int i = 0; i < powerRatio.Length; i++) {
			bandtotal += powerRatio [i];
		}
		for (int i = 0; i < powerRatio.Length; i++) {
			powerRatio [i] = powerRatio [i]/bandtotal;
		}
		return powerRatio;
	}
}