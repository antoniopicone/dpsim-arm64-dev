using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.EntityFrameworkCore.Internal;
using Mqtt.Client.AspNetCore.Services;
using Serilog;

namespace ApiSupport;

public static class Elaborazione
{
    public static double Media(IEnumerable<VillasMessage> villasMessages){

        double media = Math.Round(villasMessages.Sum(item => item.Data[0])/villasMessages.Count(),3);
        Log.Information("MEDIA su " + villasMessages.Count() + " Campioni :" + (media) + " Volt");
        return media;
    }

    public static void FFT(IEnumerable<VillasMessage> villasMessages, bool shape_window)
    {
        var signal = new double[32];
        int idx = 0;
        int sampleRate = 1_600; // 1600/32 = 50Hz di risoluzione in frequenza
        //  villas signal -f json -F 50 -r 1600 -a 1.414 sine | villas pipe -f json mqtt.conf mqtt_node
        
        foreach (VillasMessage m in villasMessages)
        {
            signal[idx] = m.Data[0];
            idx++;
        }

        // Shape the signal using a Hanning window
        if (shape_window){
            var window = new FftSharp.Windows.Hanning();
            window.ApplyInPlace(signal);
        }

        // calculate the power spectral density using FFT
        System.Numerics.Complex[] spectrum = FftSharp.FFT.Forward(signal);

        // or get the magnitude (units²) or power (dB) as real numbers
        // double[] magnitude = FftSharp.FFT.Magnitude(spectrum);                  // magnitude (units²)
        double[] power = FftSharp.FFT.Power(spectrum);                          // power spectral density (dB)
        double[] freq = FftSharp.FFT.FrequencyScale(power.Length, sampleRate);

        for (int i = 0; i < spectrum.Length/2; i++)
            Log.Information($"Indice: {i} Frequency: {freq[i]}, Power: {Math.Round(power[i],3)}");
        
        //Log.Information($"CC Frequency: {freq[0]} Hz, Power: {Math.Round(psd[0],3)} dB");
        //Log.Information($"Fodamentale Frequency: {freq[1]} Hz, Power: {Math.Round(psd[1],3)} dB");
        Log.Information("********************************************************");
    }
}
