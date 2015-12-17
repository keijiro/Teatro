//
// Reaktion - Audio Reactive Animation Toolkit
//
// Copyright (C) 2013-2015 Keijiro Takahashi
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
using UnityEngine;
using MidiJack;

namespace Reaktion
{
    [AddComponentMenu("Reaktion/Injector/Trigger Injector")]
    public class TriggerInjector : InjectorBase
    {
        [SerializeField] float _fallOffSpeed = 5;

        [Space]
        [SerializeField] float _noiseFrequency = 80;
        [SerializeField] float _noiseAmplitude = 0.3f;

        [Space]
        [SerializeField] KeyCode _keyCode1 = KeyCode.Alpha1;
        [SerializeField] KeyCode _keyCode2 = KeyCode.Alpha2;
        [SerializeField] KeyCode _keyCode3 = KeyCode.Alpha3;
        [SerializeField] KeyCode _keyCode4 = KeyCode.Alpha4;

        [Space]
        [SerializeField] MidiChannel _midiChannel = MidiChannel.All;
        [SerializeField] int _knob1 = 0;
        [SerializeField] int _knob2 = 1;
        [SerializeField] int _knob3 = 2;
        [SerializeField] int _knob4 = 3;

        float _level;

        void Kick(float energy)
        {
            _level = Mathf.Max(_level, energy);
        }

        void Update()
        {
            _level = Mathf.Max(_level - Time.deltaTime * _fallOffSpeed, 0.0f);

            if (Input.GetKey(_keyCode1)) Kick(0.3f);
            if (Input.GetKey(_keyCode2)) Kick(0.5f);
            if (Input.GetKey(_keyCode3)) Kick(0.7f);
            if (Input.GetKey(_keyCode4)) Kick(1.0f);

            Kick(MidiMaster.GetKnob(_midiChannel, _knob1) * 0.3f);
            Kick(MidiMaster.GetKnob(_midiChannel, _knob2) * 0.5f);
            Kick(MidiMaster.GetKnob(_midiChannel, _knob3) * 0.7f);
            Kick(MidiMaster.GetKnob(_midiChannel, _knob4) * 1.0f);

            var n = global::Perlin.Fbm(Time.time * _noiseFrequency, 4);
            var l = Mathf.Clamp01(_level * (1 + n * _noiseAmplitude));

            const float refLevel = 0.70710678118f; // 1/sqrt(2)
            const float zeroOffs = 1.5849e-13f;
            dbLevel = Mathf.Log(l / refLevel + zeroOffs, 10) * 20;
        }
    }
}
