﻿using System;

namespace MultipleBotFramework.Exceptions
{
    public class NullUpdateModelInMiddleWareException : Exception
    {
        public NullUpdateModelInMiddleWareException()
        : base($"Update model is null in middleware!")
        {
        }
    }
}