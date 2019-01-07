﻿//
//  Copyright 2014  Xamarin Inc.
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.using System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace XamarinCRM.Analysis
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var path = Environment.CurrentDirectory;
                for (int i = 0; i < 3; i++)
                {
                    path = Path.Combine(Path.GetDirectoryName(path), string.Empty);
                }
                var projects = new List<Solution>
                {


                    new Solution
                    {
                        Name = "Android",
                        ProjectFiles = new List<string>
                        {
                            Path.Combine(path, "XamarinCRM.Android/XamarinCRM.Android.csproj"),
                            Path.Combine(path, "XamarinCRM/XamarinCRM.csproj")
                        },
                    },

                    new Solution
                    {
                        Name = "iOS",
                        ProjectFiles = new List<string>
                        {
                            Path.Combine(path, "XamarinCRM.iOS/XamarinCRM.iOS.csproj"),
                            Path.Combine(path, "XamarinCRM/XamarinCRM.csproj")
                        },
                    },
                };

                new Program().Run(projects);

                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Environment.Exit(0);
            }
        }

        class Solution
        {
            public string Name = "";
            public List<string> ProjectFiles = new List<string>();
            public List<FileInfo> CodeFiles = new List<FileInfo>();

            public override string ToString()
            {
                return Name;
            }

            public int UniqueLinesOfCode
            {
                get
                {
                    return (from f in CodeFiles
                                           where f.Solutions.Count == 1
                                           select f.LinesOfCode).Sum();
                }
            }

            public int SharedLinesOfCode
            {
                get
                {
                    return (from f in CodeFiles
                                           where f.Solutions.Count > 1
                                           select f.LinesOfCode).Sum();
                }
            }

            public int TotalLinesOfCode
            {
                get
                {
                    return (from f in CodeFiles
                                           select f.LinesOfCode).Sum();
                }
            }
        }

        class FileInfo
        {
            public string Path = "";
            public List<Solution> Solutions = new List<Solution>();
            public int LinesOfCode = 0;

            public override string ToString()
            {
                return Path;
            }
        }

        Dictionary<string, FileInfo> _files = new Dictionary<string, FileInfo>();

        void AddRef(string path, Solution sln)
        {

            if (_files.ContainsKey(path))
            {
                _files[path].Solutions.Add(sln);
                sln.CodeFiles.Add(_files[path]);
            }
            else
            {
                var info = new FileInfo { Path = path, };
                info.Solutions.Add(sln);
                _files[path] = info;
                sln.CodeFiles.Add(info);
            }
        }

        void Run(List<Solution> solutions)
        {
            //
            // Find all the files
            //
            foreach (var sln in solutions)
            {
                foreach (var projectFile in sln.ProjectFiles)
                {
                    var dir = Path.GetDirectoryName(projectFile);
                    var projectName = Path.GetFileNameWithoutExtension(projectFile);
                    var doc = XDocument.Load(projectFile);
                    var q = from x in doc.Descendants()
                                           let e = x as XElement
                                           where e != null
                                           where e.Name.LocalName == "Compile"
                                           where e.Attributes().Any(a => a.Name.LocalName == "Include")
                                           select e.Attribute("Include").Value;
                    foreach (var inc in q)
                    {
                        //skip over some things that are added automatically
                        if (inc.Contains("Resource.designer.cs") || //auto generated
                            inc.Contains("DebugTrace.cs") || //not needed mvvmcross
                            inc.Contains("LinkerPleaseInclude.cs") || //not needed mvvmcross
                            inc.Contains("AssemblyInfo.cs") || //in every place
                            inc.Contains("Bootstrap.cs") || //not needed mvvmcross
                            inc.Contains(".designer.cs") || //auto generated, not code
                            inc.Contains(".Designer.cs") || //Android designer file
                            inc.Contains("App.xaml.cs") || //generic WP setup
                            inc.EndsWith(".xaml") ||
                            inc.EndsWith(".xml") ||
                            inc.EndsWith(".axml"))
                        {
                            continue;
                        }

                        var inc2 = inc.Replace("\\", Path.DirectorySeparatorChar.ToString());
                        AddRef(Path.GetFullPath(Path.Combine(dir, inc2)), sln);
                    }
                }
            }

            //
            // Get the lines of code
            //
            foreach (var f in _files.Values)
            {
                var lines = File.ReadAllLines(f.Path).ToList();

                f.LinesOfCode = lines.Count;
            }

            //
            // Output
            //

            const string paddedFormat = "{0,-15}";

            var platformTitle = String.Format(paddedFormat, "Platform");
            var totalLinesTitle = String.Format(paddedFormat, "Total lines");
            var uniqueLinesTitle = String.Format(paddedFormat, "Unique lines");
            var sharedLinesTitle = String.Format(paddedFormat, "Shared lines");
            var uniquePercentageTitle = String.Format(paddedFormat, "Unique %");
            var sharedPercentageTitle = String.Format(paddedFormat, "Shared %");

            Console.WriteLine();
            Console.WriteLine("{0}{1}{2}{3}{4}{5}", platformTitle, totalLinesTitle, uniqueLinesTitle, sharedLinesTitle, uniquePercentageTitle, sharedPercentageTitle);
            Console.WriteLine();
            foreach (var sln in solutions)
            {

                Console.WriteLine("{0}{1}{2}{3}{4}{5}",
                    String.Format(paddedFormat, sln.Name),
                    String.Format(paddedFormat, sln.TotalLinesOfCode),
                    String.Format(paddedFormat, sln.UniqueLinesOfCode),
                    String.Format(paddedFormat, sln.SharedLinesOfCode),
                    String.Format(paddedFormat, String.Format("{0:p}", sln.UniqueLinesOfCode / (double)sln.TotalLinesOfCode)),
                    String.Format(paddedFormat, String.Format("{0:p}", sln.SharedLinesOfCode / (double)sln.TotalLinesOfCode)));
            }
            Console.WriteLine(string.Empty);
            Console.WriteLine("DONE");
        }
    }
}