//-----------------------------------------------------------------------------
// C3dReader.cs
//
// Reads in 3D position data from C3D file passed in "infile" 
// (files should be opened before calling).
//
//
// ETRO, Vrije Universiteit Brussel
// Copyright (C) Lubos Omelina. All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;

namespace ManipAnalysis_v2
{
    public class C3DReader : IDisposable
    {
        private readonly HashSet<Parameter> _allParameters;

        private readonly Dictionary<int, ParameterGroup> _idToGroups;

        private readonly Dictionary<string, ParameterGroup> _nameToGroups;

        private float _analogGenScale;

        private float _analogRate;

        private float[] _analogScale;

        private int _analogUsed;

        private short[] _analogZeroOffset;

        private string _c3DFile;

        private int _dataStart;

        private FileStream _fs;

        private float _pointRate;

        private float _pointScale;

        private BinaryReader _reader;

        public C3DReader()
        {
            _nameToGroups = new Dictionary<string, ParameterGroup>();
            _idToGroups = new Dictionary<int, ParameterGroup>();
            _pointsLabels = new List<string>();
            _pointsLabelsToId = new Dictionary<string, int>();
            _allParameters = new HashSet<Parameter>();
            _analogLabels = new List<string>();
            _analogLabelsToId = new Dictionary<string, int>();
        }

        public Vector3 this[int key]
        {
            get
            {
                if (_points == null)
                {
                    throw new ApplicationException("You must open file and read freame first");
                }
                if (key < 0 || key >= _points.Length)
                {
                    throw new IndexOutOfRangeException();
                }
                return _points[key];
            }
        }

        public Vector3 this[string key]
        {
            get
            {
                if (_pointsLabels == null)
                {
                    throw new ApplicationException("You must open file and read frame first");
                }
                if (!_pointsLabelsToId.ContainsKey(key))
                {
                    throw new ApplicationException("Label " + key + " doesn't exist in the 3D point data section");
                }
                return _points[_pointsLabelsToId[key]];
            }
        }

        public void Dispose()
        {
            Close();
        }

        public bool Open(string c3dFile)
        {
            _c3DFile = c3dFile;
            try
            {
                _fs = new FileStream(_c3DFile, FileMode.Open, FileAccess.Read);
                _reader = new BinaryReader(_fs);
                Header = new C3DHeader();

                ReadHeader();
                ReadParameters();
                ParseRequiredParameters();

                _reader.BaseStream.Seek(_dataStart, 0);
            }
            catch (Exception
                e)
            {
                Console.Error.WriteLine("C3dReader.Open(\"" + c3dFile + "\"): " + e.Message);
                return false;
            }
            return true;
        }

        private void ParseRequiredParameters()
        {
            var labels = GetParameter<string[]>("POINT:LABELS");
            for (var i = 0;
                i < labels.Length;
                i
                    ++)
            {
                var label = labels[i].TrimEnd(' ');
                _pointsLabelsToId.Add(label, i);
                _pointsLabels.Insert(i, label);
            }


            _dataStart = 512*(Header.DataStart - 1);
            // Parameter Section Data Start Value doesnt work
            //_dataStart = 512 * (GetParameter<Int16>("POINT:DATA_START") - 1);

            NumberOfPointsInFrame = GetParameter<short>("POINT:USED");
            FramesCount = GetParameter<int>("POINT:FRAMES");
            _pointScale = GetParameter<float>("POINT:SCALE");
            _pointRate = GetParameter<float>("POINT:RATE");

            _analogRate = GetParameter<float>("ANALOG:RATE");
            _analogUsed = GetParameter<short>("ANALOG:USED");
            _analogScale = GetParameter<float[]>("ANALOG:SCALE");
            _analogGenScale = GetParameter<float>("ANALOG:GEN_SCALE");
            _analogZeroOffset = GetParameter<short[]>("ANALOG:OFFSET");

            var analogLabels = GetParameter<string[]>("ANALOG:LABELS");
            for (var i = 0;
                i < analogLabels.Length;
                i
                    ++)
            {
                var label = analogLabels[i].TrimEnd(' ').TrimEnd('\0');
                _analogLabelsToId.Add(label, i);
                _analogLabels.Insert(i, label);
            }
        }


        private void ReadHeader()
        {
            var data = new byte[512];
            _reader.Read(data, 0, 512);
            Header.SetHeader(data);
        }

