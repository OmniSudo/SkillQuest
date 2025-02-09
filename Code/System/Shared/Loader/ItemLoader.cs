using System;
using System.Diagnostics;
using System.IO;
using System.Xml.Linq;
using Sandbox;
using Sandbox.Core;

public sealed class ItemLoader : Component {
    public static ItemLoader Instance { get; set; }
    
    protected override void OnStart() {
        Instance = this;
        
        Log.Info( FileSystem.Mounted.GetFullPath( "" ));
    }

    public Item? OpenItem(string name) {
        return null;
    }

    public static Item? Init(string path) {
        return null;
    }

    public static Item? Get(string path) {
        return null;
    }
}