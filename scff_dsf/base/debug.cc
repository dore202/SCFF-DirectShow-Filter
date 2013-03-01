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

/// @file base/debug.cc
/// デバッグ用定数、関数定義

#include "base/debug.h"

#include <tchar.h>

//=====================================================================
// デバッグ用定数
//=====================================================================

const int kTraceInfo    = 1;
const int kTraceDebug   = 2;
const int kTrace        = 3;

const int kTraceCurrentLevel = kTraceInfo;

const int kErrorFatal   = 1;
const int kError        = 2;
const int kErrorWarn    = 3;

const int kErrorCurrentLevel = kErrorWarn;