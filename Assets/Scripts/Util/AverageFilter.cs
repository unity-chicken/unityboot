using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AverageFilterVector3 {
	List<Vector3> raw = new List<Vector3>();
	int pos = 0;
	int filterSize = 60;
	Vector3 sum = Vector3.zero;
	Vector3 avg = Vector3.zero;

	// properties
	//-------------------------------------------------------------------------
	public bool ready { get; private set; }
	 
	// public apis
	//-------------------------------------------------------------------------
	public AverageFilterVector3(float avgSec = 1) { 
		ready = false;
		SetSize(avgSec);
	}

	public void Clear() {
	 	ready = false;
		raw.Clear();
	 	pos = 0;
	 	sum = Vector3.zero;
	 	avg = Vector3.zero;
	}

	public void SetFilterSize(float avgSec) {
	 	ready = false;
		raw.Clear();
		SetSize(avgSec);
		pos = 0;
		sum = Vector3.zero;
		Filter(avg);
	}

	public Vector3 Filter(Vector3 value) {
		if (ready == false) {
		 	for (int i=0; i<filterSize; i++) {
		 		raw.Add(value);
		 	}
		 	sum = value * filterSize;
		 	ready = true;
		}

		sum -= raw[pos];
		sum += value;
		raw[pos] = value;
		pos++;
		pos %= filterSize;
		avg = sum / (float)filterSize;
		return avg;
	}

	void SetSize(float avgSec) {
		filterSize = Mathf.RoundToInt(60f * avgSec);
		filterSize = Mathf.Max(1, filterSize);
	}
}


public class AverageFilterFloat {
	List<float> raw = new List<float>();
	int pos = 0;
	int filterSize = 60;
	float sum = 0f;
	float avg = 0f;

	// properties
	//-------------------------------------------------------------------------
	public bool ready { get; private set; }

	// public apis
	//-------------------------------------------------------------------------
	public AverageFilterFloat(float avgSec) { 
		ready = false;
		SetSize(avgSec);
	}

	public void Clear() {
	 	ready = false;
		raw.Clear();
	 	pos = 0;
	 	sum = 0;
	 	avg = 0;
	}

	public void SetFilterSize(float avgSec) {
	 	ready = false;
		raw.Clear();
		SetSize(avgSec);
		pos = 0;
		sum = 0;
		Filter(avg);
	}

	public float Filter(float value) {
		if (ready == false) {
			for (int i=0; i<filterSize; i++) {
				raw.Add(value);
			}
			sum = value * filterSize;
			ready = true;
		}

		sum -= raw[pos];
		sum += value;
		raw[pos] = value;
		pos++;
		pos %= filterSize;
		avg = sum / (float)filterSize;
		return avg;
	}

	void SetSize(float avgSec) {
		filterSize = Mathf.RoundToInt(60f * avgSec);
		filterSize = Mathf.Max(1, filterSize);
	}
}