        private void ReadParameters()
        {
            var parameters = new byte[4];
            _reader.Read(parameters, 0, 4);
            // TODO we should not ignore first 4 bytes as it is now ?

            int nextItem;
            do
            {
                var nameLen = ParameterModel.ReadNameLength(_reader);
                var id = ParameterModel.ReadGroupID(_reader);
                var name = ParameterModel.ReadName(_reader, Math.Abs(nameLen));
                nextItem = ParameterModel.ReadNextItemOffset(_reader);

                ParameterModel param;
                var parameterDataSize = 0;
                if (id > 0) //if id > 0 then it is parameter, otherwise it is group
                {
                    param = new Parameter(_reader);
                    parameterDataSize = ((Parameter) param).C3DParameterSize;
                }
                else
                {
                    param = new ParameterGroup();
                }

                var descLen = ParameterModel.ReadDescLength(_reader);
                var desc = ParameterModel.ReadName(_reader, descLen);

                // general assignment
                param.Name = name;
                param.Id = id;
                param.Description = desc;


                if (param is Parameter)
                {
                    _allParameters.Add((Parameter) param);
                }
                else
                {
                    _nameToGroups.Add(param.Name, (ParameterGroup) param);
                    _idToGroups.Add(param.Id, (ParameterGroup) param);
                }

                // if next item is more far read unknown data
                // I assume that there will be no more than 512 bytes of unknown data

                if (nextItem > descLen + 3 + parameterDataSize)
                    // If the C3D file is correct this will never happen, but real life is far from perfect
                {
                    var toRead = nextItem - (descLen + 1);
                    _reader.Read(parameters, 0, toRead);
                }
                //else if (nextItem < (descLen + 3 + parameterDataSize)) {
                //    Console.Error.WriteLine("Error");
                //}
            } while (nextItem > 0);

            foreach (var
                p
                in
                _allParameters)
            {
                if (_idToGroups.ContainsKey(-p.Id))
                {
                    _idToGroups[-p.Id].Parameters.Add(p);
                }
            }
        }

        public T GetParameter<T>(string path)
        {
            var elements = path.Split(':');
            if (elements.Length != 2)
            {
                throw new ApplicationException("Wrong path format (use GROUP:PARAMETER)");
            }

            if (!_nameToGroups.ContainsKey(elements[0]))
            {
                throw new ApplicationException("Group " + elements[0] + " deasn't exist.");
            }

            var grp = _nameToGroups[elements[0]];

            foreach (var
                p
                in
                grp.Parameters)
            {
                if (p.Name == elements[1])
                {
                    return p.GetData<T>();
                }
            }
            throw new ApplicationException("Parameter " + elements[1] + " deasn't exist.");
        }


        public Vector3[] ReadFrame()
        {
            if (_fs == null)
            {
                throw new ApplicationException("The C3d file is not open. Open the file before reading.");
            }

            //if (_currentFrame >= _pointFrames)
            //{
            //    _reader.BaseStream.Seek(_dataStart, 0);
            //    _currentFrame = 0;
            //}

            var data = _pointScale < 0 ? ReadFloatData() : ReadIntData();

            CurrentFrame
                ++;
            //Console.WriteLine(_currentFrame);
            return data;
        }

        private Vector3[] ReadFloatData()
        {
            if (!IsFloat)
            {
                throw new ApplicationException(
                    "Data stored in C3D file are in Inetger format. You are trying to read it as a Floating-point format.");
            }

            _points = new Vector3[NumberOfPointsInFrame];
            for (var i = 0;
                i < NumberOfPointsInFrame;
                i
                    ++)
            {
                _points[i] = new Vector3( /* float x = */
                    _reader.ReadSingle(), /* float y = */
                    _reader.ReadSingle(), /* float z = */
                    _reader.ReadSingle());
                _reader.ReadSingle();
            }


            // Read Analog data
            var samplesPerFrame = (int) (_analogRate/_pointRate);

            var allData = new float[_analogUsed, samplesPerFrame];
            for (var rate = 0;
                rate < samplesPerFrame;
                rate
                    ++)
            {
                for (var variable = 0;
                    variable < _analogUsed;
                    variable
                        ++)
                {
                    var data = _reader.ReadSingle();
                    //real world value = (data value - zero offset) * channel scale * general scale
                    allData[variable, rate] = (data -
                                               (_analogZeroOffset != null && _analogZeroOffset.Length > 0
                                                   ? _analogZeroOffset[variable]
                                                   : 0))*_analogGenScale*
                                              (_analogScale != null && _analogScale.Length > 0
                                                  ? _analogScale[variable]
                                                  : 1);
                }
            }
            AnalogData = new AnalogDataArray(_analogLabels, _analogLabelsToId, allData);

            return _points;
        }

