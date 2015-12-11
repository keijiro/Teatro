/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * The Initial Developer of the Original Code is Rune Skovbo Johansen.
 * Portions created by the Initial Developer are Copyright (C) 2015
 * the Initial Developer. All Rights Reserved.
 */

using System;

public abstract class HashFunction {
	
	// Main hash function for any number of parameters.
	public abstract uint GetHash (params int[] data);
	
	// Optional optimizations for few parameters.
	// Derived classes can optimize with custom implementations.
	public virtual uint GetHash (int data) {
		return GetHash (new int[] {data});
	}
	public virtual uint GetHash (int x, int y) {
		return GetHash (new int[] {x, y});
	}
	public virtual uint GetHash (int x, int y, int z) {
		return GetHash (new int[] {x, y, z});
	}
	
	public float Value (params int[] data) {
		return GetHash (data) / (float)uint.MaxValue;
	}
	// Potentially optimized overloads for few parameters.
	public float Value (int data) {
		return GetHash (data) / (float)uint.MaxValue;
	}
	public float Value (int x, int y) {
		return GetHash (x, y) / (float)uint.MaxValue;
	}
	public float Value (int x, int y, int z) {
		return GetHash (x, y, z) / (float)uint.MaxValue;
	}
	
	public int Range (int min, int max, params int[] data) {
		return min + (int)(GetHash (data) % (max - min));
	}
	// Potentially optimized overloads for few parameters.
	public int Range (int min, int max, int data) {
		return min + (int)(GetHash (data) % (max - min));
	}
	public int Range (int min, int max, int x, int y) {
		return min + (int)(GetHash (x, y) % (max - min));
	}
	public int Range (int min, int max, int x, int y, int z) {
		return min + (int)(GetHash (x, y, z) % (max - min));
	}
	
	public float Range (float min, float max, params int[] data) {
		return min + (GetHash (data) * (float)(max - min)) / uint.MaxValue;
	}
	// Potentially optimized overloads for few parameters.
	public float Range (float min, float max, int data) {
		return min + (GetHash (data) * (float)(max - min)) / uint.MaxValue;
	}
	public float Range (float min, float max, int x, int y) {
		return min + (GetHash (x, y) * (float)(max - min)) / uint.MaxValue;
	}
	public float Range (float min, float max, int x, int y, int z) {
		return min + (GetHash (x, y, z) * (float)(max - min)) / uint.MaxValue;
	}
};
