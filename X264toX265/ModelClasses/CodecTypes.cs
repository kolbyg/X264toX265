using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace X264toX265.ModelClasses
{
    class CodecTypes
    {
        public static readonly string[] CodecNames = { "HEVC", "AVC", "Unknown" };
        private static readonly IReadOnlyCollection<string> HevcSynonyms = new HashSet<string>() { "x265", "x.265", "h265", "h.265", "hevc" };
        public static bool IsHevc(string codecCode)
        {
            return HevcSynonyms.Contains(codecCode.ToLower());
        }
        private static readonly IReadOnlyCollection<string> AvcSynonyms = new HashSet<string>() { "x264", "x.264", "h264", "h.264", "avc" };
        public static bool IsAvc(string codecCode)
        {
            return AvcSynonyms.Contains(codecCode.ToLower());
        }
        public static int GetCodecID(string codecCode)
        {
            if (IsHevc(codecCode))
                return 0;
            else if (IsAvc(codecCode))
                return 1;
            else
                return 2;
        }
    }
}
