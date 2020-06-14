namespace Vectors
{
    public readonly struct Edge
    {
        public enum PointRelativePos : byte
        {
            TOUCHING,
            CROSSING, 
            INESSENTIAL 
        }

        public readonly V2 Origin;
        public readonly V2 Destination;

        public double Len => (Origin - Destination).Len;
        public V2 Middle => (Origin + Destination) / 2;

        public Edge(V2 origin, V2 destination)
        {
            Origin = origin;
            Destination = destination;
        }

        public V2.RelativePos Classify(V2 point)
        {
            return point.Classify(Origin, Destination);
        }

        public PointRelativePos DeterminePountPos(V2 point)
        {
            V2 V = Origin;
            V2 W = Destination;

            switch (Classify(point))
            {
                case V2.RelativePos.LEFT:
                    return ((V.Y < point.Y) && (point.Y <= W.Y)) 
                        ? PointRelativePos.CROSSING 
                        : PointRelativePos.INESSENTIAL;
                case V2.RelativePos.RIGHT:
                    return ((W.Y < point.Y) && (point.Y <= V.Y)) 
                        ? PointRelativePos.CROSSING 
                        : PointRelativePos.INESSENTIAL;
                case V2.RelativePos.BETWEEN:
                case V2.RelativePos.ORIGIN:
                case V2.RelativePos.DESTINATION:
                    return PointRelativePos.TOUCHING;
                default:
                    return PointRelativePos.INESSENTIAL;
            }
        }

        public V2? CalcIntersectionPoint(Edge edge)
        {
            V2 P1 = Origin;
            V2 P2 = Destination;
            V2 P3 = edge.Origin;
            V2 P4 = edge.Destination;

            double U12 =
                ((P4.X - P3.X) * (P1.Y - P3.Y) - (P4.Y - P3.Y) * (P1.X - P3.X)) /
                ((P4.Y - P3.Y) * (P2.X - P1.X) - (P4.X - P3.X) * (P2.Y - P1.Y));
            if (U12 < 0 || U12 > 1)
            {
                return null;
            }

            double U34 =
                ((P2.X - P1.X) * (P1.Y - P3.Y) - (P2.Y - P1.Y) * (P1.X - P3.X)) /
                ((P4.Y - P3.Y) * (P2.X - P1.X) - (P4.X - P3.X) * (P2.Y - P1.Y));
            if (U34 < 0 || U34 > 1)
            {
                return null;
            }

            return new V2(P1.X + U12 * (P2.X - P1.X), P1.Y + U12 * (P2.Y - P1.Y));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="innerPoint">The point normal is directed from.</param>
        /// <returns></returns>
        public V2 GetOuterNormal(V2 innerPoint)
        {
            var n = (Origin - Destination).LPerp.Norm; // Order doesn't matter
            var dot = n.Dot(Origin - innerPoint); // Origin or Destination - doesn't matter 
            return dot >= 0 // is innerPoint lying past the edge?
                ? n
                : n.Inv;
        }
    }
}
