using System;
using System.Text;

namespace Confuser.Protections.HoLLy.Runtime.AntiMemoryEditing
{
    internal class ObfuscatedValue<T>
    {
        private static readonly Random Rand = new Random();
        private readonly int _salt;
        private T _val;

        public T Value
        {
            get {return Obfuscate(_val, true);}
            set { _val = Obfuscate(value, false);}
        }

        public ObfuscatedValue(T val)
        {
            _salt = Rand.Next(int.MinValue, int.MaxValue);
            Value = val;
        }
        
        //allow normal read/write
        public static implicit operator T(ObfuscatedValue<T> value) => value.Value;
        public static implicit operator ObfuscatedValue<T>(T value) => new ObfuscatedValue<T>(value);

        public override string ToString() => Value.ToString();

        private T Obfuscate(T val, bool reverse = false) => Obfuscate(val, _salt, reverse);

        private static T Obfuscate(T currentvalue, int salt, bool reverse)
        {
            Type type = currentvalue.GetType();

            switch (currentvalue.GetType().Name.ToString().ToLower())
            {
                case "string":
                    return (T)(object)XorString(s.ToString(), salt);
                case "int":
                    return (T)(object)(int.Parse(currentvalue.ToString()) ^ salt);
                case "double":
                    return (T)(object)(double.Parse(currentvalue.ToString()) * (reverse ? 1.0 / salt : salt));
                case "float":
                    return (T)(object)(float.Parse(currentvalue.ToString()) * (reverse ? 1f / salt : salt));
                default:
                    if (type.BaseType == typeof(Enum))
                        return (T)(object)((int)Convert.ChangeType(currentvalue, typeof(int)) ^ salt);
                    else
                        throw new NotSupportedException();
            }
        }

        private static string XorString(string str, int salt)
        {
            var sb = new StringBuilder(str.Length);
            foreach (char c in str) {
                sb.Append((char) (c ^ salt));
            }
            return sb.ToString();
        }
    }
}
