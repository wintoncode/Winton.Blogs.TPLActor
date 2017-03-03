// Copyright (c) Winton. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENCE in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;

namespace Winton.Blogs.TPLActor
{
    public sealed class Program
    {
        public static void Main(string[] args)
        {
            Test1().Wait();
            Test2().Wait();
            Test3().Wait();
            Test4().Wait();
        }

        public static async Task Test1()
        {
            Console.WriteLine("Test 1");
            Console.WriteLine("START");

            var actor = new Actor();
            var tasks = Enumerable.Range(1, 10)
                                  .Select(x => actor.Enqueue(() =>
                                                             {
                                                                 var y = x * x;
                                                                 Console.WriteLine($"{x} x {x} = {y}");
                                                             }))
                                  .ToArray();

            await Task.WhenAll(tasks);
            Console.WriteLine("END");
            Console.WriteLine();
        }

        public static async Task Test2()
        {
            Console.WriteLine("Test 2");
            var actor = new Actor();
            await actor.Enqueue(() =>
                                {
                                    Console.WriteLine("Hello!");
                                    Task.Factory.StartNew(() => Console.WriteLine("Bonjour!"));
                                });
            Console.WriteLine();
        }

        public static async Task Test3()
        {
            Console.WriteLine("Test 3");
            Console.WriteLine("START");
            await await Task.Factory.StartNew(async () =>
                                              {
                                                  var actor = new Actor();
                                                  var value = await actor.Enqueue(() => 73);
                                                  Console.WriteLine($"Value: {value}");
                                              });
            Console.WriteLine("END");
            Console.WriteLine();
        }

        public static async Task Test4()
        {
            Console.WriteLine("Test 4");
            var actor = new Actor();

            await actor.Enqueue(async () =>
                                {
                                    Console.WriteLine("Should be on actor.");
                                    await Task.Run(() => Console.WriteLine("Should be off actor."));
                                    Console.WriteLine("Should be on actor.");
                                });
            Console.WriteLine();
        }
    }
}
