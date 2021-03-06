﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

using Ragnarok.Forms.Draw;
using Ragnarok.Utility;

namespace Ragnarok.Forms.Shogi.GLUtil
{
    using Imaging = System.Drawing.Imaging;

    /// <summary>
    /// OpenGL用のテクスチャを管理します。
    /// </summary>
    public class Texture : IDisposable, ICachable
    {
        private readonly static object textureListSync = new object();
        private readonly static List<WeakReference> textureList = new List<WeakReference>();

        /// <summary>
        /// テクスチャリストに作成済みのテクスチャを追加します。
        /// </summary>
        private static void AddTexture(Texture texture)
        {
            if (texture == null)
            {
                throw new ArgumentNullException("texture");
            }

            lock (textureListSync)
            {
                textureList.Add(new WeakReference(texture));
            }
        }

        /// <summary>
        /// テクスチャリストからのテクスチャを削除します。
        /// </summary>
        private static void RemoveTexture(Texture texture)
        {
            if (texture == null)
            {
                throw new ArgumentNullException("texture");
            }

            lock (textureListSync)
            {
                textureList.RemoveIf(_ => _.Target == texture);
            }
        }

        /// <summary>
        /// 現在のコンテキストが持つテクスチャをすべて削除します。
        /// </summary>
        /// <remarks>
        /// OpenGLの終了時に呼ばれます。
        /// </remarks>
        public static void DeleteAll(IGraphicsContext context)
        {
            //var context = context;
            lock (textureListSync)
            {
                for (int index = 0; index < textureList.Count; )
                {
                    var texture = textureList[index].Target as Texture;
                    if (texture == null)
                    {
                        // 要素を削除したため、indexの更新は行いません。
                        textureList.RemoveAt(index);
                        continue;
                    }

                    if (texture.Context == context)
                    {
                        // テクスチャを削除
                        texture.Destroy();

                        // 要素を削除したため、indexの更新は行いません。
                        textureList.RemoveAt(index);
                        continue;
                    }

                    index += 1;
                }
            }
        }

        private readonly IGraphicsContext context;
        private uint glTexture;
        private bool disposed;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Texture()
        {
            this.context = GraphicsContext.CurrentContext;
            if (this.context == null)
            {
                throw new GLException(
                    "OpenGLコンテキストが設定されていません＞＜");
            }

            AddTexture(this);
        }

        /// <summary>
        /// ファイナライザ
        /// </summary>
        ~Texture()
        {
            Dispose(false);
        }

        /// <summary>
        /// テクスチャの削除を行います。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// テクスチャの削除を行います。
        /// </summary>
        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (TextureName != 0)
                    {
                        TextureDisposer.AddDeleteTexture(this.context, TextureName);
                        this.glTexture = 0;
                    }

                    RemoveTexture(this);
                }
                else
                {
                    if (TextureName != 0)
                    {
                        Log.Error(
                            "削除できないテクスチャが残りました。");
                    }
                }

