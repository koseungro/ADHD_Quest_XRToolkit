using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFB;
using System.IO;

public class DataWriter {
	private string filePath;
	private string fileTmpPath;

	private bool isRec;
	private bool isOpen;
	private Dictionary<string, StreamWriter> tempFile;

	public DataWriter(string _filePath){
		SetSavePath (_filePath);
		Init ();
	}

	public DataWriter(){
		Init ();
	}

	private void Init(){
		isRec = false;
		isOpen = true;
		tempFile = new Dictionary<string, StreamWriter> ();
	}

	public void AddDataKey(string myDataKey){
		string fullPath = fileTmpPath + "/" + myDataKey + ".csv";
		if (File.Exists (fullPath))
			File.Delete (fullPath);
		tempFile.Add (myDataKey, File.CreateText(fullPath));
	}

	public void SetSavePath(string _filePath){
		if (!isRec) {
			filePath = _filePath;
			fileTmpPath = filePath + "/Temp";
			MakeDir ();
		}
	}

	private void MakeDir(){
		try{
			if (!Directory.Exists (filePath))
				Directory.CreateDirectory (filePath);
			if (!Directory.Exists (fileTmpPath))
				Directory.CreateDirectory (fileTmpPath);
		}catch(IOException ex){
			Debug.LogError (ex.Message);
		}
	}

	public void RecordStart(){
		if (!isRec) {
			isRec = true;
			if (!isOpen) {
				MakeDir ();

				for (int i = 0; i < tempFile.Count; i++) {
					string key = tempFile.Keys.ElementAt (i);
					string fullPath = fileTmpPath + "/" + key + ".csv";
					tempFile[key] = File.CreateText(fullPath);
				}
				isOpen = true;
			}
		}else
			Debug.LogWarning ("Already record");
	}
		
	public void Record(float data, string key){
		if (isRec) {
			try {
				tempFile[key].WriteLine(data);
			} catch (FileNotFoundException ex) {
				Debug.Log (ex.Message);
			}
		}
	}

	public void Record(double data, string key){
		if (isRec) {
			try {
				tempFile[key].WriteLine(data);
			} catch (FileNotFoundException ex) {
				Debug.Log (ex.Message);
			}
		}
	}
    /// <summary>
    /// eeg Writen
    /// </summary>
    /// <param name="data"></param>
    /// <param name="key"></param>
	public void Record(float[] data, string key){
		if (isRec) {
			try {                
				for(int i=0; i<data.Length; i++){                    
					tempFile[key].Write(data[i]);
					tempFile[key].Write(',');
				}
				tempFile[key].WriteLine();
			} catch (FileNotFoundException ex) {
				Debug.Log (ex.Message);
			}
		}
	}
    /// <summary>
    /// stftCh Writen
    /// </summary>
    /// <param name="data"></param>
    /// <param name="key"></param>
	public void Record(double[] data, string key){
		if (isRec) {
			try {
				for(int i=0; i<data.Length; i++){
					tempFile[key].Write(data[i]);
					tempFile[key].Write(',');
				}
				tempFile[key].WriteLine();
			} catch (FileNotFoundException ex) {
				Debug.Log (ex.Message);
			}
		}
	}

	public void Record(uint count, float data, string key){
		if (isRec) {
			try {
				tempFile[key].Write(count);
				tempFile[key].Write(',');
				tempFile[key].WriteLine(data);
			} catch (FileNotFoundException ex) {
				Debug.Log (ex.Message);
			}
		}
	}

	public void Record(uint count, double data, string key){
		if (isRec) {
			try {
				tempFile[key].Write(count);
				tempFile[key].Write(',');
				tempFile[key].WriteLine(data);
			} catch (FileNotFoundException ex) {
				Debug.Log (ex.Message);
			}
		}
	}

	public void RecordStop(){
		if (isRec) {
			isRec = false;
			isOpen = false;
			foreach (StreamWriter sm in tempFile.Values) {
				sm.Flush ();
				sm.Close ();
			}
//			if (EditorUtility.DisplayDialog ("Please Selection On Save",
//				    "Are you sure you want save this file", "Save", "Don't Save"))
				GatherTempFile ();
//			else
//				DeleteTempFile ();
		} else
			Debug.LogWarning ("Already recording is stopped");
	}

	private void GatherTempFile(){
		Directory.Move (fileTmpPath, filePath + "/" + GetDataToString ());
	}

	private void DeleteTempFile(){
		Directory.Delete (fileTmpPath, true);
	}

	private string GetDataToString(){
		return System.DateTime.Now.ToString ("yyyy년MM월dd일tthh시mm분ss초");
	}
}