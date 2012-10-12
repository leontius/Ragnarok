﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ragnarok.NicoNico
{
    /// <summary>
    /// ニコニコ関連のURLなどを保持します。
    /// </summary>
    public static class NicoString
    {
        /// <summary>
        /// メッセージサーバーのアドレス番号を取得します。
        /// </summary>
        private static readonly Regex MsgAddressRegex = new Regex(
            @"msg([0-9]+).live.nicovideo.jp");

        /// <summary>
        /// ニコニコ生放送のトップページのＵＲＬを取得します。
        /// </summary>
        public static string LiveTopUrl()
        {
            return "http://live.nicovideo.jp/";
        }

        /// <summary>
        /// ログインURLを取得します。
        /// </summary>
        public static string LoginUrl()
        {
            return "https://secure.nicovideo.jp/secure/login?site=niconico";
        }

        /// <summary>
        /// ログイン時に使うデータを作成します。
        /// </summary>
        public static Dictionary<string, object> MakeLoginData(string mail,
                                                               string password)
        {
            var postData = new Dictionary<string, object>();

            postData["mail"] = mail;
            postData["password"] = password;
            postData["next_url"] = null;
            return postData;
        }

        /// <summary>
        /// ログインエラー時のページに含まれる文字列です。
        /// </summary>
        public static string LoginErrorText()
        {
            return "メールアドレスまたはパスワードが間違っています";
        }

        /// <summary>
        /// メッセージサーバーのアドレスからアドレス番号を取得します。
        /// </summary>
        public static int GetMessageServerNumber(string msAddress)
        {
            var m = MsgAddressRegex.Match(msAddress);
            if (!m.Success)
            {
                return -1;
            }

            return int.Parse(m.Groups[1].Value);
        }

        /// <summary>
        /// アドレス番号からメッセージサーバーのアドレスを取得します。
        /// </summary>
        public static string GetMessageServerAddress(int number)
        {
            return string.Format(
                "msg{0}.live.nicovideo.jp",
                number);
        }

        /// <summary>
        /// メッセージ受信を開始するためのメッセージを作成します。
        /// </summary>
        public static string ThreadStart(int threadId)
        {
            return string.Format(
                "<thread thread=\"{0}\" version=\"20061206\" res_from=\"-999999999\" />\0",
                threadId);
        }

        /// <summary>
        /// メッセージ受信を開始するためのメッセージを作成します。
        /// </summary>
        /// <param name="threadId">
        /// スレッドＩＤです。
        /// </param>
        /// <param name="resFrom">
        /// 今投稿されているコメントから何コメント前のコメントから受信するかです。
        /// </param>
        public static string ThreadStart(int threadId, int resFrom)
        {
            return string.Format(
                "<thread thread=\"{0}\" version=\"20061206\" res_from=\"{1}\" />\0",
                threadId, -resFrom);
        }

        #region 通常ページ
        /// <summary>
        /// マイページがあるURLを取得します。
        /// </summary>
        public static string MyPageUrl()
        {
            return string.Format(
                "http://www.nicovideo.jp/my");
        }

        /// <summary>
        /// ユーザー情報があるURLを取得します。
        /// </summary>
        public static string UserInfoUrl(int userId)
        {
            return string.Format(
                "http://www.nicovideo.jp/user/{0}",
                userId);
        }

        /// <summary>
        /// コミュニティ情報があるURLを取得します。
        /// </summary>
        public static string CommunityInfoUrl(int communityId)
        {
            return string.Format(
                "http://com.nicovideo.jp/community/co{0}",
                communityId);
        }

        /// <summary>
        /// コミュニティ情報があるURLを取得します。
        /// </summary>
        public static string CommunityInfoUrl(string communityIdString)
        {
            return string.Format(
                "http://com.nicovideo.jp/community/{0}",
                communityIdString);
        }

        /// <summary>
        /// チャンネル情報があるURLを取得します。
        /// </summary>
        public static string ChannelInfoUrl(int channelId)
        {
            return string.Format(
                "http://ch.nicovideo.jp/channel/ch{0}",
                channelId);
        }

        /// <summary>
        /// ニコ生の放送URLを取得します。
        /// </summary>
        public static string LiveUrl(long liveId)
        {
            return string.Format(
                "http://live.nicovideo.jp/watch/lv{0}",
                liveId);
        }

        /// <summary>
        /// ニコ生の放送URLを取得します。
        /// </summary>
        public static string LiveUrl(string liveIdString)
        {
            return string.Format(
                "http://live.nicovideo.jp/watch/{0}",
                liveIdString);
        }
        #endregion

        #region API
        /// <summary>
        /// playerstatusを取得するためのURLを取得します。
        /// </summary>
        public static string GetPlayerStatusUrl(long liveId)
        {
            return string.Format(
                "http://live.nicovideo.jp/api/getplayerstatus?v=lv{0}",
                liveId);
        }

        /// <summary>
        /// publishstatusを取得するためのURLを取得します。
        /// </summary>
        public static string GetPublishStatusUrl(long liveId)
        {
            return string.Format(
                "http://live.nicovideo.jp/api/getpublishstatus?v=lv{0}",
                liveId);
        }

        /// <summary>
        /// コメント投稿時に使うキーを取得するためのURLを取得します。
        /// </summary>
        public static string GetPostKeyUrl(int threadId, int blockNo)
        {
            return string.Format(
                "http://watch.live.nicovideo.jp/api/getpostkey?thread={0}&block_no={1}",
                threadId, blockNo);
        }

        /// <summary>
        /// 放送の来場者数などを取得するためのURLを取得します。
        /// </summary>
        public static string GetHeartbeatUrl(long liveId)
        {
            return string.Format(
                "http://watch.live.nicovideo.jp/api/heartbeat?v=lv{0}",
                liveId);
        }

        /// <summary>
        /// 放送主コメントを投稿するためのURLを取得します。
        /// </summary>
        public static string GetBroadcastCommentUrl(long liveId)
        {
            return string.Format(
                "http://watch.live.nicovideo.jp/api/broadcast/lv{0}",
                liveId);
        }

        /// <summary>
        /// 放送主コメントを投稿するためのデータを取得します。
        /// </summary>
        public static Dictionary<string, object>
            MakeBroadcastCommentData(string body, string mail, string name,
                                     string token)
        {
            var postData = new Dictionary<string, object>();

            postData["body"] = body;
            postData["mail"] = mail;
            postData["token"] = token;

            if (!string.IsNullOrEmpty(name))
            {
                postData["name"] = name;
            }
            return postData;
        }
        #endregion
    }
}
