using System;
using System.Linq;
using System.Text;

namespace UbiCom.Net.Structure
{
    /// <summary>
    /// SECS Item의 Value입니다.
    /// </summary>
    public class SECSValue
    {
        private int _length;
        private object _value;

        /// <summary>
        /// Value의 Length를 가져옵니다.
        /// </summary>
        public int Length
        {
            get
            {
                return this._length;
            }
        }

        /// <summary>
        /// Value에 값이 존재하는지 여부를 나타냅니다.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                if (this._value == null)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        public SECSValue()
        {
            this._value = null;
            this._length = 0;
        }

        /// <summary>
        /// 기본 생성자입니다.
        /// </summary>
        /// <param name="value">SECS Item의 Value입니다.</param>
        public SECSValue(object value)
        {
            SetValue(value);
        }

        /// <summary>
        /// 현재 인스탄스를 나타내는 문자열로 변환합니다.
        /// </summary>
        /// <returns>현재 인스탄스를 나타내는 문자열입니다.</returns>
        public override string ToString()
        {
            StringBuilder result;
            dynamic valueTemp;

            if (this._value == null)
            {
                return string.Empty;
            }
            else if (this._value is string)
            {
                return this._value.ToString();
            }
            else if (this._value is bool boolean)
            {
                return boolean.ToString();
            }
            else if (this._value is sbyte @sbyte)
            {
                return @sbyte.ToString();
            }
            else if (this._value is byte @byte)
            {
                return @byte.ToString();
            }
            else if (this._value is short @short)
            {
                return @short.ToString();
            }
            else if (this._value is ushort @ushort)
            {
                return @ushort.ToString();
            }
            else if (this._value is int @int)
            {
                return @int.ToString();
            }
            else if (this._value is uint @uint)
            {
                return @uint.ToString();
            }
            else if (this._value is long @long)
            {
                return @long.ToString();
            }
            else if (this._value is ulong @ulong)
            {
                return @ulong.ToString();
            }
            else if (this._value is float single)
            {
                return single.ToString();
            }
            else if (this._value is double @double)
            {
                return @double.ToString();
            }
            else if (this._value is string)
            {
                return this._value.ToString();
            }
            else
            {
                result = new StringBuilder();

                if (this._value is bool[])
                {
                    valueTemp = this._value as bool[];
                }
                else if (this._value is sbyte[])
                {
                    valueTemp = this._value as sbyte[];
                }
                else if (this._value is byte[])
                {
                    valueTemp = this._value as byte[];
                }
                else if (this._value is short[])
                {
                    valueTemp = this._value as short[];
                }
                else if (this._value is ushort[])
                {
                    valueTemp = this._value as ushort[];
                }
                else if (this._value is int[])
                {
                    valueTemp = this._value as int[];
                }
                else if (this._value is uint[])
                {
                    valueTemp = this._value as uint[];
                }
                else if (this._value is long[])
                {
                    valueTemp = this._value as long[];
                }
                else if (this._value is ulong[])
                {
                    valueTemp = this._value as ulong[];
                }
                else if (this._value is float[])
                {
                    valueTemp = this._value as float[];
                }
                else if (this._value is double[])
                {
                    valueTemp = this._value as double[];
                }
                else if (this._value is SECSValue)
                {
                    return (this._value as SECSValue).GetValue().ToString();
                }
                else
                {
                    return (string)this._value;
                }

                foreach (var temp in valueTemp)
                {
                    result.AppendFormat("{0} ", temp);
                }

                return result.ToString().Trim();
            }
        }

        /// <summary>
        /// 지정한 SECSValue가 현재 SECSValue와 같은지 여부를 확인합니다.
        /// </summary>
        /// <param name="obj">현재 SECSValue와 비교할 SECSValue입니다.</param>
        /// <returns>지정된 SECSValue가 현재 SECSValue와 같으면 true이고, 그렇지 않으면 false입니다.</returns>
        public override bool Equals(object obj)
        {
            bool result;

            try
            {
                if (obj == null)
                {
                    result = (this._value == null);
                }
                else if (typeof(SECSValue) == obj.GetType())
                {
                    result = (this == ((SECSValue)obj));
                }
                else
                {
                    result = this._value.Equals(obj);
                }
            }
            catch
            {
                throw;
            }

            return result;
        }

