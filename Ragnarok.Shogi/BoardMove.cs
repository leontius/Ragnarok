﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;

using ProtoBuf;

namespace Ragnarok.Shogi
{
    /// <summary>
    /// 盤面上での差し手を示します。
    /// </summary>
    /// <remarks>
    /// <para>
    /// シリアライズについて
    /// 
    /// このクラスは<see cref="Board"/>クラスと一緒にシリアライズされますが、
    /// 200手,300手の局面になってくると、シリアライズ後のデータ量が
    /// 3kbや4kbになってしまいます。
    /// そのためこのクラスでは特別に、シリアライズ時にデータの
    /// 圧縮を行っています。
    /// </para>
    /// </remarks>
    [DataContract()]
    public class BoardMove : IEquatable<BoardMove>
    {
        /// <summary>
        /// オブジェクトのコピーを作成します。
        /// </summary>
        public BoardMove Clone()
        {
            return (BoardMove)MemberwiseClone();
        }

        /// <summary>
        /// 先手の手か後手の手かを取得または設定します。
        /// </summary>
        public BWType BWType
        {
            get;
            set;
        }
        
        /// <summary>
        /// 駒の移動先を取得または設定します。
        /// </summary>
        public Position NewPosition
        {
            get;
            set;
        }

        /// <summary>
        /// 駒の移動元を取得または設定します。
        /// </summary>
        /// <remarks>
        /// 駒打ちの場合はnullになります。
        /// </remarks>
        public Position OldPosition
        {
            get;
            set;
        }

        /// <summary>
        /// 駒打ち・成りなどの動作を取得または設定します。
        /// </summary>
        public ActionType ActionType
        {
            get;
            set;
        }

        /// <summary>
        /// 駒を打つ場合の駒を取得または設定します。
        /// </summary>
        public PieceType DropPieceType
        {
            get;
            set;
        }

        /// <summary>
        /// 取った駒を取得または設定します。
        /// </summary>
        /// <remarks>
        /// 戻る操作のために必要です。
        /// </remarks>
        [DataMember(Order = 2, IsRequired = true)]
        public BoardPiece TookPiece
        {
            get;
            set;
        }

        /// <summary>
        /// オブジェクトの妥当性を検証します。
        /// </summary>
        public bool Validate()
        {
            if (BWType == BWType.None)
            {
                return false;
            }

            if (NewPosition == null || !NewPosition.Validate())
            {
                return false;
            }

            if (ActionType == ActionType.Drop)
            {
                // 駒打ちの場合
                if (OldPosition != null)
                {
                    return false;
                }

                if (DropPieceType == PieceType.None)
                {
                    return false;
                }
            }
            else
            {
                // 駒打ちでない場合
                if (OldPosition == null || !OldPosition.Validate())
                {
                    return false;
                }

                if (DropPieceType != PieceType.None)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// オブジェクトの等値性を調べます。
        /// </summary>
        public override bool Equals(object obj)
        {
            var result = this.PreEquals(obj);
            if (result.HasValue)
            {
                return result.Value;
            }

            return Equals(obj as BoardMove);
        }

        /// <summary>
        /// オブジェクトの等値性を調べます。
        /// </summary>
        public bool Equals(BoardMove other)
        {
            if ((object)other == null)
            {
                return false;
            }

            if (BWType != other.BWType)
            {
                return false;
            }

            if (NewPosition != other.NewPosition)
            {
                return false;
            }

            if (OldPosition != other.OldPosition)
            {
                return false;
            }

            if (ActionType != other.ActionType)
            {
                return false;
            }

            if (DropPieceType != other.DropPieceType)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// オブジェクトを比較します。
        /// </summary>
        public static bool operator ==(BoardMove x, BoardMove y)
        {
            return Ragnarok.Util.GenericEquals(x, y);
        }

        /// <summary>
        /// オブジェクトを比較します。
        /// </summary>
        public static bool operator !=(BoardMove x, BoardMove y)
        {
            return !(x == y);
        }

        /// <summary>
        /// ハッシュ値を計算します。
        /// </summary>
        public override int GetHashCode()
        {
            return (
                BWType.GetHashCode() ^
                (NewPosition != null ? NewPosition.GetHashCode() : 0) ^
                (OldPosition != null ? OldPosition.GetHashCode() : 0) ^
                ActionType.GetHashCode() ^
                DropPieceType.GetHashCode());
        }

        #region シリアライズ/デシリアライズ
        [ProtoMember(1, IsRequired = true, DataFormat = DataFormat.FixedSize)]
        private uint serializeBits = 0;

        /// <summary>
        /// Positionをシリアライズします。
        /// </summary>
        /// <remarks>
        /// 1*10+1 ～ 9*10+9の値にシリアライズされます。
        /// </remarks>
        private static byte SerializePosition(Position position)
        {
            if (position == null)
            {
                return 0;
            }

            // 1*10+1 ～ 9*10+9
            return (byte)(
                position.File * (Board.BoardSize + 1) +
                position.Rank);
        }

        /// <summary>
        /// Positionをデシリアライズします。
        /// </summary>
        private static Position DeserializePosition(uint bits)
        {
            if (bits == 0)
            {
                return null;
            }

            var file = (int)bits / (Board.BoardSize + 1);
            var rank = (int)bits % (Board.BoardSize + 1);
            return new Position(file, rank);
        }

        /// <summary>
        /// シリアライズを行います。
        /// </summary>
        [CLSCompliant(false)]
        public uint Serialize()
        {
            uint bits = 0;

            // 2bit
            bits |= (uint)BWType;
            // 3bit
            bits |= ((uint)ActionType << 2);
            // 4bit
            bits |= ((uint)DropPieceType << 5);
            // 1bit
            bits |= ((uint)(TookPiece != null ? 1 : 0) << 9);
            // 7bit
            bits |= (uint)(SerializePosition(NewPosition) << 10);
            // 7bit
            bits |= (uint)(SerializePosition(OldPosition) << 17);

            if (TookPiece != null)
            {
                bits |= ((uint)TookPiece.Serialize() << 24);
            }

            return bits;
        }

        /// <summary>
        /// デシリアライズを行います。
        /// </summary>
        [CLSCompliant(false)]
        public void Deserialize(uint bits)
        {
            // 2bit
            BWType = (BWType)((bits >> 0) & 0x03);
            // 3bit
            ActionType = (ActionType)((bits >> 2) & 0x07);
            // 4bit
            DropPieceType = (PieceType)((bits >> 5) & 0x0f);
            // 1bit
            var hasTookPiece = (((bits >> 9) & 0x01) != 0);
            // 7bit
            NewPosition = DeserializePosition((bits >> 10) & 0x7f);
            // 7bit
            OldPosition = DeserializePosition((bits >> 17) & 0x7f);

            if (hasTookPiece)
            {
                TookPiece = new BoardPiece();

                TookPiece.Deserialize((bits >> 24) & 0xff);
            }
        }

        /// <summary>
        /// シリアライズ前に呼ばれます。
        /// </summary>
        [OnSerializing()]
        private void BeforeSerialize(StreamingContext context)
        {
            this.serializeBits = Serialize();
        }

        /// <summary>
        /// デシリアライズ後に呼ばれます。
        /// </summary>
        [OnDeserialized()]
        private void AfterDeserialize(StreamingContext context)
        {
            Deserialize(this.serializeBits);
        }
        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BoardMove()
        {
            BWType = BWType.None;
            ActionType = ActionType.None;
            DropPieceType = PieceType.None;
        }
    }
}
