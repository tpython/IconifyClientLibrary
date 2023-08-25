using IconifyClientLibrary.API;
using System;
using System.Collections.Generic;
using System.Text;

namespace IconifyClientLibrary
{
    public class Collection
    {
        public string ID { get; internal set; }
        public string Name { get; set; }
        public int Total { get; set; }
        public string Version { get; set; }
        public Author Author { get; set; }
        public License License { get; set; }
        public List<string> Samples { get; set; }
        public int Height { get; set; }
        public int DisplayHeight { get; set; }
        public string Category { get; set; }
        public List<string> Tags { get; set; }
        public bool Palette { get; set; }
    }
}
