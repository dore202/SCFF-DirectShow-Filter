﻿// Copyright 2012-2013 Alalf <alalf.iQLc_at_gmail.com>
//
// This file is part of SCFF-DirectShow-Filter(SCFF DSF).
//
// SCFF DSF is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SCFF DSF is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with SCFF DSF.  If not, see <http://www.gnu.org/licenses/>.

/// @file SCFF.GUI/Controls/LayoutEdit.xaml.cs
/// @copydoc SCFF::GUI::Controls::LayoutEdit

namespace SCFF.GUI.Controls {

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using SCFF.Common;
using SCFF.Common.GUI;

/// WYSIWYGレイアウト編集用UserControl
///
/// LayoutEditImage内の座標系は([0-1]*Scale,[0-1]*Scale)で固定（プレビューのサイズに依存しない）
/// 逆に言うと依存させてはいけない
public partial class LayoutEdit
    : UserControl, IUpdateByProfile, IUpdateByOptions, IUpdateByRuntimeOptions {
  //===================================================================
  // 定数
  //===================================================================  

  /// カーソルをまとめたディクショナリ
  private static readonly Dictionary<HitModes, Cursor> HitModesToCursors =
      new Dictionary<HitModes,Cursor> {
    {HitModes.Neutral, null},
    {HitModes.Move, Cursors.SizeAll},
    {HitModes.SizeNW, Cursors.SizeNWSE},
    {HitModes.SizeNE, Cursors.SizeNESW},
    {HitModes.SizeSW, Cursors.SizeNESW},
    {HitModes.SizeSE, Cursors.SizeNWSE},
    {HitModes.SizeN, Cursors.SizeNS},
    {HitModes.SizeW, Cursors.SizeWE},
    {HitModes.SizeS, Cursors.SizeNS},
    {HitModes.SizeE, Cursors.SizeWE}
  };

  //===================================================================
  // コンストラクタ/デストラクタ/Closing/ShutdownStartedイベントハンドラ
  //===================================================================

  /// コンストラクタ
  public LayoutEdit() {
    Debug.WriteLine("LayoutEdit", "*** MEMORY[NEW] ***");

    InitializeComponent();
    this.Dispatcher.ShutdownStarted += OnShutdownStarted;
    
    // できるだけ軽く
    RenderOptions.SetBitmapScalingMode(this.DrawingGroup, BitmapScalingMode.LowQuality);

    // スクリーンキャプチャマネージャの準備
    this.screenCaptureTimer = new ScreenCaptureTimer(redrawTimerPeriod);
    this.screenCaptureTimer.Init();

    // BitmapSource更新用タイマーの準備
    this.StartRedrawTimer();

    // 一回DrawingGroupを生成
    this.BuildDrawingGroup();
  }

  /// Dispatcher.ShutdownStarted
  private void OnShutdownStarted(object sender, EventArgs e) {
    Debug.WriteLine("LayoutEdit", "*** MEMORY[ShutdownStarted] ***");
    this.screenCaptureTimer.End();
  }

  /// デストラクタ
  ~LayoutEdit() {
    Debug.WriteLine("LayoutEdit", "*** MEMORY[DELETE] ***");
    this.Dispatcher.ShutdownStarted -= OnShutdownStarted;
  }

  //===================================================================
  // this.DrawingGroupへの描画
  //===================================================================

  /// キャプションのフォントサイズ
  private const int CaptionFontSize = 12;

  /// レイアウト要素のヘッダーの生成
  /// @param layoutElement 対象のレイアウト要素
  /// @return DrawingContext.DrawTextで描画可能なDrawingVisualオブジェクト
  private FormattedText CreateHeader(ILayoutElementView layoutElement) {
    var isCurrent = layoutElement.Index == App.Profile.CurrentView.Index;
    var isDummy = App.RuntimeOptions.SelectedEntryIndex == -1;

    // Caption
    var header = layoutElement.GetHeaderStringForGUI(
            isCurrent, isDummy,
            App.RuntimeOptions.CurrentSampleWidth,
            App.RuntimeOptions.CurrentSampleHeight);

    // Brush
    Brush textBrush = null;
    switch (layoutElement.WindowType) {
      case WindowTypes.Normal: {
        textBrush = isCurrent ? BrushesAndPens.CurrentNormalBrush
                              : BrushesAndPens.NormalBrush;
        break;
      }
      case WindowTypes.DesktopListView: {
        textBrush = isCurrent ? BrushesAndPens.CurrentDesktopListViewBrush
                              : BrushesAndPens.DesktopListViewBrush;
        break;
      }
      case WindowTypes.Desktop: {
        textBrush = isCurrent ? BrushesAndPens.CurrentDesktopBrush
                              : BrushesAndPens.DesktopBrush;
        break;
      }
    }

    // FormattedText
    var formattedText = new FormattedText(header,
        System.Globalization.CultureInfo.CurrentUICulture,
        FlowDirection.LeftToRight,
        new Typeface("Meiryo"),
        CaptionFontSize,
        textBrush);

    // Clipping
    var boundRect = layoutElement.GetBoundRect(App.RuntimeOptions.CurrentSampleWidth,
                                               App.RuntimeOptions.CurrentSampleHeight);
    formattedText.MaxTextWidth = boundRect.Width;
    formattedText.MaxLineCount = 1;
    return formattedText;
  }

  /// プレビュー画像の描画
  /// @param dc 描画先
  /// @param layoutElement 描画対象
  private void DrawPreview(DrawingContext dc, ILayoutElementView layoutElement) {
    var bitmap = this.screenCaptureTimer.GetBitmapSource(layoutElement.Index);
    if (bitmap == null) return;

    // プレビューの描画
    if (!layoutElement.IsWindowValid) return;

    var actualBoundRect = layoutElement.GetActualBoundRect(
        App.RuntimeOptions.CurrentSampleWidth,
        App.RuntimeOptions.CurrentSampleHeight).ToRect();
    dc.DrawImage(bitmap, actualBoundRect);
  }

  /// 枠線とキャプションの描画
  /// @param dc 描画先
  /// @param layoutElement 描画対象
  private void DrawBorder(DrawingContext dc, ILayoutElementView layoutElement) {
    var isCurrent = layoutElement.Index == App.Profile.CurrentView.Index;

    // Pen
    Pen framePen = null;
    switch (layoutElement.WindowType) {
      case WindowTypes.Normal: {
        framePen = isCurrent ? BrushesAndPens.CurrentNormalPen
                             : BrushesAndPens.NormalPen;
        break;
      }
      case WindowTypes.DesktopListView: {
        framePen = isCurrent ? BrushesAndPens.CurrentDesktopListViewPen
                             : BrushesAndPens.DesktopListViewPen;
        break;
      }
      case WindowTypes.Desktop: {
        framePen = isCurrent ? BrushesAndPens.CurrentDesktopPen
                             : BrushesAndPens.DesktopPen;
        break;
      }
    }

    // フレーム(内枠)の描画
    var boundRect = layoutElement.GetBoundRect(
        App.RuntimeOptions.CurrentSampleWidth,
        App.RuntimeOptions.CurrentSampleHeight).ToRect();

    /// @todo(me) Windowが不正な場合はBorderオプションに関係なく表示しないといけない
    var frameBrush = layoutElement.IsWindowValid ? Brushes.Transparent : Brushes.Red;
    var inflateValue = framePen.Thickness / 2;
    dc.DrawRectangle(frameBrush, framePen,
                     Rect.Inflate(boundRect, -inflateValue, -inflateValue));

    // ヘッダーの描画
    var headerPoint = new Point(boundRect.X, boundRect.Y);
    var header = this.CreateHeader(layoutElement);

    // ヘッダーから縁取りを取得
    /// @todo(me) 若干重い？
    var textGeometry = header.BuildGeometry(headerPoint);
    dc.DrawGeometry(null, BrushesAndPens.DropShadowPen, textGeometry);

    dc.DrawText(header, headerPoint);
  }

  /// サンプルの幅・高さをRectに変換
  private Rect SampleRect {
    get {
      return new Rect {
        X = 0.0,
        Y = 0.0,
        Width = (double)App.RuntimeOptions.CurrentSampleWidth,
        Height = (double)App.RuntimeOptions.CurrentSampleHeight
      };
    }
  }

  /// DrawingGroupの再校正
  ///
  /// @todo(me) 負荷軽減のためFPS制限してもいいかも(30FPSでだいたい半分だがカクカク)
  private void BuildDrawingGroup() {
    using (var dc = this.DrawingGroup.Open()) {
      // 背景描画でサイズを決める
      dc.DrawRectangle(Brushes.Black, null, this.SampleRect);

      // プレビューを下に描画
      if (App.Options.LayoutPreview) {
        foreach (var layoutElement in App.Profile) {
          this.DrawPreview(dc, layoutElement);
        }
      }

      // 枠線とキャプションを描画
      if (App.Options.LayoutBorder) {
        foreach (var layoutElement in App.Profile) {
          this.DrawBorder(dc, layoutElement);
        }
      }
    }
  }

  //===================================================================
  // DispatcherTimerによる再描画
  //===================================================================

  /// 再描画間隔: 5000ミリ秒
  private const double redrawTimerPeriod = 5000;
  /// 再描画用DispatcherTimer
  private DispatcherTimer redrawTimer = new DispatcherTimer();

  /// 再描画タイマー起動
  private void StartRedrawTimer() {
    redrawTimer.Interval = TimeSpan.FromMilliseconds(redrawTimerPeriod);
    redrawTimer.Tick += redrawTimer_Tick;
    redrawTimer.Start();
  }

  /// 再描画タイマーコールバック
  void redrawTimer_Tick(object sender, EventArgs e) {
    // プレビューが必要なければ更新しない
    if (!App.Options.LayoutIsExpanded) return;
    if (!App.Options.LayoutPreview) return;
    
    // マウス操作中は更新しない
    if (this.hitMode != HitModes.Neutral) return;

    // プレビュー更新のためにリクエスト送信+再描画
    this.screenCaptureTimer.ClearRequests();
    foreach (var layoutElement in App.Profile) {
      if (!layoutElement.IsWindowValid) continue;
      this.SendRequest(layoutElement, false);
    }
    this.BuildDrawingGroup();
    Debug.WriteLine(string.Format("Redraw ({0:F2}, {1:F2})",
                                  this.LayoutEditImage.ActualWidth,
                                  this.LayoutEditImage.ActualHeight),
                    "LayoutEdit");
  }

  //===================================================================
  // イベントハンドラ
  //===================================================================

  //-------------------------------------------------------------------
  // *Changed/Checked/Unchecked以外
  //-------------------------------------------------------------------

  /// マウス座標(image座標系)を(0.0-1.0, 0.0-1.0)のRelativePointに変換
  private RelativePoint GetRelativeMousePoint(IInputElement image, MouseEventArgs e) {
    var mousePoint = e.GetPosition(image);
    var relativeX = mousePoint.X / App.RuntimeOptions.CurrentSampleWidth;
    var relativeY = mousePoint.Y / App.RuntimeOptions.CurrentSampleHeight;
    return new RelativePoint(relativeX, relativeY);
  }

  //-------------------------------------------------------------------

  /// LayoutEditImage: MouseDown
  /// @param sender 使用しない
  /// @param e Client座標系でのマウス座標(GetPosition(...))の取得が可能
  private void LayoutEditImage_MouseDown(object sender, MouseButtonEventArgs e) {
    // 左クリック以外はすぐに戻る
    if (e.ChangedButton != MouseButton.Left) return;
    
    // 前処理
    var image = (IInputElement)sender;
    var relativeMousePoint = this.GetRelativeMousePoint(image, e);

    // HitTest
    int hitIndex;
    HitModes hitMode;
    if (!HitTest.TryHitTest(App.Profile, relativeMousePoint, out hitIndex, out hitMode)) return;

    // 現在選択中のIndexではない場合はそれに変更する
    if (hitIndex != App.Profile.CurrentView.Index) {
      Debug.WriteLine(string.Format("CurrentIndex ({0:D}->{1:D})",
                                    App.Profile.CurrentView.Index + 1, hitIndex + 1),
                      "LayoutEdit");

      App.Profile.CurrentIndex = hitIndex;
      UpdateCommands.UpdateMainWindowByEntireProfile.Execute(null, null);
    }

    // マウスを押した場所を記録してマウスキャプチャー開始
    this.hitMode = hitMode;
    this.relativeMouseOffset = new RelativeMouseOffset(App.Profile.CurrentView, relativeMousePoint);
    if (App.Options.LayoutSnap) {
      this.snapGuide = new SnapGuide(App.Profile);
    } else {
      this.snapGuide = null;
    }
    image.CaptureMouse();

    this.BuildDrawingGroup();
  }

  /// LayoutEditImage: MouseMove
  /// @param sender 使用しない
  /// @param e Client座標系でのマウス座標(GetPosition(...))の取得が可能
  private void LayoutEditImage_MouseMove(object sender, MouseEventArgs e) {
    // 前処理
    var image = (IInputElement)sender;
    var relativeMousePoint = this.GetRelativeMousePoint(image, e);

    // Neutralのときだけはカーソルを変えるだけ
    if (this.hitMode == HitModes.Neutral) {
      int hitIndex;
      HitModes hitMode;
      HitTest.TryHitTest(App.Profile, relativeMousePoint, out hitIndex, out hitMode);
      this.Cursor = LayoutEdit.HitModesToCursors[hitMode];
      return;
    }

    // Move or Size
    var nextBoundLTRB = MoveAndSize.MoveOrSize(App.Profile.CurrentView, this.hitMode,
        relativeMousePoint, this.relativeMouseOffset, this.snapGuide);

    // Profileを更新
    App.Profile.Current.Open();
    App.Profile.Current.SetBoundRelativeLeft = nextBoundLTRB.Left;
    App.Profile.Current.SetBoundRelativeTop = nextBoundLTRB.Top;
    App.Profile.Current.SetBoundRelativeRight = nextBoundLTRB.Right;
    App.Profile.Current.SetBoundRelativeBottom = nextBoundLTRB.Bottom;
    App.Profile.Current.Close();
      
    /// 変更をLayoutParameterに通知
    UpdateCommands.UpdateLayoutParameterByCurrentProfile.Execute(null, null);

    this.BuildDrawingGroup();
  }

  /// LayoutEditImage: MouseUp
  /// @param sender 使用しない
  /// @param e Client座標系でのマウス座標(GetPosition(...))の取得が可能
  private void LayoutEditImage_MouseUp(object sender, MouseButtonEventArgs e) {
    if (this.hitMode != HitModes.Neutral) {
      this.LayoutEditImage.ReleaseMouseCapture();
      this.hitMode = HitModes.Neutral;
      this.BuildDrawingGroup();
    }
  }

  //-------------------------------------------------------------------
  // Checked/Unchecked
  //-------------------------------------------------------------------

  //-------------------------------------------------------------------
  // *Changed/Collapsed/Expanded
  //-------------------------------------------------------------------

  //===================================================================
  // IUpdateByOptionsの実装
  //===================================================================

  /// @copydoc IUpdateByOptions::IsEnabledByOptions
  public bool IsEnabledByOptions { get; private set; }
  /// @copydoc IUpdateByOptions::UpdateByOptions
  public void UpdateByOptions() {
    this.IsEnabledByOptions = false;
    if (App.Options.LayoutIsExpanded && App.Options.LayoutPreview) {
      this.screenCaptureTimer.Start();      // タイマー再開
      this.UpdateByEntireProfile();         // プレビュー強制更新
    } else {
      this.screenCaptureTimer.Suspend();    // タイマー停止
      if (App.Options.LayoutIsExpanded) {
        this.BuildDrawingGroup();           // 再描画
      } else {
        this.DrawingGroup.Children.Clear(); // DrawingGroupもクリア
      }
      Debug.WriteLine("Collect", "*** MEMORY[GC] ***");
      GC.Collect();
    }
    this.IsEnabledByOptions = true;
  }
  
  //===================================================================
  // IUpdateByRuntimeOptionsの実装
  //===================================================================

  /// @copydoc IUpdateByRuntimeOptions::IsEnabledByRuntimeOptions
  public bool IsEnabledByRuntimeOptions { get; private set; }
  /// @copydoc IUpdateByRuntimeOptions::UpdateByRuntimeOptions
  public void UpdateByRuntimeOptions() {
    // 再描画
    /// @todo(me) 条件分岐はしたほうがいいかも？
    this.IsEnabledByRuntimeOptions = false;
    this.BuildDrawingGroup();
    this.IsEnabledByRuntimeOptions = true;
  }

  //===================================================================
  // IUpdateByProfileの実装
  //===================================================================

  /// @copydoc IUpdateByProfile::IsEnabledByProfile
  public bool IsEnabledByProfile { get; private set; }
  /// @copydoc IUpdateByProfile::UpdateByCurrentProfile
  public void UpdateByCurrentProfile() {
    this.IsEnabledByProfile = false;
    if (!App.Profile.CurrentView.IsWindowValid) return;
    this.SendRequest(App.Profile.CurrentView, true);
    this.BuildDrawingGroup();
    Debug.WriteLine("Collect", "*** MEMORY[GC] ***");
    GC.Collect();
    this.IsEnabledByProfile = true;
  }

  /// @copydoc IUpdateByProfile::UpdateByEntireProfile
  public void UpdateByEntireProfile() {
    this.IsEnabledByProfile = false;
    this.screenCaptureTimer.ClearRequests();
    foreach (var layoutElement in App.Profile) {
      if (!layoutElement.IsWindowValid) continue;
      this.SendRequest(layoutElement, true);
    }
    this.BuildDrawingGroup();
    Debug.WriteLine("Collect", "*** MEMORY[GC] ***");
    GC.Collect();
    this.IsEnabledByProfile = true;
  }

  /// LayoutElementの内容からRequestを生成してScreenCaptureTimerに画像生成を依頼
  private void SendRequest(ILayoutElementView layoutElement, bool forceUpdate) {
    Debug.Assert(layoutElement.IsWindowValid, "Invalid Window", "LayoutEdit.SendRequest");
    var request = new ScreenCaptureRequest(layoutElement);
    this.screenCaptureTimer.SendRequest(request, forceUpdate);
  }

  //===================================================================
  // フィールド
  //===================================================================

  /// スクリーンキャプチャ用スレッド管理クラスのインスタンス
  private ScreenCaptureTimer screenCaptureTimer = null;

  //-------------------------------------------------------------------
  // LayoutEditImage_MouseDown/Move/Up時の状態変数
  //-------------------------------------------------------------------

  /// マウス座標とLeft/Right/Top/BottomのOffset
  /// @todo(me) MoveAndSizeStateとしてまとめられないだろうか？
  private RelativeMouseOffset relativeMouseOffset = null;
  /// スナップガイド
  /// @todo(me) MoveAndSizeStateとしてまとめられないだろうか？
  private SnapGuide snapGuide = null;
  /// ヒットテストの結果
  /// @todo(me) MoveAndSizeStateとしてまとめられないだろうか？
  private HitModes hitMode = HitModes.Neutral;
}
}   // namespace SCFF.GUI.Controls
