using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.Primitives;
using SixLabors.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SixLabors.Svg.Dom
{
    internal struct SvgPathOperation
    {
        public Operations Op { get; private set; }
        public bool IsRelative { get; private set; }

        public SvgUnitValue? X { get; private set; }
        public SvgUnitValue? Y { get; private set; }

        public SvgUnitValue? X1 { get; private set; }
        public SvgUnitValue? Y1 { get; private set; }

        public SvgUnitValue? X2 { get; private set; }
        public SvgUnitValue? Y2 { get; private set; }

        public static IEnumerable<SvgPathOperation> Parse(string path)
        {
            List<SvgPathOperation> operations = new List<SvgPathOperation>();
            char cmd = ' ';
            for (var i = 0; i < path.Length; i++)
            {
                switch (path[i])
                {
                    case 'Z':
                    case 'z':
                        {
                            operations.Add(SvgPathOperation.ClosePath());
                        }
                        break;
                    case 'M':
                    case 'm':
                        {
                            var relative = path[i] == 'm';
                            if (!TryReadFloat(in path, ref i, out var x) || !TryReadFloat(in path, ref i, out var y))
                            {
                                return null;
                            }

                            operations.Add(SvgPathOperation.MoveTo(x, y, relative));
                            while (TryReadFloat(in path, ref i, out x) && TryReadFloat(in path, ref i, out y))
                            {
                                operations.Add(SvgPathOperation.LineTo(x, y, relative));
                            }
                        }
                        break;
                    case 'L':
                    case 'l':
                        {
                            var relative = path[i] == 'l';
                            while (TryReadFloat(in path, ref i, out var x) && TryReadFloat(in path, ref i, out var y))
                            {
                                operations.Add(SvgPathOperation.LineTo(x, y, relative));
                            }
                        }
                        break;
                    case 'Q':
                    case 'q':
                        {
                            var relative = path[i] == 'q';
                            while (TryReadFloat(in path, ref i, out var x1) && TryReadFloat(in path, ref i, out var y1)
                                && TryReadFloat(in path, ref i, out var x) && TryReadFloat(in path, ref i, out var y))
                            {
                                operations.Add(SvgPathOperation.QuadraticTo(x1, y1, x, y, relative));
                            }
                        }
                        break;
                    case 'T':
                    case 't':
                        {
                            var relative = path[i] == 't';
                            while (TryReadFloat(in path, ref i, out var x) && TryReadFloat(in path, ref i, out var y))
                            {
                                SvgUnitValue x1;
                                SvgUnitValue y1;
                                var last = operations.Last();

                                if (last.Op != Operations.QuadraticTo)
                                {
                                    x1 = last.X.Value;
                                    y1 = last.Y.Value;
                                }
                                else
                                {
                                    x1 = new SvgUnitValue(last.X.Value.Value + (last.X.Value.Value - last.X1.Value.Value), SvgUnitValue.Units.undefined);
                                    y1 = new SvgUnitValue(last.Y.Value.Value + (last.Y.Value.Value - last.Y1.Value.Value), SvgUnitValue.Units.undefined);
                                }

                                operations.Add(SvgPathOperation.QuadraticTo(x1, y1, x, y, relative));
                            }
                        }
                        break;
                    case 'C':
                    case 'c':
                        {
                            var relative = path[i] == 'c';
                            while (TryReadFloat(in path, ref i, out var x1) && TryReadFloat(in path, ref i, out var y1)
                                && TryReadFloat(in path, ref i, out var x2) && TryReadFloat(in path, ref i, out var y2)
                                && TryReadFloat(in path, ref i, out var x) && TryReadFloat(in path, ref i, out var y))
                            {
                                operations.Add(SvgPathOperation.CubicTo(x1, y1, x2, y2, x, y, relative));
                            }
                        }
                        break;
                    case 'S':
                    case 's':
                        {
                            var relative = path[i] == 's';
                            while (TryReadFloat(in path, ref i, out var x) && TryReadFloat(in path, ref i, out var y)
                                && TryReadFloat(in path, ref i, out var x2) && TryReadFloat(in path, ref i, out var y2))
                            {
                                SvgUnitValue x1;
                                SvgUnitValue y1;
                                var last = operations.Last();

                                if (last.Op != Operations.CubicTo)
                                {
                                    x1 = last.X.Value;
                                    y1 = last.Y.Value;
                                }
                                else
                                {
                                    x1 = new SvgUnitValue(last.X.Value.Value + (last.X.Value.Value - last.X2.Value.Value), SvgUnitValue.Units.undefined);
                                    y1 = new SvgUnitValue(last.Y.Value.Value + (last.Y.Value.Value - last.Y2.Value.Value), SvgUnitValue.Units.undefined);
                                }

                                operations.Add(SvgPathOperation.CubicTo(x1, y1, x2, y2, x, y, relative));
                            }
                        }
                        break;
                    default:
                        if (char.IsWhiteSpace(path[i]) || path[i] == ',' || path[i] == ' ')
                        {
                            continue;
                        }
                        else
                        {
                            return null;
                        }
                }

            }
            return operations;
        }

        private static bool TryReadFloat(in string str, ref int position, out SvgUnitValue value)
        {
            // look for the first character that can be ins float
            int floatStart = 0;

            for (position++; position < str.Length; position++)
            {
                if (floatStart == 0)
                {
                    if (char.IsWhiteSpace(str[position]) || str[position] == ',')
                    {
                        continue;
                    }
                    // we can skip 'whitespace'
                }
                if (char.IsDigit(str[position]) || (str[position] == '-' && floatStart == 0) || str[position] == '.')
                {
                    if (floatStart == 0) { floatStart = position; }
                }
                else
                {
                    position--;
                    break;
                }
            }

            if (floatStart == 0)
            {

                value = SvgUnitValue.Unset;
                return false;
            }

            if (position == str.Length)
            {
                value = new SvgUnitValue(float.Parse(str.Substring(floatStart)), SvgUnitValue.Units.undefined);
            }
            else
            {
                value = new SvgUnitValue(float.Parse(str.Substring(floatStart, position - floatStart + 1)), SvgUnitValue.Units.undefined);
            }

            return true;
        }


        public enum Operations
        {
            MoveTo,
            LineTo,
            ClosePath,
            QuadraticTo,
            CubicTo
        }

        private static SvgPathOperation CubicTo(SvgUnitValue x1, SvgUnitValue y1, SvgUnitValue x2, SvgUnitValue y2, SvgUnitValue x, SvgUnitValue y, bool relative)
        {
            return new SvgPathOperation()
            {
                IsRelative = relative,
                Op = Operations.CubicTo,
                X = x,
                Y = y,
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2
            };
        }
        private static SvgPathOperation QuadraticTo(SvgUnitValue x1, SvgUnitValue y1, SvgUnitValue x, SvgUnitValue y, bool relative)
        {
            return new SvgPathOperation()
            {
                IsRelative = relative,
                Op = Operations.QuadraticTo,
                X = x,
                Y = y,
                X1 = x1,
                Y1 = y1
            };
        }

        public static SvgPathOperation MoveTo(SvgUnitValue x, SvgUnitValue y, bool relative)
        {
            return new SvgPathOperation()
            {
                IsRelative = relative,
                Op = Operations.MoveTo,
                X = x,
                Y = y
            };
        }
        public static SvgPathOperation LineTo(SvgUnitValue x, SvgUnitValue y, bool relative)
        {
            return new SvgPathOperation()
            {
                IsRelative = relative,
                Op = Operations.LineTo,
                X = x,
                Y = y
            };
        }

        internal static SvgPathOperation ClosePath()
        {
            return new SvgPathOperation()
            {
                Op = Operations.ClosePath,
            };
        }
    }

    internal static class SvgPathOperationExtensions
    {
        private static PointF Point<TPixel>(Image<TPixel> img, PointF currentPoint, SvgUnitValue x, SvgUnitValue y) where TPixel : struct, IPixel<TPixel>
        {
            var p = new PointF(x.AsPixelXAxis(img), y.AsPixelYAxis(img));
            return currentPoint + p;
        }
        public static PointF PointMain<TPixel>(this SvgPathOperation operation, Image<TPixel> img, PointF currentPoint) where TPixel : struct, IPixel<TPixel>
        {
            return Point(img, operation.IsRelative ? currentPoint : default, operation.X.Value, operation.Y.Value);
        }
        public static PointF Point1<TPixel>(this SvgPathOperation operation, Image<TPixel> img, PointF currentPoint) where TPixel : struct, IPixel<TPixel>
        {
            return Point(img, operation.IsRelative ? currentPoint : default, operation.X1.Value, operation.Y1.Value);
        }
        public static PointF Point2<TPixel>(this SvgPathOperation operation, Image<TPixel> img, PointF currentPoint) where TPixel : struct, IPixel<TPixel>
        {
            return Point(img, operation.IsRelative ? currentPoint : default, operation.X2.Value, operation.Y2.Value);
        }

        public static IPath GeneratePath<TPixel>(this IEnumerable<SvgPathOperation> operations, Image<TPixel> img) where TPixel : struct, IPixel<TPixel>
        {
            var pb = new PathRenderer();

            foreach (var op in operations)
            {
                switch (op.Op)
                {
                    case SvgPathOperation.Operations.MoveTo:
                        pb.MoveTo(op.PointMain(img, pb.CurrentPoint));
                        break;
                    case SvgPathOperation.Operations.LineTo:
                        pb.LineTo(op.PointMain(img, pb.CurrentPoint));
                        break;
                    case SvgPathOperation.Operations.QuadraticTo:
                        pb.QuadraticBezierTo(op.Point1(img, pb.CurrentPoint), op.PointMain(img, pb.CurrentPoint));
                        break;
                    case SvgPathOperation.Operations.CubicTo:
                        pb.CubicBezierTo(op.Point1(img, pb.CurrentPoint), op.Point2(img, pb.CurrentPoint), op.PointMain(img, pb.CurrentPoint));
                        break;
                    case SvgPathOperation.Operations.ClosePath:
                        pb.Close();
                        break;
                    default:
                        break;
                }
            }

            return pb.Path();
        }

        internal class PathRenderer
        {
            /// <summary>
            /// The builder. TODO: Should this be a property?
            /// </summary>
            // ReSharper disable once InconsistentNaming
            private readonly PathBuilder builder;
            private readonly List<IPath> paths = new List<IPath>();
            private PointF currentPoint = default(PointF);
            private PointF initalPoint = default(PointF);

            public PointF CurrentPoint => currentPoint;
            public PointF InitalPoint => initalPoint;

            /// <summary>
            /// Initializes a new instance of the <see cref="BaseGlyphBuilder"/> class.
            /// </summary>
            public PathRenderer()
            {
                // glyphs are renderd realative to bottom left so invert the Y axis to allow it to render on top left origin surface
                this.builder = new PathBuilder();
            }

            /// <summary>
            /// Gets the paths that have been rendered by this.
            /// </summary>
            public IPath Path()
            {
                return this.builder.Build();
            }

            /// <summary>
            /// Draws a cubic bezier from the current point  to the <paramref name="point"/>
            /// </summary>
            /// <param name="secondControlPoint">The second control point.</param>
            /// <param name="thirdControlPoint">The third control point.</param>
            /// <param name="point">The point.</param>
            public void CubicBezierTo(PointF secondControlPoint, PointF thirdControlPoint, PointF point)
            {
                this.builder.AddBezier(this.currentPoint, secondControlPoint, thirdControlPoint, point);
                this.currentPoint = point;
            }

            /// <summary>
            /// Draws a line from the current point  to the <paramref name="point"/>.
            /// </summary>
            /// <param name="point">The point.</param>
            public void LineTo(PointF point)
            {
                this.builder.AddLine(this.currentPoint, point);
                this.currentPoint = point;
            }

            /// <summary>
            /// Moves to current point to the supplied vector.
            /// </summary>
            /// <param name="point">The point.</param>
            public void MoveTo(PointF point)
            {
                this.builder.StartFigure();
                this.currentPoint = point;
                this.initalPoint = point;
            }

            /// <summary>
            /// Draws a quadratics bezier from the current point  to the <paramref name="point"/>
            /// </summary>
            /// <param name="secondControlPoint">The second control point.</param>
            /// <param name="point">The point.</param>
            public void QuadraticBezierTo(PointF secondControlPoint, PointF point)
            {
                this.builder.AddBezier(this.currentPoint, secondControlPoint, point);
                this.currentPoint = point;
            }

            internal void Close()
            {
                this.LineTo(initalPoint);
                //this.builder.CloseFigure();
                //this.currentPoint = this.initalPoint;
            }
        }
    }

}
