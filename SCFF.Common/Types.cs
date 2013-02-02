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

/// @file SCFF.Common/Types.cs
/// SCFF.Commonモジュールで利用する型

/// SCFF共有クラスライブラリ
namespace SCFF.Common {

using System;

//=====================================================================
// 列挙型
//=====================================================================

/// Options用WindowState。System.Windows.WindowStateと相互に変換する。
public enum WindowState {
  Normal,     ///< 標準状態
  Minimized,  ///< 最小化
  Maximized   ///< 最大化
}

/// Profile用Windowの種類
public enum WindowTypes {
  Normal,           ///< 標準のWindow
  DesktopListView,  ///< OS別デスクトップWindow
  Desktop,          ///< ルートWindow
}

//=====================================================================
// クラス・構造体
//=====================================================================

// これをC#のGenericsで綺麗にやろうと思うとかなりめんどくさいことになる（＝放置）

/// WindowsのRectが使えないので自前で用意したPoint<int>
public class IntPoint {
  /// コンストラクタ
  public IntPoint(int x, int y) {
    this.X = x;
    this.Y = y;
  }
  /// X座標
  public int X { get; private set; }
  /// Y座標
  public int Y { get; private set; }
}

/// WindowsのRectが使えないので自前で用意したRect<int>
public class IntRect {
  /// コンストラクタ
  public IntRect(int x, int y, int width, int height) {
    this.X = x;
    this.Y = y;
    this.Width = width;
    this.Height = height;
  }
  /// 左上端のX座標
  public int X { get; private set; }
  /// 左上端のY座標
  public int Y { get; private set; }
  /// 幅
  public int Width { get; private set; }
  /// 高さ
  public int Height { get; private set; }
  /// 右下端のX座標
  public int Right { get { return this.X + this.Width; } }
  /// 右下端のY座標
  public int Bottom { get { return this.Y + this.Height; } }

  /// 交差判定
  public bool IntersectsWith(IntRect other) {
    return !(this.X > other.Right || other.X > this.Right ||
             this.Y > other.Bottom || other.Y > this.Bottom);
  }
  /// 交差
  public void Intersect(IntRect other) {
    if (!this.IntersectsWith(other)) return;
    var newX = Math.Max(this.X, other.X);
    var newY = Math.Max(this.Y, other.Y);
    var newRight = Math.Min(this.Right, other.Right);
    var newBottom = Math.Min(this.Bottom, other.Bottom);
    this.X = newX;
    this.Y = newY;
    this.Width = newRight - newX;
    this.Height = newBottom - newY;
  }

  /// 含有判定
  public bool Contains(IntRect other) {
    return this.X <= other.X && other.Right <= this.Right &&
           this.Y <= other.Y && other.Bottom <= this.Bottom;
  }
}

/// WindowsのRectが使えないので自前で用意したPoint<double>
public class DoublePoint {
  /// コンストラクタ
  public DoublePoint(double x, double y) {
    this.X = x;
    this.Y = y;
  }
  /// X座標
  public double X { get; private set; }
  /// Y座標
  public double Y { get; private set; }
}

/// 自前で用意したLTRB<double>
public class DoubleLTRB {
  /// コンストラクタ
  public DoubleLTRB(double left, double top, double right, double bottom) {
    this.Left = left;
    this.Top = top;
    this.Right = right;
    this.Bottom = bottom;
  }
  /// 左上端のX座標
  public double Left { get; private set; }
  /// 左上端のY座標
  public double Top { get; private set; }
  /// 右下端のX座標
  public double Right { get; private set; }
  /// 右上端のY座標
  public double Bottom { get; private set; }
}

/// WindowsのRectが使えないので自前で用意したPoint<double>
public class DoubleRect {
  /// コンストラクタ
  public DoubleRect(double x, double y, double width, double height) {
    this.X = x;
    this.Y = y;
    this.Width = width;
    this.Height = height;
  }
  /// 左上端のX座標
  public double X { get; private set; }
  /// 左上端のY座標
  public double Y { get; private set; }
  /// 幅
  public double Width { get; private set; }
  /// 高さ
  public double Height { get; private set; }
  /// 右下端のX座標
  public double Right { get { return this.X + this.Width; } }
  /// 右下端のY座標
  public double Bottom { get { return this.Y + this.Height; } }

  /// 含有判定
  /// @param point 判定対象のPoint
  /// @return 含有しているか
  /// @todo(me) 浮動小数点数の比較
  public bool Contains(DoublePoint point) {
    return this.X <= point.X && point.X <= this.Right &&
           this.Y <= point.Y && point.Y <= this.Bottom;
  }
}

//---------------------------------------------------------------------
// サンプル座標系のPoint/Rect etc.
//---------------------------------------------------------------------

/// サンプル座標系のRect
public class SampleRect : IntRect {
  /// コンストラクタ
  public SampleRect(int x, int y, int width, int height)
      : base(x, y, width, height) {}
}

//---------------------------------------------------------------------
// Client座標系のPoint/Rect etc.
//---------------------------------------------------------------------

/// Client座標系のRect
public class ClientRect : IntRect {
  /// コンストラクタ
  public ClientRect(int x, int y, int width, int height)
      : base(x, y, width, height) {}
}

//---------------------------------------------------------------------
// Screen座標系のPoint/Rect etc.
//---------------------------------------------------------------------

/// Screen座標系のPoint
public class ScreenPoint : IntPoint {
  /// コンストラクタ
  public ScreenPoint(int x, int y) : base(x, y) {}
}

/// Screen座標系のRect
public class ScreenRect : IntRect {
  /// コンストラクタ
  public ScreenRect(int x, int y, int width, int height)
      : base(x, y, width, height) {}
}

//---------------------------------------------------------------------
// 相対座標系([0-1], [0-1])のPoint/Rect etc.
//---------------------------------------------------------------------

/// ([0-1], [0-1])の相対座標系のPoint
public class RelativePoint : DoublePoint {
  /// コンストラクタ
  public RelativePoint(double x, double y) : base(x, y) {}
}

/// ([0-1], [0-1])の相対座標系内の領域を示すLTRB
public class RelativeLTRB : DoubleLTRB {
  /// コンストラクタ
  /// @param left left
  /// @param top top
  /// @param right right
  /// @param bottom bottom
  public RelativeLTRB(double left, double top, double right, double bottom)
      : base(left, top, right, bottom) {
    /// @todo(me) 浮動小数点数の比較
    ///           Debug.Assert書く
  }
}

/// ([0-1], [0-1])の相対座標系に収まるRect
public class RelativeRect : DoubleRect {
  /// コンストラクタ
  /// @param x x
  /// @param y y
  /// @param width width
  /// @param height height
  public RelativeRect(double x, double y, double width, double height)
      : base(x, y, width, height) {
    /// @todo(me) 浮動小数点数の比較
    ///           Debug.Assert書く
  }
}
}   // namespace SCFF.Common