        private Vector3[] ReadIntData()
        {
            if (!IsInterger)
            {
                throw new ApplicationException(
                    "Data stored in C3D file are in Floating-point format. You are trying to read it as a Integer format.");
            }

            _points = new Vector3[NumberOfPointsInFrame];
            for (var i = 0;
                i < NumberOfPointsInFrame;
                i
                    ++)
            {
                _points[i] = new Vector3( /* float x = */
                    _reader.ReadInt16()*_pointScale, /* float y = */
                    _reader.ReadInt16()*_pointScale, /* float z = */
                    _reader.ReadInt16()*_pointScale);
                _reader.ReadInt16();
            }

            // reading of analog data
            var samplesPerFrame = (int) (_analogRate/_pointRate);
            var allData = new float[_analogUsed, samplesPerFrame];
            for (var rate = 0;
                rate < samplesPerFrame;
                rate
                    ++)
            {
                for (var variable = 0;
                    variable < _analogUsed;
                    variable
                        ++)
                {
                    float data = _reader.ReadInt16();
                    // real world value = (data value - zero offset) * channel scale * general scale
                    allData[variable, rate] = (data -
                                               (_analogZeroOffset != null && _analogZeroOffset.Length > 0
                                                   ? _analogZeroOffset[variable]
                                                   : 0))*_analogGenScale*
                                              (_analogScale != null && _analogScale.Length > 0
                                                  ? _analogScale[variable]
                                                  : 1);
                }
            }
            AnalogData = new AnalogDataArray(_analogLabels, _analogLabelsToId, allData);

            return _points;
        }


        public bool Close()
        {
            _reader.Close();
            _fs.Close();
            return true;
        }

        #region Properties

        private readonly List<string> _pointsLabels;

        private readonly Dictionary<string, int> _pointsLabelsToId;

        internal List<string> _analogLabels;

        internal Dictionary<string, int> _analogLabelsToId;

        public Vector3[] _points;


        public IList<string> Labels => _pointsLabels.AsReadOnly();

        public IList<string> AnalogLabels => _analogLabels.AsReadOnly();

        public int CurrentFrame { get; private set; }

        public AnalogDataArray AnalogData { get; private set; }

        public int AnalogChannels => (int) (_analogRate/_pointRate);

        public Vector3[] Points => _points;

        public int FramesCount { get; private set; }

        public int NumberOfPointsInFrame { get; set; }

        public C3DHeader Header { get; private set; }

        public bool IsInterger => _pointScale >= 0;

        public bool IsFloat => _pointScale < 0;

        #endregion
    }

    public class AnalogDataArray
    {
        internal AnalogDataArray(List<string> analogLabels, Dictionary<string, int> analogLabelsToId,
            float[,] analogData)
        {
            _analogLabels = analogLabels;
            _analogLabelsToId = analogLabelsToId;
            Data = analogData;
        }

        private List<string> _analogLabels { get; }

        private Dictionary<string, int> _analogLabelsToId { get; }

        public float[,] Data { get; }

        public IList<string> Labels
        {
            get { return _analogLabels.AsReadOnly(); }
        }

        public float this[int key, int channel]
        {
            get
            {
                if (Data == null)
                {
                    throw new ApplicationException("You must open file and read freame first");
                }
                if (key < 0 || key >= Data.Length)
                {
                    throw new IndexOutOfRangeException();
                }
                return Data[key, channel];
            }
        }

        public float this[int key]
        {
            get
            {
                if (Data == null)
                {
                    throw new ApplicationException("You must open file and read freame first");
                }
                if (key < 0 || key >= Data.Length)
                {
                    throw new IndexOutOfRangeException();
                }
                return Data[key, 0];
            }
        }

        public float this[string key, int channel]
        {
            get
            {
                if (_analogLabels == null)
                {
                    throw new ApplicationException("You must open file and read freame first");
                }
                if (!_analogLabelsToId.ContainsKey(key))
                {
                    throw new ApplicationException("Analog data label " + key +
                                                   " doesn't exist in the 3D point data section");
                }
                return Data[_analogLabelsToId[key], channel];
            }
        }

        public float this[string key]
        {
            get
            {
                if (_analogLabels == null)
                {
                    throw new ApplicationException("You must open file and read freame first");
                }
                if (!_analogLabelsToId.ContainsKey(key))
                {
                    throw new ApplicationException("Analog data label " + key +
                                                   " doesn't exist in the 3D point data section");
                }
                return Data[_analogLabelsToId[key], 0];
            }
        }
    }
}