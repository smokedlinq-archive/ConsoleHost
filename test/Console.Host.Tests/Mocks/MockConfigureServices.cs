﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Tests
{
    public class MockConfigureServices : IConfigureServices
    {
        public void Configure(IServiceCollection container)
        {
        }
    }
}