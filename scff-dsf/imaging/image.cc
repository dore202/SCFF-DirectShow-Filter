﻿
// Copyright 2012 Alalf <alalf.iQLc_at_gmail.com>
//
// This file is part of SCFF DSF.
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

/// @file imaging/image.cc
/// @brief imaging::Imageの定義

#include "imaging/image.h"

#include "base/debug.h"

namespace imaging {

//=====================================================================
// imaging::Image
//=====================================================================

// コンストラクタ
Image::Image()
    : pixel_format_(kInvalidPixelFormat),   // ありえない値
      width_(-1),                           // ありえない値
      height_(-1) {                         // ありえない値
  // nop
}

// デストラクタ
Image::~Image() {
  // nop
}

// Getter: イメージのピクセルフォーマット
ImagePixelFormat Image::pixel_format() const {
  return pixel_format_;
}

/// Getter: イメージの幅
int Image::width() const {
  return width_;
}

/// Getter: イメージの高さ
int Image::height() const {
  return height_;
}

//-------------------------------------------------------------------
// 実体を生成する
ErrorCode Image::Create(ImagePixelFormat pixel_format, int width, int height) {
  if (!IsEmpty()) {
    ASSERT(false);
    return kMultipleCreateError;
  }

  pixel_format_ = pixel_format;
  width_ = width;
  height_ = height;
  return kNoError;
}
//-------------------------------------------------------------------
}   // namespace imaging
