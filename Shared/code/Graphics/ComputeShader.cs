using Godot;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace SkillQuest.Graphics;

public class ComputeShader : IDisposable {
    private static RenderingDevice RD = RenderingServer.CreateLocalRenderingDevice();

    public RenderingDevice Device => RD;

    public Rid ID;

    private string rootFile;

    public ComputeShader(string file) {
        rootFile = file;

        ID = Compile(
            string.Join( System.Environment.NewLine, ProcessIncludes( file ) )
        );
    }

    private Rid Compile(string data) {
        var src = new RDShaderSource();
        src.SourceCompute = data;

        var spirv = RD.ShaderCompileSpirVFromSource( src );
        var err = spirv.GetStageCompileError( RenderingDevice.ShaderStage.Compute );

        if (err is null || err.Length == 0) {
            GD.Print( "Compiled " + rootFile + " as a compute shader" );
            return RD.ShaderCreateFromSpirV( spirv );
        } else {
            throw new InvalidOperationException( err.ToString() );
        }
    }

    private Dictionary<string, string[]> sources = new();

    private Dictionary<string, FileSystemWatcher> _watchers = new();

    private string[] ProcessIncludes(string file) {
        file = File.Exists( file ) ? file : ProjectSettings.GlobalizePath( file );
        var data = File.ReadAllLines( file );

        FileSystemWatcher watcher = new FileSystemWatcher();
        watcher.Path = Path.GetDirectoryName(file) ?? ".";
        watcher.Filter = Path.GetFileName(file);
        watcher.NotifyFilter = NotifyFilters.LastWrite;
        watcher.Changed += OnChanged;
        watcher.EnableRaisingEvents = true;

        sources.Add( file, data );
        _watchers.Add( file, watcher );
        
        var src = new List<string>();

        foreach (var line in data) {
            var match = Regex.Match( line, $"#include \"(.*?)\"" );

            if (match.Success) {
                if (!sources.ContainsKey( match.Groups[1].Value )) {
                    src.AddRange( sources[match.Groups[1].Value] = ProcessIncludes( match.Groups[1].Value ) );
                }
            } else {
                src.Add( line );
            }
        }

        return src.ToArray();
    }

    private void OnChanged(object sender, FileSystemEventArgs e) {
        
        sources.Clear();

        foreach (var watcher in _watchers) {
            watcher.Value.Dispose();
        }
        
        _watchers.Clear();

        try {
            var compile = Compile(
                string.Join( System.Environment.NewLine, ProcessIncludes( e.FullPath ) )
            );

            RD.FreeRid( ID );
            ID = compile;
        } catch (Exception exception) {
            GD.PrintErr( $"Failed to compile compute shader:\n{exception.Message}");
        }
    }

    public void Dispose() {
        RD.FreeRid( ID );
        foreach (var watcher in _watchers) {
            watcher.Value.Dispose();
        }
    }
}