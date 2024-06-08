using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace ABSoftware
{
    public class AudioEngine
    {
        [DllImport("winmm.dll", EntryPoint = "waveOutOpen", SetLastError = true)]
        public static extern int waveOutOpen(out IntPtr hWaveOut, int uDeviceID, WAVEFORMATEX lpFormat, WaveOutProc dwCallback, int dwInstance, ulong dwFlags);

        [DllImport("winmm.dll", EntryPoint = "waveOutPrepareHeader", SetLastError = true)]
        public static extern int waveOutPrepareHeader(IntPtr hWaveOut, ref WAVEHDR lpWaveOutHdr, int uSize);
        [DllImport("winmm.dll", EntryPoint = "waveOutUnprepareHeader", SetLastError = true)]
        public static extern int waveOutUnprepareHeader(IntPtr hWaveOut, ref WAVEHDR lpWaveOutHdr, int uSize);
        [DllImport("winmm.dll", EntryPoint = "waveOutWrite", SetLastError = true)]
        public static extern int waveOutWrite(IntPtr hWaveOut, ref WAVEHDR lpWaveOutHdr, int uSize);


        [StructLayout(LayoutKind.Sequential)]
        public struct WAVEHDR
        {
            public IntPtr lpData;
            public uint dwBufferLength;
            public uint dwBytesRecorded;
            public IntPtr dwUser;
            public uint dwFlags;
            public uint dwLoops;
            public IntPtr lpNext;
            public uint reserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WAVEFORMATEX
        {
            public ushort wFormatTag;
            public ushort nChannels;
            public uint nSamplesPerSec;
            public uint nAvgBytesPerSec;
            public ushort nBlockAlign;
            public ushort wBitsPerSample;
            public ushort cbSize;
        }

        public const float PI = 3.141592653589f; 

        public delegate void WaveOutProc(IntPtr hWaveOut, uint uMsg, uint dwInstance, uint dwParam1, uint dwParam2);

        static WaveOutProc waveOutProc;

        public bool Active;
        public float GlobalTime;
        private uint sampleRate, channels, blockCount, blocksFree, blockCurrent, blockSamples;
        private short[] blockMemory;
        private WAVEHDR[] waveHeaders;
        private IntPtr audioDevice = IntPtr.Zero;
        private Thread audioThread;

        private Func<int, float, float> userFunction;

        public void SetUserFunction(Func<int, float, float> userFunction)
        {
            this.userFunction = userFunction;
        }

        public void Create(uint sampleRate = 44100, uint channels = 1, uint blocks = 8, uint blockSamples = 512)
        {
            this.sampleRate = sampleRate;
            this.channels = channels;
            this.blockCount = blocks;
            this.blocksFree = blocks;
            this.blockSamples = blockSamples;
            this.blockCurrent = 0;
            this.blockMemory = null;
            this.waveHeaders = null;

            WAVEFORMATEX format = new WAVEFORMATEX()
            {
                wFormatTag = 1,
                nSamplesPerSec = (ushort)sampleRate,
                wBitsPerSample = 16,
                nChannels = (ushort)channels
            };
            format.nBlockAlign = (ushort)(channels * 2);
            format.nAvgBytesPerSec = format.nSamplesPerSec * format.nBlockAlign;
            format.cbSize = (ushort)Marshal.SizeOf(format);

            waveOutProc = WaveOutProcWrap;

            if (waveOutOpen(out audioDevice, -1, format, waveOutProc, 0, 0x00030000l) != 0)
                Close();

            blockMemory = new short[blockCount * blockSamples];
            waveHeaders = new WAVEHDR[blockCount];

            unsafe
            {
                fixed (short* memory = blockMemory)
                {
                    for (uint n = 0; n < blockCount; n++)
                    {
                        waveHeaders[n].dwBufferLength = (uint)(blockSamples * 2);
                        waveHeaders[n].lpData = (IntPtr)(memory + (n * blockSamples));
                    }
                }
            }

            Active = true;
            audioThread = new Thread(AudioThread);
            audioThread.Start();
        }

        public float AngularVelocity(float frequency)
        {
            return frequency * 2.0f * PI;
        }

        public float Oscilator(float frequency, float time, OscilatorType type)
        {
            switch(type)
            {
                case OscilatorType.Sin:
                    return (float)Math.Sin(AngularVelocity(frequency) * time);
                case OscilatorType.Square:
                    return (Math.Sin(AngularVelocity(frequency) * time) > 0) ? 1f : -1f;

                default:
                    return 0f;
            }
        }

        public enum OscilatorType
        {
            Sin,
            Square
        }


        public void Close()
        {
            Active = false;
        }

        void WaveOutProcWrap(IntPtr hWaveOut, uint uMsg, uint dwInstance, uint dwParam1, uint dwParam2)
        {
            if (uMsg != 0x3BD)
                return;

            blocksFree++;
        }

        float clip(float sample, float max)
        {
            if (sample > 0f)
                return Math.Min(sample, max);
            else
                return Math.Max(sample, -max);
        }

        float DefaultUserFunction(int channel, float time)
        {
            return 0f;
        }

        void AudioThread()
        {
            int waveHeaderSize = Marshal.SizeOf(waveHeaders[blockCurrent]);
            GlobalTime = 0f;
            float timeStep = 1f / sampleRate;
            float maxSample = (float)(Math.Pow(2, (2 * 8) - 1) - 1);

            while (Active)
            {
                if (blocksFree == 0)
                    while (blocksFree == 0)
                        Thread.Sleep(1);

                blocksFree--;

                short newSample = 0;
                int currentBlock = (int)(blockCurrent * blockSamples);

                if ((waveHeaders[blockCurrent].dwFlags & 2) != 0)
                    waveOutUnprepareHeader(audioDevice, ref waveHeaders[blockCurrent], waveHeaderSize);

                for (uint n = 0; n < blockSamples; n += channels)
                {
                    for (int c = 0; c < channels; c++)
                    {
                        if (userFunction != null)
                            newSample = (short)(clip(userFunction.Invoke(c, GlobalTime), 1f) * maxSample);
                        else
                            newSample = (short)(clip(DefaultUserFunction(c, GlobalTime), 1f) * maxSample);
                        blockMemory[currentBlock + n + c] = newSample;
                    }

                    GlobalTime += timeStep;
                }

                waveOutPrepareHeader(audioDevice, ref waveHeaders[blockCurrent], waveHeaderSize);
                waveOutWrite(audioDevice, ref waveHeaders[blockCurrent], waveHeaderSize);

                blockCurrent++;
                blockCurrent %= blockCount;
            }
        }
    }

    public class ADSREnvelope
    {
        public float attackTime, decayTime, releaseTime;
        public float sustainAmplitude, startAmplitude;
        public float onTime, offTime;
        public bool noteDown;

        public ADSREnvelope(float attackTime = 0.1f, float decayTime = 0.01f, float releaseTime = 0.2f, float sustainAmplitude = 0.8f, float startAmplitude = 1f)
        {
            this.attackTime = attackTime;
            this.decayTime = decayTime;
            this.releaseTime = releaseTime;
            this.sustainAmplitude = sustainAmplitude;
            this.startAmplitude = startAmplitude;
            this.onTime = 0f;
            this.offTime = 0f;
            this.noteDown = false;
        }

        public void PressNote(float time)
        {
            this.onTime = time;
            this.noteDown = true;
        }

        public void ReleaseNote(float time)
        {
            this.offTime = time;
            this.noteDown = false;
        }

        public float GetCurrentAmplitude(float time)
        {
            float amplitude = 0f;
            float lifetime = time - onTime;
            
            if (noteDown)
            {
                if(lifetime <= attackTime)
                {
                    amplitude = (lifetime / attackTime) * startAmplitude;
                }

                if(lifetime > attackTime && lifetime <= (attackTime + decayTime))
                {
                    amplitude = ((lifetime - attackTime) / decayTime) * (sustainAmplitude - startAmplitude) + startAmplitude;
                }

                if(lifetime > (attackTime + decayTime))
                {
                    amplitude = sustainAmplitude;
                }
            }
            else
            {
                amplitude = ((time - offTime) / releaseTime) * (0f - sustainAmplitude) + sustainAmplitude;
            }

            if (amplitude <= 0.0001f)
                amplitude = 0f;

            return amplitude;
        }
    }
}
