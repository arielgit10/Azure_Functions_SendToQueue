﻿using System;

namespace FunctionAppAriel
{
    public class Account
    {
        public Guid AccountId { get; set; }
        public string Alias { get; set; } = string.Empty;
        public string Cbu { get; set; } = string.Empty;
        public decimal Balance { get; set; }
    }
}