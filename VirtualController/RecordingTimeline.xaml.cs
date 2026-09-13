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

        private static readonly int labelWidth = 32;
        private static readonly int frameWidth = 16;
        private static readonly int buttonHeight = 32;

        private static SKFont labelFont = new SKFont();

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
            int numFrames = e.Info.Width / frameWidth;

            canvas.Clear(SKColors.DarkGray);

            RenderRecording(canvas, e.Info);
            RenderGrid(canvas, numFrames);
        }

        private void RenderRecording(SKCanvas canvas, SKImageInfo imageInfo)
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

                float curFrameX = (frame.FrameIdx * frameWidth) + labelWidth;
                float nextFrameX = (nextFrame != null ? nextFrame.FrameIdx * frameWidth : imageInfo.Width) + labelWidth;

                for (int buttonIdx = 0; buttonIdx < GamepadButtons.Buttons.Count; buttonIdx++)
                {
                    var button = GamepadButtons.GetGamepadButton(buttonIdx);
                    if (frame.State.HasFlag(button))
                    {
                        canvas.DrawRect(new SKRect(curFrameX, buttonIdx * buttonHeight, nextFrameX, (buttonIdx + 1) * buttonHeight), pressedFramePaint);
                    }
                }
            }
        }

        private void RenderGrid(SKCanvas canvas, int numFrames)
        {
            for (int buttonIdx = 0; buttonIdx < GamepadButtons.Buttons.Count; buttonIdx++)
            {
                var button = GamepadButtons.GetGamepadButton(buttonIdx);
                string label = GamepadButtons.ButtonLabels[button];
                float labelY = (buttonIdx * buttonHeight) + (buttonHeight / 2) + (labelFont.Size / 2);
                canvas.DrawText(label, labelWidth / 2, labelY, SKTextAlign.Center, labelFont, framePaint);
            }

            for (int frameIdx = 0; frameIdx < numFrames; frameIdx++)
            {
                for (int buttonIdx = 0; buttonIdx < GamepadButtons.Buttons.Count; buttonIdx++)
                {
                    canvas.DrawRect(GetFrameRect(frameIdx, buttonIdx), framePaint);
                }
            }
        }

        static SKRect GetFrameRect(int frameIdx, int buttonIdx)
        {
            float x = (frameIdx * frameWidth) + labelWidth;
            float y = buttonIdx * buttonHeight;
            return new SKRect(x, y, x + frameWidth, y + buttonHeight);
        }

        private static void OnRecordingChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            RecordingTimeline timeline = (RecordingTimeline)dependencyObject;

            Recording? recording = e.NewValue as Recording;
            timeline.Canvas.Width = recording != null ? recording.FrameCount * frameWidth : 0;
        }
    }
}
