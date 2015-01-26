using System;

namespace BF2Statistics
{
    /// <summary>
    /// The Key Value item is similar to the KeyValuePair object,
    /// but rather this object only contains a string key and value
    /// pair.
    /// </summary>
    class KeyValueItem
    {
        /// <summary>
        /// The Item Key
        /// </summary>
        private string _key = "";

        /// <summary>
        /// The Item Value
        /// </summary>
        private string _value = "";

        /// <summary>
        /// Returns this objects Key value
        /// </summary>
        public string Key
        { 
            get{ return _key; } 
        }

        /// <summary>
        /// Returns this objects value
        /// </summary>
        public string Value
        {
            get { return _value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Key">The item key</param>
        /// <param name="Value">The item value</param>
        public KeyValueItem(string Key, string Value)
        {
            this._key = Key;
            this._value = Value;
        }

        /// <summary>
        /// ToString() returns the value of this object by default.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Value;
        }
    }
}
