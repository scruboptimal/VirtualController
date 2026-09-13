using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using SkiaSharp;
using SkiaSharp.Views.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using VirtualControllerNative.Interop;
using VirtualControllerShared;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace VirtualController
{
    public sealed partial class RecordingTimeline : UserControl
    {
        public static readonly DependencyProperty RecordingDependencyProperty = DependencyProperty.Register(nameof(Recording),
            typeof(Recording), typeof(RecordingTimeline), new PropertyMetadata(null, OnRecordingChanged));

        private static readonly int frameWidth = 8;
        private static readonly int buttonHeight = 16;

        private static SKPaint framePaint = new()
        {
            Color = SKColors.Black,
            Style = SKPaintStyle.Stroke,
        };

        private static SKPaint pressedFramePaint = new()
        {
            Color = SKColors.Red,
            Style = SKPaintStyle.Fill,
        };

        public Recording? Recording
        {
            get { return (Recording?)GetValue(RecordingDependencyProperty); }
            set { SetValue(RecordingDependencyProperty, value); }
        }

        public RecordingTimeline()
        {
            InitializeComponent();
        }

        private void OnTimelinePaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            int numButtons = Enum.GetValues(typeof(GamepadButton)).Length;
            int numFrames = e.Info.Width / frameWidth;

            canvas.Clear(SKColors.DarkGray);

            RenderRecording(canvas, numButtons, numFrames, e.Info);
            RenderGrid(canvas, numButtons, numFrames);
        }

        private void RenderRecording(SKCanvas canvas, int numButtons, int numFrames, SKImageInfo imageInfo)
        {
            if (this.Recording == null)
            {
                return;
            }

            for (int i = 0; i < this.Recording.Frames.Count; i++)
            {
                var frame = this.Recording.Frames[i];
                var nextFrame = i + 1 < this.Recording.Frames.Count ? this.Recording.Frames[i + 1] : null;
                if (frame.State == GamepadButton.None)
                {
                    // Nothing to do
                    continue;
                }

                float curFrameX = frame.FrameIdx * frameWidth;
                float nextFrameX = nextFrame != null ? nextFrame.FrameIdx * frameWidth : imageInfo.Width;

                for (int buttonIdx = 1; buttonIdx < numButtons; buttonIdx++)
                {
                    var button = (GamepadButton)(1 << (buttonIdx - 1));
                    if (frame.State.HasFlag(button))
                    {
                        canvas.DrawRect(new SKRect(curFrameX, buttonIdx * buttonHeight, nextFrameX, (buttonIdx + 1) * buttonHeight), pressedFramePaint);
                    }
                }
            }
        }

        private void RenderGrid(SKCanvas canvas, int numButtons, int numFrames)
        {
            for (int frameIdx = 0; frameIdx < numFrames; frameIdx++)
            {
                for (int buttonIdx = 0; buttonIdx < numButtons; buttonIdx++)
                {
                    canvas.DrawRect(GetRect(frameIdx, buttonIdx), framePaint);
                }
            }
        }

        static SKRect GetRect(int frameIdx, int buttonIdx)
        {
            float x = frameIdx * frameWidth;
            float y = buttonIdx * buttonHeight;
            return new SKRect(x, y, x + frameWidth, y + buttonHeight);
        }

        private static void OnRecordingChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            RecordingTimeline timeline = (RecordingTimeline)dependencyObject;
            timeline.TimelineCanvas.Invalidate();
        }
    }
}
