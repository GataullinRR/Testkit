using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Extensions;
using Utilities.Types;

namespace Vectors
{
    public class Polygon
    {
        public enum RelPoint : byte
        {
            INSIDE,
            OUTSIDE,
            BOUNDARY
        }

        public readonly V2 Center;
        public readonly V4 Rect;
        public readonly V2[] Vertices;
        public readonly Edge[] Edges;

        public Polygon(IEnumerable<V2> vertices)
        {
            Vertices = vertices.ToArray();
            Edges = extractEdges();
            Center = calcCenter();
            Rect = calcRect();

            Edge[] extractEdges()
            {
                if (Vertices.Length < 2)
                {
                    return new Edge[0];
                }
                V2 LastVert = Vertices[0];
                V2 CurrVert = V2.Zero;
                Edge[] Buff = new Edge[] { };
                Array.Resize(ref Buff, Vertices.Length);
                for (int i = 0; i < Vertices.Length - 1; i++)
                {
                    CurrVert = Vertices[i + 1];
                    Buff[i] = new Edge(LastVert, CurrVert);
                    LastVert = CurrVert;
                }
                CurrVert = Vertices[0];
                Buff[Vertices.Length - 1] = new Edge(LastVert, CurrVert);
                LastVert = CurrVert;

                return Buff;
            }
            V2 calcCenter()
            {
                //POINT, MASS
                Dictionary<V2, double> Mass = new Dictionary<V2, double>();
                Edge E;
                for (int i = 0; i < Edges.Length; i++)
                {
                    E = Edges[i];
                    Mass.Add(E.Middle, E.Len);
                }
                double X = 0;
                double Y = 0;
                foreach (KeyValuePair<V2, double> KVP in Mass)
                {
                    X += KVP.Key.X;
                    Y += KVP.Key.Y;
                }
                X /= Edges.Length;
                Y /= Edges.Length;

                return new V2(X, Y);
            }
            V4 calcRect()
            {
                if (Vertices.Length == 0)
                    return V4.Zero;

                V2 V = Vertices[0];
                var MinX = V.X;
                var MinY = V.Y;
                var MaxX = V.X;
                var MaxY = V.Y;
                for (int i = 1; i < Vertices.Length; i++)
                {
                    V = Vertices[i];
                    if (V.X < MinX) MinX = V.X;
                    else if (V.X > MaxX)
                        MaxX = V.X;

                    if (V.Y < MinY) MinY = V.Y;
                    else if (V.Y > MaxY)
                        MaxY = V.Y;
                }

                return new V4(MinX, MinY, MaxX - MinX, MaxY - MinY);
            }
        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="vertices">It must be a convex shape!</param>
        /// <param name="width">Shouldn't be too big comparing to the size of the <paramref name="vertices"/> 
        /// or overlapping will occur!</param>
        /// <returns>Doesn't check calculated <see cref="Polygon"/> for overlapping!</returns>
        //public static Polygon CreateClosedPolygonFromCurve(IEnumerable<V2> vertices, double width)
        //{
        //    if (!vertices.CountNotLessOrEqual(3))
        //    {
        //        throw new ArgumentException("Must be at least 3 vertices");
        //    }

        //    vertices = vertices.MakeCached();
        //    return new Polygon(getPolygonVertices());

        //    IEnumerable<V2> getPolygonVertices()
        //    {
        //        var pushLength = width / 2;
        //        foreach (var normal in getResultantNormals())
        //        {

        //        }
        //    }

        //    // Returns normal for each point. It's implied that normal to the point - 
        //    // is resultant between same directed normals to two edges it's located between.
        //    IEnumerable<V2> getResultantNormals()
        //    {
        //        var outerNormalsBuffer = new DisplaceCollection<V2>(2);
        //        var outerNormals = getOuterNormals();
        //        var firstNormal = outerNormals.FirstItem();
        //        outerNormalsBuffer.Add(firstNormal);
        //        foreach (var normal in outerNormals.SkipFirstItem().Concat(firstNormal))
        //        {
        //            outerNormalsBuffer.Add(normal);
        //            var resultantNormal = (outerNormalsBuffer[0] + outerNormalsBuffer[1]).Norm;
        //            yield return resultantNormal;
        //        }

        //        IEnumerable<V2> getOuterNormals()
        //        {
        //            var center = new Polygon(vertices).Center;
        //            var edgePoints = new DisplaceCollection<V2>(2);
        //            edgePoints.Add(vertices.FirstItem());
        //            foreach (var verticle in vertices.SkipFirstItem().Concat(vertices.FirstItem()))
        //            {
        //                edgePoints.Add(verticle);
        //                var edge = new Edge(edgePoints[0], edgePoints[1]);
        //                yield return edge.GetOuterNormal(center);
        //            }
        //        }
        //    }
        //}

        //#region ##### TRANSFORMATION #####

        //public void Rotate(float angle, V2 rotPoint)
        //{
        //    int VLeng = Vertices.Length;
        //    for (int i = 0; i < VLeng; i++)
        //    {
        //        V2 V = Vertices[i];
        //        Vertices[i].X = rotPoint.X + (V.X - rotPoint.X) * Math.Cos(angle) - (V.Y - rotPoint.Y) * Math.Sin(angle);
        //        Vertices[i].Y = rotPoint.Y + (V.Y - rotPoint.Y) * Math.Cos(angle) + (V.X - rotPoint.X) * Math.Sin(angle);
        //    }
        //}
        //public void Resize(V2 scale)
        //{
        //    int VLeng = Vertices.Length;
        //    for (int i = 0; i < VLeng; i++) Vertices[i] *= scale;
        //}
        //public void Add(V2 vertex)
        //{
        //    if (Vertices == null) Vertices = new V2[0] { };
        //    int VLeng = Vertices.Length;
        //    Array.Resize<V2>(ref Vertices, VLeng + 1);
        //    Vertices[VLeng] = vertex;
        //}
        //public void Add(V2[] vertices)
        //{
        //    if (Vertices == null) Vertices = new V2[0] { };
        //    int VLeng = Vertices.Length;
        //    Array.Resize<V2>(ref Vertices, VLeng + vertices.Length);
        //    Array.Copy(vertices, 0, Vertices, VLeng, Vertices.Length);
        //}
        //public void Remove(int index)
        //{
        //    if (index > Vertices.Length) throw new ArgumentOutOfRangeException("index");
        //    int VLeng = Vertices.Length;
        //    V2[] Buff = new V2[] { };
        //    Array.Resize<V2>(ref Buff, VLeng - 1);
        //    Array.Copy(Vertices, Buff, VLeng - (VLeng - index));

        //    Array.Copy(Vertices, index + 1, Buff, index, VLeng - index - 1);
        //    Vertices = Buff;
        //}
        //public void Insert(int index, V2 vertex)
        //{
        //    if (index > Vertices.Length) throw new ArgumentOutOfRangeException("index");
        //    Array.Resize<V2>(ref Vertices, Vertices.Length + 1);
        //    Array.Copy(Vertices, index, Vertices, index + 1, Vertices.Length - 1 - index);
        //    Vertices[index] = vertex;
        //}
        //public void AddOffcet(V2 off)
        //{
        //    for (int i = 0; i < Vertices.Length; i++) Vertices[i] += off;
        //}

        //#endregion

        #region ##### HELPERS #####

        public RelPoint Contains(V2 point)
        {
            int parity = 0;
            for (int i = 0; i < Edges.Length; i++)
            {
                Edge e = Edges[i];
                switch (e.DeterminePountPos(point))
                {
                    case Edge.PointRelativePos.TOUCHING:
                        return RelPoint.BOUNDARY;
                    case Edge.PointRelativePos.CROSSING:
                        parity = 1 - parity;
                        break;
                }
            }

            return parity == 1 
                ? RelPoint.INSIDE 
                : RelPoint.OUTSIDE;
        }
        public bool Contains(Polygon polygon)
        {
            int P_VLeng = polygon.Vertices.Length;
            for (int k = 0; k < P_VLeng; k++)
                if (Contains(polygon.Vertices[k]) == RelPoint.OUTSIDE) return false;
            return true;
        }

        public bool Collide(Polygon polygon)
        {
            Edge[] P_Es = polygon.Edges;
            int P_EsLen = P_Es.Length;
            Edge[] Es = Edges;
            int EsLen = Es.Length;
            Edge P_E;
            for (int i = 0; i < P_EsLen; i++)
            {
                P_E = P_Es[i];
                for (int k = 0; k < EsLen; k++)
                    if (Es[k].CalcIntersectionPoint(P_E) != null) return true;
            }
            return false;
        }

        #endregion
    }
}
