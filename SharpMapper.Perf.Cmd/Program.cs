using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SharpMapper.Perf.Cmd {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("Start:");
            var times = 100000;

            var user = new User {
                Name = "foo",
                Age = 1,
                Tools = new List<Tool>()
            };
            user.Tools.Add(new Tool { Name = "tool1" });
            user.Tools.Add(new Tool { Name = "tool2" });
            user.Tools.Add(new Tool { Name = "tool3" });

            var sw = new Stopwatch();
            sw.Start();
            for (var i = 0; i < times; i++) {
                Mapper.Map<User>(user);
            }
            Console.WriteLine($"End: {sw.Elapsed}");
        }
    }

    public class User {
        public string Name { get; set; }
        public int Age { get; set; }
        public List<Tool> Tools { get; set; }
    }

    public class Tool {
        public string Name { get; set; }
    }

    public class ToolTool : Tool {
        public Tool Tool { get; set; }
    }
}