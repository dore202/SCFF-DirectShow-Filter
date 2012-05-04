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

/// @file imaging/request.h
/// @brief imaging::Requestの宣言

#ifndef SCFF_DSF_IMAGING_REQUEST_H_
#define SCFF_DSF_IMAGING_REQUEST_H_

#include "imaging/engine.h"

namespace imaging {

/// @brief エンジンに対するリクエストをカプセル化したクラス
class Request {
 public:
  /// @brief デストラクタ
  virtual ~Request() {
    // nop
  }
  /// @brief ダブルディスパッチ用
  virtual void SendTo(Engine *engine) const = 0;
 protected:
  /// @brief コンストラクタ
  Request() {
    // nop
  }
};

/// @brief リクエスト: ResetLayout
class ResetLayoutRequest : public Request {
 public:
  /// @brief コンストラクタ
  ResetLayoutRequest() : Request() {
    // このリクエストは特に情報を必要としない
  }
  /// @brief デストラクタ
  ~ResetLayoutRequest() {
    // nop
  }
  /// @brief ダブルディスパッチ用
  void SendTo(Engine *engine) const {
    engine->DoResetLayout();
  }
};

/// @brief リクエスト: SetNativeLayout
class SetNativeLayoutRequest : public Request {
 public:
  /// @brief コンストラクタ
  SetNativeLayoutRequest(const ScreenCaptureParameter& parameter,
                         bool stretch, bool keep_aspect_ratio)
      : Request(),
        parameter_(parameter),
        stretch_(stretch),
        keep_aspect_ratio_(keep_aspect_ratio) {
    // nop
  }
  /// @brief デストラクタ
  ~SetNativeLayoutRequest() {
    // nop
  }
  /// @brief ダブルディスパッチ用
  void SendTo(Engine *engine) const {
    engine->DoSetNativeLayout(parameter_, stretch_, keep_aspect_ratio_);
  }

 private:
  /// @copydoc NativeLayout::parameter_
  const ScreenCaptureParameter parameter_;
  /// @copydoc NativeLayout::stretch_
  const bool stretch_;
  /// @copydoc NativeLayout::keep_aspect_ratio_
  const bool keep_aspect_ratio_;
};

}   // namespace imaging

#endif  // SCFF_DSF_IMAGING_REQUEST_H_
