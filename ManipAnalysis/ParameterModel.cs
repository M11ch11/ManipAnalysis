//-----------------------------------------------------------------------------
// ParameterModel.cs
//
// The class representation of c3d parameters and parameter groups
//
//
// ETRO, Vrije Universiteit Brussel
// Copyright (C) Lubos Omelina. All rights reserved.
//-----------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ManipAnalysis_v2
{

    #region 3D point representation

    public struct Vector3
    {
        public float X, Y, Z;

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }

    #endregion

    #region ParameterBase

    public abstract class ParameterModel
    {
        public const int BLOCK_SIZE = 512;

        private long _offsetInFile = -1;

        protected ParameterModel()
        {
            Name = "";
            Description = "";
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public sbyte Id { get; set; }

        internal long OffsetInFile
        {
            get { return _offsetInFile; }
            set
            {
                if (_offsetInFile == -1)
                {
                    _offsetInFile = value;
                }
                else
                {
                    throw new ApplicationException("FileOffset has been set already for parameter " + Name);
                }
            }
        }

        protected abstract short GetContentLength();

        protected abstract void WriteContent(BinaryWriter writer);

        public void WriteTo(BinaryWriter writer, bool isLast = false)
        {
            writer.Write((sbyte) Name.Length);
            writer.Write(Id);
            writer.Write(Name.ToCharArray());
            //  string name = ParameterModel.ReadName(_reader, Math.Abs(nameLen));

            // compute offset of the next item
            var nextItem = (short) (isLast
                ? 0
                : Description.Length + 2 // next item number 
                  + 1 // desc length number
                  + GetContentLength());


            writer.Write(nextItem);
            WriteContent(writer);

            writer.Write((byte) Description.Length);
            writer.Write(Description.ToCharArray());
        }

        //
        // static member for C3D format reading
        //

        public static sbyte ReadGroupID(BinaryReader reader)
        {
            return reader.ReadSByte();
        }

        public static sbyte ReadNameLength(BinaryReader reader)
        {
            return reader.ReadSByte();
        }

        public static byte ReadDescLength(BinaryReader reader)
        {
            return reader.ReadByte();
        }

        public static short ReadNextItemOffset(BinaryReader reader)
        {
            return reader.ReadInt16();
        }

        public static string ReadName(BinaryReader reader, int length)
        {
            return ReadString(reader, length);
        }

        public static string ReadDesc(BinaryReader reader, int length)
        {
            return ReadString(reader, length);
        }

        private static string ReadString(BinaryReader reader, int length)
        {
            return Encoding.UTF8.GetString(reader.ReadBytes(length), 0, length);
        }
    }

    #endregion ParameterBase

    #region ParameterGroup

    public class ParameterGroup : ParameterModel
    {
        public ParameterGroup()
        {
            Parameters = new HashSet<Parameter>();
        }

        public HashSet<Parameter> Parameters { get; }

        public bool HasParameter(string name)
        {
            return Parameters.Any(p => p.Name == name);
        }

        public Parameter GetParameter(string name)
        {
            return Parameters.FirstOrDefault(p => p.Name == name);
        }


        protected override short GetContentLength()
        {
            return 0;
        }

        // ParameterGroup doesn't have content (NOTE: Parameters are children, not content)

        protected override void WriteContent(BinaryWriter writer)
        {
        }
    }

    #endregion ParameterGroup

    #region Parameter

    public class Parameter : ParameterModel
    {
        private int[] _dimensions;

        private sbyte _paramType;

        private byte[] _vectorData;

        public Parameter(BinaryReader reader)
        {
            var position = reader.BaseStream.Position;
            // read parameter
            _paramType = reader.ReadSByte();
            var dimensions = reader.ReadByte();
            // 0 means that parameter is scalar

            if (dimensions > 0)
            {
                IsScalar = false;
                ReadMatrix(reader, GetType(_paramType), dimensions);
            }
            else
            {
                IsScalar = true;
                ReadScalar(reader, GetType(_paramType));
            }

            C3DParameterSize = (int) (reader.BaseStream.Position - position);
        }


        public bool IsScalar { get; set; }

        public int C3DParameterSize { get; }

        public int Length { get; private set; }

        private void ReadMatrix(BinaryReader reader, Type t, int dimensions)
        {
            Length = 1;
            _dimensions = new int[dimensions];
            for (var i = 0;
                i < dimensions;
                i
                    ++)
            {
                _dimensions[i] = reader.ReadByte();
                Length *= _dimensions[i];
            }
            Length *= GetSize(_paramType);

            // "todo" [from Matthias], but I dont know what..? Works fine so far...
            _vectorData = new byte[Length];
            reader.Read(_vectorData, 0, Length);
        }

        private void ReadScalar(BinaryReader reader, Type t)
        {
            _vectorData = new byte[GetSize(_paramType)];
            reader.Read(_vectorData, 0, GetSize(_paramType));
            // "todo" [from Matthias], but I dont know what...? Works fine so far...
        }

        private static Type GetType(sbyte c3dDataType)
        {
            switch (c3dDataType)
            {
                case -1:
                    return typeof (char);
                case 1:
                    return typeof (byte);
                case 2:
                    return typeof (short);
                case 4:
                    return typeof (float);
                default:
                    throw new ApplicationException("Unknown data type of c3d parameter");
            }
        }

        private static int GetSize(sbyte c3dDataType)
        {
            switch (c3dDataType)
            {
                case -1:
                    return 1;
                case 1:
                    return 1;
                case 2:
                    return 2;
                case 4:
                    return 4;
                default:
                    throw new ApplicationException("Unknown data type of c3d parameter");
            }
        }

        #region Serialization of data

        //public void SetData<T>(T data) 
        //{ 
        //    SetData<T>(data);k
        //}

        protected override short GetContentLength()
        {
            return (short) (_vectorData.Length + 1 + // to store parameter type (sbyte)
                            1 + // to store number of dimensions type (byte)
                            _dimensions.Length
                // +
                //_vectorData.Length
                );
        }

        protected override void WriteContent(BinaryWriter writer)
        {
            writer.Write(_paramType);
            writer.Write((byte) _dimensions.Length);
            for (var i = 0;
                i < _dimensions.Length;
                i
                    ++)
            {
                writer.Write((byte) _dimensions[i]);
            }
            writer.Write(_vectorData);
        }

        //TODO: Remove because it is not used?
        public void SetData<T>(T data)
        {
            //T ret;
            // = default(T);
            //
            //  BASIC TYPES
            //
            if (typeof (T) == typeof (char))
            {
                _paramType = -1;
                _dimensions = new int[] {};
                _vectorData = BitConverter.GetBytes((char) (object) data);
                Length = 1;
                IsScalar = true;
            }
            else if (typeof (T) == typeof (byte))
            {
                _dimensions = new int[] {};
                _paramType = 1;
                _vectorData = BitConverter.GetBytes((byte) (object) data);
                Length = 1;
                IsScalar = true;
            }
            else if (typeof (T) == typeof (short))
            {
                _dimensions = new int[] {};
                _paramType = 2;
                _vectorData = BitConverter.GetBytes((short) (object) data);
                Length = 1;
                IsScalar = true;
            }
            else if (typeof (T) == typeof (float))
            {
                _dimensions = new int[] {};
                _paramType = 4;
                _vectorData = BitConverter.GetBytes((float) (object) data);
                Length = 1;
                IsScalar = true;
            }
            else if (typeof (T) == typeof (string))
            {
                _dimensions = new[] {((string) (object) data).Length};
                _paramType = -1;
                _vectorData = Encoding.ASCII.GetBytes((string) (object) data);
                Length = _vectorData.Length;
                // it is the same length as it is in string because ASCII encoding
                IsScalar = false;
            }

            //
            // 1D Arrays
            //
            else if (typeof (T) == typeof (string[]))
            {
                var count = ((string[]) (object) data).Length;
                var maxLen = 0;
                foreach (var
                    s
                    in
                    (string[]) (object) data)
                {
                    maxLen = Math.Max(s.Length, maxLen);
                }
                _dimensions = new[] {maxLen, count};
                _paramType = -1;
                _vectorData = new byte[count*maxLen];


                // in C# there is really no other method for initialising arrays to a non-default value (without creating temporary objects)
                // this is fastest way, see this -> http://www.dotnetperls.com/initialize-array
                // but yes, it's ugly, indeed
                for (var i = 0; i < _vectorData.Length; i++)
                {
                    _vectorData[i] = 32;
                }


                Length = _vectorData.Length;
                // it is the same length as it is in string because ASCII encoding
                for (var i = 0;
                    i < count;
                    i
                        ++)
                {
                    var s = ((string[]) (object) data)[i];
                    Encoding.ASCII.GetBytes(s, 0, s.Length, _vectorData, i*maxLen);
                }
                IsScalar = false;
            }
            else if (typeof (T) == typeof (float[]))
            {
                var count = ((float[]) (object) data).Length;
                _dimensions = new[] {count};
                _paramType = 4;
                _vectorData = new byte[count*GetSize(4)];

                Length = _vectorData.Length;
                // it is the same length as it is in string because ASCII encoding
                for (var i = 0;
                    i < count;
                    i
                        ++)
                {
                    var f = ((float[]) (object) data)[i];
                    Array.Copy(BitConverter.GetBytes(f), 0, _vectorData, i*GetSize(_paramType), GetSize(_paramType));
                }
                IsScalar = false;
            }
            else if (typeof (T) == typeof (short[]))
            {
                var count = ((short[]) (object) data).Length;
                _dimensions = new[] {count};
                _paramType = 2;
                _vectorData = new byte[count*GetSize(2)];
                Length = _vectorData.Length;
                // it is the same length as it is in string because ASCII encoding
                for (var i = 0;
                    i < count;
                    i
                        ++)
                {
                    var n = ((short[]) (object) data)[i];

                    // TODO : check this ?
                    Array.Copy(BitConverter.GetBytes(n), 0, _vectorData, i*GetSize(_paramType), GetSize(_paramType));
                }
                IsScalar = false;
            }
            //else if (typeof(T) == typeof(char[]))
            //{
            //    ret = (T)(object)Get1DArray<char>();
            //}
            //else if (typeof(T) == typeof(byte[]))
            //{
            //    ret = (T)(object)Get1DArray<byte>();
            //}
            //else if (typeof(T) == typeof(Int16[]))
            //{
            //    ret = (T)(object)Get1DArray<Int16>();
            //}
            //else if (typeof(T) == typeof(float[]))
            //{
            //    ret = (T)(object)Get1DArray<float>();
            //}
            ////
            //// 2D Arrays
            ////
            //// TODO: DO IT IF YOU NEED IT :) ?
            ////
            ////    else if (typeof(T) == typeof(Int16 [,]))
            ////    {
            ////        ret = (T)(object)Get2DArray<Int16>();
            ////    }

            //else
            //{
            //    throw new ApplicationException("Unknown type of parameter");
            //}
            //return ret;
        }

        #endregion

        #region Extraction of data

        public T GetData<T>()
        {
            return GetData<T>(0);
        }

        public T GetData<T>(int i)
        {
            T ret;
            // = default(T);
            //
            //  BASIC TYPES
            //
            if (typeof (T) == typeof (char))
            {
                ret = (T) (object) BitConverter.ToChar(_vectorData, i);
            }
            else if (typeof (T) == typeof (byte))
            {
                ret = (T) (object) _vectorData[i];
            }
            else if (typeof (T) == typeof (short))
            {
                ret = (T) (object) BitConverter.ToInt16(_vectorData, i*sizeof (short));
            }
            else if (typeof (T) == typeof (ushort))
            {
                ret = (T) (object) BitConverter.ToUInt16(_vectorData, i * sizeof(ushort));
            }
            else if (typeof (T) == typeof (float))
            {
                ret = (T) (object) BitConverter.ToSingle(_vectorData, i*sizeof (float));
            }
            else if (typeof (T) == typeof (string))
            {
                ret = (T) (object) DataToString();
            }

            //// new
            else if (typeof(T) == typeof(int))
            {
                ret = (T)(object)i;
            }

            //
            // 1D Arrays
            //
            else if (typeof (T) == typeof (string[]))
            {
                ret = (T) (object) DataToStringArray();
            }
            else if (typeof (T) == typeof (char[]))
            {
                ret = (T) (object) Get1DArray<char>();
            }
            else if (typeof (T) == typeof (byte[]))
            {
                ret = (T) (object) Get1DArray<byte>();
            }
            else if (typeof (T) == typeof (short[]))
            {
                ret = (T) (object) Get1DArray<short>();
            }
            else if (typeof(T) == typeof(ushort[]))
            {
                ret = (T)(object)Get1DArray<ushort>();
            }
            else if (typeof (T) == typeof (float[]))
            {
                ret = (T) (object) Get1DArray<float>();
            }
            //
            // 2D Arrays
            //
            // TODO: DO IT IF YOU NEED IT :)
            //
            //    else if (typeof(T) == typeof(Int16 [,]))
            //    {
            //        ret = (T)(object)Get2DArray<Int16>();
            //    }

            else
            {
                throw new ApplicationException("Unknown type of parameter");
            }
            return ret;
        }

        //TODO: Remove this function, because it is not used anyways?
        private T[,] Get2DArray<T>()
        {
            if (_dimensions.Length != 2)
            {
                throw new ApplicationException("Parameter " + Name + " is not 2D array.");
            }
            var array = new T[_dimensions[0], _dimensions[1]];
            for (var x = 0;
                x < _dimensions[0];
                x
                    ++)
            {
                for (var y = 0;
                    y < _dimensions[1];
                    y
                        ++)
                {
                    // TODO: still need to test following line ? [from Matthias]
                    array[x, y] = GetData<T>(x + y*x);
                }
            }
            return array;
        }


        private T[] Get1DArray<T>()
        {
            if (_dimensions.Length != 1)
            {
                throw new ApplicationException("Parameter " + Name + " is not 1D array.");
            }
            var array = new T[_dimensions[0]];
            for (var i = 0;
                i < _dimensions[0];
                i
                    ++)
            {
                array[i] = GetData<T>(i);
            }
            return array;
        }


        private string DataToString()
        {
            if (_dimensions.Length != 1 || _paramType != -1)
            {
                throw new ApplicationException("Parameter " + Name + " is not string type.");
            }
            return Encoding.UTF8.GetString(_vectorData, 0, _dimensions[0]).TrimEnd(' ').TrimEnd('\0');
        }

        private string[] DataToStringArray()
        {
            if (_dimensions.Length != 2 || _paramType != -1)
            {
                throw new ApplicationException("Parameter " + Name + " is not string array type.");
            }

            var retArray = new string[_dimensions[1]];

            for (var i = 0;
                i < _dimensions[1];
                i
                    ++)
            {
                retArray[i] =
                    Encoding.UTF8.GetString(_vectorData, i*_dimensions[0], _dimensions[0]).TrimEnd(' ').TrimEnd('\0');
            }
            return retArray;
        }

        #endregion
    }

    #endregion Parameter
}