#if TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using NUnit.Framework;

namespace Ragnarok.NicoNico.Video.Tests
{
    [TestFixture()]
    internal sealed class VideoDataTest
    {
        enum RemoveTagType
        {
            None,
            All,
            ATagOnly,
        }

        private CookieContainer cc;

        [SetUp()]
        public void Setup()
        {
            this.cc = Login.Loginer.LoginWithBrowser(
                Net.CookieGetter.BrowserType.Firefox, true);
            Assert.IsNotNull(cc);
        }

        /// <summary>
        /// htmlのタグを削除します。
        /// </summary>
        private string RemoveTag(RemoveTagType removeTagType, string html)
        {
            switch (removeTagType)
            {
                case RemoveTagType.None:
                    return html;
                    
                case RemoveTagType.All:
                    var result = Regex.Replace(html, @"<\w+[^>]*?>", "");
                    return Regex.Replace(result, @"</?\w+( /)?>", "");

                case RemoveTagType.ATagOnly:
                    return Regex.Replace(html, @"<a[^>]*?>((sm|mylist|user).*?)</a>", "$1");
            }

            return string.Empty;
        }

        private void TestSM9(VideoData video, RemoveTagType removeTagType)
        {
            Assert.IsNotNull(video);

            Assert.AreEqual("sm9", video.IdString);
            Assert.AreEqual(-1, video.ThreadId);
            Assert.AreEqual("新・豪血寺一族 -煩悩解放 - レッツゴー！陰陽師", video.Title);
            Assert.AreEqual("レッツゴー！陰陽師（フルコーラスバージョン）", video.Description);
            Assert.AreEqual(DateTime.Parse("2007-03-06T00:33:00+09:00"), video.StartTime);
            Assert.GreaterOrEqual(video.ViewCounter, 15104451);
            Assert.GreaterOrEqual(video.CommentCounter, 4320605);
            Assert.GreaterOrEqual(video.MylistCounter, 159164);

            var tags = new string[]
            {
                "陰陽師", "レッツゴー！陰陽師", "公式", "音楽", "ゲーム"
            };
            Assert.LessOrEqual(tags.Count(), video.TagList.Count());
            tags.ForEach(_ => Assert.True(video.TagList.Contains(_)));
        }

        private void TestSM941537(VideoData video, RemoveTagType removeTagType)
        {
            Assert.AreEqual("sm941537", video.IdString);
            Assert.AreEqual(-1, video.ThreadId);
            Assert.AreEqual("ボーカロイド　初音ミク　デモソング", video.Title);
            Assert.AreEqual(RemoveTag(removeTagType,
                "クリプトン開発のVOCALOID。つまり音声合成ソフトです　　　" +
                "　　　　　　　　　　　　　　　　　　　　　　　　　　　　" +
                "　　　　　　　　　CV：藤田咲（主な出演作＊ TVアニメ「と" +
                "きめきメモリアルOnly Love」弥生水奈役ＴＶアニメ「がくえんゆー" +
                "とぴあ まなびストレート！」小鳥桃葉役 TVアニメ「つよきすCool×Sweet」" +
                "　蟹沢きぬ、 TVアニメ「吉永さん家のガーゴイル」　など）　　　　　" +
                "詳しくはこちらhttp://www.crypton.co.jp/mp/pages/prod/vocaloid/cv01.jsp　" +
                "私もミクで遊んでみました " +
                "<a href=\"http://www.nicovideo.jp/mylist/4883031\" target=\"_blank\">mylist/4883031</a>"),
                video.Description);
            Assert.AreEqual(DateTime.Parse("2007-08-29T14:02:39+09:00"), video.StartTime);
            Assert.GreaterOrEqual(video.ViewCounter, 165419);
            Assert.GreaterOrEqual(video.CommentCounter, 1948);
            Assert.GreaterOrEqual(video.MylistCounter, 2363);

            var tags = new string[]
            {
                "音楽", "初音ミク", "公式デモ", "VOCALOID"
            };
            Assert.LessOrEqual(tags.Count(), video.TagList.Count());
            tags.ForEach(_ => Assert.True(video.TagList.Contains(_)));
        }

        private void TestSM500873(VideoData video, RemoveTagType removeTagType)
        {
            Assert.AreEqual("sm500873", video.IdString);
            Assert.AreEqual(-1, video.ThreadId);
            Assert.AreEqual("組曲『ニコニコ動画』 ", video.Title);
            Assert.AreEqual(RemoveTag(removeTagType,
                "<font size=\"+2\">700万再生、ありがとうございました。<br />" +
                "記念動画公開中です ⇒ (<a href=\"http://www.nicovideo.jp/watch/sm14242201\" class=\"watch\">sm14242201</a>)<br />" +
                "</font><br />" +
                "ニコニコ動画(β・γ)で人気のあった曲などを繋いでひとつの曲にしてみました(2度目)。全33曲。<br />" +
                "<font size=\"-2\">※多くの方を誤解させてしまっているようですが(申し訳ないです)、厳密には「組曲」ではなく「メドレー」です。<br />" +
                "「組曲という名前のメドレー」だと思ってください。</font><br /><br />" +
                "<a href=\"http://www.nicovideo.jp/mylist/1535765\" target=\"_blank\">mylist/1535765</a><br />" +
                "<a href=\"http://www.nicovideo.jp/user/145217\" target=\"_blank\">user/145217</a>"),
                video.Description);
            Assert.AreEqual(DateTime.Parse("2007-06-23T18:27:06+09:00"), video.StartTime);
            Assert.GreaterOrEqual(video.ViewCounter, 8882231);
            Assert.GreaterOrEqual(video.CommentCounter, 4412089);
            Assert.GreaterOrEqual(video.MylistCounter, 130981);

            var tags = new string[]
            {
                "音楽", "アレンジ", "組曲『ニコニコ動画』", "空気の読めるWMP",
                "ニコニコオールスター",
            };
            Assert.LessOrEqual(tags.Count(), video.TagList.Count());
            tags.ForEach(_ => Assert.True(video.TagList.Contains(_)));
        }

