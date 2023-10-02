using Jacobi.Vst.Core;
using Jacobi.Vst.Plugin.Framework;
using System;

namespace TheoSynth
{
    public class TheoSynth : VstPluginBase
    {
        private readonly SineSynth _sineSynth;

        public TheoSynth()
            : base("TheoSynth", new VstProductInfo("TheoSynth", "MyCompany", 1000), VstPluginFlags.NoFlags)
        {
            _sineSynth = new SineSynth();

            // Parameter for Gain
            Parameters.Add(new VstParameterInfo
            {
                CanBeAutomated = true,
                Name = "Gain",
                ShortLabel = "Gain",
                MinValue = 0.0f,
                MaxValue = 1.0f,
                DefaultValue = 0.5f,
                LargeStepFloat = 0.1f,
                SmallStepFloat = 0.01f,
                StepFloat = 0.05f,
                Label = string.Empty,
                DisplayIndex = 0,
                Category = new VstParameterCategory(),
                CanRamp = true
            });
        }

        public override IVstPluginAudioProcessor AudioProcessor => _sineSynth;
        public override IVstMidiProcessor MidiProcessor => _sineSynth;

        public override void ProcessParameterValueChange(VstParameterChangeEventArgs e)
        {
            base.ProcessParameterValueChange(e);
            if (e.Parameter.Name == "Gain")
            {
                _sineSynth.Gain = e.NewValue;
            }
        }
    }
}
