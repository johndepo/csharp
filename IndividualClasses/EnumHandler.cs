/*
 *  - JA 15/02/2013
 *      EnumHandler enum
 *      - Generic Enum Class
 *      
 *  SAMPLE USAGE
 *  
 *  public class DatabaseType : Enumhandler
 *  {
 *      public static readonly DatabaseType MSSQL
 *          = new DatabaseType(0, "mssql");
 *      public static readonly Database Type MYSQL
 *          = new DatabaseType(0, "mysql");
 *          
 *      private DatabaseType() {}
 *      private DatabaseType(int value, string displayName) : base(value, displayName) {}
 *  }
 */

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;
using System.Reflection;

namespace TestProj
{
    public abstract class EnumHandler : IComparable
    {
        private readonly int value;
        private readonly string displayName;

        public int Value { get { return this.value; } }
        public string DisplayName { get { return this.displayName; } }

        protected EnumHandler()
        {
        }

        protected EnumHandler(int value, string displayName)
        {
            this.value = value;
            this.displayName = displayName;
        }

        public override string ToString()
        {
            return DisplayName;
        }

        public static IEnumerable<T> GetAll<T>() where T : EnumHandler, new()
        {
            var type = typeof(T);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

            foreach (var info in fields)
            {
                var instance = new T();
                var locatedValue = info.GetValue(instance) as T;

                if (locatedValue != null)
                    yield return locatedValue;
            }
        }

        public override bool Equals(object obj)
        {
            var otherValue = obj as EnumHandler;

            if (otherValue == null)
                return false;

            var typeMatches = GetType().Equals(obj.GetType());
            var valueMatches = this.value.Equals(otherValue.Value);

            return typeMatches && valueMatches;
        }

        public override int GetHashCode()
        {
            return this.value.GetHashCode();
        }

        public static int AbsoluteDifference(EnumHandler firstValue, EnumHandler secondValue)
        {
            var absoluteDifference = Math.Abs(firstValue.Value - secondValue.Value);
            return absoluteDifference;
        }

        public int CompareTo(object other)
        {
            return Value.CompareTo(((EnumHandler)other).Value);
        }
    }
}
