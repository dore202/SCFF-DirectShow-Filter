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

/// @file scff_imaging/padding.cc
/// scff_imaging::Paddingの定義

#include "scff_imaging/padding.h"

#include "scff_imaging/avpicture_image.h"

namespace scff_imaging {

//=====================================================================
// scff_imaging::Padding
//=====================================================================

Padding::Padding(int padding_left, int padding_right,
                 int padding_top, int padding_bottom)
    : padding_left_(padding_left),        // ありえない値
      padding_right_(padding_right),      // ありえない値
      padding_top_(padding_top),          // ありえない値
      padding_bottom_(padding_bottom) {   // ありえない値
  // 明示的に初期化していない
  // draw_context_
  // padding_color_
}

Padding::~Padding() {
  // nop
}

//-------------------------------------------------------------------

ErrorCodes Padding::Init() {
  ASSERT(GetInputImage()->pixel_format() == GetOutputImage()->pixel_format());

  // パディング用のコンテキストの初期化
  const int error_init =
      ff_draw_init(&draw_context_,
                   GetOutputImage()->av_pixel_format(),
                   0);
  ASSERT(error_init == 0);

  // パディング用のカラーを真っ黒に設定
  uint8_t rgba_padding_color[4] = {0};
  ff_draw_color(&draw_context_,
                &padding_color_,
                rgba_padding_color);

  return InitDone();
}

ErrorCodes Padding::Run() {
  // 左の枠を書く
  ff_fill_rectangle(&draw_context_, &padding_color_,
                    GetOutputImage()->avpicture()->data,
                    GetOutputImage()->avpicture()->linesize,
                    0,
                    padding_top_,
                    padding_left_,
                    GetInputImage()->height());

  // 右の枠を書く
  ff_fill_rectangle(&draw_context_, &padding_color_,
                    GetOutputImage()->avpicture()->data,
                    GetOutputImage()->avpicture()->linesize,
                    padding_left_ + GetInputImage()->width(),
                    padding_top_,
                    padding_right_,
                    GetInputImage()->height());

  // 上の枠を書く
  ff_fill_rectangle(&draw_context_, &padding_color_,
                    GetOutputImage()->avpicture()->data,
                    GetOutputImage()->avpicture()->linesize,
                    0,
                    0,
                    GetOutputImage()->width(),
                    padding_top_);

  // 中央に画像を配置する
  ff_copy_rectangle2(&draw_context_,
                     GetOutputImage()->avpicture()->data,
                     GetOutputImage()->avpicture()->linesize,
                     GetInputImage()->avpicture()->data,
                     GetInputImage()->avpicture()->linesize,
                     padding_left_,
                     padding_top_,
                     0,
                     0,
                     GetInputImage()->width(),
                     GetInputImage()->height());

  // 下の枠を書く
  ff_fill_rectangle(&draw_context_, &padding_color_,
                    GetOutputImage()->avpicture()->data,
                    GetOutputImage()->avpicture()->linesize,
                    0,
                    padding_top_ + GetInputImage()->height(),
                    GetOutputImage()->width(),
                    padding_bottom_);

  return GetCurrentError();
}

}   // namespace scff_imaging
