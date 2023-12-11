using Microcharts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Entry = Microcharts.ChartEntry;
using LineChart = Microcharts.LineChart;


namespace PocketBookClient.UI
{
   public class MultiLineChart : LineChart
    {
        public List<List<Entry>> multiline_entries { get; set; }

        public List<String> legend_names { get; set; }

        private bool inited = false;

        private float multiline_min;
        private float multiline_max;


        private void init()
        {
            if (inited)
            {
                return;
            }

            inited = true;

            foreach (List<Entry> l in multiline_entries)
            {
                foreach (Entry e in l)
                {
                    if (e.Value > multiline_max)
                    {
                        multiline_max = e.Value??0;
                    }
                    if (e.Value < multiline_min)
                    {
                        multiline_min = e.Value ?? 0;
                    }
                }
            }
        }

        public override void DrawContent(SKCanvas canvas, int width, int height)
        {
            init();


            Entries = multiline_entries[0];
            var valueLabelSizes = MeasureValueLabels();
            var footerHeight = CalculateFooterHeight(valueLabelSizes.Values.ToArray());
            var headerHeight = CalculateHeaderHeight(valueLabelSizes);
            var itemSize = CalculateItemSize(width, height, footerHeight, headerHeight);
            var origin = CalculateYOrigin(itemSize.Height, headerHeight);

            foreach (List<Entry> l in multiline_entries)
            {
                Entries = l;
                var points = this.CalculateMultilinePoints(itemSize, origin, headerHeight);

                this.DrawLine(canvas, points, itemSize);
                this.DrawPoints(canvas, points);

                if (multiline_entries.IndexOf(l) == 0)
                {
                    this.DrawArea(canvas, points, itemSize, origin);
               //     this.DrawFooter(canvas, points, itemSize, height, footerHeight);
                    //DrawValueLabel(canvas, points, itemSize, height, valueLabelSizes);
                }
            }
        }

        private Dictionary<ChartEntry, SKRect> MeasureValueLabels()
        {
            var dict = new Dictionary<ChartEntry, SKRect>();
            using (var paint = new SKPaint())
            {
                paint.TextSize = LabelTextSize;
                foreach (var e in Entries)
                {
                    SKRect bounds;
                    if (string.IsNullOrEmpty(e.ValueLabel))
                    {
                        bounds = SKRect.Empty;
                    }
                    else
                    {
                        bounds = new SKRect();
                        paint.MeasureText(e.ValueLabel, ref bounds);
                    }

                    dict.Add(e, bounds);
                }
            }

            return dict;
        }

        protected float CalculateYOrigin(float itemHeight, float headerHeight)
        {
            if (MaxValue <= 0)
            {
                return headerHeight;
            }

            if (MinValue > 0)
            {
                return headerHeight + itemHeight;
            }

            return headerHeight + ((MaxValue / ValueRange) * itemHeight);
        }

        protected SKSize CalculateItemSize(int width, int height, float footerHeight, float headerHeight)
        {
            var total = Entries.Count();
            var w = (width - ((total + 1) * Margin)) / total;
            var h = height - Margin - footerHeight - headerHeight;
            return new SKSize(w, h);
        }


        private void DrawLegend(SKCanvas canvas)
        {
            if (!legend_names.Any()) { return; }

            List<SKColor> colors = new List<SKColor> { };

            foreach (List<Entry> l in multiline_entries)
            {
                colors.Add(l[0].Color);
            }

            int radius_size = 20;

            using (var paint = new SKPaint())
            {
                paint.TextSize = this.LabelTextSize;
                paint.IsAntialias = true;
                paint.IsStroke = false;

                float x = radius_size * 2;
                float y = 50;

                foreach (String legend in legend_names)
                {
                    paint.Color = SKColor.Parse("#000000");
                    canvas.DrawText(legend, x + radius_size, y, paint);

                    paint.Color = colors.ElementAt(legend_names.IndexOf(legend));
                    canvas.DrawCircle(x, y - radius_size / 2 - radius_size / 4, radius_size, paint);

                    x += radius_size * 2 + this.LabelTextSize * (legend.Length / 2 + 2);
                }
            }
        }

