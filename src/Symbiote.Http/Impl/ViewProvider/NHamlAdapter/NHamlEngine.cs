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
using System.Reflection;
using NHaml;
using NHaml.Compilers.CSharp4;
using NHaml.TemplateResolution;
using Symbiote.Core.Collections;
using Symbiote.Http.Config;

namespace Symbiote.Http.Impl.ViewProvider.NHamlAdapter 
{
    public class NHamlEngine : ITemplateContentProvider, IViewEngine
    {
        public readonly string EXTENSION = "haml";
        public TemplateEngine TemplateEngine { get; set; }
        public Type BaseTemplateType { get; set; }
        public ExclusiveConcurrentDictionary<Tuple<string, Type>, CompiledTemplate> CompiledTemplates { get; set; }
        public IViewLocator ViewLocator { get; set; }
        public IList<string> PathSources { get; set; }

        public IViewSource GetViewSource( string templateFile )
        {
            var info = new FileInfo( templateFile );
            return new FileViewSource( info );
        }

        public IViewSource GetViewSource( string templatePath, IList<IViewSource> parentViewSourceList )
        {
            return GetViewSource( templatePath );   
        }

        public void AddPathSource( string pathSource )
        {
            PathSources.Add( pathSource );
        }

        private void InitializeTemplateEngine()
        {
            foreach ( var referencedAssembly in Assembly.GetEntryAssembly().GetReferencedAssemblies() )
            {
                TemplateEngine.Options.AddReference( Assembly.Load(referencedAssembly).Location );
            } 
            TemplateEngine.Options.AddReference( typeof( Owin ).Assembly.Location );
            TemplateEngine.Options.AddReference( typeof( TemplateEngine ).Assembly.Location );

            TemplateEngine.Options.TemplateBaseType = BaseTemplateType;
            TemplateEngine.Options.TemplateContentProvider = this;
            TemplateEngine.Options.IndentSize = 4;
            TemplateEngine.Options.OutputDebugFiles = true;
            TemplateEngine.Options.UseTabs = true;
            TemplateEngine.Options.TemplateCompiler = new CSharp4TemplateCompiler();
        }

        public CompiledTemplate GetCompiledTemplateFor<TModel>(string view)
        {
            Type type = typeof(TModel);
            return CompiledTemplates.ReadOrWrite( Tuple.Create( view, type ),
                () => TemplateEngine.Compile( view, typeof( NHamlView<> ).MakeGenericType( type ) ) );
        }

        public CompiledTemplate GetCompiledTemplateFor<TModel>(string view, string layout)
        {
            Type type = typeof(TModel);
            var combineView = string.Join( "-", layout, view );
            return CompiledTemplates.ReadOrWrite( Tuple.Create( combineView, type ),
                () => TemplateEngine.Compile( new[] {layout, view} , typeof( NHamlView<> ).MakeGenericType( type ) ) );
        }

        public void Render<TModel>(string view, TModel model, TextWriter writer )
        {
            var viewPath = ViewLocator.GetViewPath<TModel>( view, EXTENSION );
            var layoutPath = ViewLocator.GetDefaultLayout<TModel>( EXTENSION );
            CompiledTemplate template = layoutPath == null 
                ? GetCompiledTemplateFor<TModel>( viewPath )
                : GetCompiledTemplateFor<TModel>( viewPath, layoutPath );
            var instance = ( IView ) template.CreateInstance();
            instance.SetModel( model );
            instance.Render( writer );
        }

        public void Render<TModel>(string view, string layout, TModel model, TextWriter writer )
        {
            var viewPath = ViewLocator.GetViewPath<TModel>( view, EXTENSION );
            var layoutPath = ViewLocator.GetViewPath<TModel>( view, EXTENSION );
            CompiledTemplate template = GetCompiledTemplateFor<TModel>(viewPath, layoutPath);
            var instance = ( IView ) template.CreateInstance();
            instance.SetModel( model );
            instance.Render( writer );
        }

        public NHamlEngine( IViewLocator viewLocator )
        {
            ViewLocator = viewLocator;
            PathSources = new List<string>();
            CompiledTemplates = new ExclusiveConcurrentDictionary<Tuple<string, Type>, CompiledTemplate>();
            BaseTemplateType = typeof(NHamlView<>);
            TemplateEngine = new TemplateEngine();
            InitializeTemplateEngine();
            PathSources.Add( "Views" );
            PathSources.Add( "Shared" );
        }
    }
}
