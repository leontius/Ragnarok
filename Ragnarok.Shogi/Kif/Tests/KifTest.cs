﻿#if TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Ragnarok.Shogi.Kif.Tests
{
    using File.Tests;

    [TestFixture()]
    internal sealed class KifTest
    {
        /// <summary>
        /// 棋譜から手数を取得します。
        /// </summary>
        public static int? GetMoveCount(string text)
        {
            var m = Regex.Match(text,
                string.Format(@"(\d+)\s+({0})", KifUtil.SpecialMoveText),
                RegexOptions.Multiline);
            if (!m.Success)
            {
                return null;
            }

            return int.Parse(m.Groups[1].Value);
        }

        /// <summary>
        /// 棋譜の読み込みテストを行います。
        /// </summary>
        private static void TestKif(string path)
        {
            var text = string.Empty;
            using (var reader = new StreamReader(path, KifuObject.DefaultEncoding))
            {
                text = reader.ReadToEnd();
            }

            // 棋譜の読み込み
            var kifu = KifuReader.LoadFrom(text);
            Assert.NotNull(kifu);

            // 手数を取得
            var countObj = GetMoveCount(text);
            Assert.NotNull(countObj);

            var count = countObj.Value + (kifu.Error != null ? -2 : -1);
            Assert.LessOrEqual(count, kifu.MoveList.Count());

            // 入出力テストを行います。
            TestUtil.ReadWriteTest(kifu, KifuFormat.Kif, count);
        }

        /// <summary>
        /// すべての棋譜のテストを行います。
        /// </summary>
#if false
        [Test()]
#endif
        public void AllKifTest()
        {
            TestUtil.KifTest(
                "file.list", "*.kif",
                _ => TestKif(_));
        }

        /// <summary>
        /// 正しく読み込めていない棋譜のテストを行います。
        /// </summary>
        [Test()]
        public void Test()
        {
            /*var pathList = TestUtil.LoadPathList("file.list");

            //var path = @"E:\Dropbox\NicoNico\shogi\test_kif\1600-1979\kif\18720111その他大矢小野有105.KIF";
            foreach (var path in TestUtil.FileList("*.kif", pathList))
            {
                Console.WriteLine(path);

                TestKif(path);
            }*/
        }

        /// <summary>
        /// 変化が含まれる棋譜のテスト
        /// </summary>
        [Test()]
        public void VariationTest()
        {
            var text = SampleKif.VariationKif;

            // 棋譜の読み込み
            var kifu = KifuReader.LoadFrom(text);
            Assert.NotNull(kifu);
            Assert.Null(kifu.Error);

            // 手数を確認
            var count = 133;
            Assert.AreEqual(count, kifu.MoveList.Count());

            // 入出力テストを行います。
            TestUtil.ReadWriteTest(kifu, KifuFormat.Kif, count);
        }

        /// <summary>
        /// 消費時間のテスト
        /// </summary>
        [Test()]
        public void DurationTest()
        {
            var content =
                "手合割：平手\n" +
                "先手：人間\n" +
                "後手：test\n" +
                "手数----指手---------消費時間--\n" +
                "   1 ７六歩(77)    (01:38 / 00:01:38)\n" +
                "   2 ３四歩(33)    (00:01 / 00:00:01)\n" +
                "   3 ２六歩(27)    (02:17 / 00:03:55)\n" +
                "   4 ３三角(22)    (00:01 / 00:00:02)\n" +
                "   5 同　角成(88)  (01:38 / 00:05:33)\n" +
                "   6 同　桂(21)    (00:01 / 00:00:03)\n" +
                "   7 ６八玉(59)    (02:19 / 00:07:52)\n" +
                "   8 ４二飛(82)    (00:01 / 00:00:04)\n" +
                "   9 ７八玉(68)    (01:26 / 00:09:18)\n" +
                "  10 ６二玉(51)    (00:01 / 00:00:05)\n" +
                "  11 ８八玉(78)    (01:10 / 00:10:28)\n" +
                "  12 ７二玉(62)    (01:01 / 00:01:06)\n" +
                "  13 ４八銀(39)    (01:40 / 00:12:08)";
            var durations = new string[]
            {
                "00:01:38", "00:00:01", "00:02:17", "00:00:01", "00:01:38",
                "00:00:01", "00:02:19", "00:00:01", "00:01:26", "00:00:01",
                "00:01:10", "00:01:01", "00:01:40",
            };
            var totalDurations = new string[]
            {
                "00:01:38", "00:00:01", "00:03:55", "00:00:02", "00:05:33",
                "00:00:03", "00:07:52", "00:00:04", "00:09:18", "00:00:05",
                "00:10:28", "00:01:06", "00:12:08",
            };

            // 棋譜の読み込み
            var kifu = KifuReader.LoadFrom(content);
            Assert.NotNull(kifu);

            var node = kifu.RootNode;
            for (var i = 0; i < durations.Count(); ++i)
            {
                node = node.NextNode;
                Assert.AreEqual(TimeSpan.Parse(durations[i]), node.Duration);
                Assert.AreEqual(TimeSpan.Parse(totalDurations[i]), node.TotalDuration);
            }
        }

        /// <summary>
        /// コメントが含まれる棋譜のテスト
        /// </summary>
        [Test()]
        public void CommentTest()
        {
            var content =
                "# ----  サンプル棋譜ファイル  ----\n" +
                "手合割：平手\n" +
                "手数----指手---------消費時間--\n" +
                "*今王将戦は因縁の師弟対決となりました。\n" +
                "*受けの気風の師匠に対して、弟子の平方は完全な攻め将棋。\n" +
                "   1 ７六歩(77)   (00:00:01 / 00:00:01)\n" +
                "*最初はオーソドックスに\n" +
                "   2 ３四歩(33)   (00:00:01 / 00:00:01)\n" +
                "   3 ２六歩(27)   (00:00:02 / 00:00:01)\n" +
                "   4 ８四歩(83)   (00:00:03 / 00:00:01)\n" +
                "*よくある進行になりました。\n" +
                "   5 ２五歩(26)   (00:00:03 / 00:00:01)\n" +
                "   6 ８五歩(84)   (00:00:04 / 00:00:01)\n" +
                "   7 ２四歩(25)   (00:00:04 / 00:00:01)\n" +
                "   8 同　歩(23)   (00:00:05 / 00:00:01)\n" +
                "   9 同　飛(28)   (00:00:05 / 00:00:01)\n" +
                "  10 ８六歩(85)   (00:00:05 / 00:00:01)\n" +
                "  11 同　歩(87)   (00:00:05 / 00:00:01)\n" +
                "  12 同　飛(82)   (00:00:06 / 00:00:01)\n" +
                "*お互い金を上がらずにここまで来ました。しかもノータイム。\n" +
                "*これは先手側不利と言われている展開ですが、果たしてどうなるでしょうか。\n" +
                "  13 ３四飛(24)   (00:00:01 / 00:00:01)\n" +
                "*先手は横歩を取りましたね。\n" +
                "  14 ７六飛(86)   (00:00:01 / 00:00:01)\n" +
                "*後手も横歩を取りましたが、これは大丈夫なのでしょうか？\n" +
                "*嫌な変化が見ているような気もしますが。\n";

            // 棋譜の読み込み
            var kifu = KifuReader.LoadFrom(content);
            Assert.NotNull(kifu);
            Assert.AreEqual(14, kifu.MoveList.Count());

            // コメント一覧
            var comments = new string[15];
            comments[0] =
                "今王将戦は因縁の師弟対決となりました。\n" +
                "受けの気風の師匠に対して、弟子の平方は完全な攻め将棋。";
            comments[1] =
                "最初はオーソドックスに";
            comments[4] = 
                "よくある進行になりました。";
            comments[12] =
                "お互い金を上がらずにここまで来ました。しかもノータイム。\n" +
                "これは先手側不利と言われている展開ですが、果たしてどうなるでしょうか。";
            comments[13] =
                "先手は横歩を取りましたね。";
            comments[14] =
                "後手も横歩を取りましたが、これは大丈夫なのでしょうか？\n" +
                "嫌な変化が見ているような気もしますが。";

            for (var node = kifu.RootNode; node != null; node = node.NextNode)
            {
                Assert.AreEqual(comments[node.MoveCount], node.Comment);
            }

            // 入出力テストを行います。
            TestUtil.ReadWriteTest(kifu, KifuFormat.Kif, 14);
        }

        /// <summary>
        /// コメントが含まれる棋譜のテスト2
        /// </summary>
        [Test()]
        public void CommentTest2()
        {
            var text = SampleKif.CommentKif;

            // 棋譜の読み込み
            var kifu = KifuReader.LoadFrom(text);
            Assert.NotNull(kifu);
            Assert.Null(kifu.Error);

            // 手数の確認
            var count = 134;
            Assert.AreEqual(count, kifu.MoveList.Count());

            // コメントの確認
            var commentList = SampleKif.GetCommentList(SampleKif.CommentKif);
            for (var node = kifu.RootNode; node != null; node = node.NextNode)
            {
                Assert.AreEqual(commentList[node.MoveCount], node.Comment);
            }

            // 入出力テストを行います。
            TestUtil.ReadWriteTest(kifu, KifuFormat.Kif, count);
        }

        /// <summary>
        /// 81Dojoの棋譜読み込みテスト
        /// </summary>
        [Test()]
        public void Duration81DojoTest()
        {
            var content =
                "#KIF version=2.0 encoding=UTF-8\n" +
                "開始日時：2014/01/01\n" +
                "場所：81Dojo (ver.2014/1/1)\n" +
                "持ち時間：5分+30秒\n" +
                "手合割：平手\n" +
                "先手：test-san1\n" +
                "後手：test-san2\n" +
                "手数----指手---------消費時間--\n" +
                "   1 ７六歩(77)   ( 0:5/)\n" +
                "   2 ３四歩(33)   ( 0:1/)\n" +
                "   3 ２六歩(27)   ( 0:3/)\n" +
                "   4 ５四歩(53)   ( 0:1/)\n" +
                "   5 ４八銀(39)   ( 0:3/)\n" +
                "   6 ８四歩(83)   ( 0:4/)\n";

            var durations = new int[]
            {
                5, 1, 3, 1, 3, 4
            };
            var totalDurations = new int[]
            {
                5, 1, 8, 2, 11, 6
            };

            // 棋譜の読み込み
            var kifu = KifuReader.LoadFrom(content);
            Assert.NotNull(kifu);
            Assert.AreEqual(6, kifu.MoveList.Count());

            var node = kifu.RootNode;
            for (var i = 0; i < durations.Count(); ++i)
            {
                node = node.NextNode;
                Assert.AreEqual(TimeSpan.FromSeconds(durations[i]), node.Duration);
                Assert.AreEqual(TimeSpan.FromSeconds(totalDurations[i]), node.TotalDuration);
            }
        }

        /// <summary>
        /// うさぴょんの棋譜読み込みテスト
        /// </summary>
        [Test()]
        public void DurationUsapyonTest()
        {
            var content =
                "先手：あなた\n" +
                "後手：うさぴょんLv2\n" +
                "手合い割り：平手\n" +
                "  1: ▲７六歩         6s\n" +
                "  2: △８四歩         1s\n" +
                "  3: ▲５八金右       3s\n" +
                "  4: △３四歩         1s\n" +
                "  5: ▲６八金寄       3s\n" +
                "  6: △５二王        10s\n";

            var durations = new int[]
            {
                6, 1, 3, 1, 3, 10
            };
            var totalDurations = new int[]
            {
                6, 1, 9, 2, 12, 12
            };

            // 棋譜の読み込み
            var kifu = KifuReader.LoadFrom(content);
            Assert.NotNull(kifu);
            Assert.AreEqual(6, kifu.MoveList.Count());

            var node = kifu.RootNode;
            for (var i = 0; i < durations.Count(); ++i)
            {
                node = node.NextNode;
                Assert.AreEqual(TimeSpan.FromSeconds(durations[i]), node.Duration);
                Assert.AreEqual(TimeSpan.FromSeconds(totalDurations[i]), node.TotalDuration);
            }
        }
    }
}
#endif
