using Jacobi.Vst.Core;
using Jacobi.Vst.Plugin.Framework;
using Jacobi.Vst.Plugin.Interop;

using System;

namespace TheoSynth
{
    public class SineSynth : IVstPluginAudioProcessor, IVstMidiProcessor
    {
        private double _phase;
        private double _frequency = 440; // Default to A4 note
        private readonly double _twoPi = 2 * Math.PI;
        private bool _noteOn;
        public float SampleRate { get; set; } = 44100; // Default to 44.1kHz
        public float Gain { get; set; } = 0.5f; // default gain

        // Define proper channel counts
        int IVstPluginAudioProcessor.OutputCount => 2; // stereo audio channel
        int IVstMidiProcessor.ChannelCount => 1; // mono MIDI channel




        public int InputCount => 2;  // Let's say 2 input channels for stereo
        public int OutputCount => 2; // 2 output channels for stereo
        public int TailSize => 0;    // No tail size, for simplicity
        public bool NoSoundInStop => true; // No sound when input is silence

        private float _sampleRate = 44100f; // Default sample rate


        private int _blockSize = 1024; // Default block size
        public int BlockSize
        {
            get { return _blockSize; }
            set { _blockSize = value; } // Update internal structures if needed
        }



        public bool SetPanLaw(VstPanLaw type, float gain)
        {
            // Assume we do not support setting the pan law
            return false;
        }


        public void Process(VstAudioBuffer[] inChannels, VstAudioBuffer[] outChannels)
        {
            if (_noteOn)
            {
                for (int i = 0; i < outChannels[0].SampleCount; i++)
                {
                    double sample = Math.Sin(_phase) * Gain;
                    foreach (var outChannel in outChannels)
                    {
                        outChannel[i] = (float)sample;
                    }

                    _phase += _twoPi * _frequency / SampleRate;
                    if (_phase >= _twoPi) _phase -= _twoPi;
                }
            }
        }

        public void Process(VstEventCollection events)
        {
            foreach (VstEvent ev in events)
            {
                if (ev.EventType == VstEventTypes.MidiEvent)
                {
                    VstMidiEvent midiEvent = (VstMidiEvent)ev;
                    byte status = midiEvent.Data[0];
                    byte midiNote = midiEvent.Data[1];

                    if ((status & 0xF0) == 0x90) // note on
                    {
                        NoteOn(midiNote);
                    }
                    else if ((status & 0xF0) == 0x80) // note off
                    {
                        NoteOff();
                    }
                }
            }
        }

        public void NoteOn(int midiNoteNumber)
        {
            _frequency = 440.0 * Math.Pow(2, (midiNoteNumber - 69) / 12.0);
            _noteOn = true;
        }

        public void NoteOff()
        {
            _noteOn = false;
        }

        // Implement other required methods, but you can leave them empty
        public bool SupportsSampleRate(int sampleRate) => true;

        public bool SetSampleRate(int sampleRate)
        {
            SampleRate = sampleRate;
            return true;
        }

        public void Start() { }
        public void Stop() { }

        public bool ProcessReplacing(VstAudioBuffer[] inChannels, VstAudioBuffer[] outChannels)
        {
            Process(inChannels, outChannels);
            return true;
        }

    }
}
