using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Filter {
	private int chNum;
	private FilterConstants FIlter_bp;
	private FilterConstants[] filters;

	public Filter(string filterFilePath, int _chNum){
		chNum = _chNum;
		SetFiletFile (filterFilePath);
		FIlter_bp = new FilterConstants(new double[1]{1d}, new double[1]{1d},"null","null");
		filter_reset ();
	}

	public void SetFiletFile(string filterFilePath){
		filters = GetFilterFormTxt (filterFilePath);
	}

	//1-50 Hz band pass filter, 2nd Order Butterworth: [b, a] = butter(2,[1.0 50.0]/(fs_Hz / 2.0))
	public void SetFilter(int index){
		if(filters == null){
			Debug.LogError("Get filters before set filte");
			return;
		}

		FIlter_bp = filters[index];
		filter_reset ();
	}

	public string[] GetfilterNames(){
		if(filters == null){
			Debug.LogError("Get filters before set filte");
			return new string[]{ };
		}
		int len = filters.Length;

		string[] names = new string[len];

		for (int i = 0; i < len; i++) {
			names [i] = filters [i].name;
		}

		return names;
	}

	private FilterConstants[] GetFilterFormTxt(string filePath){
		if (!File.Exists (filePath))
			Debug.LogError ("Can't get filter txt. Not exists this file : " + filePath);

		string[] lines = File.ReadAllLines (filePath);

		if (lines.Length % 4 != 0) {
			Debug.LogError ("File Not match!!");
			return null;
		}

		int leng = lines.Length/4;
		FilterConstants[] filters = new FilterConstants[leng];

		for (int i = 0; i < leng; i++) {
			string[] bStr = lines [i * 4 + 2].Split ('\t');
			string[] aStr = lines [i * 4 + 3].Split ('\t');

			if (bStr.Length != aStr.Length) {
				Debug.LogError ("Can't Get filters form txt. the value num is not match");
				return null;
			}

			int valueLen = bStr.Length;
			double[] b = new double[valueLen];
			double[] a = new double[valueLen];

			for (int j = 0; j < valueLen; j++) {
				b [j] = double.Parse (bStr [j]);
				a [j] = double.Parse (aStr [j]);
			}

			filters [i].name = lines [i * 4];
			filters [i].short_name = lines [i * 4 + 1];
			filters [i].b = b;
			filters [i].a = a;
		}

		return filters;
	}

	public string GetCurrentFilterName(){
		return FIlter_bp.name;
	}

	private double[,] y;
	private double[,] x;
	private int ab_num;

	public void filter(double[] data){
		for(int i=0; i<chNum; i++){
			x[i, 0] = data[i];
			y[i, 0] = 0;
			for (int j=0; j<ab_num; j++){
				y[i, 0] += FIlter_bp.b[j] * x[i, j] - FIlter_bp.a[j] * y[i, j];
			}
			for (int j=ab_num-1; j>0; j--){
				x[i, j] = x[i, j-1];
				y[i, j] = y[i, j-1];
			}
			data[i] = y[i, 0];
		}
	}

	public float filter(float data){
		x[0, 0] = data;
		y[0, 0] = 0;
		for (int j=0; j<ab_num; j++){
			y[0, 0] += FIlter_bp.b[j] * x[0, j] - FIlter_bp.a[j] * y[0, j];
		}
		for (int j=ab_num-1; j>0; j--){
			x[0, j] = x[0, j-1];
			y[0, j] = y[0, j-1];
		}
		return (float)y[0, 0];
	}

	public void filter_reset(){
		ab_num = FIlter_bp.b.Length;
		y = new double[chNum, ab_num];
		x = new double[chNum, ab_num];

		for (int i = 0; i < chNum; i++)
		{
			for (int j = 0; j < ab_num; j++)
			{
				y[i, j] = 0;
				x[i, j] = 0;
			}
		}
	}

	private struct FilterConstants {
		public double[] a;
		public double[] b;
		public string name;
		public string short_name;
		public FilterConstants(double[] b_given, double[] a_given, string name_given, string short_name_given) {
			b = new double[b_given.Length]; a = new double[b_given.Length];
			for (int i = 0; i < b.Length; i++) { b[i] = b_given[i]; }
			for (int i = 0; i < a.Length; i++) { a[i] = a_given[i]; }
			name = name_given;
			short_name = short_name_given;
		}
	}
}

