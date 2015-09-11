#if TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using NUnit.Framework;

namespace Ragnarok.NicoNico.Video.Tests
{
    [TestFixture()]
    internal sealed class VideoDataTest
    {
        private CookieContainer cc;

        [SetUp()]
        public void Setup()
        {
            this.cc = Login.Loginer.LoginWithBrowser(
                Net.CookieGetter.BrowserType.Firefox, true);
            Assert.IsNotNull(cc);
        }

        /// <summary>
        /// 一般の動画によるテスト
        /// </summary>
        [Test()]
        public void NormalTest1()
        {
            var video = VideoData.CreateFromPage(
                "http://www.nicovideo.jp/watch/sm9", this.cc);
            Assert.IsNotNull(video);

            Assert.AreEqual("sm9", video.IdString);
            Assert.AreEqual(-1, video.ThreadId);
            Assert.AreEqual("新・豪血寺一族 -煩悩解放 - レッツゴー！陰陽師", video.Title);
            Assert.AreEqual("レッツゴー！陰陽師（フルコーラスバージョン）", video.Description);
            Assert.GreaterOrEqual(video.ViewCounter, 15104451);
            Assert.GreaterOrEqual(video.CommentCounter, 4320605);
            Assert.GreaterOrEqual(video.MylistCounter, 159164);

            var tags = new string[]
            {
                "陰陽師", "レッツゴー！陰陽師", "公式", "音楽", "ゲーム"
            };
            Assert.LessOrEqual(tags.Count(), video.Tags.Count());
            tags.ForEachWithIndex((_, i) => Assert.AreEqual(_, video.Tags[i]));
        }

        /// <summary>
        /// 一般の動画によるテスト2
        /// </summary>
        [Test()]
        public void NormalTest2()
        {
            var video = VideoData.CreateFromPage(
                "http://www.nicovideo.jp/watch/sm941537", this.cc);
            Assert.IsNotNull(video);

            Assert.AreEqual("sm941537", video.IdString);
            Assert.AreEqual(-1, video.ThreadId);
            Assert.AreEqual("ボーカロイド　初音ミク　デモソング", video.Title);
            Assert.AreEqual(
                "クリプトン開発のVOCALOID。つまり音声合成ソフトです　　　" +
                "　　　　　　　　　　　　　　　　　　　　　　　　　　　　" + 
                "　　　　　　　　　CV：藤田咲（主な出演作＊ TVアニメ「と" +
                "きめきメモリアルOnly Love」弥生水奈役ＴＶアニメ「がくえんゆー" + 
                "とぴあ まなびストレート！」小鳥桃葉役 TVアニメ「つよきすCool×Sweet」" +
                "　蟹沢きぬ、 TVアニメ「吉永さん家のガーゴイル」　など）　　　　　" +
                "詳しくはこちらhttp://www.crypton.co.jp/mp/pages/prod/vocaloid/cv01.jsp　" + 
                "私もミクで遊んでみました " +
                "<a href=\"http://www.nicovideo.jp/mylist/4883031\" target=\"_blank\">mylist/4883031</a>",
                video.Description);
            Assert.GreaterOrEqual(video.ViewCounter, 165419);
            Assert.GreaterOrEqual(video.CommentCounter, 1948);
            Assert.GreaterOrEqual(video.MylistCounter, 2363);

            var tags = new string[]
            {
                "音楽", "初音ミク", "公式デモ", "VOCALOID"
            };
            Assert.LessOrEqual(tags.Count(), video.Tags.Count());
            tags.ForEachWithIndex((_, i) => Assert.AreEqual(_, video.Tags[i]));
        }

        /// <summary>
        /// チャンネルのCM動画によるテスト
        /// </summary>
        [Test()]
        public void ChannelTest1()
        {
            var video = VideoData.CreateFromPage(
                "http://www.nicovideo.jp/watch/1441099865", this.cc);
            Assert.IsNotNull(video);

            Assert.AreEqual("so27063885", video.IdString);
            Assert.AreEqual(1441099865, video.ThreadId);
            Assert.AreEqual("【9/30まで】将棋プレミアム【無料トライアルキャンペーン実施中】", video.Title);
            Assert.IsNotNullOrEmpty(video.Description);
            Assert.GreaterOrEqual(video.ViewCounter, 0);
            Assert.GreaterOrEqual(video.CommentCounter, 0);
            Assert.GreaterOrEqual(video.MylistCounter, 0);

            var tags = new string[]
            {
                "ゲーム", "将棋", "生放送", "CM", "実験動画", "糸谷哲郎",
                "将棋プレミアム", "囲碁将棋チャンネル"
            };
            Assert.AreEqual(tags.Count(), video.Tags.Count());
            tags.ForEachWithIndex((_, i) => Assert.AreEqual(_, video.Tags[i]));
        }
    }
}
#endif