        /// <summary>
        /// 특정 형식에 대한 해시 함수 역할을 합니다.
        /// </summary>
        /// <returns>현재 SECSValue의 해시 코드입니다.</returns>
        public override int GetHashCode()
        {
            int result;

            try
            {
                result = this._value.GetHashCode();
            }
            catch
            {
                throw;
            }

            return result;
        }

        /// <summary>
        /// 변환 연산자 : SECSValue -> bool
        /// </summary>
        /// <param name="value">SECSValue입니다.</param>
        public static implicit operator bool(SECSValue value)
        {
            bool result;

            try
            {
                if (value._value is string)
                {
                    if (value._value.ToString() == "1" || value._value.ToString().ToUpper() == "TRUE")
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
                else if (value._value is SECSValue)
                {
                    if (value._value.ToString() == "1" || value._value.ToString().ToUpper() == "TRUE")
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
                else
                {
                    result = Convert.ToBoolean(value._value);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Convert failed:ToBoolean({value._value})\r\n{ex.Message}", ex);
            }

            return result;
        }

        /// <summary>
        /// 변환 연산자 : SECSValue -> bool[]
        /// </summary>
        /// <param name="value">SECSValue입니다.</param>
        public static implicit operator bool[](SECSValue value)
        {
            bool[] result;

            try
            {
                result = value._value as bool[];
            }
            catch (Exception ex)
            {
                throw new Exception($"Convert failed:bool[]({value._value})\r\n{ex.Message}", ex);
            }

            return result;
        }

        /// <summary>
        /// 변환 연산자 : SECSValue -> sbyte
        /// </summary>
        /// <param name="value">SECSValue입니다.</param>
        public static implicit operator sbyte(SECSValue value)
        {
            sbyte result;

            try
            {
                if (value._value is string && string.IsNullOrEmpty(value._value.ToString()) == true)
                {
                    result = 0;
                }
                else if (value._value is SECSValue)
                {
                    result = Convert.ToSByte(value._value.ToString());
                }
                else
                {
                    result = Convert.ToSByte(value._value);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Convert failed:ToSByte({value._value})\r\n{ex.Message}", ex);
            }

            return result;
        }

        /// <summary>
        /// 변환 연산자 : SECSValue -> sbyte[]
        /// </summary>
        /// <param name="value">SECSValue입니다.</param>
        public static implicit operator sbyte[](SECSValue value)
        {
            sbyte[] result;

            try
            {
                result = value._value as sbyte[];
            }
            catch (Exception ex)
            {
                throw new Exception($"Convert failed:sbyte[]({value._value})\r\n{ex.Message}", ex);
            }

            return result;
        }

        /// <summary>
        /// 변환 연산자 : SECSValue -> byte
        /// </summary>
        /// <param name="value">SECSValue입니다.</param>
        public static implicit operator byte(SECSValue value)
        {
            byte result;

            try
            {
                if (value._value is string && string.IsNullOrEmpty(value._value.ToString()) == true)
                {
                    result = 0;
                }
                else if (value._value is SECSValue)
                {
                    result = Convert.ToByte(value._value.ToString());
                }
                else
                {
                    result = Convert.ToByte(value._value);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Convert failed:ToByte({value._value})\r\n{ex.Message}", ex);
            }

            return result;
        }

        /// <summary>
        /// 변환 연산자 : SECSValue -> byte[]
        /// </summary>
        /// <param name="value">SECSValue입니다.</param>
        public static implicit operator byte[](SECSValue value)
        {
            byte[] result;

            try
            {
                result = value._value as byte[];
            }
            catch (Exception ex)
            {
                throw new Exception($"Convert failed:byte[]({value._value})\r\n{ex.Message}", ex);
            }

            return result;
        }

        /// <summary>
        /// 변환 연산자 : SECSValue -> short
        /// </summary>
        /// <param name="value">SECSValue입니다.</param>
        public static implicit operator short(SECSValue value)
        {
            short result;

            try
            {
                if (value._value is string && string.IsNullOrEmpty(value._value.ToString()) == true)
                {
                    result = 0;
                }
                else if (value._value is SECSValue)
                {
                    result = Convert.ToInt16(value._value.ToString());
                }
                else
                {
                    result = Convert.ToInt16(value._value);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Convert failed:ToInt16({value._value})\r\n{ex.Message}", ex);
            }

            return result;
        }

        /// <summary>
        /// 변환 연산자 : SECSValue -> short[]
        /// </summary>
        /// <param name="value">SECSValue입니다.</param>
        public static implicit operator short[](SECSValue value)
        {
            short[] result;

            try
            {
                result = value._value as short[];
            }
            catch (Exception ex)
            {
                throw new Exception($"Convert failed:short[]({value._value})\r\n{ex.Message}", ex);
            }

            return result;
        }

        /// <summary>
        /// 변환 연산자 : SECSValue -> ushort
        /// </summary>
        /// <param name="value">SECSValue입니다.</param>
        public static implicit operator ushort(SECSValue value)
        {
            ushort result;

            try
            {
                if (value._value is string && string.IsNullOrEmpty(value._value.ToString()) == true)
                {
                    result = 0;
                }
                else if (value._value is SECSValue)
                {
                    result = Convert.ToUInt16(value._value.ToString());
                }
                else
                {
                    result = Convert.ToUInt16(value._value);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Convert failed:ToUInt16({value._value})\r\n{ex.Message}", ex);
            }

            return result;
        }

        /// <summary>
        /// 변환 연산자 : SECSValue -> ushort[]
        /// </summary>
        /// <param name="value">SECSValue입니다.</param>
        public static implicit operator ushort[](SECSValue value)
        {
            ushort[] result;

            try
            {
                result = value._value as ushort[];
            }
            catch (Exception ex)
            {
                throw new Exception($"Convert failed:ushort[]({value._value})\r\n{ex.Message}", ex);
            }

            return result;
        }

        /// <summary>
        /// 변환 연산자 : SECSValue -> int
        /// </summary>
        /// <param name="value">SECSValue입니다.</param>
        public static implicit operator int(SECSValue value)
        {
            int result;

            try
            {
                if (value._value is string && string.IsNullOrEmpty(value._value.ToString()) == true)
                {
                    result = 0;
                }
                else if (value._value is SECSValue)
                {
                    result = Convert.ToInt32(value._value.ToString());
                }
                else
                {
                    result = Convert.ToInt32(value._value);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Convert failed:ToInt32({value._value})\r\n{ex.Message}", ex);
            }

            return result;
        }

        /// <summary>
        /// 변환 연산자 : SECSValue -> int[]
        /// </summary>
        /// <param name="value">SECSValue입니다.</param>
        public static implicit operator int[](SECSValue value)
        {
            int[] result;

            try
            {
                result = value._value as int[];
            }
            catch (Exception ex)
            {
                throw new Exception($"Convert failed:int[]({value._value})\r\n{ex.Message}", ex);
            }

            return result;
        }

        /// <summary>
        /// 변환 연산자 : SECSValue -> uint
        /// </summary>
        /// <param name="value">SECSValue입니다.</param>
        public static implicit operator uint(SECSValue value)
        {
            uint result;

            try
            {
                if (value._value is string && string.IsNullOrEmpty(value._value.ToString()) == true)
                {
                    result = 0;
                }
                else if (value._value is SECSValue)
                {
                    result = Convert.ToUInt32(value._value.ToString());
                }
                else
                {
                    result = Convert.ToUInt32(value._value);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Convert failed:ToUInt32({value._value})\r\n{ex.Message}", ex);
            }

            return result;
        }

        /// <summary>
        /// 변환 연산자 : SECSValue -> uint[]
        /// </summary>
        /// <param name="value">SECSValue입니다.</param>
        public static implicit operator uint[](SECSValue value)
        {
            uint[] result;

            try
            {
                result = value._value as uint[];
            }
            catch (Exception ex)
            {
                throw new Exception($"Convert failed:uint[]({value._value})\r\n{ex.Message}", ex);
            }

            return result;
        }

        /// <summary>
        /// 변환 연산자 : SECSValue -> long
        /// </summary>
        /// <param name="value">SECSValue입니다.</param>
        public static implicit operator long(SECSValue value)
        {
            long result;

            try
            {
                if (value._value is string && string.IsNullOrEmpty(value._value.ToString()) == true)
                {
                    result = 0;
                }
                else if (value._value is SECSValue)
                {
                    result = Convert.ToInt64(value._value.ToString());
                }
                else
                {
                    result = Convert.ToInt64(value._value);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Convert failed:ToInt64({value._value})\r\n{ex.Message}", ex);
            }

            return result;
        }

        /// <summary>
        /// 변환 연산자 : SECSValue -> long[]
        /// </summary>
        /// <param name="value">SECSValue입니다.</param>
        public static implicit operator long[](SECSValue value)
        {
            long[] result;

            try
            {
                result = value._value as long[];
            }
            catch (Exception ex)
            {
                throw new Exception($"Convert failed:long[]({value._value})\r\n{ex.Message}", ex);
            }

            return result;
        }

        /// <summary>
        /// 변환 연산자 : SECSValue -> ulong
        /// </summary>
        /// <param name="value">SECSValue입니다.</param>
        public static implicit operator ulong(SECSValue value)
        {
            ulong result;

            try
            {
                if (value._value is string && string.IsNullOrEmpty(value._value.ToString()) == true)
                {
                    result = 0;
                }
                else if (value._value is SECSValue)
                {
                    result = Convert.ToUInt64(value._value.ToString());
                }
                else
                {
                    result = Convert.ToUInt64(value._value);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Convert failed:ToUInt64({value._value})\r\n{ex.Message}", ex);
            }

            return result;
        }

        /// <summary>
        /// 변환 연산자 : SECSValue -> ulong[]
        /// </summary>
        /// <param name="value">SECSValue입니다.</param>
        public static implicit operator ulong[](SECSValue value)
        {
            ulong[] result;

            try
            {
                result = value._value as ulong[];
            }
            catch (Exception ex)
            {
                throw new Exception($"Convert failed:ulong[]({value._value})\r\n{ex.Message}", ex);
            }

            return result;
        }

        /// <summary>
        /// 변환 연산자 : SECSValue -> float
        /// </summary>
        /// <param name="value">SECSValue입니다.</param>
        public static implicit operator float(SECSValue value)
        {
            float result;

            try
            {
                if (value._value is string && string.IsNullOrEmpty(value._value.ToString()) == true)
                {
                    result = 0;
                }
                else if (value._value is SECSValue)
                {
                    result = Convert.ToSingle(value._value.ToString());
                }
                else
                {
                    result = Convert.ToSingle(value._value);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Convert failed:ToSingle({value._value})\r\n{ex.Message}", ex);
            }

            return result;
        }

        /// <summary>
        /// 변환 연산자 : SECSValue -> float[]
        /// </summary>
        /// <param name="value">SECSValue입니다.</param>
        public static implicit operator float[](SECSValue value)
        {
            float[] result;

            try
            {
                result = value._value as float[];
            }
            catch (Exception ex)
            {
                throw new Exception($"Convert failed:float[]({value._value})\r\n{ex.Message}", ex);
            }

            return result;
        }

        /// <summary>
        /// 변환 연산자 : SECSValue -> double
        /// </summary>
        /// <param name="value">SECSValue입니다.</param>
        public static implicit operator double(SECSValue value)
        {
            double result;

            try
            {
                if (value._value is string && string.IsNullOrEmpty(value._value.ToString()) == true)
                {
                    result = 0;
                }
                else if (value._value is SECSValue)
                {
                    result = Convert.ToDouble(value._value.ToString());
                }
                else
                {
                    result = Convert.ToDouble(value._value);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Convert failed:ToDouble({value._value})\r\n{ex.Message}", ex);
            }

            return result;
        }

        /// <summary>
        /// 변환 연산자 : SECSValue -> double[]
        /// </summary>
        /// <param name="value">SECSValue입니다.</param>
        public static implicit operator double[](SECSValue value)
        {
            double[] result;

            try
            {
                result = value._value as double[];
            }
            catch (Exception ex)
            {
                throw new Exception($"Convert failed:double[]({value._value})\r\n{ex.Message}", ex);
            }

            return result;
        }

        /// <summary>
        /// 변환 연산자 : SECSValue -> string
        /// </summary>
        /// <param name="value">SECSValue입니다.</param>
        public static implicit operator string(SECSValue value)
        {
            string result;

            try
            {
                if (value._value != null)
                {
                    result = Convert.ToString(value._value);
                }
                else
                {
                    result = string.Empty;
                }
            }
            catch
            {
                result = string.Empty;
            }

            return result;
        }

        /// <summary>
        /// 변환 연산자 : SECSValue -> string[]
        /// </summary>
        /// <param name="value">SECSValue입니다.</param>
        public static implicit operator string[](SECSValue value)
        {
            string[] result;

            try
            {
                result = value._value as string[];
            }
            catch (Exception ex)
            {
                throw new Exception($"Convert failed:string[]({value._value})\r\n{ex.Message}", ex);
            }

            return result;
        }

        /// <summary>
        /// 변환 연산자 : bool -> SECSValue
        /// </summary>
        /// <param name="value">Value입니다.</param>
        public static implicit operator SECSValue(bool value)
        {
            SECSValue result;

            try
            {
                result = new SECSValue(value);
            }
            catch
            {
                throw;
            }

            return result;
        }

        /// <summary>
        /// 변환 연산자 : bool[] -> SECSValue
        /// </summary>
        /// <param name="value">Value입니다.</param>
        public static implicit operator SECSValue(bool[] value)
        {
            SECSValue result;

            try
            {
                result = new SECSValue(value);
            }
            catch
            {
                throw;
            }

            return result;
        }

        /// <summary>
        /// 변환 연산자 : sbyte -> SECSValue
        /// </summary>
        /// <param name="value">Value입니다.</param>
        public static implicit operator SECSValue(sbyte value)
        {
            SECSValue result;

            try
            {
                result = new SECSValue(value);
            }
            catch
            {
                throw;
            }

            return result;
        }

        /// <summary>
        /// 변환 연산자 : sbyte[] -> SECSValue
        /// </summary>
        /// <param name="value">Value입니다.</param>
        public static implicit operator SECSValue(sbyte[] value)
        {
            SECSValue result;

            try
            {
                result = new SECSValue(value);
            }
            catch
            {
                throw;
            }

            return result;
        }

        /// <summary>
        /// 변환 연산자 : byte -> SECSValue
        /// </summary>
        /// <param name="value">Value입니다.</param>
        public static implicit operator SECSValue(byte value)
        {
            SECSValue result;

            try
            {
                result = new SECSValue(value);
            }
            catch
            {
                throw;
            }

            return result;
        }

        /// <summary>
        /// 변환 연산자 : byte[] -> SECSValue
        /// </summary>
        /// <param name="value">Value입니다.</param>
        public static implicit operator SECSValue(byte[] value)
        {
            SECSValue result;

            try
            {
                result = new SECSValue(value);
            }
            catch
            {
                throw;
            }

            return result;
        }

        /// <summary>
        /// 변환 연산자 : short -> SECSValue
        /// </summary>
        /// <param name="value">Value입니다.</param>
        public static implicit operator SECSValue(short value)
        {
            SECSValue result;

            try
            {
                result = new SECSValue(value);
            }
            catch
            {
                throw;
            }

            return result;
        }

        /// <summary>
        /// 변환 연산자 : short[] -> SECSValue
        /// </summary>
        /// <param name="value">Value입니다.</param>
        public static implicit operator SECSValue(short[] value)
        {
            SECSValue result;

            try
            {
                result = new SECSValue(value);
            }
            catch
            {
                throw;
            }

            return result;
        }

        /// <summary>
        /// 변환 연산자 : ushort -> SECSValue
        /// </summary>
        /// <param name="value">Value입니다.</param>
        public static implicit operator SECSValue(ushort value)
        {
            SECSValue result;

            try
            {
                result = new SECSValue(value);
            }
            catch
            {
                throw;
            }

            return result;
        }

        /// <summary>
        /// 변환 연산자 : ushort[] -> SECSValue
        /// </summary>
        /// <param name="value">Value입니다.</param>
        public static implicit operator SECSValue(ushort[] value)
        {
            SECSValue result;

            try
            {
                result = new SECSValue(value);
            }
            catch
            {
                throw;
            }

            return result;
        }

        /// <summary>
        /// 변환 연산자 : int -> SECSValue
        /// </summary>
        /// <param name="value">Value입니다.</param>
        public static implicit operator SECSValue(int value)
        {
            SECSValue result;

            try
            {
                result = new SECSValue(value);
            }
            catch
            {
                throw;
            }

            return result;
        }

        /// <summary>
        /// 변환 연산자 : int[] -> SECSValue
        /// </summary>
        /// <param name="value">Value입니다.</param>
        public static implicit operator SECSValue(int[] value)
        {
            SECSValue result;

            try
            {
                result = new SECSValue(value);
            }
            catch
            {
                throw;
            }

            return result;
        }

        /// <summary>
        /// 변환 연산자 : uint -> SECSValue
        /// </summary>
        /// <param name="value">Value입니다.</param>
        public static implicit operator SECSValue(uint value)
        {
            SECSValue result;

            try
            {
                result = new SECSValue(value);
            }
            catch
            {
                throw;
            }

            return result;
        }

        /// <summary>
        /// 변환 연산자 : uint[] -> SECSValue
        /// </summary>
        /// <param name="value">Value입니다.</param>
        public static implicit operator SECSValue(uint[] value)
        {
            SECSValue result;

            try
            {
                result = new SECSValue(value);
            }
            catch
            {
                throw;
            }

            return result;
        }

        /// <summary>
        /// 변환 연산자 : long -> SECSValue
        /// </summary>
        /// <param name="value">Value입니다.</param>
        public static implicit operator SECSValue(long value)
        {
            SECSValue result;

            try
            {
                result = new SECSValue(value);
            }
            catch
            {
                throw;
            }

            return result;
        }

        /// <summary>
        /// 변환 연산자 : long[] -> SECSValue
        /// </summary>
        /// <param name="value">Value입니다.</param>
        public static implicit operator SECSValue(long[] value)
        {
            SECSValue result;

            try
            {
                result = new SECSValue(value);
            }
            catch
            {
                throw;
            }

            return result;
        }

        /// <summary>
        /// 변환 연산자 : ulong -> SECSValue
        /// </summary>
        /// <param name="value">Value입니다.</param>
        public static implicit operator SECSValue(ulong value)
        {
            SECSValue result;

            try
            {
                result = new SECSValue(value);
            }
            catch
            {
                throw;
            }

            return result;
        }

        /// <summary>
        /// 변환 연산자 : ulong[] -> SECSValue
        /// </summary>
        /// <param name="value">Value입니다.</param>
        public static implicit operator SECSValue(ulong[] value)
        {
            SECSValue result;

            try
            {
                result = new SECSValue(value);
            }
            catch
            {
                throw;
            }

            return result;
        }

        /// <summary>
        /// 변환 연산자 : float -> SECSValue
        /// </summary>
        /// <param name="value">Value입니다.</param>
        public static implicit operator SECSValue(float value)
        {
            SECSValue result;

            try
            {
                result = new SECSValue(value);
            }
            catch
            {
                throw;
            }

            return result;
        }

        /// <summary>
        /// 변환 연산자 : float[] -> SECSValue
        /// </summary>
        /// <param name="value">Value입니다.</param>
        public static implicit operator SECSValue(float[] value)
        {
            SECSValue result;

            try
            {
                result = new SECSValue(value);
            }
            catch
            {
                throw;
            }

            return result;
        }

        /// <summary>
        /// 변환 연산자 : double -> SECSValue
        /// </summary>
        /// <param name="value">Value입니다.</param>
        public static implicit operator SECSValue(double value)
        {
            SECSValue result;

            try
            {
                result = new SECSValue(value);
            }
            catch
            {
                throw;
            }

            return result;
        }

        /// <summary>
        /// 변환 연산자 : double[] -> SECSValue
        /// </summary>
        /// <param name="value">Value입니다.</param>
        public static implicit operator SECSValue(double[] value)
        {
            SECSValue result;

            try
            {
                result = new SECSValue(value);
            }
            catch
            {
                throw;
            }

            return result;
        }

        /// <summary>
        /// 변환 연산자 : string -> SECSValue
        /// </summary>
        /// <param name="value">Value입니다.</param>
        public static implicit operator SECSValue(string value)
        {
            SECSValue result;

            try
            {
                result = new SECSValue(value);
            }
            catch
            {
                throw;
            }

            return result;
        }

        /// <summary>
        /// 변환 연산자 : double[] -> SECSValue
        /// </summary>
        /// <param name="value">Value입니다.</param>
        public static implicit operator SECSValue(string[] value)
        {
            SECSValue result;

            try
            {
                result = new SECSValue(value);
            }
            catch
            {
                throw;
            }

            return result;
        }

        /// <summary>
        /// SECS Value를 설정합니다.
        /// </summary>
        /// <param name="value">설정할 Value입니다.</param>
        public void SetValue(object value)
        {
            if (value is System.Collections.IList)
            {
                if (value is System.Collections.IList list)
                {
                    this._length = list.Count;
                }
                else
                {
                    this._length = 0;
                }
            }
            else if (value is System.Collections.IEnumerable)
            {
                if (value is System.Collections.IEnumerable enumerable)
                {
                    this._length = enumerable.Cast<object>().Count();
                }
                else
                {
                    this._length = 0;
                }
            }
            else if (value is string)
            {
                this._length = System.Text.Encoding.Default.GetByteCount(value.ToString());
            }
            else if (value == null)
            {
                this._length = 0;
            }
            else
            {
                this._length = 1;
            }

            this._value = value;
        }

        /// <summary>
        /// SECS Value를 설정합니다.
        /// </summary>
        /// <param name="format">SECS Itemd의 format입니다.</param>
        /// <param name="value">설정할 Value입니다.</param>
        public void SetValue(SECSItemFormat format, object value)
        {
            if (value is System.Collections.IList)
            {
                if (value is System.Collections.IList list)
                {
                    this._length = list.Count;
                }
                else
                {
                    this._length = 0;
                }
            }
            else if (value is string)
            {
                if (format == SECSItemFormat.A)
                {
                    this._length = System.Text.Encoding.Default.GetByteCount(value.ToString());
                }
                else
                {
                    if (value != null && string.IsNullOrEmpty(value.ToString()) == false)
                    {
                        this._length = 1;
                    }
                    else
                    {
                        this._length = 0;
                    }
                }
            }
            else if (value is System.Collections.IEnumerable)
            {
                if (value is System.Collections.IEnumerable enumerable)
                {
                    this._length = enumerable.Cast<object>().Count();
                }
                else
                {
                    this._length = 0;
                }
            }
            else if (value == null)
            {
                this._length = 0;
            }
            else
            {
                this._length = 1;
            }

            this._value = value;
        }

        /// <summary>
        /// SECS Value를 가져옵니다.
        /// </summary>
        /// <returns>Value(object)입니다.</returns>
        public object GetValue()
        {
            return this._value;
        }

        /// <summary>
        /// 현재 인스턴스의 System.Type을 가져옵니다.
        /// </summary>
        /// <returns>현재 인스턴스의 런타임 형식을 나타내는 System.Type 인스턴스입니다.</returns>
        public new Type GetType()
        {
            return this._value.GetType();
        }
    }
}