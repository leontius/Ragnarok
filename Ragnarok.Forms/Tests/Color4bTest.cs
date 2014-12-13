﻿#if TESTS
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;

using Ragnarok.Utility;

namespace Ragnarok.Forms.Tests
{
    /// <summary>
    /// Ragnarok.Utility.Color4bのテストを行います。
    /// </summary>
    /// <remarks>
    /// Drawing.Colorの色リストと値を比較するため、
    /// このアセンブリでテストを行っています。
    /// </remarks>
    [TestFixture]
    public sealed class Color4bTest
    {
        private List<Tuple<string, Color4b>> GetColor4bList()
        {
            var flags = BindingFlags.GetProperty | BindingFlags.Public |
                        BindingFlags.Static;

            return typeof(Color4bs)
                .GetProperties(flags)
                .Where(_ => _ != null)
                .Where(_ => _.PropertyType == typeof(Color4b))
                .Select(_ => Tuple.Create(_.Name, (Color4b)_.GetValue(null, null)))
                .ToList();
        }

        private List<Tuple<string, Color>> GetDrawingColorList()
        {
            var flags = BindingFlags.GetProperty | BindingFlags.Public |
                        BindingFlags.Static;

            return typeof(Color)
                .GetProperties(flags)
                .Where(_ => _ != null)
                .Where(_ => _.PropertyType == typeof(Color))
                .Select(_ => Tuple.Create(_.Name, (Color)_.GetValue(null, null)))
                .ToList();
        }

        /// <summary>
        /// RagnarokとDrawingの色リストを比較します。
        /// </summary>
        [Test]
        public void ColorListTest()
        {
            var ragnarokList = GetColor4bList();
            var drawingList = GetDrawingColorList();

            ragnarokList.Sort((x, y) => x.Item1.CompareTo(y.Item1));
            drawingList.Sort((x, y) => x.Item1.CompareTo(y.Item1));

            Assert.AreEqual(drawingList.Count(), ragnarokList.Count());
            for (var i = 0; i < drawingList.Count(); ++i)
            {
                var ragnarokPair = ragnarokList[i];
                var drawingPair = drawingList[i];

                Assert.AreEqual(drawingPair.Item1, ragnarokPair.Item1);
                Assert.AreEqual(drawingPair.Item2.A, ragnarokPair.Item2.A);
                Assert.AreEqual(drawingPair.Item2.R, ragnarokPair.Item2.R);
                Assert.AreEqual(drawingPair.Item2.G, ragnarokPair.Item2.G);
                Assert.AreEqual(drawingPair.Item2.B, ragnarokPair.Item2.B);
            }
        }
    }
}
#endif
