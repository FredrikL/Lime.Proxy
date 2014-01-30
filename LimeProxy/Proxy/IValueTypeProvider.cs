using System;
using System.Runtime.InteropServices;

namespace LimeProxy.Proxy
{
    public interface IValueTypeProvider
    {
        int GetForStoredProcedure(object value);
        string GetForQuery(object value);
    }

    public class ValueTypeProvider : IValueTypeProvider
    {
        public int GetForStoredProcedure(object value)
        {
            return (int) GetVarEnum(value);
        }

        public string GetForQuery(object value)
        {
             Type type = value.GetType();
            if (type == typeof(int) ||
                type == typeof(long) ||
                type == typeof(float) ||
                type == typeof(double) ||
                type == typeof(Decimal))
                return "numeric";

            if (type == typeof (DateTime))
                return "date";

            return "string";
        }

        private VarEnum GetVarEnum(object value)
        {
            if (value == null)
                return VarEnum.VT_NULL;
            Type type = value.GetType();
            if (type == typeof(int) || type == typeof(long))
                return VarEnum.VT_I4;
            if (type == typeof(float))
                return VarEnum.VT_R4;
            if (type == typeof(double))
                return VarEnum.VT_R8;
            if (type == typeof(Decimal))
                return VarEnum.VT_DECIMAL;
            if (type == typeof(DateTime))
                return VarEnum.VT_DATE;
            if (type == typeof(bool))
                return VarEnum.VT_BOOL;
            if (type == typeof(byte))
                return VarEnum.VT_UI1;
            if (type == typeof(string))
                return VarEnum.VT_BSTR;

            throw new ArgumentException("Unknown datatype", "value");
        }
    }
}