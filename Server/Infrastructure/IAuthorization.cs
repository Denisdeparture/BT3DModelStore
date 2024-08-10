﻿using DomainModel.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    internal interface IAuthorization
    {
        public void Login(User user);
        public void Logout();
    }
}
