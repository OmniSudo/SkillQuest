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

    public ComputeShader(string file) {
        var data = string.Join( System.Environment.NewLine, ProcessIncludes( file ) );
        
        var src = new RDShaderSource();
        src.SourceCompute = data;

        var spirv = RD.ShaderCompileSpirVFromSource( src );
        var err = spirv.GetStageCompileError( RenderingDevice.ShaderStage.Compute );
        
        if (err is null || err.Length == 0) {
            ID = RD.ShaderCreateFromSpirV( spirv );
        } else {
            GD.PrintErr( err.ToString() );
        }
    } 
    
    private Dictionary< string, string[] > sources = new();

    private string[] ProcessIncludes(string file) {
        var data = File.ReadAllLines( ProjectSettings.GlobalizePath( file ));
        sources.Add( file, data );
        
        var src = new List<string>();

        foreach (var line in data) {
            var match = Regex.Match( line, $"#include \"(.*?)\"" );
            
            if (match.Success) {
                if (!sources.ContainsKey( match.Groups[1].Value )) {
                    src.AddRange( sources[ match.Groups[1].Value ] = ProcessIncludes( match.Groups[1].Value)  );
                }
            } else {
                src.Add( line );
            }
        }
        
        return src.ToArray();
    }
    
    public void Dispose() {
        RD.FreeRid(ID);
        
        
    }
}