        private SKPoint[] CalculateMultilinePoints(SKSize itemSize, float origin, float headerHeight)
        {
            var result = new List<SKPoint>();

            for (int i = 0; i < this.Entries.Count(); i++)
            {
                var entry = this.Entries.ElementAt(i);

                var x = this.Margin + (itemSize.Width / 2) + (i * (itemSize.Width + this.Margin));
                var y = headerHeight + (((multiline_max - entry.Value) / (multiline_max - multiline_min)) * itemSize.Height);
                var point = new SKPoint(x, y:0);
                result.Add(point);
            }

            return result.ToArray();
        }
        /// <summary>
        /// Calculates the height of the footer.
        /// </summary>
        /// <returns>The footer height.</returns>
        /// <param name="valueLabelSizes">Value label sizes.</param>
        /// <param name="orientation">orientation of content</param>
        protected float CalculateFooterHeight(SKRect[] valueLabelSizes, Orientation orientation = Orientation.Vertical)
        {
            var result = Margin;

            if (Entries.Any(e => !string.IsNullOrEmpty(e.Label)))
            {
                if (orientation == Orientation.Vertical)
                {
                    var maxValueWidth = valueLabelSizes.Max(x => x.Width);
                    if (maxValueWidth > 0)
                    {
                        result += maxValueWidth + Margin;
                    }
                }
                else
                {
                    result += LabelTextSize + Margin;
                }
            }

            return result;
        }

        protected  void DrawFooter(SKCanvas canvas, string[] labels, SKRect[] labelSizes, SKPoint[] points, SKSize itemSize, int height, float footerHeight)
        {
            DrawLabels(canvas,
                            labels,
                            points.Select(p => new SKPoint(p.X, height - footerHeight + Margin)).ToArray(),
                            labelSizes,
                            Entries.Select(x => LabelColor).ToArray(),
                            LabelOrientation,
                            false,
                            itemSize,
                            height);
        }

