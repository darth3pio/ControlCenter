namespace BF2Statistics
{
    /// <summary>
    /// Similar to the <see cref="System.Collections.Generic.KeyValuePair"/>, this object is strictly strings
    /// </summary>
    public struct KeyValuePair
    {
        /// <summary>
        /// Gets or Sets the Key
        /// </summary>
        public string Key;

        /// <summary>
        /// Gets or Sets the Value
        /// </summary>
        public string Value;

        public KeyValuePair(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }

        public override string ToString()
        {
            return this.Value;
        }
    }
}
