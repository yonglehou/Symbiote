﻿// /* 
// Copyright 2008-2011 Alex Robson
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// */
using System;
using System.Collections.Generic;
using System.IO;
using Symbiote.Core.Utility;
using Symbiote.Daemon.BootStrap.Config;
using System.Linq;
using Symbiote.Messaging;

namespace Symbiote.Daemon.BootStrap
{
    public class Watcher
    {
        public BootStrapConfiguration Configuration { get; set; }
        public IList<FileSystemWatcher> SystemEvents { get; set; }
        public IList<IObservable<IEvent<FileSystemEventArgs>>> SystemObservers { get; set; }
        public IBus Bus { get; set; }

        public Watcher( BootStrapConfiguration configuration )
        {
            Configuration = configuration;
        }
        
        public FileSystemWatcher ConfigureWatcher(string path, string patterns)
        {
            var fullPath = Path.GetFullPath( path );
            var watcher = new FileSystemWatcher( );
            watcher.Path = fullPath;
            watcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastAccess;
            watcher.IncludeSubdirectories = true;
            Observable
                .FromEvent<FileSystemEventHandler, FileSystemEventArgs>(
                    c => c.Invoke,
                    h =>
                    {
                        watcher.Created += h;
                        watcher.Changed += h;
                        watcher.Deleted += h;
                    },
                    h =>
                    {
                        watcher.Created -= h;
                        watcher.Changed -= h;
                        watcher.Deleted -= h;
                    })
                .DistinctUntilChanged()
                .BufferWithTime(TimeSpan.FromSeconds( 10 ))
                .Subscribe(l =>
                {
                    var e = l.FirstOrDefault();
                    if ( e != null )
                        OnSystemChange(
                            e.Sender,
                            e.EventArgs);
                });
            watcher.EnableRaisingEvents = true;
            return watcher;
        }

        public string IsPathToAFile(FileSystemEventArgs e)
        {
            return Path.GetExtension(e.Name);
        }

        public void OnSystemChange(object sender, FileSystemEventArgs args)
        {
            switch( args.ChangeType )
            {
                case WatcherChangeTypes.Changed:
                    Bus.Publish("local", new ApplicationChanged() { DirectoryPath = args.FullPath });
                    break;
                case WatcherChangeTypes.Created:
                    Bus.Publish("local", new NewApplication() { DirectoryPath = args.FullPath });
                    break;
                case WatcherChangeTypes.Deleted:
                    Bus.Publish("local", new ApplicationDeleted() { DirectoryPath = args.FullPath });
                    break;
            }
        }

        public void Start()
        {
            var patterns = DelimitedBuilder.Construct(Configuration.FileExtensions, "; ");
            SystemEvents = Configuration.WatchPaths.Select(x => ConfigureWatcher(x, "")).ToList();
        }
    }
}