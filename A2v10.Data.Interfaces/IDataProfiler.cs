﻿// Copyright © 2015-2018 Alex Kukhtin. All rights reserved.

using System;

namespace A2v10.Data.Interfaces
{
	public interface IDataProfiler
	{
		IDisposable Start(String command);
	}
}