                this.disposed = true;
            }
        }

        /// <summary>
        /// テクスチャ名のコンテキストを取得します。
        /// </summary>
        public IGraphicsContext Context
        {
            get { return this.context; }
        }

        /// <summary>
        /// テクスチャ名(ID)を取得します。
        /// </summary>
        [CLSCompliant(false)]
        public uint TextureName
        {
            get { return this.glTexture; }
        }

        /// <summary>
        /// テクスチャが使用可能か調べます。
        /// </summary>
        public bool IsAvailable
        {
            get { return (TextureName != 0); }
        }

        /// <summary>
        /// テクスチャ画像の幅をPixel数で取得します。
        /// </summary>
        /// <remarks>
        /// 読み込まれた画像サイズとは違う可能性があります。
        /// </remarks>
        public int Width
        {
            get;
            private set;
        }

        /// <summary>
        /// テクスチャ画像の高さをPixel数で取得します。
        /// </summary>
        /// <remarks>
        /// 読み込まれた画像サイズとは違う可能性があります。
        /// </remarks>
        public int Height
        {
            get;
            private set;
        }

        /// <summary>
        /// 読み込みに使われたオリジナル画像の幅をPixel数で取得します。
        /// </summary>
        public int OriginalWidth
        {
            get;
            private set;
        }

        /// <summary>
        /// 読み込みに使われたオリジナル画像の高さをPixel数で取得します。
        /// </summary>
        public int OriginalHeight
        {
            get;
            private set;
        }
        
        /// <summary>
        /// α乗算済みテクスチャかどうかを取得します。
        /// </summary>
        public bool IsPremultipliedAlpha
        {
            get;
            private set;
        }

        /// <summary>
        /// ビットマップサイズをbyte数で取得します。
        /// </summary>
        /// <remarks>
        /// ICachableインターフェースの実装に必要です。
        /// </remarks>
        public long ObjectSize
        {
            get { return (4 * Width * Height); }
        }

        /// <summary>
        /// コンテキストの確認を行います。
        /// </summary>
        private void ValidateContext()
        {
            if (this.context != GraphicsContext.CurrentContext)
            {
                throw new GLException(
                    "OpenGLコンテキストが正しく設定れていません＞＜");
            }
        }

        /// <summary>
        /// テクスチャに関する属性を保存します。
        /// </summary>
        public virtual void Push()
        {
            if (TextureName == 0)
            {
                throw new InvalidOperationException(
                    "テクスチャの作成が完了していません。");
            }

            ValidateContext();
            GL.PushAttrib(AttribMask.TextureBit);
            Bind();
        }

        /// <summary>
        /// テクスチャに関する属性を呼び出します。
        /// </summary>
        public virtual void Pop()
        {
            if (TextureName == 0)
            {
                throw new InvalidOperationException(
                    "テクスチャの作成が完了していません。");
            }

            ValidateContext();
            GL.PopAttrib();
        }

        /// <summary>
        /// テクスチャをバインドします。
        /// </summary>
        public void Bind()
        {
            if (TextureName == 0)
            {
                throw new InvalidOperationException(
                    "テクスチャの作成が完了していません。");
            }

            ValidateContext();
            GL.BindTexture(TextureTarget.Texture2D, TextureName);
        }
        
        /// <summary>
        /// テクスチャのバインドを解除します。
        /// </summary>
        public void Unbind()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        /// <summary>
        /// テクスチャデータの作成を行います。
        /// </summary>
        /// <remarks>
        /// イメージからテクスチャを作成するのみで
        /// サイズの変更などの余計なことは一切行いません。
        ///
        /// またこのメソッドには所有権を渡してもよいイメージオブジェクトを
        /// 与えてください。
        /// </remarks>
        private bool CreateInternal(Bitmap image, Size originalSize,
                                    bool toPremultipliedAlpha)
        {
            uint texture = 0;

            GL.GenTextures(1, out texture);

            //  Lock the image bits (so that we can pass them to OGL).
            var bitmapData = image.LockBits(
                new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly,
                Imaging.PixelFormat.Format32bppArgb);

            try
            {
                if (toPremultipliedAlpha)
                {
                    MakePremutipliedAlpha(bitmapData);
                }

                GL.BindTexture(TextureTarget.Texture2D, texture);

                //  テクスチャデータをセットします。
#if true
                GL.TexImage2D(
                    TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                    image.Width, image.Height, 0,
                    OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                    PixelType.UnsignedByte,
                    bitmapData.Scan0);
#else
                GL.Build2DMipmaps(
                    TextureTarget.Texture2D, (int)OpenGL.GL_RGBA,
                    image.Width, image.Height,
                    OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE,
                    bitmapData.Scan0);
#endif
            }
            finally
            {
                image.UnlockBits(bitmapData);
            }

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);

            if (Misc.HasExtension("GL_SGIS_generate_mipmap"))
            {
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            }
            else
            {
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            }

            GL.BindTexture(TextureTarget.Texture2D, 0);
            
            // テクスチャの作成に成功したら、古いテクスチャを削除します。
            Destroy();

            this.glTexture = texture;
            Width = image.Width;
            Height = image.Height;
            OriginalWidth = originalSize.Width;
            OriginalHeight = originalSize.Height;
            IsPremultipliedAlpha = toPremultipliedAlpha;
            return true;
        }

        /// <summary>
        /// α乗算済み画像データに変換します。
        /// </summary>
        private void MakePremutipliedAlpha(BitmapData data)
        {
            if (data.PixelFormat != Imaging.PixelFormat.Format32bppArgb)
            {
                throw new InvalidOperationException(
                    "対応していないピクセルフォーマットです。");
            }

            unsafe
            {
                for (var y = 0; y < data.Height; ++y)
                {
                    byte* p = (byte*)data.Scan0 + (data.Stride * y);

                    for (var x = 0; x < data.Width; ++x)
                    {
                        var a = p[x * 4 + 0];

                        p[x * 4 + 1] = (byte)((p[x * 4 + 1] * a + 255) >> 8);
                        p[x * 4 + 2] = (byte)((p[x * 4 + 2] * a + 255) >> 8);
                        p[x * 4 + 3] = (byte)((p[x * 4 + 3] * a + 255) >> 8);
                    }
                }
            }
        }

        /// <summary>
        /// イメージデータからテクスチャの作成を行います。
        /// </summary>
        public bool Create(Bitmap image, bool toPremultipliedAlpha = false)
        {
            if (image == null)
            {
                throw new ArgumentNullException("image");
            }

            ValidateContext();

            int[] textureMaxSize = { 0 };
            GL.GetInteger(GetPName.MaxTextureSize, textureMaxSize);

            // 2のn乗値の中から、元の画像サイズを超えない範囲で
            // 一番大きな値を探します。
            int targetWidth = textureMaxSize[0];
            for (int size = 1; size <= textureMaxSize[0]; size *= 2)
            {
                if (image.Width < size)
                {
                    targetWidth = size / 2;
                    break;
                }
            }

            int targetHeight = textureMaxSize[0];
            for (int size = 1; size <= textureMaxSize[0]; size *= 2)
            {
                if (image.Height < size)
                {
                    targetHeight = size / 2;
                    break;
                }
            }

            // 画像のリサイズが必要な場合
            var result = false;
            if (image.Width != targetWidth || image.Height != targetHeight)
            {
                using (var newImage = image.ResizeHighQuality(
                    targetWidth, targetHeight))
                {
                    result = CreateInternal(newImage, image.Size, toPremultipliedAlpha);
                }
            }
            else
            {
                // imageの内容が変わる可能性があるため、ここでCloneしています。
                using (var newImage = (Bitmap)image.Clone())
                {
                    result = CreateInternal(newImage, image.Size, toPremultipliedAlpha);
                }
            }

            GC.Collect();
            return result;
        }

        /// <summary>
        /// ファイルからテクスチャを作成します。
        /// </summary>
        public bool Load(string filepath, bool toPremultipliedAlpha = false)
        {
            using (var image = new Bitmap(filepath))
            {
                if (image == null)
                {
                    return false;
                }

                return Create(image, toPremultipliedAlpha);
            }
        }

        /// <summary>
        /// テクスチャを削除します。
        /// </summary>
        public void Destroy()
        {
            ValidateContext();

            if (this.glTexture != 0)
            {
                GL.DeleteTextures(1, ref this.glTexture);
                this.glTexture = 0;
            }
            
            Width = 0;
            Height = 0;
            OriginalWidth = 0;
            OriginalHeight = 0;
            IsPremultipliedAlpha = false;
        }
    }
}