        protected  void DrawPoints(SKCanvas canvas, SKPoint[] points)
        {
            if (points.Length > 0 && PointMode != PointMode.None)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    var entry = Entries.ElementAt(i);
                    var point = points[i];
                  //  canvas.DrawPoint(point, entry.Color, PointSize, PointMode);
                }
            }
        }

        protected  void DrawLabels(SKCanvas canvas, string[] texts, SKPoint[] points, SKRect[] sizes, SKColor[] colors, Orientation orientation, bool isTop, SKSize itemSize, float height)
        {
            if (points.Length > 0)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    var entry = Entries.ElementAt(i);
                    if (!string.IsNullOrEmpty(entry.ValueLabel))
                    {
                        DrawLabel(canvas, orientation, isTop, itemSize, points[i], colors[i], sizes[i], texts[i], LabelTextSize, Typeface);
                    }
                }
            }
        }

        protected  void DrawAreas(SKCanvas canvas, SKPoint[] points, SKSize itemSize, float origin, float headerHeight)
        {
            DrawArea(canvas, points, itemSize, origin);
            DrawLine(canvas, points, itemSize);
        }

        protected void DrawLine(SKCanvas canvas, SKPoint[] points, SKSize itemSize)
        {
            if (points.Length > 1 && LineMode != LineMode.None)
            {
                using (var paint = new SKPaint
                {
                    Style = SKPaintStyle.Stroke,
                    Color = SKColors.White,
                    StrokeWidth = LineSize,
                    IsAntialias = true,
                })
                {
                    using (var shader = CreateXGradient(points))
                    {
                        paint.Shader = shader;

                        var path = new SKPath();

                        path.MoveTo(points.First());

                        var last = (LineMode == LineMode.Spline) ? points.Length - 1 : points.Length;
                        for (int i = 0; i < last; i++)
                        {
                            if (LineMode == LineMode.Spline)
                            {
                                var entry = Entries.ElementAt(i);
                                var nextEntry = Entries.ElementAt(i + 1);
                                var cubicInfo = CalculateCubicInfo(points, i, itemSize);
                                path.CubicTo(cubicInfo.control, cubicInfo.nextControl, cubicInfo.nextPoint);
                            }
                            else if (LineMode == LineMode.Straight)
                            {
                                path.LineTo(points[i]);
                            }
                        }

                        canvas.DrawPath(path, paint);
                    }
                }
            }
        }

        protected void DrawArea(SKCanvas canvas, SKPoint[] points, SKSize itemSize, float origin)
        {
            if (LineAreaAlpha > 0 && points.Length > 1)
            {
                using (var paint = new SKPaint
                {
                    Style = SKPaintStyle.Fill,
                    Color = SKColors.White,
                    IsAntialias = true,
                })
                {
                    using (var shaderX = CreateXGradient(points, (byte)(LineAreaAlpha * AnimationProgress)))
                    using (var shaderY = CreateYGradient(points, (byte)(LineAreaAlpha * AnimationProgress)))
                    {
                        paint.Shader = EnableYFadeOutGradient ? SKShader.CreateCompose(shaderY, shaderX, SKBlendMode.SrcOut) : shaderX;

                        var path = new SKPath();

                        path.MoveTo(points.First().X, origin);
                        path.LineTo(points.First());

                        var last = (LineMode == LineMode.Spline) ? points.Length - 1 : points.Length;
                        for (int i = 0; i < last; i++)
                        {
                            if (LineMode == LineMode.Spline)
                            {
                                var entry = Entries.ElementAt(i);
                                var nextEntry = Entries.ElementAt(i + 1);
                                var cubicInfo = CalculateCubicInfo(points, i, itemSize);
                                path.CubicTo(cubicInfo.control, cubicInfo.nextControl, cubicInfo.nextPoint);
                            }
                            else if (LineMode == LineMode.Straight)
                            {
                                path.LineTo(points[i]);
                            }
                        }

                        path.LineTo(points.Last().X, origin);

                        path.Close();

                        canvas.DrawPath(path, paint);
                    }
                }
            }
        }

        private (SKPoint point, SKPoint control, SKPoint nextPoint, SKPoint nextControl) CalculateCubicInfo(SKPoint[] points, int i, SKSize itemSize)
        {
            var point = points[i];
            var nextPoint = points[i + 1];
            var controlOffset = new SKPoint(itemSize.Width * 0.8f, 0);
            var currentControl = point + controlOffset;
            var nextControl = nextPoint - controlOffset;
            return (point, currentControl, nextPoint, nextControl);
        }

        private SKShader CreateXGradient(SKPoint[] points, byte alpha = 255)
        {
            var startX = points.First().X;
            var endX = points.Last().X;
            var rangeX = endX - startX;

            return SKShader.CreateLinearGradient(
                new SKPoint(startX, 0),
                new SKPoint(endX, 0),
                Entries.Select(x => x.Color.WithAlpha(alpha)).ToArray(),
                null,
                SKShaderTileMode.Clamp);
        }

        private SKShader CreateYGradient(SKPoint[] points, byte alpha = 255)
        {
            var startY = points.Max(i => i.Y);
            var endY = 0;

            return SKShader.CreateLinearGradient(
                new SKPoint(0, startY),
                new SKPoint(0, endY),
                new[] { SKColors.White.WithAlpha(alpha), SKColors.White.WithAlpha(0) },
                null,
                SKShaderTileMode.Clamp);
        }

        internal static void DrawLabel(SKCanvas canvas, Orientation orientation, bool isTop, SKSize itemSize, SKPoint point, SKColor color, SKRect bounds, string text, float textSize, SKTypeface typeface)
        {
            using (new SKAutoCanvasRestore(canvas))
            {
                using (var paint = new SKPaint())
                {
                    paint.TextSize = textSize;
                    paint.IsAntialias = true;
                    paint.Color = color;
                    paint.IsStroke = false;
                    paint.Typeface = typeface;

                    if (orientation == Orientation.Vertical)
                    {
                        var y = point.Y;

                        if (isTop)
                        {
                            y -= bounds.Width;
                        }

                        canvas.RotateDegrees(90);
                        canvas.Translate(y, -point.X + (bounds.Height / 2));
                    }
                    else
                    {
                        if (bounds.Width > itemSize.Width)
                        {
                            text = text.Substring(0, Math.Min(3, text.Length));
                            paint.MeasureText(text, ref bounds);
                        }

                        if (bounds.Width > itemSize.Width)
                        {
                            text = text.Substring(0, Math.Min(1, text.Length));
                            paint.MeasureText(text, ref bounds);
                        }

                        var y = point.Y;

                        if (isTop)
                        {
                            y -= bounds.Height;
                        }

                        canvas.Translate(point.X - (bounds.Width / 2), y);
                    }

                    canvas.DrawText(text, 0, 0, paint);
                }
            }
        }

    }
}
