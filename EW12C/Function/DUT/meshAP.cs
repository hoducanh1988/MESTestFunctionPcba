using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using EW12C.Function.Protocol;

namespace EW12C.Function.DUT {
    public class meshAP<T, S> : RS232<T, S> where T : class, new() where S : class, new() {

        public meshAP(T t, S s, string uart_log, string serial_port) : base(t, s, uart_log, serial_port) { }

    }
}
