//-----------------------------------------------------------------------------
// C3dHeader.cs
//
// Class representing C3D file header and exposing information as properties
//
// ETRO, Vrije Universiteit Brussel
// Copyright (C) Lubos Omelina. All rights reserved.
//-----------------------------------------------------------------------------


using System;

namespace ManipAnalysis_v2
{
    public class C3DHeader
    {
        private readonly byte[] _data;

        internal C3DHeader()
        {
            _data = new byte[512];
            FirstWord = 0x5002;
            NumberOfPoints = 21;
            FirstSampleNumber = 1;
            LastSampleNumber = 1;
            FrameRate = 30;
            AnalogSamplesPerFrame = 0;
            AnalogChannels = 0;
            ScaleFactor = -1f;
            Support4CharEventLabels = true;
        }

        public short FirstWord
        {
            get { return BitConverter.ToInt16(_data, 0); }
            set { Array.Copy(BitConverter.GetBytes(value), 0, _data, 0, sizeof (short)); }
        }

        public byte FirstParameterBlock
        {
            get { return _data[0]; }
            set { _data[0] = value; }
        }

        public short NumberOfPoints
        {
            get { return BitConverter.ToInt16(_data, 2); }
            set { Array.Copy(BitConverter.GetBytes(value), 0, _data, 2, sizeof (short)); }
        }

        public short AnalogChannels
        {
            get { return BitConverter.ToInt16(_data, 4); }
            set { Array.Copy(BitConverter.GetBytes(value), 0, _data, 4, sizeof (short)); }
        }

        public short FirstSampleNumber
        {
            get { return BitConverter.ToInt16(_data, 6); }
            set { Array.Copy(BitConverter.GetBytes(value), 0, _data, 6, sizeof (short)); }
        }

        public short LastSampleNumber
        {
            get { return BitConverter.ToInt16(_data, 8); }
            set { Array.Copy(BitConverter.GetBytes(value), 0, _data, 8, sizeof (short)); }
        }

        public short MaxInterpolationGaps
        {
            get { return BitConverter.ToInt16(_data, 10); }
            set { Array.Copy(BitConverter.GetBytes(value), 0, _data, 10, sizeof (short)); }
        }

        public float ScaleFactor
        {
            get { return BitConverter.ToSingle(_data, 12); }
            set { Array.Copy(BitConverter.GetBytes(value), 0, _data, 12, sizeof (float)); }
        }

        public short DataStart
        {
            get { return BitConverter.ToInt16(_data, 16); }
            set { Array.Copy(BitConverter.GetBytes(value), 0, _data, 16, sizeof (short)); }
        }

        public short AnalogSamplesPerFrame
        {
            get { return BitConverter.ToInt16(_data, 18); }
            set { Array.Copy(BitConverter.GetBytes(value), 0, _data, 18, sizeof (short)); }
        }

        public float FrameRate
        {
            get { return BitConverter.ToSingle(_data, 20); }
            set { Array.Copy(BitConverter.GetBytes(value), 0, _data, 20, sizeof (float)); }
        }

        public bool Support4CharEventLabels
        {
            get { return BitConverter.ToInt16(_data, 149*2) == 12345; }
            set { Array.Copy(BitConverter.GetBytes(value ? 12345 : 0), 0, _data, 149*2, sizeof (short)); }
        }

        internal void SetHeader(byte[] headerData)
        {
            Array.Copy(headerData, _data, 512);
        }

        internal byte[] GetRawData()
        {
            return _data;
        }
    }
}