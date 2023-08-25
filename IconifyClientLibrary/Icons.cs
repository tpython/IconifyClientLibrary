using System;
using System.Collections;
using System.Collections.Generic;

namespace IconifyClientLibrary
{
    public class Icons : IReadOnlyCollection<Icon>
    {
        internal List<Icon> Data { get; set; } = new List<Icon>();

        internal void Add(Icon icon)
        {
            Data.Add(icon);
        }

        public Icon this[int index] => Data[index];

        public int Count => ((IReadOnlyCollection<Icon>)Data).Count;

        public IEnumerator<Icon> GetEnumerator()
        {
            return ((IEnumerable<Icon>)Data).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Data).GetEnumerator();
        }
    }
}