        private void Test1441099865(VideoData video, RemoveTagType removeTagType)
        {
            Assert.IsNotNull(video);

            Assert.AreEqual("so27063885", video.IdString);
            Assert.AreEqual(removeTagType == RemoveTagType.None ? 1441099865 : -1, video.ThreadId);
            Assert.AreEqual("【9/30まで】将棋プレミアム【無料トライアルキャンペーン実施中】", video.Title);
            Assert.AreEqual(RemoveTag(removeTagType,
                "囲碁・将棋チャンネルの新会員サービス【将棋プレミアム】が8月10日(月)よりスタート！<br>" +
                "いつでもどこでも見られるオンデマンドサービスをはじめ、会員イベントやプレミアムグッズプレゼントなど将棋ファン必見のサービスです！<br><br>" +
                "9月30日(水)まで無料メルマガ会員登録を行うと、すべてのコンテンツが見放題となる<br>" +
                "「無料トライアルキャンペーン 」も実施しています。<br><br>" +
                "詳しくは将棋プレミアム(<a href=\"http://www.igoshogi.net/shogipremium/\" target=\"_blank\">http://www.igoshogi.net/shogipremium/</a>)へ今すぐアクセス☆"),
                video.Description);
            Assert.AreEqual(DateTime.Parse("2015-09-01T18:30:00+09:00"), video.StartTime);
            Assert.GreaterOrEqual(video.ViewCounter, 190);
            Assert.GreaterOrEqual(video.CommentCounter, 0);
            Assert.GreaterOrEqual(video.MylistCounter, 0);

            var tags = new string[]
            {
                "ゲーム", "将棋", "生放送", "CM", "実験動画", "糸谷哲郎",
                "将棋プレミアム", "囲碁将棋チャンネル"
            };
            Assert.AreEqual(tags.Count(), video.TagList.Count());
            tags.ForEach(_ => Assert.True(video.TagList.Contains(_)));
        }

        /// <summary>
        /// CreateFromApiのテスト
        /// </summary>
        [Test()]
        public void CreateFromApiTest()
        {
            TestSM9(VideoData.CreateFromApi("sm9"), RemoveTagType.All);
            TestSM941537(VideoData.CreateFromApi("sm941537"), RemoveTagType.All);
            TestSM500873(VideoData.CreateFromApi("sm500873"), RemoveTagType.All);
            Test1441099865(VideoData.CreateFromApi(
                "http://www.nicovideo.jp/watch/1441099865?eco=1"), RemoveTagType.All);

            Assert.Catch(() =>
                VideoData.CreateFromApi("sm44422222222222222"));
            Assert.Catch(() =>
                VideoData.CreateFromApi("134444444444444444"));
        }

        /// <summary>
        /// CreateFromPageのテスト
        /// </summary>
        [Test()]
        public void CreateFromPageTest()
        {
            TestSM9(VideoData.CreateFromPage("sm9", this.cc), RemoveTagType.None);
            Console.WriteLine("Waiting ...");
            Thread.Sleep(4000);
            TestSM941537(VideoData.CreateFromPage("sm941537", this.cc), RemoveTagType.None);
            Console.WriteLine("Waiting ...");
            Thread.Sleep(4000);
            TestSM500873(VideoData.CreateFromPage("sm500873", this.cc), RemoveTagType.None);
            Console.WriteLine("Waiting ...");
            Thread.Sleep(4000);
            Test1441099865(VideoData.CreateFromPage(
                "http://www.nicovideo.jp/watch/1441099865?eco=1", this.cc), RemoveTagType.None);
            Thread.Sleep(4000);

            Assert.Catch(() =>
                VideoData.CreateFromPage("sm44422222222222222", this.cc));
            Thread.Sleep(4000);
            Assert.Catch(() =>
                VideoData.CreateFromPage("134444444444444444", this.cc));
        }

        /// <summary>
        /// SnapshotApiのテスト
        /// </summary>
        [Test()]
        public void SnapshotApiTest()
        {
            var vs = SnapshotApi.Search("レッツゴー！陰陽師（フルコーラスバージョン）");
            TestSM9(vs.OrderByDescending(_ => _.ViewCounter).FirstOrDefault(), RemoveTagType.ATagOnly);

            vs = SnapshotApi.Search("クリプトン開発のVOCALOID。つまり音声合成ソフトです");
            TestSM941537(vs.OrderByDescending(_ => _.ViewCounter).FirstOrDefault(), RemoveTagType.ATagOnly);

            vs = SnapshotApi.Search("ニコニコ動画(β・γ)で人気のあった曲などを繋いで");
            TestSM500873(vs.OrderByDescending(_ => _.ViewCounter).FirstOrDefault(), RemoveTagType.ATagOnly);

            vs = SnapshotApi.Search("CM 実験動画 囲碁将棋チャンネル 糸谷哲郎", false);
            Test1441099865(vs.OrderByDescending(_ => _.ViewCounter).FirstOrDefault(), RemoveTagType.ATagOnly);

            vs = SnapshotApi.Search("どｊどういｔｓｄｋｊぁ", false);
            Assert.AreEqual(0, vs.Count());
        }
    }
}
#endif
