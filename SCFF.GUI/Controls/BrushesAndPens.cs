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

/// @file SCFF.GUI/Controls/BrushesAndPens.cs
/// @copydoc SCFF::GUI::Controls::BrushesAndPens

/// SCFF.GUIで利用するUserControls
namespace SCFF.GUI.Controls {

using System.Windows.Media;

/// UserControl共通のブラシ・ペン
public static class BrushesAndPens {
  /// WindowTypes.NormalでCurrent時のブラシ
  public static readonly Brush CurrentNormalBrush;
  /// WindowTypes.Normalの半透明ブラシ
  public static readonly Brush TransparentNormalBrush;
  /// WindowTypes.Normalのブラシ
  public static readonly Brush NormalBrush;

  /// WindowTypes.DesktopListViewでCurrent時のブラシ
  public static readonly Brush CurrentDesktopListViewBrush;
  /// WindowTypes.DesktopListViewの半透明ブラシ
  public static readonly Brush TransparentDesktopListViewBrush;
  /// WindowTypes.DesktopListViewのブラシ
  public static readonly Brush DesktopListViewBrush;

  /// WindowTypes.DesktopでCurrent時のブラシ
  public static readonly Brush CurrentDesktopBrush;
  /// WindowTypes.Desktopの半透明ブラシ
  public static readonly Brush TransparentDesktopBrush;
  /// WindowTypes.Desktopの半透明ブラシ
  public static readonly Brush DesktopBrush;

  /// ドロップシャドウ描画用ブラシ
  public static readonly Brush DropShadowBrush;

  /// ペンの太さ(ダミー)
  /// @attention ダミーなので実際に使う場合はCloneしたのちにFreezeすること
  private const double dummyPenThickness = 2;

  /// WindowTypes.NormalでCurrent時のペン
  public static readonly Pen CurrentNormalPen;
  /// WindowTypes.Normalのペン
  public static readonly Pen NormalPen;
  /// WindowTypes.DesktopListViewでCurrent時のペン
  public static readonly Pen CurrentDesktopListViewPen;
  /// WindowTypes.DesktopListViewのペン
  public static readonly Pen DesktopListViewPen;
  /// WindowTypes.DesktopでCurrent時のペン
  public static readonly Pen CurrentDesktopPen;
  /// WindowTypes.Desktopのペン
  public static readonly Pen DesktopPen;

  /// staticコンストラクタ
  static BrushesAndPens() {
    // Brushes
    BrushesAndPens.CurrentNormalBrush = Brushes.DarkOrange;
    BrushesAndPens.CurrentNormalBrush.Freeze();
    BrushesAndPens.TransparentNormalBrush =
        new SolidColorBrush(Color.FromArgb(0x99, 0xFF, 0x8C, 0x00));
    BrushesAndPens.TransparentNormalBrush.Freeze();
    BrushesAndPens.NormalBrush =
        new SolidColorBrush(Color.FromRgb(0x7F, 0x44, 0x00));
    BrushesAndPens.NormalBrush.Freeze();

    BrushesAndPens.CurrentDesktopListViewBrush = Brushes.DarkCyan;
    BrushesAndPens.CurrentDesktopListViewBrush.Freeze();
    BrushesAndPens.TransparentDesktopListViewBrush =
        new SolidColorBrush(Color.FromArgb(0x99, 0x00, 0x8B, 0x8B));
    BrushesAndPens.TransparentDesktopListViewBrush.Freeze();
    BrushesAndPens.DesktopListViewBrush =
        new SolidColorBrush(Color.FromRgb(0x00, 0x3F, 0x3F));
    BrushesAndPens.DesktopListViewBrush.Freeze();

    BrushesAndPens.CurrentDesktopBrush = Brushes.DarkGreen;
    BrushesAndPens.CurrentDesktopBrush.Freeze();
    BrushesAndPens.TransparentDesktopBrush =
        new SolidColorBrush(Color.FromArgb(0x99, 0x00, 0x64, 0x00));
    BrushesAndPens.TransparentDesktopBrush.Freeze();
    BrushesAndPens.DesktopBrush =
        new SolidColorBrush(Color.FromRgb(0x00, 0x33, 0x00));
    BrushesAndPens.DesktopBrush.Freeze();

    BrushesAndPens.DropShadowBrush = Brushes.Black;
    BrushesAndPens.DropShadowBrush.Freeze();

    // Pens
    BrushesAndPens.CurrentNormalPen =
        new Pen(BrushesAndPens.CurrentNormalBrush, BrushesAndPens.dummyPenThickness);
    BrushesAndPens.CurrentNormalPen.Freeze();
    BrushesAndPens.NormalPen =
        new Pen(BrushesAndPens.NormalBrush, BrushesAndPens.dummyPenThickness);
    BrushesAndPens.NormalPen.Freeze();
    BrushesAndPens.CurrentDesktopListViewPen =
        new Pen(BrushesAndPens.CurrentDesktopListViewBrush, BrushesAndPens.dummyPenThickness);
    BrushesAndPens.CurrentDesktopListViewPen.Freeze();
    BrushesAndPens.DesktopListViewPen =
        new Pen(BrushesAndPens.DesktopListViewBrush, BrushesAndPens.dummyPenThickness);
    BrushesAndPens.DesktopListViewPen.Freeze();
    BrushesAndPens.CurrentDesktopPen =
        new Pen(BrushesAndPens.CurrentDesktopBrush, BrushesAndPens.dummyPenThickness);
    BrushesAndPens.CurrentDesktopPen.Freeze();
    BrushesAndPens.DesktopPen =
        new Pen(BrushesAndPens.DesktopBrush, BrushesAndPens.dummyPenThickness);
    BrushesAndPens.DesktopPen.Freeze();
  }
}
}   // SCFF.GUI.Controls
