﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StayInTarkov.Networking
{
    public interface IGameClient
    {
        public void SendDataToServer(byte[] data);
    }
}