using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities.Extensions;

namespace Utilities.Types
{
    public class UPath
    {
        enum SegmentType
        {
            ROOT,
            PATH
        }

        class SegmentInfo
        {
            public SegmentInfo(SegmentType type, string segment)
            {
                Type = type;
                Segment = segment;
            }

            public SegmentType Type { get; set; }
            public string Segment { get; set; }
        }

        readonly List<SegmentInfo> _segments = new List<SegmentInfo>();
        readonly PathFormat _defaultFormat;

        public bool IsRelative => !_segments.Any(s => s.Type == SegmentType.ROOT);

        public UPath(PathFormat pathFormat, params string[] pathParts)
        {
            _defaultFormat = pathFormat;

            var fixedParts = pathParts.SkipNulls().ToArray();
            if (fixedParts.Length > 0)
            {
                if (pathFormat.RootMarker != null && fixedParts[0].Contains(pathFormat.RootMarker))
                {
                    var splitted = fixedParts[0].Split(pathFormat.RootMarker);
                    if (splitted.Length != 2)
                    {
                        throw new ArgumentOutOfRangeException("Path part is bad");
                    }
                    else
                    {
                        _segments.Add(new SegmentInfo(SegmentType.ROOT, splitted[0]));
                        fixedParts[0] = splitted[1];
                    }
                }

                foreach (var segment in fixedParts.Select(part => part.Split(pathFormat.PartsSeparator)).Flatten())
                {
                    _segments.Add(new SegmentInfo(SegmentType.PATH, segment));
                }
            }
        }
        UPath(PathFormat format, List<SegmentInfo> segments)
        {
            _defaultFormat = format;
            _segments = segments.ToList(); // Create a copy
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format">The format of <paramref name="pathParts"/></param>
        /// <param name="pathParts"></param>
        /// <returns></returns>
        public UPath AppendSelf(PathFormat format, params string[] pathParts)
        {
            var subPath = new UPath(format, pathParts);

            return AppendSelf(subPath);
        }
        public UPath AppendSelf(UPath pathParts)
        {
            validateAppend(this, pathParts);
            _segments.AddRange(pathParts._segments);

            return this;
        }
        void validateAppend(UPath sourcePath, UPath pathToAppend)
        {
            if ((!sourcePath.IsRelative && !pathToAppend.IsRelative) || (sourcePath._segments.Count > 0 && !pathToAppend.IsRelative))
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        public UPath GetPath(PathFormat format)
        {
            return new UPath(format, _segments);
        }
        public UPath GetCopy()
        {
            return new UPath(_defaultFormat, _segments);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            if (IsRelative)
            {
                sb.Append(_defaultFormat.PartsSeparator);
            }
            foreach (var i in _segments.Count.Range())
            {
                var segment = _segments[i];
                sb.Append(segment.Segment);
                if (i != _segments.Count - 1 || segment.Type != SegmentType.PATH)
                {
                    sb.Append(getSeparator(segment.Type));
                }

                string getSeparator(SegmentType segmentType)
                {
                    switch (segmentType)
                    {
                        case SegmentType.ROOT:
                            return _defaultFormat.RootMarker;
                        case SegmentType.PATH:
                            return _defaultFormat.PartsSeparator;
                        
                        default:
                            throw new NotSupportedException();
                    }
                }
            }

            return sb.ToString();
        }
    }
}
