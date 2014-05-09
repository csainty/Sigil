﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestRunner
{
    class Program
    {
        static int Main(string[] args)
        {
            var classes = Assembly.Load(new AssemblyName("SigilTests")).GetTypes().Where(t => t.IsPublic).ToList();

            bool success = true;

            foreach (var clazz in classes.OrderBy(o => o.Name))
            {
                if (clazz.Name.Equals("Break")) continue;

                var inst = clazz.GetConstructor(Type.EmptyTypes).Invoke(new object[0]);

                foreach (var test in clazz.GetMethods().Where(m => m.GetCustomAttribute<FactAttribute>() != null).OrderBy(o => o.Name))
                {
                    var name = clazz.Name + "." + test.Name;

                    Console.Write(name + "...");

                    try
                    {
                        test.Invoke(inst, new object[0]);
                        Console.WriteLine(" Passed");
                    }
                    catch (Exception failure)
                    {
                        success = false;

                        Console.WriteLine("Failure!");
                        Console.WriteLine();
                        Console.WriteLine(failure.ToString());

                        Console.ReadKey();
                    }
                }
            }

            return success ? 0 : -1;
        }
    }
}
