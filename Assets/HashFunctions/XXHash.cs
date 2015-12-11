/*
C# implementation of xxHash optimized for producing random numbers from one or more input integers.
Copyright (C) 2015, Rune Skovbo Johansen. (https://bitbucket.org/runevision/random-numbers-testing/)

Based on C# implementation Copyright (C) 2014, Seok-Ju, Yun. (https://github.com/noricube/xxHashSharp)

Original C Implementation Copyright (C) 2012-2014, Yann Collet. (https://code.google.com/p/xxhash/)
BSD 2-Clause License (http://www.opensource.org/licenses/bsd-license.php)

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are
met:

    * Redistributions of source code must retain the above copyright
      notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above
      copyright notice, this list of conditions and the following
      disclaimer in the documentation and/or other materials provided
      with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
"AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;

public class XXHash : HashFunction {
	private uint seed;
	
	const uint PRIME32_1 = 2654435761U;
	const uint PRIME32_2 = 2246822519U;
	const uint PRIME32_3 = 3266489917U;
	const uint PRIME32_4 = 668265263U;
	const uint PRIME32_5 = 374761393U;
	
	public XXHash (int seed) {
		this.seed = (uint)seed;
	}
	
	public uint GetHash (byte[] buf) {
		uint h32;
		int index = 0;
		int len = buf.Length;
		
		if (len >= 16) {
			int limit = len - 16;
			uint v1 = seed + PRIME32_1 + PRIME32_2;
			uint v2 = seed + PRIME32_2;
			uint v3 = seed + 0;
			uint v4 = seed - PRIME32_1;
			
			do {
				v1 = CalcSubHash (v1, buf, index);
				index += 4;
				v2 = CalcSubHash (v2, buf, index);
				index += 4;
				v3 = CalcSubHash (v3, buf, index);
				index += 4;
				v4 = CalcSubHash (v4, buf, index);
				index += 4;
			} while (index <= limit);
			
			h32 = RotateLeft (v1, 1) + RotateLeft (v2, 7) + RotateLeft (v3, 12) + RotateLeft (v4, 18);
		}
		else {
			h32 = seed + PRIME32_5;
		}
		
		h32 += (uint)len;
		
		while (index <= len - 4) {
			h32 += BitConverter.ToUInt32 (buf, index) * PRIME32_3;
			h32 = RotateLeft (h32, 17) * PRIME32_4;
			index += 4;
		}
		
		while (index<len) {
			h32 += buf[index] * PRIME32_5;
			h32 = RotateLeft (h32, 11) * PRIME32_1;
			index++;
		}
		
		h32 ^= h32 >> 15;
		h32 *= PRIME32_2;
		h32 ^= h32 >> 13;
		h32 *= PRIME32_3;
		h32 ^= h32 >> 16;
		
		return h32;
	}
	
	public uint GetHash (params uint[] buf) {
		uint h32;
		int index = 0;
		int len = buf.Length;
		
		if (len >= 4) {
			int limit = len - 4;
			uint v1 = seed + PRIME32_1 + PRIME32_2;
			uint v2 = seed + PRIME32_2;
			uint v3 = seed + 0;
			uint v4 = seed - PRIME32_1;
			
			do {
				v1 = CalcSubHash (v1, buf[index]);
				index++;
				v2 = CalcSubHash (v2, buf[index]);
				index++;
				v3 = CalcSubHash (v3, buf[index]);
				index++;
				v4 = CalcSubHash (v4, buf[index]);
				index++;
			} while (index <= limit);
			
			h32 = RotateLeft (v1, 1) + RotateLeft (v2, 7) + RotateLeft (v3, 12) + RotateLeft (v4, 18);
		}
		else {
			h32 = seed + PRIME32_5;
		}
		
		h32 += (uint)len * 4;
		
		while (index < len) {
			h32 += buf[index] * PRIME32_3;
			h32 = RotateLeft (h32, 17) * PRIME32_4;
			index++;
		}
		
		h32 ^= h32 >> 15;
		h32 *= PRIME32_2;
		h32 ^= h32 >> 13;
		h32 *= PRIME32_3;
		h32 ^= h32 >> 16;
		
		return h32;
	}
	
	public override uint GetHash (params int[] buf) {
		uint h32;
		int index = 0;
		int len = buf.Length;
		
		if (len >= 4) {
			int limit = len - 4;
			uint v1 = (uint)seed + PRIME32_1 + PRIME32_2;
			uint v2 = (uint)seed + PRIME32_2;
			uint v3 = (uint)seed + 0;
			uint v4 = (uint)seed - PRIME32_1;
			
			do {
				v1 = CalcSubHash (v1, (uint)buf[index]);
				index++;
				v2 = CalcSubHash (v2, (uint)buf[index]);
				index++;
				v3 = CalcSubHash (v3, (uint)buf[index]);
				index++;
				v4 = CalcSubHash (v4, (uint)buf[index]);
				index++;
			} while (index <= limit);
			
			h32 = RotateLeft (v1, 1) + RotateLeft (v2, 7) + RotateLeft (v3, 12) + RotateLeft (v4, 18);
		}
		else {
			h32 = (uint)seed + PRIME32_5;
		}
		
		h32 += (uint)len * 4;
		
		while (index < len) {
			h32 += (uint)buf[index] * PRIME32_3;
			h32 = RotateLeft (h32, 17) * PRIME32_4;
			index++;
		}
		
		h32 ^= h32 >> 15;
		h32 *= PRIME32_2;
		h32 ^= h32 >> 13;
		h32 *= PRIME32_3;
		h32 ^= h32 >> 16;
		
		return h32;
	}
	
	public override uint GetHash (int buf) {
		uint h32 = (uint)seed + PRIME32_5;
		h32 += 4U;
		h32 += (uint)buf * PRIME32_3;
		h32 = RotateLeft (h32, 17) * PRIME32_4;
		h32 ^= h32 >> 15;
		h32 *= PRIME32_2;
		h32 ^= h32 >> 13;
		h32 *= PRIME32_3;
		h32 ^= h32 >> 16;
		return h32;
	}
	
	private static uint CalcSubHash (uint value, byte[] buf, int index) {
		uint read_value = BitConverter.ToUInt32 (buf, index);
		value += read_value * PRIME32_2;
		value = RotateLeft (value, 13);
		value *= PRIME32_1;
		return value;
	}
	
	private static uint CalcSubHash (uint value, uint read_value) {
		value += read_value * PRIME32_2;
		value = RotateLeft (value, 13);
		value *= PRIME32_1;
		return value;
	}
	
	private static uint RotateLeft (uint value, int count) {
		return (value << count) | (value >> (32 - count));
	}
}

