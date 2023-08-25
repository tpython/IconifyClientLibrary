using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IconifyClientLibrary
{
    public class Icon
    {
        private IconifyClient Client;
        public string Name { get; internal set; }
        public string CollectionID { get; internal set; }
        public Collection Collection { get; internal set; }

        // cache
        private string svg = null;
        private SVGParams svgParam = null;

        internal Icon(IconifyClient client)
        {
            Client = client;
        }

        internal class SVGParams : IEquatable<SVGParams>
        {
            internal string color = null;
            internal int? width = null;
            internal int? height = null;
            internal IconFlips flip = IconFlips.None;
            internal IconRotates rotate = IconRotates.None;
            internal bool box = false;

            public static bool Equals(SVGParams x, SVGParams y)
            {
                if ((object)x == null || (object)y == null)
                    return false;
                return
                    x.color == y.color &&
                    x.width == y.width &&
                    x.height == y.height &&
                    x.flip == y.flip &&
                    x.rotate == y.rotate &&
                    x.box == y.box;
            }

            public static int GetHashCode(SVGParams obj)
            {
                if (obj == null)
                    return 0;
                return $"{obj.color}:{obj.width}:{obj.height}:{obj.flip}:{obj.rotate}:{obj.box}".GetHashCode();
            }

            public override bool Equals(object obj)
            {
                return SVGParams.Equals(this, obj as SVGParams);
            }

            public override int GetHashCode()
            {
                return SVGParams.GetHashCode(this);
            }

            public bool Equals(SVGParams other)
            {
                return SVGParams.Equals(this, other as SVGParams);
            }

            public static bool operator ==(SVGParams p1, SVGParams p2)
            {
                return SVGParams.Equals(p1, p2);
            }

            public static bool operator !=(SVGParams p1, SVGParams p2)
            {
                return !SVGParams.Equals(p1, p2);
            }
        }

        public async Task<string> GetSVG(string color = null, int? width = null, int? height = null, IconFlips flip = IconFlips.None, IconRotates rotate = IconRotates.None, bool box = false)
        {
            SVGParams p = new SVGParams
            {
                color = color,
                width = width,
                height = height,
                flip = flip,
                rotate = rotate,
                box = box
            };

            if (svg == null || p != svgParam)
            {
                svg = await DownloadSVG(p);
                svgParam = p;
            }

            return svg;
        }

        public Task<string> GetSVG(Color color, int? width = null, int? height = null, IconFlips flip = IconFlips.None, IconRotates rotate = IconRotates.None, bool box = false)
        {
            string colorTxt = "#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
            return GetSVG(colorTxt, width, height, flip, rotate, box);
        }

        private async Task<string> DownloadSVG(SVGParams param)
        {
            var uriQry = new Dictionary<string, string>()
            {
                { "color", param.color },
                { "width", param.width?.ToString() },
                { "height", param.height?.ToString() }
            };
            if (param.flip != IconFlips.None)
            {
                string flipTxt = "";
                if (param.flip.HasFlag(IconFlips.Horizontal))
                    flipTxt += ",horizontal";
                if (param.flip.HasFlag(IconFlips.Vertical))
                    flipTxt += ",vertical";
                uriQry["flip"] = flipTxt.TrimStart(',');
            }
            if (param.rotate != IconRotates.None)
            {
                uriQry["rotate"] = ((int)param.rotate).ToString();
            }
            if (param.box)
            {
                uriQry["box"] = "1";
            }

            UriBuilder uri = new UriBuilder(Client.URL);
            uri.Path = $"{CollectionID}/{Name}.svg";
            uri.Query = string.Join("&", uriQry.Where(x => x.Value != null).Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value)}"));

            var http = Client.GetHttpClient();
            var result = await http.GetStringAsync(uri.Uri);

            return result;
        }
    }
}
