﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Ragnarok.Presentation.Extra.Entity
{
    /// <summary>
    /// FPS値をカウントします。
    /// </summary>
    public class FpsCounter : EntityObject
    {
        /// <summary>
        /// FPS値が変わったときに呼ばれるイベントです。
        /// </summary>
        public event EventHandler FpsChanged;

        /// <summary>
        /// FPSの値を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty FpsProperty =
            DependencyProperty.Register(
                "Fps", typeof(double), typeof(FpsCounter),
                new UIPropertyMetadata(0.0));

        /// <summary>
        /// FPSの値を取得します。
        /// </summary>
        public double Fps
        {
            get { return (double)GetValue(FpsProperty); }
            private set { SetValue(FpsProperty, value); }
        }
        
        private DateTime prevTime = DateTime.Now;
        private int count;

        /// <summary>
        /// フレームごとに呼ばれます。
        /// </summary>
        protected override void OnEnterFrame(EnterFrameEventArgs e)
        {
            base.OnEnterFrame(e);

            this.count += 1;

            var now = DateTime.Now;
            var diff = now - this.prevTime;
            if (diff >= TimeSpan.FromSeconds(1.0))
            {
                Fps = this.count / diff.TotalSeconds;
                FpsChanged.SafeRaiseEvent(this, EventArgs.Empty);

                this.prevTime = now;
                this.count = 0;
            }
        }
    }
}
