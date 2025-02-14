using Sandbox.SQ.Action;
using System;

namespace Sandbox.SQ;

public class Item : Component {
	[Property] public string ID { get; set; }

	[Property] public Use Primary { get; set; }
	
	[Property] public Use Secondary { get; set; }
	
	[Property] public Use Use { get; set; }
}
