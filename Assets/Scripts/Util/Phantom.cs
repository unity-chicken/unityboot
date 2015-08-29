using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Phantom {
	private int scaler = 1;
	private int adder = 0;
	private int prefix = 0;
	private Logger logger = new Logger("[Phantom]");
	private string targetClassName = "";

	public Phantom(string targetClassName) {
		this.targetClassName = targetClassName;
	}

	public float Encrypt(float data) {
		return data * scaler + adder;
	}

	public float Decrypt(float data) {
		return (data-adder) / scaler;
	}

	public int Encrypt(int data) {
		if (data >= 0) {
			data <<= 4;
			data += 5;
		}
		else {
			data = -data;
			data <<= 4;
			data += 10; 
		}
		return data;
	}

	public int Decrypt(int data) {
		int padding = (int)((System.UInt32)data % 16);

		// positive
		if (padding == 5) {
			return (int)((System.UInt32)data >> 4);
		}
		// negative
		else if (padding == 10) {
			return -(int)((System.UInt32)data >> 4);
		}

		// hacking 
		logger.LogError("Memory Decrypt failed:" + data);
		return 0;
	}

	public long Encrypt(long data) {
		if (data >= 0) {
			data <<= 4;
			data += 5;
		}
		else {
			data = -data;
			data <<= 4;
			data += 10; 
		}
		return data;
	}

	public long Decrypt(long data) {
		long padding = (long)((System.UInt64)data % 16);

		// positive
		if (padding == 5) {
			return (long)((System.UInt64)data >> 4);
		}
		// negative
		else if (padding == 10) {
			return -(long)((System.UInt64)data >> 4);
		}

		logger.LogError("Memory Decrypt failed:" + data);
		// hacking 
		return 0;
	}

	public static float encryptValue0 = 620;
	public static float encryptValue1 = 622;
	public static int encryptValue0Int = 5;
	public static int encryptValue1Int = 21;
}