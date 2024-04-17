using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Statistics {
	public List<float> data;

	public Statistics(){
		data = new List<float>();
	}

	public void Clear(){
		data.Clear ();
	}

	public void AddData(float aData){
		data.Add (aData);
	}

	public float GetStd(){
		float avr = GetAvr ();
		float sumOfSquaresOfDifferences = data.Sum (val => (val - avr) * (val - avr));
		return Mathf.Sqrt (sumOfSquaresOfDifferences / data.Count ());
	}

	public float GetRmssd(){
		float sumOfRootOfDifferences = 0f;
		for (int i = 1; i < data.Count; i++) {
			float diff = data [i - 1] - data [i];
			sumOfRootOfDifferences += (diff * diff);
		}
		return Mathf.Sqrt(sumOfRootOfDifferences / (data.Count () - 1f));
	}

	public float GetAvr(){
		return data.Average ();
	}
}